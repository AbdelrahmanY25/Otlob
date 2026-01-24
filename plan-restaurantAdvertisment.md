\# 🎯 Full Plan: Restaurant Advertisement Feature with Monthly Subscription



\## 📋 Overview



This feature allows restaurants to create paid advertisements that appear on the customer's home page, increasing visibility and driving more orders. The system includes subscription management, approval workflows, and analytics.



---



\## 🏗️ Architecture Overview



```

┌─────────────────┐     ┌──────────────────┐     ┌─────────────────┐

│  Restaurant     │────▶│   Super Admin    │────▶│    Customer     │

│  Admin Panel    │     │   Approval       │     │   Home Page     │

│  (Create Ads)   │     │   (Review Ads)   │     │   (View Ads)    │

└─────────────────┘     └──────────────────┘     └─────────────────┘

&nbsp;        │                       │                        │

&nbsp;        └───────────────────────┴────────────────────────┘

&nbsp;                                │

&nbsp;                   ┌────────────▼────────────┐

&nbsp;                   │     Database Layer      │

&nbsp;                   │  (Ads, Subscriptions,   │

&nbsp;                   │   Placements, Analytics)│

&nbsp;                   └─────────────────────────┘

```



---



\## 📊 Database Schema Design



\### 1. \*\*AdvertisementPlan\*\* (Subscription Plans)

| Column | Type | Description |

|--------|------|-------------|

| Id | GUID | Primary Key |

| Name | string | "Basic", "Premium", "Featured" |

| NameAr | string | Arabic name |

| Description | string | Plan description |

| DescriptionAr | string | Arabic description |

| PricePerMonth | decimal | Monthly cost (e.g., 500, 1000, 2000 EGP) |

| DurationInDays | int | 30, 60, 90 days |

| MaxImpressions | int? | Impression limit (null = unlimited) |

| MaxClicks | int? | Click limit (null = unlimited) |

| PlacementPriority | int | 1=Highest, 10=Lowest |

| Features | string (JSON) | Additional features |

| IsActive | bool | Plan availability |

| CreatedAt | DateTime | |

| UpdatedAt | DateTime | |



\### 2. \*\*AdvertisementPlacement\*\* (Where Ads Appear)

| Column | Type | Description |

|--------|------|-------------|

| Id | GUID | Primary Key |

| Name | string | "HomePage\_Banner", "HomePage\_Carousel", "Restaurant\_List\_Top", "Search\_Results", "Category\_Page" |

| DisplayName | string | Human readable name |

| Description | string | Placement description |

| Dimensions | string | "1200x400", "600x300" |

| MaxAdsCount | int | Max ads per placement |

| IsActive | bool | |



\### 3. \*\*Advertisement\*\* (Main Ad Entity)

| Column | Type | Description |

|--------|------|-------------|

| Id | GUID | Primary Key |

| RestaurantId | GUID | FK to Restaurant |

| BranchId | GUID? | Optional specific branch |

| AdvertisementPlanId | GUID | FK to Plan |

| Title | string(100) | Ad title |

| TitleAr | string(100) | Arabic title |

| Description | string(500) | Ad description |

| DescriptionAr | string(500) | Arabic description |

| ImageUrl | string | Main ad image |

| MobileImageUrl | string | Mobile-optimized image |

| TargetUrl | string? | Deep link or URL |

| TargetType | enum | Restaurant, Branch, Meal, Category, PromoCode |

| TargetEntityId | GUID? | Target entity ID |

| Status | enum | Draft, PendingApproval, Approved, Rejected, Active, Paused, Expired, Cancelled |

| RejectionReason | string? | Why rejected |

| StartDate | DateTime | Campaign start |

| EndDate | DateTime | Campaign end |

| Budget | decimal? | Optional budget limit |

| SpentAmount | decimal | Amount spent |

| Priority | int | Display priority |

| CreatedAt | DateTime | |

| UpdatedAt | DateTime | |

| ApprovedAt | DateTime? | |

| ApprovedByUserId | GUID? | Super admin who approved |



\### 4. \*\*AdvertisementPlacementMapping\*\* (Ad-to-Placement)

| Column | Type | Description |

|--------|------|-------------|

| Id | GUID | Primary Key |

| AdvertisementId | GUID | FK |

| AdvertisementPlacementId | GUID | FK |

| Position | int? | Specific position |

| IsActive | bool | |



\### 5. \*\*AdvertisementSubscription\*\* (Payment/Subscription)

| Column | Type | Description |

|--------|------|-------------|

| Id | GUID | Primary Key |

| AdvertisementId | GUID | FK |

| RestaurantId | GUID | FK |

| AdvertisementPlanId | GUID | FK |

| SubscriptionStartDate | DateTime | |

| SubscriptionEndDate | DateTime | |

| Amount | decimal | Total amount |

| Currency | string | "EGP", "USD" |

| PaymentStatus | enum | Pending, Paid, Failed, Refunded |

| PaymentMethod | string | Card, Wallet, BankTransfer |

| TransactionId | string? | Payment gateway ref |

| InvoiceNumber | string | |

| AutoRenew | bool | Auto-renewal flag |

| RenewalCount | int | Times renewed |

| CreatedAt | DateTime | |

| UpdatedAt | DateTime | |



\### 6. \*\*AdvertisementAnalytics\*\* (Performance Tracking)

| Column | Type | Description |

|--------|------|-------------|

| Id | GUID | Primary Key |

| AdvertisementId | GUID | FK |

| Date | DateOnly | Analytics date |

| Impressions | int | Views count |

| Clicks | int | Click count |

| UniqueViews | int | Unique users |

| OrdersGenerated | int | Orders from ad |

| RevenueGenerated | decimal | Revenue from orders |

| CTR | decimal | Click-through rate |

| ConversionRate | decimal | Orders/Clicks |



\### 7. \*\*AdvertisementInteraction\*\* (Detailed Tracking)

| Column | Type | Description |

|--------|------|-------------|

| Id | GUID | Primary Key |

| AdvertisementId | GUID | FK |

| CustomerId | GUID? | FK (nullable for guests) |

| SessionId | string | Session tracking |

| InteractionType | enum | View, Click, Dismiss, Share |

| PlacementId | GUID | Where interaction occurred |

| DeviceType | string | Mobile, Web, iOS, Android |

| IPAddress | string? | For fraud detection |

| UserAgent | string? | |

| CreatedAt | DateTime | |



\### 8. \*\*AdvertisementTargeting\*\* (Audience Targeting)

| Column | Type | Description |

|--------|------|-------------|

| Id | GUID | Primary Key |

| AdvertisementId | GUID | FK |

| TargetingType | enum | Location, Age, OrderHistory, Cuisine, TimeOfDay |

| TargetingValue | string (JSON) | Targeting criteria |

| IsActive | bool | |



---



\## 🔄 Status Flow Diagram



```

┌─────────┐     ┌─────────────────┐     ┌──────────┐

│  Draft  │────▶│ PendingApproval │────▶│ Approved │

└─────────┘     └─────────────────┘     └──────────┘

&nbsp;                       │                     │

&nbsp;                       ▼                     ▼

&nbsp;                 ┌──────────┐          ┌──────────┐

&nbsp;                 │ Rejected │          │  Active  │

&nbsp;                 └──────────┘          └──────────┘

&nbsp;                       │                     │

&nbsp;                       ▼               ┌─────┴─────┐

&nbsp;                 ┌──────────┐          ▼           ▼

&nbsp;                 │  Draft   │    ┌─────────┐ ┌─────────┐

&nbsp;                 │(Re-edit) │    │ Paused  │ │ Expired │

&nbsp;                 └──────────┘    └─────────┘ └─────────┘

&nbsp;                                      │

&nbsp;                                      ▼

&nbsp;                                ┌───────────┐

&nbsp;                                │ Cancelled │

&nbsp;                                └───────────┘

```



---



\## 📁 Project Structure



\### Core Layer (Entities \& Contracts)

```

RepositoryPatternWithUOW.Core/

├── Entities/

│   └── Advertisements/

│       ├── Advertisement.cs

│       ├── AdvertisementPlan.cs

│       ├── AdvertisementPlacement.cs

│       ├── AdvertisementPlacementMapping.cs

│       ├── AdvertisementSubscription.cs

│       ├── AdvertisementAnalytics.cs

│       ├── AdvertisementInteraction.cs

│       └── AdvertisementTargeting.cs

│

├── Contracts/

│   └── Advertisements/

│       ├── IAdvertisementRepository.cs

│       ├── IAdvertisementPlanRepository.cs

│       ├── IAdvertisementSubscriptionRepository.cs

│       └── IAdvertisementAnalyticsRepository.cs

```



\### Utility Layer (Enums \& Constants)

```

Utility/

├── Enums/

│   └── Advertisements/

│       ├── AdvertisementStatus.cs

│       ├── AdvertisementTargetType.cs

│       ├── InteractionType.cs

│       ├── PaymentStatus.cs

│       └── TargetingType.cs

│

├── Consts/

│   └── Advertisements/

│       ├── AdvertisementConstants.cs

│       └── PlacementTypes.cs

```



\### EF Layer (Configurations \& Repositories)

```

RepositoryPatternWithUOW.EF/

├── Configurations/

│   └── Advertisements/

│       ├── AdvertisementConfiguration.cs

│       ├── AdvertisementPlanConfiguration.cs

│       ├── AdvertisementPlacementConfiguration.cs

│       ├── AdvertisementSubscriptionConfiguration.cs

│       └── AdvertisementAnalyticsConfiguration.cs

│

├── BaseRepository/

│   └── Advertisements/

│       ├── AdvertisementRepository.cs

│       ├── AdvertisementPlanRepository.cs

│       ├── AdvertisementSubscriptionRepository.cs

│       └── AdvertisementAnalyticsRepository.cs

```



\### Main Otlob Project (Services, Controllers, Views)

```

Otlob/

├── Errors/

│   └── AdvertisementErrors.cs

│

├── IServices/

│   └── IAdvertisementService.cs

│   └── IAdvertisementSubscriptionService.cs

│   └── IAdvertisementAnalyticsService.cs

│

├── Services/

│   └── AdvertisementService.cs

│   └── AdvertisementSubscriptionService.cs

│   └── AdvertisementAnalyticsService.cs

│

├── Areas/

│   ├── RestaurantAdmin/

│   │   ├── Controllers/

│   │   │   └── AdvertisementController.cs

│   │   └── Views/

│   │       └── Advertisement/

│   │           ├── Index.cshtml (List all ads)

│   │           ├── Create.cshtml (Create new ad)

│   │           ├── Edit.cshtml (Edit ad)

│   │           ├── Details.cshtml (View ad details)

│   │           ├── Analytics.cshtml (View performance)

│   │           ├── Plans.cshtml (View available plans)

│   │           └── Subscription.cshtml (Manage subscription)

│   │

│   ├── SuperAdmin/

│   │   ├── Controllers/

│   │   │   └── AdvertisementManagementController.cs

│   │   └── Views/

│   │       └── AdvertisementManagement/

│   │           ├── Index.cshtml (All ads dashboard)

│   │           ├── PendingApprovals.cshtml

│   │           ├── Approve.cshtml (Review \& approve)

│   │           ├── Plans.cshtml (Manage plans)

│   │           ├── Placements.cshtml (Manage placements)

│   │           ├── Analytics.cshtml (Overall analytics)

│   │           └── Revenue.cshtml (Revenue reports)

│   │

│   └── Customer/

│       └── Controllers/

│           └── HomeController.cs (Modified to show ads)

│

├── ViewModels/

│   └── Advertisements/

│       ├── AdvertisementViewModel.cs

│       ├── CreateAdvertisementViewModel.cs

│       ├── EditAdvertisementViewModel.cs

│       ├── AdvertisementPlanViewModel.cs

│       ├── AdvertisementAnalyticsViewModel.cs

│       ├── ApproveAdvertisementViewModel.cs

│       └── CustomerAdvertisementViewModel.cs

```



---



\## 🎨 Feature Breakdown



\### 1. \*\*Advertisement Plans (Packages)\*\*



| Plan | Price/Month | Features |

|------|-------------|----------|

| \*\*Basic\*\* | 500 EGP | 1 placement, 10K impressions, Standard position |

| \*\*Premium\*\* | 1,000 EGP | 3 placements, 50K impressions, Priority position |

| \*\*Featured\*\* | 2,000 EGP | All placements, Unlimited impressions, Top position, Analytics dashboard |

| \*\*Enterprise\*\* | Custom | Custom deal, Dedicated support, Advanced targeting |



\### 2. \*\*Placement Locations\*\*



| Placement | Description | Dimensions |

|-----------|-------------|------------|

| `HomePage\_Hero\_Banner` | Main banner at top of home | 1200x400 |

| `HomePage\_Carousel` | Rotating carousel | 800x400 |

| `Restaurant\_List\_Sponsored` | "Sponsored" in restaurant list | 400x200 |

| `Search\_Results\_Top` | Top of search results | 400x150 |

| `Category\_Page\_Banner` | Category page header | 1000x300 |

| `Checkout\_Suggestions` | Checkout page suggestions | 300x200 |



\### 3. \*\*Targeting Options\*\*



\- \*\*Location-based\*\*: Target specific areas/cities

\- \*\*Time-based\*\*: Show ads during specific hours (lunch, dinner)

\- \*\*User behavior\*\*: Based on order history, cuisine preferences

\- \*\*New users\*\*: Target first-time customers

\- \*\*Re-engagement\*\*: Target inactive users



---



\## 🔌 API Endpoints



\### Restaurant Admin APIs

```

POST   /api/restaurant-admin/advertisements                    - Create advertisement

GET    /api/restaurant-admin/advertisements                    - List my advertisements

GET    /api/restaurant-admin/advertisements/{id}               - Get advertisement details

PUT    /api/restaurant-admin/advertisements/{id}               - Update advertisement

DELETE /api/restaurant-admin/advertisements/{id}               - Delete/Cancel advertisement

POST   /api/restaurant-admin/advertisements/{id}/submit        - Submit for approval

POST   /api/restaurant-admin/advertisements/{id}/pause         - Pause advertisement

POST   /api/restaurant-admin/advertisements/{id}/resume        - Resume advertisement



GET    /api/restaurant-admin/advertisement-plans               - Get available plans

POST   /api/restaurant-admin/advertisement-subscriptions       - Create subscription

GET    /api/restaurant-admin/advertisement-subscriptions       - List my subscriptions

POST   /api/restaurant-admin/advertisement-subscriptions/{id}/renew - Renew subscription

POST   /api/restaurant-admin/advertisement-subscriptions/{id}/cancel - Cancel subscription



GET    /api/restaurant-admin/advertisements/{id}/analytics     - Get ad analytics

GET    /api/restaurant-admin/advertisements/analytics/summary  - Get overall summary

```



\### Super Admin APIs

```

GET    /api/super-admin/advertisements                         - List all advertisements

GET    /api/super-admin/advertisements/pending                 - List pending approvals

POST   /api/super-admin/advertisements/{id}/approve            - Approve advertisement

POST   /api/super-admin/advertisements/{id}/reject             - Reject advertisement

PUT    /api/super-admin/advertisements/{id}/priority           - Update priority

DELETE /api/super-admin/advertisements/{id}                    - Force remove ad



\# Plan Management

GET    /api/super-admin/advertisement-plans                    - List all plans

POST   /api/super-admin/advertisement-plans                    - Create plan

PUT    /api/super-admin/advertisement-plans/{id}               - Update plan

DELETE /api/super-admin/advertisement-plans/{id}               - Delete plan



\# Placement Management

GET    /api/super-admin/advertisement-placements               - List placements

POST   /api/super-admin/advertisement-placements               - Create placement

PUT    /api/super-admin/advertisement-placements/{id}          - Update placement



\# Analytics \& Revenue

GET    /api/super-admin/advertisements/analytics               - Overall analytics

GET    /api/super-admin/advertisements/revenue                 - Revenue reports

GET    /api/super-admin/advertisements/revenue/by-restaurant   - Revenue by restaurant

```



\### Customer APIs

```

GET    /api/customer/advertisements/home                       - Get home page ads

GET    /api/customer/advertisements/placement/{placementName}  - Get ads for placement

POST   /api/customer/advertisements/{id}/click                 - Track click

POST   /api/customer/advertisements/{id}/impression            - Track impression

```



---



\## 💼 Business Logic \& Services



\### IAdvertisementService Methods

```csharp

// CRUD Operations

Task<Result<AdvertisementDto>> CreateAsync(CreateAdvertisementRequest request);

Task<Result<AdvertisementDto>> UpdateAsync(Guid id, UpdateAdvertisementRequest request);

Task<Result> DeleteAsync(Guid id);

Task<Result<AdvertisementDto>> GetByIdAsync(Guid id);

Task<Result<PaginatedList<AdvertisementDto>>> GetByRestaurantAsync(Guid restaurantId, AdvertisementFilter filter);



// Status Management

Task<Result> SubmitForApprovalAsync(Guid id);

Task<Result> ApproveAsync(Guid id, Guid approvedByUserId);

Task<Result> RejectAsync(Guid id, string reason);

Task<Result> PauseAsync(Guid id);

Task<Result> ResumeAsync(Guid id);

Task<Result> ExpireAsync(Guid id);



// Customer Facing

Task<Result<List<CustomerAdvertisementDto>>> GetActiveAdsForPlacementAsync(string placementName, AdvertisementContext context);

Task<Result<List<CustomerAdvertisementDto>>> GetHomePageAdsAsync(Guid? customerId, string? location);



// Tracking

Task<Result> TrackImpressionAsync(Guid advertisementId, TrackingRequest request);

Task<Result> TrackClickAsync(Guid advertisementId, TrackingRequest request);

```



\### IAdvertisementSubscriptionService Methods

```csharp

Task<Result<SubscriptionDto>> CreateSubscriptionAsync(CreateSubscriptionRequest request);

Task<Result> ProcessPaymentAsync(Guid subscriptionId, PaymentRequest request);

Task<Result> RenewSubscriptionAsync(Guid subscriptionId);

Task<Result> CancelSubscriptionAsync(Guid subscriptionId);

Task<Result<List<SubscriptionDto>>> GetByRestaurantAsync(Guid restaurantId);

Task ProcessExpiredSubscriptionsAsync(); // Background job

Task ProcessAutoRenewalsAsync(); // Background job

```



\### IAdvertisementAnalyticsService Methods

```csharp

Task<Result<AdvertisementAnalyticsDto>> GetAnalyticsAsync(Guid advertisementId, DateRange range);

Task<Result<RestaurantAdsAnalyticsSummary>> GetRestaurantSummaryAsync(Guid restaurantId, DateRange range);

Task<Result<OverallAdsAnalytics>> GetOverallAnalyticsAsync(DateRange range); // Super Admin

Task<Result<RevenueReport>> GetRevenueReportAsync(DateRange range); // Super Admin

Task AggregateAnalyticsAsync(DateOnly date); // Background job

```



---



\## 📅 Implementation Phases



\### \*\*Phase 1: Foundation (Week 1-2)\*\*

1\. Create all entity classes

2\. Create enums and constants

3\. Create EF configurations

4\. Create migration

5\. Create repositories

6\. Update Unit of Work



\### \*\*Phase 2: Core Services (Week 2-3)\*\*

1\. Implement `AdvertisementService`

2\. Implement `AdvertisementSubscriptionService`

3\. Implement `AdvertisementAnalyticsService`

4\. Create error classes

5\. Write unit tests



\### \*\*Phase 3: Restaurant Admin Panel (Week 3-4)\*\*

1\. Create controller \& views for ad management

2\. Create subscription management UI

3\. Create analytics dashboard

4\. Image upload functionality

5\. Plan selection UI



\### \*\*Phase 4: Super Admin Panel (Week 4-5)\*\*

1\. Create approval workflow UI

2\. Create plan management UI

3\. Create placement management UI

4\. Create revenue reporting dashboard

5\. Create overall analytics dashboard



\### \*\*Phase 5: Customer Experience (Week 5-6)\*\*

1\. Modify home page to display ads

2\. Implement ad carousel component

3\. Implement sponsored restaurant badges

4\. Implement click/impression tracking

5\. A/B testing setup



\### \*\*Phase 6: Payment \& Automation (Week 6-7)\*\*

1\. Integrate payment gateway

2\. Create subscription payment flow

3\. Implement auto-renewal logic

4\. Create background jobs for:

&nbsp;  - Expiring ads

&nbsp;  - Processing renewals

&nbsp;  - Aggregating analytics



\### \*\*Phase 7: Testing \& Optimization (Week 7-8)\*\*

1\. Integration testing

2\. Performance optimization

3\. Caching implementation

4\. Load testing

5\. Security audit



---



\## 🔒 Security Considerations



1\. \*\*Authorization\*\*: Ensure restaurants can only manage their own ads

2\. \*\*Rate Limiting\*\*: Prevent impression/click fraud

3\. \*\*IP Tracking\*\*: Detect and block fraudulent activity

4\. \*\*Image Validation\*\*: Validate uploaded images for malware

5\. \*\*Payment Security\*\*: PCI compliance for payment processing

6\. \*\*Data Privacy\*\*: GDPR compliance for tracking data



---



\## 📈 Analytics \& Reporting



\### Restaurant Dashboard Metrics

\- Total Impressions

\- Total Clicks

\- Click-Through Rate (CTR)

\- Orders Generated

\- Revenue Generated

\- Cost Per Click (CPC)

\- Return on Ad Spend (ROAS)



\### Super Admin Dashboard Metrics

\- Total Active Ads

\- Pending Approvals

\- Total Revenue

\- Revenue by Plan

\- Revenue by Restaurant

\- Top Performing Ads

\- Placement Performance



---



\## 🔧 Background Jobs (Hangfire/Quartz)



```csharp

// Daily Jobs

\[Schedule("0 0 \* \* \*")] // Midnight

public class ExpireAdvertisementsJob { }



\[Schedule("0 1 \* \* \*")] // 1 AM

public class AggregateAnalyticsJob { }



\[Schedule("0 6 \* \* \*")] // 6 AM

public class ProcessAutoRenewalsJob { }



// Hourly Jobs

\[Schedule("0 \* \* \* \*")]

public class UpdateAdStatusJob { } // Activate/deactivate based on schedule



// Weekly Jobs

\[Schedule("0 0 \* \* 0")] // Sunday midnight

public class GenerateWeeklyReportsJob { }

```



---



\## 🎨 UI/UX Mockup Descriptions



\### Customer Home Page

```

┌────────────────────────────────────────────────┐

│  🎯 HERO BANNER AD (Full Width Carousel)       │

│  \[Restaurant Special Offer - 30% Off!]         │

└────────────────────────────────────────────────┘



│ Categories: 🍕 🍔 🍜 🥗 🍰                      │



┌────────────────────────────────────────────────┐

│ 📍 Sponsored Restaurants                       │

│ ┌──────┐ ┌──────┐ ┌──────┐                    │

│ │ Ad 1 │ │ Ad 2 │ │ Ad 3 │  ← Horizontal     │

│ │ ⭐   │ │ ⭐   │ │ ⭐   │    Scroll          │

│ └──────┘ └──────┘ └──────┘                    │

└────────────────────────────────────────────────┘



│ 🏪 Restaurants Near You                        │

│ \[Sponsored Badge] Restaurant A - ⭐ 4.5        │

│ Restaurant B - ⭐ 4.2                          │

│ Restaurant C - ⭐ 4.8                          │

```



\### Restaurant Admin - Create Ad

```

┌─────────────────────────────────────────────────┐

│ Create New Advertisement                         │

├─────────────────────────────────────────────────┤

│ Step 1: Choose Plan                             │

│ ○ Basic (500 EGP/mo)                            │

│ ● Premium (1000 EGP/mo) ✓                       │

│ ○ Featured (2000 EGP/mo)                        │

├─────────────────────────────────────────────────┤

│ Step 2: Ad Content                              │

│ Title: \[\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_]            │

│ Description: \[\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_]             │

│ Image: \[Upload] 📷                              │

│ Target: ○ Restaurant ○ Specific Meal ○ Promo   │

├─────────────────────────────────────────────────┤

│ Step 3: Schedule                                │

│ Start Date: \[📅] End Date: \[📅]                │

│ ☑ Auto-renew subscription                      │

├─────────────────────────────────────────────────┤

│ Step 4: Review \& Submit                         │

│ \[Preview] \[Save Draft] \[Submit for Approval]   │

└─────────────────────────────────────────────────┘

```



---



\## ✅ Summary Checklist



\- \[ ] \*\*Entities\*\*: 8 new entities

\- \[ ] \*\*Enums\*\*: 5 new enums

\- \[ ] \*\*Repositories\*\*: 4 new repositories

\- \[ ] \*\*Services\*\*: 3 new services

\- \[ ] \*\*Controllers\*\*: 2 new controllers (RestaurantAdmin, SuperAdmin)

\- \[ ] \*\*Views\*\*: ~15 new views

\- \[ ] \*\*ViewModels\*\*: ~10 new view models

\- \[ ] \*\*APIs\*\*: ~25 new endpoints

\- \[ ] \*\*Background Jobs\*\*: 5 scheduled jobs

\- \[ ] \*\*Migrations\*\*: 1 migration with 8 tables



---



\## 🚀 Next Steps



1\. \*\*Creating the entity classes\*\* in the Core layer

2\. \*\*Creating the enums\*\* in the Utility layer

3\. \*\*Creating the EF configurations\*\* and migration

4\. \*\*Creating the services and interfaces\*\*

5\. \*\*Creating the controllers and views\*\*



