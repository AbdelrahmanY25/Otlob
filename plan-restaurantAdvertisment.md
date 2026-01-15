# \## 🎯 Restaurant Advertisement Feature - Full Implementation Plan

# 

# ---

# 

# \### 📋 Feature Overview

# 

# A comprehensive advertisement system allowing restaurants to promote their business through paid monthly subscriptions, with a complete approval workflow and management system for both Restaurant Admins and Super Admins.

# 

# ---

# 

# \### 🏗️ System Architecture

# 

# \#### 1. Advertisement Types (Like Talabat/Uber Eats)

# 

# | Ad Type | Description | Display Location | Priority |

# |---------|-------------|------------------|----------|

# | \*\*Featured Banner\*\* | Large banner at top of home page | Main Home Page Carousel | Highest |

# | \*\*Sponsored Listing\*\* | Restaurant appears at top of search results | Restaurant List / Search Results | High |

# | \*\*Category Spotlight\*\* | Featured in specific cuisine category | Category Pages | Medium |

# | \*\*Story/Promotion Ad\*\* | Instagram-style story ads | Home Page Stories Section | Medium |

# | \*\*Push Notification Ad\*\* | Promotional notifications to users | Customer Mobile/Web Notifications | High |

# 

# \#### 2. Subscription Plans

# 

# | Plan | Duration | Features | Price (Example) |

# |------|----------|----------|-----------------|

# | \*\*Basic\*\* | 1 Month | Sponsored Listing only | 500 L.E/month |

# | \*\*Standard\*\* | 1 Month | Sponsored Listing + Category Spotlight | 1,000 L.E/month |

# | \*\*Premium\*\* | 1 Month | All Ad Types + Priority Support + Analytics | 2,500 L.E/month |

# | \*\*Enterprise\*\* | 1 Month | Custom package + Dedicated Account Manager | Custom Pricing |

# 

# ---

# 

# \### 🗄️ Database Design

# 

# \#### New Entities

# 

# ```

# ┌─────────────────────────────────────────────────────────────────┐

# │                    AdvertisementPlan                            │

# ├─────────────────────────────────────────────────────────────────┤

# │ Id (PK)                                                         │

# │ Name (Basic/Standard/Premium/Enterprise)                        │

# │ Description                                                     │

# │ Price                                                           │

# │ DurationInDays (30)                                             │

# │ Features (JSON - list of allowed ad types)                      │

# │ MaxAdsPerMonth                                                  │

# │ IncludesAnalytics                                               │

# │ IncludesPrioritySupport                                         │

# │ IsActive                                                        │

# │ CreatedAt, UpdatedAt                                            │

# └─────────────────────────────────────────────────────────────────┘

# 

# ┌─────────────────────────────────────────────────────────────────┐

# │                 AdvertisementSubscription                       │

# ├─────────────────────────────────────────────────────────────────┤

# │ Id (PK)                                                         │

# │ RestaurantId (FK)                                               │

# │ PlanId (FK)                                                     │

# │ StartDate                                                       │

# │ EndDate                                                         │

# │ Status (Active/Expired/Cancelled/PendingPayment)                │

# │ AutoRenew                                                       │

# │ PaymentMethod                                                   │

# │ TotalPaid                                                       │

# │ CreatedAt, UpdatedAt                                            │

# └─────────────────────────────────────────────────────────────────┘

# 

# ┌─────────────────────────────────────────────────────────────────┐

# │                      Advertisement                              │

# ├─────────────────────────────────────────────────────────────────┤

# │ Id (PK)                                                         │

# │ RestaurantId (FK)                                               │

# │ SubscriptionId (FK)                                             │

# │ Title                                                           │

# │ Description                                                     │

# │ AdType (FeaturedBanner/SponsoredListing/CategorySpotlight/etc.) │

# │ ImageUrl                                                        │

# │ BannerImageUrl (for large banners)                              │

# │ TargetUrl (deep link to restaurant/offer)                       │

# │ StartDate                                                       │

# │ EndDate                                                         │

# │ Status (Draft/PendingApproval/Approved/Rejected/Active/Paused/  │

# │         Expired/Cancelled)                                      │

# │ Priority (display order)                                        │

# │ TargetAudience (JSON - location, preferences)                   │

# │ CategoryId (FK, nullable - for category spotlight)              │

# │ CreatedAt, UpdatedAt                                            │

# │ ApprovedAt, ApprovedBy                                          │

# │ RejectionReason                                                 │

# └─────────────────────────────────────────────────────────────────┘

# 

# ┌─────────────────────────────────────────────────────────────────┐

# │                   AdvertisementAnalytics                        │

# ├─────────────────────────────────────────────────────────────────┤

# │ Id (PK)                                                         │

# │ AdvertisementId (FK)                                            │

# │ Date                                                            │

# │ Impressions (times shown)                                       │

# │ Clicks                                                          │

# │ UniqueViews                                                     │

# │ OrdersGenerated                                                 │

# │ RevenueGenerated                                                │

# │ ClickThroughRate (calculated)                                   │

# │ ConversionRate (calculated)                                     │

# └─────────────────────────────────────────────────────────────────┘

# 

# ┌─────────────────────────────────────────────────────────────────┐

# │                 AdvertisementPayment                            │

# ├─────────────────────────────────────────────────────────────────┤

# │ Id (PK)                                                         │

# │ SubscriptionId (FK)                                             │

# │ Amount                                                          │

# │ PaymentMethod (Card/BankTransfer/Wallet)                        │

# │ TransactionId                                                   │

# │ Status (Pending/Completed/Failed/Refunded)                      │

# │ PaymentDate                                                     │

# │ InvoiceNumber                                                   │

# │ CreatedAt                                                       │

# └─────────────────────────────────────────────────────────────────┘

# ```

# 

# ---

# 

# \### 🔄 Workflow \& Status Flow

# 

# ```

# ┌─────────────────────────────────────────────────────────────────────────────┐

# │                        ADVERTISEMENT LIFECYCLE                               │

# └─────────────────────────────────────────────────────────────────────────────┘

# 

# Restaurant Admin                    Super Admin                    System

# &nbsp;     │                                  │                            │

# &nbsp;     │  1. Create Ad (Draft)            │                            │

# &nbsp;     ├─────────────────►                │                            │

# &nbsp;     │                                  │                            │

# &nbsp;     │  2. Submit for Approval          │                            │

# &nbsp;     ├──────────────────────────────────►                            │

# &nbsp;     │         (Status: PendingApproval)│                            │

# &nbsp;     │                                  │                            │

# &nbsp;     │                    3. Review Ad  │                            │

# &nbsp;     │                    ┌─────────────┤                            │

# &nbsp;     │                    │             │                            │

# &nbsp;     │         ┌──────────▼──────────┐  │                            │

# &nbsp;     │         │  Approve / Reject   │  │                            │

# &nbsp;     │         └──────────┬──────────┘  │                            │

# &nbsp;     │                    │             │                            │

# &nbsp;     │    ┌───────────────┴───────────────┐                          │

# &nbsp;     │    │                               │                          │

# &nbsp;     │    ▼                               ▼                          │

# &nbsp;     │ Approved                       Rejected                       │

# &nbsp;     │ (Status: Approved)             (Status: Rejected)             │

# &nbsp;     │    │                               │                          │

# &nbsp;     │    │                               │  Notification to         │

# &nbsp;     │    │                               │  Restaurant Admin        │

# &nbsp;     │    │                               ├─────────────────────────►│

# &nbsp;     │    │                                                          │

# &nbsp;     │    │  4. System checks StartDate                              │

# &nbsp;     │    ├──────────────────────────────────────────────────────────►

# &nbsp;     │    │                              (Background Job)            │

# &nbsp;     │    │                                                          │

# &nbsp;     │    ▼                                                          │

# &nbsp;     │ Active (Status: Active)                                       │

# &nbsp;     │ \[Displayed to Customers]                                      │

# &nbsp;     │    │                                                          │

# &nbsp;     │    │  5. Track Analytics                                      │

# &nbsp;     │    ├──────────────────────────────────────────────────────────►

# &nbsp;     │    │                                                          │

# &nbsp;     │    │  6. EndDate reached                                      │

# &nbsp;     │    ├──────────────────────────────────────────────────────────►

# &nbsp;     │    │                                                          │

# &nbsp;     │    ▼                                                          │

# &nbsp;     │ Expired (Status: Expired)                                     │

# &nbsp;     │                                                               │

# &nbsp;     │  \[Optional: Auto-renew subscription]                          │

# &nbsp;     └───────────────────────────────────────────────────────────────┘

# ```

# 

# ---

# 

# \### 📁 Project Structure

# 

# ```

# Otlob/

# ├── Areas/

# │   ├── RestaurantAdmin/

# │   │   ├── Controllers/

# │   │   │   ├── AdvertisementsController.cs

# │   │   │   └── AdvertisementSubscriptionsController.cs

# │   │   └── Views/

# │   │       ├── Advertisements/

# │   │       │   ├── Index.cshtml (List all ads)

# │   │       │   ├── Create.cshtml

# │   │       │   ├── Edit.cshtml

# │   │       │   ├── Details.cshtml

# │   │       │   └── Analytics.cshtml

# │   │       └── AdvertisementSubscriptions/

# │   │           ├── Plans.cshtml (View available plans)

# │   │           ├── Subscribe.cshtml

# │   │           ├── MySubscription.cshtml

# │   │           └── PaymentHistory.cshtml

# │   │

# │   ├── SuperAdmin/

# │   │   ├── Controllers/

# │   │   │   ├── AdvertisementsManagementController.cs

# │   │   │   ├── AdvertisementPlansController.cs

# │   │   │   └── AdvertisementAnalyticsController.cs

# │   │   └── Views/

# │   │       ├── AdvertisementsManagement/

# │   │       │   ├── Index.cshtml (All ads from all restaurants)

# │   │       │   ├── PendingApprovals.cshtml

# │   │       │   ├── Review.cshtml

# │   │       │   ├── Details.cshtml

# │   │       │   └── AllSubscriptions.cshtml

# │   │       ├── AdvertisementPlans/

# │   │       │   ├── Index.cshtml

# │   │       │   ├── Create.cshtml

# │   │       │   └── Edit.cshtml

# │   │       └── AdvertisementAnalytics/

# │   │           ├── Overview.cshtml (Platform-wide stats)

# │   │           ├── Revenue.cshtml

# │   │           └── PerformanceReport.cshtml

# │   │

# │   └── Customer/

# │       └── Views/

# │           └── Home/

# │               └── Index.cshtml (Display ads)

# │

# ├── IServices/

# │   ├── IAdvertisementService.cs

# │   ├── IAdvertisementSubscriptionService.cs

# │   ├── IAdvertisementPlanService.cs

# │   ├── IAdvertisementAnalyticsService.cs

# │   └── IAdvertisementPaymentService.cs

# │

# ├── Services/

# │   ├── AdvertisementService.cs

# │   ├── AdvertisementSubscriptionService.cs

# │   ├── AdvertisementPlanService.cs

# │   ├── AdvertisementAnalyticsService.cs

# │   └── AdvertisementPaymentService.cs

# │

# ├── Errors/

# │   └── AdvertisementErrors.cs

# │

# └── BackgroundJobs/

# &nbsp;   ├── AdvertisementStatusUpdaterJob.cs (Activate/Expire ads)

# &nbsp;   ├── SubscriptionRenewalJob.cs

# &nbsp;   └── AnalyticsAggregationJob.cs

# 

# RepositoryPatternWithUOW.Core/

# ├── Entities/

# │   ├── AdvertisementPlan.cs

# │   ├── AdvertisementSubscription.cs

# │   ├── Advertisement.cs

# │   ├── AdvertisementAnalytics.cs

# │   └── AdvertisementPayment.cs

# │

# ├── Contracts/

# │   └── Advertisement/

# │       ├── AdvertisementRequest.cs

# │       ├── AdvertisementResponse.cs

# │       ├── AdvertisementPlanResponse.cs

# │       ├── SubscriptionRequest.cs

# │       ├── SubscriptionResponse.cs

# │       ├── AdAnalyticsResponse.cs

# │       └── AdReviewRequest.cs

# │

# └── IBaseRepository/

# &nbsp;   ├── IAdvertisementRepository.cs

# &nbsp;   ├── IAdvertisementPlanRepository.cs

# &nbsp;   └── IAdvertisementSubscriptionRepository.cs

# 

# Utility/

# └── Enums/

# &nbsp;   ├── AdType.cs

# &nbsp;   ├── AdStatus.cs

# &nbsp;   ├── SubscriptionStatus.cs

# &nbsp;   └── AdPaymentStatus.cs

# ```

# 

# ---

# 

# \### 🎨 UI/UX Design

# 

# \#### Customer Home Page Layout

# 

# ```

# ┌─────────────────────────────────────────────────────────────────┐

# │  ┌───────────────────────────────────────────────────────────┐  │

# │  │         🎯 FEATURED BANNER CAROUSEL (Auto-slide)          │  │

# │  │    \[Premium Ads - Large Restaurant Promotions]            │  │

# │  └───────────────────────────────────────────────────────────┘  │

# │                                                                 │

# │  ┌─────┐ ┌─────┐ ┌─────┐ ┌─────┐ ┌─────┐                       │

# │  │Story│ │Story│ │Story│ │Story│ │Story│  ← Story Ads          │

# │  └─────┘ └─────┘ └─────┘ └─────┘ └─────┘                       │

# │                                                                 │

# │  ─────────────── Sponsored Restaurants ───────────────         │

# │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐               │

# │  │ 🏷️ SPONSORED │ │ 🏷️ SPONSORED │ │ 🏷️ SPONSORED │               │

# │  │  Restaurant  │ │  Restaurant  │ │  Restaurant  │               │

# │  │    Card      │ │    Card      │ │    Card      │               │

# │  └─────────────┘ └─────────────┘ └─────────────┘               │

# │                                                                 │

# │  ─────────────── All Restaurants ───────────────               │

# │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐               │

# │  │  Restaurant  │ │  Restaurant  │ │  Restaurant  │               │

# │  │    Card      │ │    Card      │ │    Card      │               │

# │  └─────────────┘ └─────────────┘ └─────────────┘               │

# │                                                                 │

# └─────────────────────────────────────────────────────────────────┘

# ```

# 

# \#### Restaurant Admin - Advertisement Dashboard

# 

# ```

# ┌─────────────────────────────────────────────────────────────────┐

# │  📊 Advertisement Dashboard                                     │

# ├─────────────────────────────────────────────────────────────────┤

# │                                                                 │

# │  ┌──────────────┐ ┌──────────────┐ ┌──────────────┐            │

# │  │ Active Ads   │ │ Impressions  │ │ Clicks       │            │

# │  │     3        │ │   12,450     │ │    892       │            │

# │  └──────────────┘ └──────────────┘ └──────────────┘            │

# │                                                                 │

# │  ┌─────────────────────────────────────────────────────────┐   │

# │  │ Current Subscription: Premium Plan                       │   │

# │  │ Valid Until: Feb 15, 2026 │ Auto-Renew: ON              │   │

# │  │ \[Manage Subscription] \[View Plans]                       │   │

# │  └─────────────────────────────────────────────────────────┘   │

# │                                                                 │

# │  \[+ Create New Advertisement]                                   │

# │                                                                 │

# │  ┌─────────────────────────────────────────────────────────┐   │

# │  │ My Advertisements                                        │   │

# │  ├─────────────────────────────────────────────────────────┤   │

# │  │ Title          │ Type      │ Status   │ Impressions │ ⚙️ │   │

# │  │ Summer Sale    │ Banner    │ ✅ Active │ 5,230      │ ...│   │

# │  │ Free Delivery  │ Sponsored │ ⏳ Pending│ -          │ ...│   │

# │  │ New Menu       │ Story     │ ❌ Rejected│ -         │ ...│   │

# │  └─────────────────────────────────────────────────────────┘   │

# │                                                                 │

# └─────────────────────────────────────────────────────────────────┘

# ```

# 

# \#### Super Admin - Approval Queue

# 

# ```

# ┌─────────────────────────────────────────────────────────────────┐

# │  🔍 Pending Advertisement Approvals (12)                        │

# ├─────────────────────────────────────────────────────────────────┤

# │                                                                 │

# │  ┌─────────────────────────────────────────────────────────┐   │

# │  │ 🍕 Pizza Palace - "50% Off Weekend Deal"                 │   │

# │  │ Type: Featured Banner │ Submitted: 2 hours ago          │   │

# │  │ ┌─────────────────────────────────────────────────────┐ │   │

# │  │ │          \[Ad Preview Image]                         │ │   │

# │  │ └─────────────────────────────────────────────────────┘ │   │

# │  │ Plan: Premium │ Subscription Valid: ✅                  │   │

# │  │                                                         │   │

# │  │ \[👁️ View Details] \[✅ Approve] \[❌ Reject]               │   │

# │  └─────────────────────────────────────────────────────────┘   │

# │                                                                 │

# │  ┌─────────────────────────────────────────────────────────┐   │

# │  │ 🍔 Burger King - "New Whopper Launch"                    │   │

# │  │ Type: Sponsored Listing │ Submitted: 5 hours ago        │   │

# │  │ ...                                                      │   │

# │  └─────────────────────────────────────────────────────────┘   │

# │                                                                 │

# └─────────────────────────────────────────────────────────────────┘

# ```

# 

# ---

# 

# \### 📝 Implementation Phases

# 

# \#### Phase 1: Foundation (Week 1-2)

# \- \[ ] Create database entities and migrations

# \- \[ ] Set up repository interfaces and implementations

# \- \[ ] Create enums (AdType, AdStatus, SubscriptionStatus)

# \- \[ ] Implement base services interfaces

# \- \[ ] Add error classes

# 

# \#### Phase 2: Subscription System (Week 2-3)

# \- \[ ] Implement AdvertisementPlanService (CRUD for plans)

# \- \[ ] Implement AdvertisementSubscriptionService

# \- \[ ] Create SuperAdmin views for managing plans

# \- \[ ] Create RestaurantAdmin views for subscribing to plans

# \- \[ ] Implement payment integration (if applicable)

# 

# \#### Phase 3: Advertisement Management (Week 3-4)

# \- \[ ] Implement AdvertisementService (Create, Edit, Submit)

# \- \[ ] Create RestaurantAdmin advertisement CRUD views

# \- \[ ] Implement image upload for ad banners

# \- \[ ] Add ad preview functionality

# 

# \#### Phase 4: Approval Workflow (Week 4-5)

# \- \[ ] Implement approval/rejection logic in SuperAdmin

# \- \[ ] Create pending approvals dashboard

# \- \[ ] Add notification system for status changes

# \- \[ ] Implement rejection reason feedback

# 

# \#### Phase 5: Customer Display (Week 5-6)

# \- \[ ] Modify Customer Home page to display ads

# \- \[ ] Implement ad rotation/carousel for banners

# \- \[ ] Add "Sponsored" badges to restaurant cards

# \- \[ ] Implement story ads display

# \- \[ ] Add click tracking

# 

# \#### Phase 6: Analytics \& Reporting (Week 6-7)

# \- \[ ] Implement AdvertisementAnalyticsService

# \- \[ ] Create analytics tracking middleware

# \- \[ ] Build RestaurantAdmin analytics dashboard

# \- \[ ] Build SuperAdmin platform-wide reports

# \- \[ ] Add revenue tracking

# 

# \#### Phase 7: Background Jobs \& Automation (Week 7-8)

# \- \[ ] Implement ad status updater job (activate/expire)

# \- \[ ] Implement subscription renewal job

# \- \[ ] Add auto-renewal functionality

# \- \[ ] Set up email notifications for expiring subscriptions

# 

# \#### Phase 8: Testing \& Polish (Week 8-9)

# \- \[ ] Unit testing for all services

# \- \[ ] Integration testing

# \- \[ ] UI/UX refinements

# \- \[ ] Performance optimization

# \- \[ ] Documentation

# 

# ---

# 

# \### 🔐 Security Considerations

# 

# 1\. \*\*Authorization\*\*: Restaurant can only manage their own ads

# 2\. \*\*Validation\*\*: Strict image/content validation before submission

# 3\. \*\*Rate Limiting\*\*: Prevent spam ad submissions

# 4\. \*\*Content Moderation\*\*: SuperAdmin review before activation

# 5\. \*\*Payment Security\*\*: Secure payment processing

# 

# ---

# 

# \### 📊 Key Metrics to Track

# 

# | Metric | Description |

# |--------|-------------|

# | \*\*Impressions\*\* | Number of times ad was displayed |

# | \*\*Clicks\*\* | Number of times ad was clicked |

# | \*\*CTR\*\* | Click-Through Rate (Clicks/Impressions) |

# | \*\*Conversions\*\* | Orders placed after clicking ad |

# | \*\*Revenue Generated\*\* | Total order value from ad clicks |

# | \*\*ROI\*\* | Return on Investment for restaurant |

# | \*\*Platform Revenue\*\* | Total subscription revenue |

# 

# ---

# 

# \### 💰 Revenue Model for Platform

# 

# ```

# Monthly Revenue = Σ (Active Subscriptions × Plan Price)

# 

# Example:

# \- 50 Basic Plans × 500 L.E = 25,000 L.E

# \- 30 Standard Plans × 1,000 L.E = 30,000 L.E  

# \- 20 Premium Plans × 2,500 L.E = 50,000 L.E

# ─────────────────────────────────────────────

# Total Monthly Revenue = 105,000 L.E

# ```

# 

# ---

# 

# \### 🚀 Next Steps

# 

# 1\. Review and refine this plan

# 2\. Prioritize which phases to implement first

# 3\. Begin with Phase 1: Foundation

# 4\. Iterate based on feedback



