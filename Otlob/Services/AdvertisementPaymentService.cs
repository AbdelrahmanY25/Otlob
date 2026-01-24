using Otlob.Errors;
using Stripe;
using Stripe.Checkout;

namespace Otlob.Services;

public class AdvertisementPaymentService : IAdvertisementPaymentService
{
    private readonly IUnitOfWorkRepository _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AdvertisementPaymentService> _logger;
    private readonly string _webhookSecret;

    public AdvertisementPaymentService(
        IUnitOfWorkRepository unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AdvertisementPaymentService> logger,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _webhookSecret = configuration["Stripe:WebhookSecret"] ?? string.Empty;
    }

    #region Stripe Checkout Session Payment

    public Result<string> CreateStripeSession(Guid advertisementId, int restaurantId)
    {
        try
        {
            // Get advertisement with plan
            var advertisement = _unitOfWork.Advertisements.GetOne(
                expression: a => a.Id == advertisementId && a.RestaurantId == restaurantId,
                includeProps: [a => a.AdvertisementPlan, a => a.Restaurant]);

            if (advertisement is null)
                return Result.Failure<string>(AdvertisementErrors.NotFound);

            if (advertisement.Status != AdvertisementStatus.PendingPayment)
                return Result.Failure<string>(AdvertisementErrors.NotPendingPayment);

            // Check if payment already exists and succeeded
            var existingPayment = _unitOfWork.AdvertisementPayments.GetOne(
                expression: p => p.AdvertisementId == advertisementId);

            if (existingPayment is not null && existingPayment.PaymentStatus == AdvertisementPaymentStatus.Succeeded)
                return Result.Failure<string>(AdvertisementErrors.AlreadyPaid);

            var amount = advertisement.AdvertisementPlan.PricePerMonth;
            var durationDays = (advertisement.EndDate - advertisement.StartDate).Days;

            // Build Stripe session
            var request = _httpContextAccessor.HttpContext!.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = ["card"],
                LineItems =
                [
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "EGP",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Advertisement: {advertisement.Title}",
                                Description = $"Plan: {advertisement.AdvertisementPlan.Name} | Duration: {durationDays} days | Start: {advertisement.StartDate:MMM dd, yyyy}"
                            },
                            UnitAmount = (long)Math.Ceiling(amount * 100),
                        },
                        Quantity = 1,
                    }
                ],
                Mode = "payment",
                SuccessUrl = $"{baseUrl}/RestaurantAdmin/Advertisements/FinishPayment?advertisementId={advertisementId}",
                CancelUrl = $"{baseUrl}/RestaurantAdmin/Advertisements/Payment/{advertisementId}",
                Metadata = new Dictionary<string, string>
                {
                    { "advertisement_id", advertisementId.ToString() },
                    { "restaurant_id", restaurantId.ToString() },
                    { "plan_id", advertisement.AdvertisementPlanId.ToString() }
                }
            };

            var service = new SessionService();
            var session = service.Create(options);

            // Create or update payment record
            if (existingPayment is null)
            {
                var payment = new AdvertisementPayment
                {
                    Id = Guid.NewGuid(),
                    AdvertisementId = advertisementId,
                    RestaurantId = restaurantId,
                    Amount = amount,
                    Currency = "EGP",
                    StripeSessionId = session.Id,
                    PaymentStatus = AdvertisementPaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                _unitOfWork.AdvertisementPayments.Add(payment);
            }
            else
            {
                existingPayment.StripeSessionId = session.Id;
                existingPayment.PaymentStatus = AdvertisementPaymentStatus.Pending;
                _unitOfWork.AdvertisementPayments.Update(existingPayment);
            }

            _unitOfWork.SaveChanges();

            _logger.LogInformation(
                "Stripe Checkout Session created for Advertisement {AdvertisementId}, SessionId: {SessionId}",
                advertisementId, session.Id);

            return Result.Success(session.Url!);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error creating session for Advertisement {AdvertisementId}", advertisementId);
            return Result.Failure<string>(AdvertisementErrors.StripeErrorWithMessage(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Stripe session for Advertisement {AdvertisementId}", advertisementId);
            return Result.Failure<string>(AdvertisementErrors.StripeSessionFailed);
        }
    }

    public Result FinishPayment(Guid advertisementId)
    {
        try
        {
            // Get payment record
            var payment = _unitOfWork.AdvertisementPayments.GetOne(
                expression: p => p.AdvertisementId == advertisementId);

            if (payment is null)
                return Result.Failure(AdvertisementErrors.PaymentNotFound);

            if (string.IsNullOrEmpty(payment.StripeSessionId))
                return Result.Failure(AdvertisementErrors.PaymentNotFound);

            // Retrieve Session from Stripe to verify payment
            var sessionService = new SessionService();
            var session = sessionService.Get(payment.StripeSessionId);

            if (session.PaymentStatus != "paid")
                return Result.Failure(AdvertisementErrors.PaymentNotSucceeded);

            // Update payment record
            payment.StripePaymentIntentId = session.PaymentIntentId;
            payment.PaymentStatus = AdvertisementPaymentStatus.Succeeded;
            payment.PaidAt = DateTime.UtcNow;

            // Get charge details for card info
            if (!string.IsNullOrEmpty(session.PaymentIntentId))
            {
                try
                {
                    var paymentIntentService = new PaymentIntentService();
                    var paymentIntent = paymentIntentService.Get(session.PaymentIntentId);
                    payment.StripeChargeId = paymentIntent.LatestChargeId;

                    if (!string.IsNullOrEmpty(paymentIntent.LatestChargeId))
                    {
                        var chargeService = new ChargeService();
                        var charge = chargeService.Get(paymentIntent.LatestChargeId);
                        if (charge?.PaymentMethodDetails?.Card is not null)
                        {
                            payment.CardLast4 = charge.PaymentMethodDetails.Card.Last4;
                            payment.CardBrand = charge.PaymentMethodDetails.Card.Brand;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to get card details for Advertisement {AdvertisementId}", advertisementId);
                }
            }

            _unitOfWork.AdvertisementPayments.Update(payment);

            // Update advertisement status to Pending (waiting for admin approval)
            var advertisement = _unitOfWork.Advertisements.GetOne(
                expression: a => a.Id == advertisementId);

            if (advertisement is not null)
            {
                advertisement.Status = AdvertisementStatus.Pending;
                advertisement.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Advertisements.Update(advertisement);
            }

            _unitOfWork.SaveChanges();

            _logger.LogInformation(
                "Payment completed for Advertisement {AdvertisementId}, SessionId: {SessionId}",
                advertisementId, payment.StripeSessionId);

            return Result.Success();
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error finishing payment for Advertisement {AdvertisementId}", advertisementId);
            return Result.Failure(AdvertisementErrors.StripeErrorWithMessage(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finishing payment for Advertisement {AdvertisementId}", advertisementId);
            return Result.Failure(AdvertisementErrors.StripeError);
        }
    }

    public async Task<Result> HandleStripeWebhook(string json, string signature)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json, signature, _webhookSecret);

            _logger.LogInformation("Received Stripe webhook event: {EventType}", stripeEvent.Type);

            switch (stripeEvent.Type)
            {
                case "checkout.session.completed":
                    await HandleCheckoutSessionCompleted(stripeEvent);
                    break;

                case "payment_intent.succeeded":
                    await HandlePaymentIntentSucceeded(stripeEvent);
                    break;

                case "payment_intent.payment_failed":
                    await HandlePaymentIntentFailed(stripeEvent);
                    break;

                case "charge.refunded":
                    await HandleChargeRefunded(stripeEvent);
                    break;

                default:
                    _logger.LogInformation("Unhandled webhook event type: {EventType}", stripeEvent.Type);
                    break;
            }

            return Result.Success();
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe webhook error");
            return Result.Failure(AdvertisementErrors.StripeErrorWithMessage(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook");
            return Result.Failure(AdvertisementErrors.StripeError);
        }
    }

    private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
    {
        var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
        if (session is null) return;

        var payment = _unitOfWork.AdvertisementPayments.GetOne(
            expression: p => p.StripeSessionId == session.Id);

        if (payment is null)
        {
            _logger.LogWarning("Payment not found for Checkout Session {SessionId}", session.Id);
            return;
        }

        if (session.PaymentStatus == "paid")
        {
            payment.StripePaymentIntentId = session.PaymentIntentId;
            payment.PaymentStatus = AdvertisementPaymentStatus.Succeeded;
            payment.PaidAt = DateTime.UtcNow;

            // Get charge details
            if (!string.IsNullOrEmpty(session.PaymentIntentId))
            {
                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.GetAsync(session.PaymentIntentId);
                payment.StripeChargeId = paymentIntent.LatestChargeId;

                if (!string.IsNullOrEmpty(paymentIntent.LatestChargeId))
                {
                    var chargeService = new ChargeService();
                    var charge = await chargeService.GetAsync(paymentIntent.LatestChargeId);
                    if (charge?.PaymentMethodDetails?.Card is not null)
                    {
                        payment.CardLast4 = charge.PaymentMethodDetails.Card.Last4;
                        payment.CardBrand = charge.PaymentMethodDetails.Card.Brand;
                    }
                }
            }

            _unitOfWork.AdvertisementPayments.Update(payment);

            // Update advertisement status
            var advertisement = _unitOfWork.Advertisements.GetOne(
                expression: a => a.Id == payment.AdvertisementId);
            if (advertisement is not null && advertisement.Status == AdvertisementStatus.PendingPayment)
            {
                advertisement.Status = AdvertisementStatus.Pending;
                advertisement.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Advertisements.Update(advertisement);
            }

            _unitOfWork.SaveChanges();

            _logger.LogInformation(
                "Webhook: Checkout session completed for Advertisement {AdvertisementId}",
                payment.AdvertisementId);
        }
    }

    private async Task HandlePaymentIntentSucceeded(Event stripeEvent)
    {
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        if (paymentIntent is null) return;

        var payment = _unitOfWork.AdvertisementPayments.GetOne(
            expression: p => p.StripePaymentIntentId == paymentIntent.Id);

        if (payment is null)
        {
            _logger.LogWarning("Payment not found for PaymentIntent {PaymentIntentId}", paymentIntent.Id);
            return;
        }

        payment.PaymentStatus = AdvertisementPaymentStatus.Succeeded;
        payment.PaidAt = DateTime.UtcNow;
        payment.StripeChargeId = paymentIntent.LatestChargeId;

        // Get card details
        if (!string.IsNullOrEmpty(paymentIntent.LatestChargeId))
        {
            var chargeService = new ChargeService();
            var charge = await chargeService.GetAsync(paymentIntent.LatestChargeId);
            if (charge?.PaymentMethodDetails?.Card is not null)
            {
                payment.CardLast4 = charge.PaymentMethodDetails.Card.Last4;
                payment.CardBrand = charge.PaymentMethodDetails.Card.Brand;
            }
        }

        _unitOfWork.AdvertisementPayments.Update(payment);

        // Update advertisement status
        var advertisement = _unitOfWork.Advertisements.GetOne(
            expression: a => a.Id == payment.AdvertisementId);
        if (advertisement is not null && advertisement.Status == AdvertisementStatus.PendingPayment)
        {
            advertisement.Status = AdvertisementStatus.Pending;
            advertisement.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Advertisements.Update(advertisement);
        }

        _unitOfWork.SaveChanges();

        _logger.LogInformation(
            "Webhook: Payment succeeded for Advertisement {AdvertisementId}",
            payment.AdvertisementId);
    }

    private async Task HandlePaymentIntentFailed(Event stripeEvent)
    {
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        if (paymentIntent is null) return;

        var payment = _unitOfWork.AdvertisementPayments.GetOne(
            expression: p => p.StripePaymentIntentId == paymentIntent.Id);

        if (payment is null)
        {
            _logger.LogWarning("Payment not found for PaymentIntent {PaymentIntentId}", paymentIntent.Id);
            return;
        }

        payment.PaymentStatus = AdvertisementPaymentStatus.Failed;
        _unitOfWork.AdvertisementPayments.Update(payment);
        _unitOfWork.SaveChanges();

        _logger.LogWarning(
            "Webhook: Payment failed for Advertisement {AdvertisementId}",
            payment.AdvertisementId);

        await Task.CompletedTask;
    }

    private async Task HandleChargeRefunded(Event stripeEvent)
    {
        var charge = stripeEvent.Data.Object as Charge;
        if (charge is null) return;

        var payment = _unitOfWork.AdvertisementPayments.GetOne(
            expression: p => p.StripeChargeId == charge.Id);

        if (payment is null)
        {
            _logger.LogWarning("Payment not found for Charge {ChargeId}", charge.Id);
            return;
        }

        payment.PaymentStatus = AdvertisementPaymentStatus.Refunded;
        payment.RefundedAt = DateTime.UtcNow;
        
        var refund = charge.Refunds?.Data?.FirstOrDefault();
        if (refund is not null)
        {
            payment.StripeRefundId = refund.Id;
        }

        _unitOfWork.AdvertisementPayments.Update(payment);
        _unitOfWork.SaveChanges();

        _logger.LogInformation(
            "Webhook: Refund processed for Advertisement {AdvertisementId}",
            payment.AdvertisementId);

        await Task.CompletedTask;
    }

    #endregion

    #region Refund

    public Result RefundPayment(Guid advertisementId, string reason)
    {
        try
        {
            var payment = _unitOfWork.AdvertisementPayments.GetOne(
                expression: p => p.AdvertisementId == advertisementId && 
                     p.PaymentStatus == AdvertisementPaymentStatus.Succeeded);

            if (payment is null)
                return Result.Failure(AdvertisementErrors.PaymentNotFound);

            if (string.IsNullOrEmpty(payment.StripeChargeId))
                return Result.Failure(AdvertisementErrors.RefundNotPossible);

            // Create refund in Stripe
            var refundOptions = new RefundCreateOptions
            {
                Charge = payment.StripeChargeId,
                Reason = RefundReasons.RequestedByCustomer,
                Metadata = new Dictionary<string, string>
                {
                    { "advertisement_id", advertisementId.ToString() },
                    { "reason", reason }
                }
            };

            var refundService = new RefundService();
            var refund = refundService.Create(refundOptions);

            // Update payment record
            payment.PaymentStatus = AdvertisementPaymentStatus.Refunded;
            payment.RefundedAt = DateTime.UtcNow;
            payment.RefundReason = reason;
            payment.StripeRefundId = refund.Id;

            _unitOfWork.AdvertisementPayments.Update(payment);
            _unitOfWork.SaveChanges();

            _logger.LogInformation(
                "Refund processed for Advertisement {AdvertisementId}, RefundId: {RefundId}",
                advertisementId, refund.Id);

            return Result.Success();
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error refunding payment for Advertisement {AdvertisementId}", advertisementId);
            return Result.Failure(AdvertisementErrors.StripeErrorWithMessage(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refunding payment for Advertisement {AdvertisementId}", advertisementId);
            return Result.Failure(AdvertisementErrors.StripeError);
        }
    }

    #endregion

    #region Restaurant Costs Dashboard

    public Result<RestaurantCostsResponse> GetRestaurantCosts(int restaurantId)
    {
        var restaurant = _unitOfWork.Restaurants.GetOne(
            expression: r => r.Id == restaurantId, tracked: false);
            
        if (restaurant is null)
            return Result.Failure<RestaurantCostsResponse>(AdvertisementErrors.RestaurantNotFound);

        var payments = _unitOfWork.AdvertisementPayments.Get(
            expression: p => p.RestaurantId == restaurantId,
            includeProps: [p => p.Advertisement],
            tracked: false)?
            .ToList() ?? [];

        var advertisements = _unitOfWork.Advertisements.Get(
            expression: a => a.RestaurantId == restaurantId,
            tracked: false)?
            .ToList() ?? [];

        var thisMonthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        var response = new RestaurantCostsResponse
        {
            RestaurantId = restaurantId,
            RestaurantName = restaurant.Name,
            
            TotalSpent = payments
                .Where(p => p.PaymentStatus == AdvertisementPaymentStatus.Succeeded)
                .Sum(p => p.Amount),
            
            ActiveAdsCost = payments
                .Where(p => p.PaymentStatus == AdvertisementPaymentStatus.Succeeded &&
                            p.Advertisement?.Status == AdvertisementStatus.Active)
                .Sum(p => p.Amount),
            
            ThisMonthSpent = payments
                .Where(p => p.PaymentStatus == AdvertisementPaymentStatus.Succeeded &&
                            p.PaidAt >= thisMonthStart)
                .Sum(p => p.Amount),
            
            TotalRefunded = payments
                .Where(p => p.PaymentStatus == AdvertisementPaymentStatus.Refunded)
                .Sum(p => p.Amount),
            
            TotalAdsCount = advertisements.Count,
            
            ActiveAdsCount = advertisements.Count(a => a.Status == AdvertisementStatus.Active),
            
            PendingAdsCount = advertisements.Count(a => 
                a.Status == AdvertisementStatus.Pending || 
                a.Status == AdvertisementStatus.PendingPayment),
            
            PaymentHistory = payments
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => MapToPaymentHistoryResponse(p))
                .ToList()
        };

        return Result.Success(response);
    }

    public List<PaymentHistoryResponse> GetPaymentHistory(int restaurantId)
    {
        var payments = _unitOfWork.AdvertisementPayments.Get(
            expression: p => p.RestaurantId == restaurantId,
            includeProps: [p => p.Advertisement, p => p.Advertisement.AdvertisementPlan],
            tracked: false)?
            .OrderByDescending(p => p.CreatedAt)
            .ToList() ?? [];

        return payments.Select(p => MapToPaymentHistoryResponse(p)).ToList();
    }

    private static PaymentHistoryResponse MapToPaymentHistoryResponse(AdvertisementPayment payment)
    {
        return new PaymentHistoryResponse
        {
            Id = payment.Id,
            AdvertisementId = payment.AdvertisementId,
            AdvertisementTitle = payment.Advertisement?.Title ?? string.Empty,
            PlanName = payment.Advertisement?.AdvertisementPlan?.Name ?? string.Empty,
            Amount = payment.Amount,
            Currency = payment.Currency,
            PaymentStatus = payment.PaymentStatus,
            CardLast4 = payment.CardLast4,
            CardBrand = payment.CardBrand,
            PaidAt = payment.PaidAt,
            RefundedAt = payment.RefundedAt,
            RefundReason = payment.RefundReason,
            CreatedAt = payment.CreatedAt
        };
    }

    #endregion

    #region Super Admin Revenue Dashboard

    public Result<RevenueReportResponse> GetRevenueReport(DateTime? startDate = null, DateTime? endDate = null)
    {
        // Default to all time if no dates specified
        var start = startDate ?? DateTime.MinValue;
        var end = endDate ?? DateTime.MaxValue;

        var payments = _unitOfWork.AdvertisementPayments.Get(
            expression: p => p.CreatedAt >= start && p.CreatedAt <= end,
            includeProps: [p => p.Advertisement, p => p.Advertisement.AdvertisementPlan, p => p.Restaurant],
            tracked: false)?
            .ToList() ?? [];

        var advertisements = _unitOfWork.Advertisements.Get(
            expression: a => a.CreatedAt >= start && a.CreatedAt <= end,
            tracked: false)?
            .ToList() ?? [];

        var thisMonthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var thisWeekStart = DateTime.UtcNow.AddDays(-(int)DateTime.UtcNow.DayOfWeek);

        var succeededPayments = payments
            .Where(p => p.PaymentStatus == AdvertisementPaymentStatus.Succeeded)
            .ToList();

        var response = new RevenueReportResponse
        {
            TotalRevenue = succeededPayments.Sum(p => p.Amount),
            
            ThisMonthRevenue = succeededPayments
                .Where(p => p.PaidAt >= thisMonthStart)
                .Sum(p => p.Amount),
            
            ThisWeekRevenue = succeededPayments
                .Where(p => p.PaidAt >= thisWeekStart)
                .Sum(p => p.Amount),
            
            TotalRefunds = payments
                .Where(p => p.PaymentStatus == AdvertisementPaymentStatus.Refunded)
                .Sum(p => p.Amount),
            
            TotalAdsCount = advertisements.Count,
            ActiveAdsCount = advertisements.Count(a => a.Status == AdvertisementStatus.Active),
            PendingAdsCount = advertisements.Count(a => a.Status == AdvertisementStatus.Pending),
            RejectedAdsCount = advertisements.Count(a => a.Status == AdvertisementStatus.Rejected),
            
            RevenueByPlan = GetRevenueByPlan(succeededPayments),
            TopRestaurants = GetTopRestaurants(succeededPayments, advertisements)
        };

        return Result.Success(response);
    }

    private static List<RevenueByPlanResponse> GetRevenueByPlan(List<AdvertisementPayment> payments)
    {
        var totalRevenue = payments.Sum(p => p.Amount);
        if (totalRevenue == 0) totalRevenue = 1; // Avoid division by zero

        var revenueByPlan = payments
            .Where(p => p.Advertisement?.AdvertisementPlan is not null)
            .GroupBy(p => new 
            { 
                p.Advertisement!.AdvertisementPlanId, 
                p.Advertisement.AdvertisementPlan!.Name 
            })
            .Select(g => new RevenueByPlanResponse
            {
                PlanId = g.Key.AdvertisementPlanId,
                PlanName = g.Key.Name,
                AdsCount = g.Count(),
                Revenue = g.Sum(p => p.Amount),
                PercentageOfTotal = (g.Sum(p => p.Amount) / totalRevenue) * 100
            })
            .OrderByDescending(r => r.Revenue)
            .ToList();

        return revenueByPlan;
    }

    private static List<TopRestaurantResponse> GetTopRestaurants(
        List<AdvertisementPayment> payments, 
        List<Advertisement> advertisements)
    {
        var topRestaurants = payments
            .Where(p => p.Restaurant is not null)
            .GroupBy(p => new 
            { 
                p.RestaurantId, 
                p.Restaurant!.Name, 
                p.Restaurant.Image 
            })
            .Select(g => new TopRestaurantResponse
            {
                RestaurantId = g.Key.RestaurantId,
                RestaurantName = g.Key.Name,
                RestaurantImage = g.Key.Image,
                TotalPaid = g.Sum(p => p.Amount),
                TotalAdsCount = advertisements.Count(a => a.RestaurantId == g.Key.RestaurantId),
                ActiveAdsCount = advertisements.Count(a => 
                    a.RestaurantId == g.Key.RestaurantId && 
                    a.Status == AdvertisementStatus.Active)
            })
            .OrderByDescending(r => r.TotalPaid)
            .Take(10)
            .ToList();

        return topRestaurants;
    }

    #endregion
}
