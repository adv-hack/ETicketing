<Serializable()>
Public Class DEAgentPermissions

    Public Property IsAdministrator() As Boolean

    'Familiy Area Override
    Public Property CanOverrideFamilyAreaRestrictionCheck() As Boolean

    'Product Pre-Req Overrides
    Public Property CanOverrideProductPreReqCheck() As Boolean
    Public Property CanOverrideProductPreReqCheckFamilyArea() As Boolean

    'Ticket Limit Overrides
    Public Property CanOverrideMaxTicketCheck() As Boolean
    Public Property CanOverrideTransactionLimit() As Boolean
    Public Property CanOverrideProductLimitPerTransaction() As Boolean
    Public Property CanOverrideMembershipLimitPerTransaction() As Boolean
    Public Property CanOverrideStandMaxTicketLimit() As Boolean
    Public Property CanOverrideFreeSeatsLimit() As Boolean
    Public Property CanApplyTicketingPackageDiscounts() As Boolean

    'Refunds/Discounts
    Public Property CanGiveAdHocRefunds() As Boolean
    Public Property CanGiveDirectDebitRefund() As Boolean

    'CRM Authorities for Company/Contact
    Public Property CanAddCompany() As Boolean
    Public Property CanMaintainCompany() As Boolean
    Public Property CanAddContactToCompany() As Boolean
    Public Property CanRemoveContactFromCompany() As Boolean

    'Despatch Print Override
    Public Property CanPrintDespatchCards() As Boolean

    'CAT
    Public Property CanCancelTicket() As Boolean
    Public Property CanTransferTicket() As Boolean
    Public Property CanAmendTicket() As Boolean
    Public Property CanCancelPastDatedTickets() As Boolean

    'General Module Access
    Public Property CanAccessFriendsAndFamily() As Boolean
    Public Property CanAccessPPS() As Boolean
    Public Property CanAccessBuybacks() As Boolean
    Public Property CanAccessTicketExchange() As Boolean
    Public Property CanAccessECash() As Boolean
    Public Property CanAccessOnaccount() As Boolean
    Public Property CanAccessActivities() As Boolean
    Public Property CanAccessAttributes() As Boolean
    Public Property CanAccessEmergencyContactDetails() As Boolean
    Public Property CanAccessReservations() As Boolean
    Public Property CanAccessSystemDefaults() As Boolean
    Public Property CanAccessPurchaseHistory() As Boolean
    Public Property CanBuyWithInvoice() As Boolean
    Public Property CanAmendSystemAttributesForCustomers() As Boolean

    'Basket Additions and Amendments
    Public Property CanAddOrAmendCustomer() As Boolean
    Public Property CanCreateReservationFromBasket() As Boolean
    Public Property CanAddHomeGameToBasket() As Boolean
    Public Property CanAddAwayGameToBasket() As Boolean
    Public Property CanAddSeasonGameToBasket() As Boolean
    Public Property CanAddEventProductToBasket() As Boolean
    Public Property CanAddTravelProductToBasket() As Boolean
    Public Property CanAddMembershipsProductToBasket() As Boolean

    'Hospitality
    Public Property CanAccessHospitalityBookingEnquiry() As Boolean
    Public Property CanPrintHospitalityTickets() As Boolean
    'Public Property CanReserveHospitalityBookings() As Boolean 'have added this authority for hospitality reservation but it get conflicted in mixed basket scenario so commented out it
End Class
