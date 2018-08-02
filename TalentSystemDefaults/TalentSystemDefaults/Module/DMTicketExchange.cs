using System.Collections.Generic;
namespace TalentSystemDefaults
{
    namespace TalentModules
    {
        public class DMTicketExchange : DMBase
        {
            static private bool _EnableAsModule = true;
            public static bool EnableAsModule
            {
                get
                {
                    return _EnableAsModule;
                }
                set
                {
                    _EnableAsModule = value;
                }
            }
            static private string _ModuleTitle = "Ticket Exchange";
            public static string ModuleTitle
            {
                get
                {
                    return _ModuleTitle;
                }
                set
                {
                    _ModuleTitle = value;
                }
            }

            public const string XF010B = "0rrveQOP-71W5VQOi-arWvewq-arXBE9";
            public const string XF014B = "0rrveQOP-71o5Vpui-arWvewq-arXBEa";
            public const string XF015B = "0rrveQOP-71X5Vfyi-arWvewq-arXBE0";
            public const string SHOW_TE_PROGRESS_BAR = "jlefG3WV-1dGAcBjk-arWvefh-arXP09";
            public const string XF016B = "0rrveQOP-71L5V2hi-arWvef8-arXBA2";
            public const string VL001B = "0rrveQOP-K9N5Jw3x-arWvewh-arcKCf";
            public const string VL002B = "0rrveQOP-K9Y5Jmqx-arWvewh-arcKC2";
            public const string VL003B = "0rrveQOP-K9c5J6dx-arWvewh-arcKC9";
            public const string XF013B = "0rrveQOP-71c5V6di-arWvef8-arXjRp";

            // Customer Ticket Exchange Summary
            public const string ltlTicketingExchangeProductsHeader = "rOxfXOxB-cVa2GDwT-arWvRQq-arXjRa";
            public const string ltlSummaryLine1 = "rOxfXOxB-cl2EHOo9-arWPE9u-arXjZw";
            public const string ltlSummaryLine2 = "rOxfXOxB-cl2EHOo9-arWPAQ8-arXjZm";
            public const string ltlSummaryLine3 = "rOxfXOxB-cl2EHOo9-arWPEQu-arXjZ6";

            // Customer Product List Page
            public const string DefaultDaysHistory = "rOEEXOfK-QjbNG10c-arWve2y-arXTC0";
            public const string ShowCompetitionDescription = "rOEEXOfK-Jlj6OgzG-arWve9y-arXjeQ";
            public const string ShowProductDate = "rOEEXOfK-JAE1thL#-arWve2u-arXjew";
            public const string ShowProductDescription = "rOEEXOfK-JAE168zF-arWve93-arXjem";
            public const string ShowTotalHandlingFeePending = "rOEEXOfK-JjvrGvlB-arWve9h-arXje6";
            public const string ShowTotalHandlingFeeSold = "rOEEXOfK-JjvrGVSX-arWve9d-arXjep";
            public const string ShowTotalPendingOnTicketExchange = "rOEEXOfK-Jjxr6IxL-arWvea3-arXjef";
            public const string ShowTotalPurchased = "rOEEXOfK-JjzQm7fE-arWve28-arXje2";
            public const string ShowTotalPurchasedPrice = "rOEEXOfK-JjzQOIzB-arWve9q-arXje9";
            public const string ShowTotalResalePricePending = "rOEEXOfK-Jjx#GvlZ-arWve9h-arXjea";
            public const string ShowTotalResalePriceSold = "rOEEXOfK-Jjx#GVSX-arWve9d-arXje0";
            public const string ShowTotalSoldOnTicketExchange = "rOEEXOfK-JjpkGAoL-arWve9r-arXjRQ";
            public const string ShowTotalWithCustomer = "rOEEXOfK-Jjj6Gqxd-arWve9O-arXjRw";
            public const string ProductDateHeader = "rOxfXOxB-WjxN5Lw3-arWve2h-arXjRf";
            public const string ActionHeader = "rOxfXOxB-jfxQGIzY-arWve2d-arXjR2";
            public const string ltlSelect = "rOxfXOxB-cOEkdIL9-arWve2O-arXjR9";
            public const string TotalPendingOnTicketExchangeHeader = "rOxfXOxB-BOa2dLwT-arWveah-arXjR0";
            public const string ltlFromDate = "rOxfXOxB-clxkQVUK-arWve9u-arXjMQ";
            public const string TotalHandlingFeeSoldHeaderinfo = "rOxfXOxB-BWjr2OiB-arWvRQy-arXjMw";
            public const string btnFromDateText = "rOxfXOxB-clxlOypv-arWve2h-arXjMm";
            public const string TotalWithCustomerHeader = "rOxfXOxB-Bcz#QxGG-arWveah-arXjM6";
            public const string ActionErrorDesc_NA = "rOxfXOxB-jde1gD4X-arWvRQy-arXjMp";
            public const string TotalResalePriceSoldHeaderinfo = "rOxfXOxB-BOxi2OiB-arWvRQr-arXjMf";
            public const string TotalSoldOnTicketExchangeHeaderinfo = "rOxfXOxB-BAalgvJQ-arWvRw3-arXjM2";
            public const string TotalPendingOnTicketExchangeHeaderinfo = "rOxfXOxB-BOa2dLwQ-arWvRwh-arXjM9";
            public const string TotalWithCustomerHeaderinfo = "rOxfXOxB-Bcz#QxzB-arWvRQr-arXjMa";
            public const string CompetitionDescriptionHeaderInfo = "rOxfXOxB-Bca16RDa-arWvear-arXjM0";
            public const string ProductDescriptionHeaderInfo = "rOxfXOxB-Wj6QGIjQ-arWveau-arXjEQ";
            public const string ProductDateHeaderinfo = "rOxfXOxB-WjxNwxNY-arWvea8-arXjEw";
            public const string TotalPurchasedHeaderInfo = "rOxfXOxB-BDv#O3sQ-arWve0q-arXjE6";
            public const string TotalPurchasedHeader = "rOxfXOxB-BDv#OgKE-arWveaO-arXjEp";
            public const string TotalPurchasedPriceHeaderInfo = "rOxfXOxB-BDv#PhzL-arWvRQO-arXjEf";
            public const string TotalResalePricePendingHeaderinfo = "rOxfXOxB-BOxi2J4G-arWvRwa-arXjE2";
            public const string TotalHandlingFeePendingHeaderinfo = "rOxfXOxB-BWjr2J4T-arWvRwu-arXjE9";
            public const string ActionHeaderInfo = "rOxfXOxB-jfxQ6RDQ-arWve9u-arXBEf";
            public const string ActionErrorDesc_PD = "rOxfXOxB-jde1gD4X-arWvRwa-arXjEa";
            public const string CompetitionDescriptionHeader = "rOxfXOxB-Bca16RQf-arWveau-arXjE0";
            public const string ltlNotAvailable = "rOxfXOxB-cuSe5Gs9-arWve9d-arXjAQ";
            public const string ltlSellYourTickets = "rOxfXOxB-c7eldI4y-arWveaO-arXjAw";
            public const string ProductDescriptionHeader = "rOxfXOxB-Wj6QGIfY-arWve9h-arXjAf";
            public const string TotalHandlingFeePendingHeader = "rOxfXOxB-BWjr2J4E-arWvRQq-arXjA2";
            public const string TotalHandlingFeeSoldHeader = "rOxfXOxB-BWjr2OLY-arWvea3-arXjA9";
            public const string TotalPurchasedPriceHeader = "rOxfXOxB-BDv#PhxB-arWve0h-arXjAa";
            public const string TotalResalePricePendingHeader = "rOxfXOxB-BOxi2J4E-arWve0u-arXjA0";
            public const string TotalResalePriceSoldHeader = "rOxfXOxB-BOxi2OLY-arWvead-arXj1Q";
            public const string TotalSoldOnTicketExchangeHeader = "rOxfXOxB-BAalgvJn-arWvear-arXj1w";

            // Ticket Exchange Process Pages
            public const string ltlTicketExchangeSelectionHeader = "rOxfXOxB-cVIdUgx4-arWvR6r-arXBE6";
            public const string ShowClubFee = "rOEEXOfK-JDxsV8fF-arWve23-arXjZp";
            public const string ShowCustomer = "rOEEXOfK-J8xQdIQV-arWve2q-arXjZf";
            public const string ShowFaceValue = "rOEEXOfK-JESXgOzZ-arWve2d-arXjZ2";
            public const string ShowOwner = "rOEEXOfK-J0TumBl#-arWvefa-arXjZ9";
            public const string ShowPaymentRef = "rOEEXOfK-JPEtQIbd-arWve2u-arXjZa";
            public const string ShowSaleprice = "rOEEXOfK-J7jdUxz9-arWve2d-arXjZ0";
            public const string ShowSeat = "rOEEXOfK-JWpcjObB-arWvefr-arXjsQ";
            public const string ShowTicketExchangeId = "rOEEXOfK-JEgo2DoL-arWve9O-arXjsw";
            public const string ShowYouWillEarn = "rOEEXOfK-JDSLl44#-arWve2y-arXjsm";
            public const string SliderStepNumber = "rOEEXOfK-FUwX28Uv-arWve2h-arXjs6";
            public const string SeatedCustomerHeader = "rOxfXOxB-Qap7OO0Y-arWve9u-arXjsp";
            public const string SeatHeader = "rOxfXOxB-QWV4OOoB-arWve2O-arXjsf";
            public const string StatusHeader = "rOxfXOxB-cfxQGIfY-arWve2u-arXjs2";
            public const string FaceValueHeader = "rOxfXOxB-r7xe#1o8-arWve93-arXjs9";
            public const string ltlSold = "rOxfXOxB-cLVuUgs#-arWvef8-arXjs0";
            public const string ltlSetAllTicketsToThisPrice = "rOxfXOxB-cujdmcLk-arWvRQq-arXj0Q";
            public const string btnnext = "rOxfXOxB-cja4BgWQ-arWvef8-arXj0w";
            public const string btnprevious = "rOxfXOxB-cQb05rIK-arWve2y-arXj0m";
            public const string btnconfirm = "rOxfXOxB-cNC6Sv4Q-arWve2d-arXj06";
            public const string YouWillEarnHeader = "rOxfXOxB-B7aNHVw3-arWve9h-arXj0p";
            public const string NoTicketErrorMessage = "rOxfXOxB-BOeuG72n-arWve0r-arXj0f";
            public const string ltlPriceChanged = "rOxfXOxB-cEvrOIo9-arWve9u-arXj02";
            public const string ConfirmErrorText = "rOxfXOxB-BlpQwkSB-arWvR2O-arXj09";
            public const string TakingOffSaleHeaderText = "rOxfXOxB-r4vkFgYK-arWvRQh-arXj0a";
            public const string PriceChangeHeaderText = "rOxfXOxB-WYxNBxxZ-arWvRQ8-arXj00";
            public const string PlacingOnSaleHeaderText = "rOxfXOxB-FKvkFgeK-arWvRQh-arXjCQ";
            public const string FaceValueHeaderInfo = "rOxfXOxB-r7xeVIwy-arWvRwu-arXjCm";
            public const string PaymentOwnerHeaderInfo = "rOxfXOxB-rjxQWcL4-arWve0u-arXjC6";
            public const string SalePriceHeaderInfo = "rOxfXOxB-rcxe9IwZ-arWveay-arXjCp";
            public const string TicketExchangeIdHeader = "rOxfXOxB-tdvr272L-arWve0d-arXBeQ";
            public const string SalePriceHeader = "rOxfXOxB-rcxetLo#-arWve93-arXBew";
            public const string PaymentRefHeader = "rOxfXOxB-rjy4SgxY-arWve9a-arXBem";
            public const string SeatHeaderInfo = "rOxfXOxB-QWArGIbK-arWve9h-arXBe6";
            public const string YouWillEarnHeaderInfo = "rOxfXOxB-B7aNwD2I-arWve0h-arXBep";
            public const string SeatedCustomerHeaderInfo = "rOxfXOxB-Qap7O3oQ-arWvRQ3-arXBef";
            public const string PaymentRefHeaderInfo = "rOxfXOxB-rjy45ioQ-arWve03-arXBe2";
            public const string TicketExchangeIdHeaderInfo = "rOxfXOxB-tdvr25KY-arWve08-arXBe9";
            public const string PaymentOwnerHeader = "rOxfXOxB-rjxQQIoQ-arWvea3-arXBe0";
            public const string ltlPlacingOnSale = "rOxfXOxB-cEGrtLb4-arWve98-arXBRQ";
            public const string ltlCurrentlyOnSale = "rOxfXOxB-cvSwd1sK-arWve93-arXBRw";
            public const string ltlTakingOffSale = "rOxfXOxB-ccPRFdb4-arWve98-arXBRm";
            public const string SelectHeader = "rOxfXOxB-QfxQGIKY-arWve2u-arXBMQ";
            public const string SelectHeaderInfo = "rOxfXOxB-QfxQGRDQ-arWve0r-arXBMw";
            public const string StatusHeaderInfo = "rOxfXOxB-cfxQdRDQ-arWvea3-arXBRp";
            public const string ltlCurrentlyOffSale = "rOxfXOxB-cvSwUeLB-arWve9h-arXBR2";
            public const string btnFinished = "rOxfXOxB-ccK0mrgK-arWve2y-arXBR9";
            public const string NoActionSelectedErrorMessage = "rOxfXOxB-BAS45IcG-arWvRmO-arXBRa";
            public const string SeatedCustomerMask = "rOxfXOxB-Qap7GIzK-arWvRaO-arXBM6";
            public const string PaymentOwnerMask = "rOxfXOxB-rjxQSgk4-arWvR0q-arXBMp";
            public const string SeatMask = "rOxfXOxB-Q8v6dCk4-arWvEQh-arXBMf";
            public const string ClubFeeHeader = "rOxfXOxB-FOK4GOfB-arWvR93-arXBM9";
            public const string ClubFeeHeaderInfo = "rOxfXOxB-FOK4HIoB-arWvRfu-arXBMa";
            public const string PriceHeaderInfo = "rOxfXOxB-WOepGOiE-arWvMQ3-arXBEw";
            public const string PriceHeader = "rOxfXOxB-WOei2coG-arWvR9r-arXBEm";
            public const string btnCancel = "rOxfXOxB-cEErdOLv-arWve23-arXBA9";
            public const string btnreset = "rOxfXOxB-cOaQOIoa-arWve23-arXBAQ";
            public const string ShowProduct = "rOEEXOfK-JAEsH8zF-arWve23-arXBAw";
            public const string ProductMask = "rOxfXOxB-WjsiQcLG-arWvR9q-arXBAm";
            public const string ProductHeaderInfo = "rOxfXOxB-WjK45LoB-arWve2d-arXBA6";
            public const string ProductHeader = "rOxfXOxB-WjK4dOzK-arWve2u-arXBAp";
            public const string TicketExchangeItemInfoI0 = "rOxfXOxB-tdvrW5oX-arWvR6y-arXHCa";
            public const string TicketExchangeItemInfoI1 = "rOxfXOxB-tdvrW5oX-arWvRah-arXHC0";
            public const string TicketExchangeItemInfoI2 = "rOxfXOxB-tdvrW5oX-arWvMQO-arXKeQ";
            public const string TicketExchangeItemInfoI3 = "rOxfXOxB-tdvrW5oX-arWvR0r-arXKew";
            public const string TicketExchangeItemInfoI4 = "rOxfXOxB-tdvrW5oX-arWve0a-arXKem";
            public const string TicketExchangeItemInfoI5 = "rOxfXOxB-tdvrW5oX-arWvRf3-arXKe6";
            public const string TicketExchangeItemInfoI6 = "rOxfXOxB-tdvrW5oX-arWvRpu-arXKE2";
            public const string TicketExchangeItemInfoI7 = "rOxfXOxB-tdvrW5oX-arWvR6d-arXKE9";
            public const string TicketExchangeItemInfoI8 = "rOxfXOxB-tdvrW5oX-arWvR6O-arXKEa";


            // Ticket Exchange Defaults

            public const string ltlAllowPurchase = "rOxfXOxB-cAedzDl4-arWvRQO-arXKep";
            public const string ltlAllowPlaceOnSale = "rOxfXOxB-cAvdUXsd-arWvRQa-arXKef";
            public const string ltlTicketingExchangeSummaryHeader = "rOxfXOxB-cVa2Gkoh-arWvRQ3-arXKe2";
            public const string ltlSummarySoldHeader = "rOxfXOxB-clVuOV4V-arWvear-arXKe9";
            public const string ltlSummaryPendingHeader = "rOxfXOxB-cl34QxUG-arWve0u-arXKea";
            public const string ltlSummaryExpiredHeader = "rOxfXOxB-clgoQxUG-arWve0u-arXKe0";
            public const string ValidateProductMinPrice = "rOxfXOxB-rjpZOIxB-arWvR6O-arXKRQ";
            public const string ValidateProductMaxPrice = "rOxfXOxB-rjpZOIxB-arWvR6d-arXKRw";
            public const string AllowPurchaseHeader = "rOxfXOxB-FDv#zDlG-arWvRQd-arXKRm";
            public const string AllowPlaceOnSaleHeader = "rOxfXOxB-F7Gr2Xs9-arWvRwq-arXKR6";
            public const string ltlDefaults = "rOxfXOxB-cWbkUVxK-arWve2u-arXKRp";

            public const string ltlProductDescription = "rOxfXOxB-cLH45gfE-arWveay-arXBs6";
            public const string ltlProductCode = "rOxfXOxB-cLru5LsE-arWve2h-arXBsp";
            public const string ltlTotalfee = "rOxfXOxB-cWxkwVxK-arWve2y-arXBsf";
            public const string ltlTotalPrice = "rOxfXOxB-cWjdQxs4-arWve9y-arXBs2";
            public const string ltlTotalPending = "rOxfXOxB-cWaZ5Ve9-arWve9a-arXBs9";
            public const string ltlTotalSold = "rOxfXOxB-cWSZUVzX-arWve28-arXBsa";
            public const string ltlClubFee = "rOxfXOxB-cnS6GVoy-arWve2d-arXBs0";
            public const string ltlProductMinPrice = "rOxfXOxB-cLlEdxkK-arWveaO-arXB0Q";
            public const string ltlClubTypeFee = "rOxfXOxB-cnxUHisO-arWve9q-arXB06";
            public const string FeeTypeP = "rOxfXOxB-QOxlFItf-arWve2d-arXB0p";
            public const string FeeTypeF = "rOxfXOxB-QOxlFItf-arWvefr-arXB0f";
            public const string btnFetchDataForProduct = "rOxfXOxB-cEEeHrKK-arWve9u-arXB02";
            public const string ltlProductMaxPrice = "rOxfXOxB-cLledxkK-arWveaO-arXB00";
            public const string StandHeader = "rOxfXOxB-cOes28oK-arWve23-arXBCQ";
            public const string AreaHeader = "rOxfXOxB-WW5QOIoB-arWvefa-arXBCw";
            public const string CapacityHeader = "rOxfXOxB-rjvZ6RQ4-arWve28-arXBCm";
            public const string SoldHeader = "rOxfXOxB-BWVuOVoB-arWvefa-arXBC6";
            public const string AllowTESalesHeader = "rOxfXOxB-Fdx#UEo9-arWve9d-arXBCp";
            public const string AllowTEReturnsHeader = "rOxfXOxB-FdzQOVok-arWve98-arXBCf";
            public const string lblValueOfTEFeesSoldOnTicketExchange = "rOxfXOxB-#DqL272t-arWve0u-arXBC2";
            public const string lblValueOfTicketsSoldOnTicketExchange = "rOxfXOxB-#DqEUvLp-arWve0q-arXBC9";
            public const string lblValueOfTicketsExpiredOnTicketExchange = "rOxfXOxB-#DqEXIK9-arWve0r-arXBCa";
            public const string lblValueOfTicketsPendingOnTicketExchange = "rOxfXOxB-#DqEWvK9-arWve0r-arXBC0";
            public const string lblSumOfTicketsSoldOnTicketExchange = "rOxfXOxB-#469odpZ-arWve0O-arXHeQ";
            public const string lblClubFeeType = "rOxfXOxB-#nqwHIsO-arWve9q-arXHew";
            public const string lblClubFee = "rOxfXOxB-#nS0GVoy-arWve2q-arXHem";
            public const string lblSumOfTicketsExpiredOnTicketExchange = "rOxfXOxB-#469OBot-arWve0h-arXHe6";
            public const string lblSumOfTicketsPendingOnTicketExchange = "rOxfXOxB-#4696Bot-arWve0h-arXHep";
            public const string lblSumOfTicketsBooked = "rOxfXOxB-#469Gri#-arWve9r-arXHef";
            public const string lblSumOfTicketsAllocatedSold = "rOxfXOxB-#469gIs9-arWve0q-arXHe2";
            public const string lblValueOfTEFeesExpiredOnTicketExchange = "rOxfXOxB-#DqL6h#y-arWvRQO-arXHe9";
            public const string lblValueOfTEFeesPendingOnTicketExchange = "rOxfXOxB-#DqL2J#y-arWve08-arXHea";
            public const string btnUpdate = "rOxfXOxB-cWErdHLv-arWve2O-arXHRQ";
            public const string lblStandAreaMask = "rOxfXOxB-#0xe9vk4-arWvM9a-arXHRw";
            public const string ValueOfTEFeesSoldOnTicketExchangeHeader = "rOxfXOxB-rNx4Fblt-arWve0h-arXHR6";
            public const string ValueOfTEFeesExpiredOnTicketExchangeHeader = "rOxfXOxB-rNx4272y-arWvRQq-arXHRp";
            public const string ValueOfTEFeesPendingOnTicketExchangeHeader = "rOxfXOxB-rNx4h72y-arWvRQq-arXHRf";
            public const string ValueOfTicketsSoldOnTicketExchangeHeader = "rOxfXOxB-rNs4WLKp-arWve0q-arXHR2";
            public const string ValueOfTicketsExpiredOnTicketExchangeHeader = "rOxfXOxB-rNs4GvL9-arWve0r-arXHR9";
            public const string ValueOfTicketsPendingOnTicketExchangeHeader = "rOxfXOxB-rNs4WvL9-arWve0r-arXHRa";
            public const string SumOfTicketsSoldOnTicketExchangeHeader = "rOxfXOxB-5cE#6IxZ-arWveaa-arXHR0";
            public const string SumOfTicketsExpiredOnTicketExchangeHeader = "rOxfXOxB-5cE#odpt-arWve0h-arXHMQ";
            public const string SumOfTicketsPendingOnTicketExchangeHeader = "rOxfXOxB-5cE#odpt-arWve0h-arXHMw";
            public const string SumOfTicketsBookedHeader = "rOxfXOxB-5cE#GIiY-arWve9r-arXHMm";
            public const string SumOfTicketsAllocatedSoldHeader = "rOxfXOxB-5cE#GDJ9-arWvear-arXHM6";
            public const string lblCustomerRetainsMaximumLimit = "rOxfXOxB-#jetQkgL-arWve03-arXHM2";
            public const string lblCustomerRetainsPrerequisite = "rOxfXOxB-#jetOILL-arWvear-arXHM9";
            public const string lblMinMaxBoundaryType = "rOxfXOxB-#qpXXrxU-arWvea8-arXHMa";
            public const string ltlTicketingExchangeDefaultHeader = "rOxfXOxB-cVa2G3oh-arWvRQq-arXHM0";
            public const string SuccesfulUpdate = "rOxfXOxB-5NDZG1L#-arWve9a-arXHEQ";
            public const string StandAreaMaskHeader = "rOxfXOxB-cvv#9vkB-arWve9u-arXH0p";
            public const string YearsOfPastProductsToShow = "rOEEXOfK-QNEiFq4X-arWvead-arXHCp";
            public const string ltlAllProducts = "rOxfXOxB-cwzdUDs1-arWve28-arXHCf";
            public const string ProductCodeDateDescriptionMask = "rOxfXOxB-Wjx1Og0P-arWvM6q-arXHC9";                                                      

            public DMTicketExchange(DESettings settings, bool initialiseData)
                : base(ref settings, initialiseData)
            {
            }
            public override void SetModuleConfiguration()
            {
                // General Ticket Exchange Defaults
                MConfigs.Add(XF010B, DataType.BOOL_YN, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SHOW_TE_PROGRESS_BAR, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(VL001B, DataType.TEXT, DisplayTabs.TabHeaderGeneral, FieldValidation.Add(new List<VG> { VG.Mandatory, VG.Numeric }));
                MConfigs.Add(VL002B, DataType.TEXT, DisplayTabs.TabHeaderGeneral, FieldValidation.Add(new List<VG> { VG.Mandatory, VG.Numeric }));
                MConfigs.Add(XF016B, DataType.DROPDOWN, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(VL003B, DataType.TEXT, DisplayTabs.TabHeaderGeneral, FieldValidation.Add(new List<VG> { VG.Mandatory, VG.Numeric }));
                MConfigs.Add(XF013B, DataType.DROPDOWN, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(XF014B, DataType.BOOL_YN, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(XF015B, DataType.BOOL_YN, DisplayTabs.TabHeaderGeneral);

                // Customer Ticket Exchange Summary
                MConfigs.Add(ltlTicketingExchangeProductsHeader, DataType.TEXTAREA, DisplayTabs.TabHeaderTicketExchangeCustomerSummary); 
                MConfigs.Add(ltlSummaryLine1, DataType.TEXTAREA, DisplayTabs.TabHeaderTicketExchangeCustomerSummary); 
                MConfigs.Add(ltlSummaryLine2, DataType.TEXTAREA, DisplayTabs.TabHeaderTicketExchangeCustomerSummary);
                MConfigs.Add(ltlSummaryLine3, DataType.TEXTAREA, DisplayTabs.TabHeaderTicketExchangeCustomerSummary);

                // Customer Ticket Exchange Product List
                MConfigs.Add(ltlSellYourTickets, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(ltlFromDate, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(DefaultDaysHistory, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(btnFromDateText, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);

                MConfigs.Add(ShowProductDate, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(ProductDateHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(ProductDateHeaderinfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);

                MConfigs.Add(ShowProductDescription, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(ProductDescriptionHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(ProductDescriptionHeaderInfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);

                MConfigs.Add(ShowCompetitionDescription, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(CompetitionDescriptionHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(CompetitionDescriptionHeaderInfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);

                MConfigs.Add(ShowTotalPurchased, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(TotalPurchasedHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(TotalPurchasedHeaderInfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                
                MConfigs.Add(ShowTotalPurchasedPrice, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(TotalPurchasedPriceHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(TotalPurchasedPriceHeaderInfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);

                MConfigs.Add(ShowTotalWithCustomer, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(TotalWithCustomerHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(TotalWithCustomerHeaderinfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);

                MConfigs.Add(ShowTotalSoldOnTicketExchange, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(TotalSoldOnTicketExchangeHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(TotalSoldOnTicketExchangeHeaderinfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);

                MConfigs.Add(ShowTotalResalePriceSold, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(TotalResalePriceSoldHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(TotalResalePriceSoldHeaderinfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);

                MConfigs.Add(ShowTotalHandlingFeeSold, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(TotalHandlingFeeSoldHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(TotalHandlingFeeSoldHeaderinfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);

                MConfigs.Add(ShowTotalPendingOnTicketExchange, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(TotalPendingOnTicketExchangeHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(TotalPendingOnTicketExchangeHeaderinfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);

                MConfigs.Add(ShowTotalResalePricePending, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(TotalResalePricePendingHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(TotalResalePricePendingHeaderinfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);

                MConfigs.Add(ShowTotalHandlingFeePending, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(TotalHandlingFeePendingHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(TotalHandlingFeePendingHeaderinfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                
                MConfigs.Add(ActionHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(ActionHeaderInfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(ltlSelect, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(ltlNotAvailable, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(ActionErrorDesc_NA, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);
                MConfigs.Add(ActionErrorDesc_PD, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeCustomerProductList);

                // Ticket Exchange Process Pages
                MConfigs.Add(ltlTicketExchangeSelectionHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(ltlSetAllTicketsToThisPrice, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);

                MConfigs.Add(StatusHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(StatusHeaderInfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(ltlSold, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(ltlCurrentlyOnSale, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(ltlPlacingOnSale, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(ltlCurrentlyOffSale, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(ltlTakingOffSale, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(ltlPriceChanged, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(TicketExchangeItemInfoI0, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(TicketExchangeItemInfoI1, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(TicketExchangeItemInfoI2, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(TicketExchangeItemInfoI3, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(TicketExchangeItemInfoI4, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(TicketExchangeItemInfoI5, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(TicketExchangeItemInfoI6, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(TicketExchangeItemInfoI7, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(TicketExchangeItemInfoI8, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
     
                MConfigs.Add(SelectHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(SelectHeaderInfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);

                MConfigs.Add(ShowProduct, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(ProductHeaderInfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(ProductHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(ProductMask, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);

                MConfigs.Add(ShowSeat, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(SeatHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(SeatHeaderInfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(SeatMask, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);

                MConfigs.Add(ShowOwner, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(PaymentOwnerHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(PaymentOwnerHeaderInfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(PaymentOwnerMask, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);

                MConfigs.Add(ShowPaymentRef, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(PaymentRefHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(PaymentRefHeaderInfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);

                MConfigs.Add(ShowSaleprice, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(SalePriceHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(SalePriceHeaderInfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);

                MConfigs.Add(ShowCustomer, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(SeatedCustomerHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(SeatedCustomerHeaderInfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(SeatedCustomerMask, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);

                MConfigs.Add(ShowFaceValue, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(FaceValueHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(FaceValueHeaderInfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);

                MConfigs.Add(PriceHeaderInfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(PriceHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(SliderStepNumber, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);

                MConfigs.Add(ShowClubFee, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(ClubFeeHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(ClubFeeHeaderInfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);

                MConfigs.Add(ShowYouWillEarn, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(YouWillEarnHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(YouWillEarnHeaderInfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                
                MConfigs.Add(ShowTicketExchangeId, DataType.BOOL, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(TicketExchangeIdHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(TicketExchangeIdHeaderInfo, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);

                MConfigs.Add(TakingOffSaleHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(PriceChangeHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(PlacingOnSaleHeaderText, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);

                MConfigs.Add(NoTicketErrorMessage, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(ConfirmErrorText, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(NoActionSelectedErrorMessage, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);

                MConfigs.Add(btnCancel, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(btnreset, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(btnprevious, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(btnnext, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(btnconfirm, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);
                MConfigs.Add(btnFinished, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeProcess);


                // Ticket Exchange Defaults

                MConfigs.Add(ltlTicketingExchangeDefaultHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ltlTicketingExchangeSummaryHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ltlSummarySoldHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ltlSummaryPendingHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ltlSummaryExpiredHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ltlProductDescription, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ltlProductCode, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ProductCodeDateDescriptionMask, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ltlTotalfee, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ltlTotalPrice, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ltlTotalPending, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ltlTotalSold, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ltlClubFee, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);

                MConfigs.Add(ltlDefaults, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ltlAllowPlaceOnSale, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ltlAllowPurchase, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ltlClubTypeFee, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(FeeTypeP, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(FeeTypeF, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ltlProductMinPrice, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ltlProductMaxPrice, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ValidateProductMinPrice, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ValidateProductMaxPrice, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(lblValueOfTEFeesSoldOnTicketExchange, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(lblValueOfTicketsSoldOnTicketExchange, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(lblValueOfTicketsExpiredOnTicketExchange, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(lblValueOfTicketsPendingOnTicketExchange, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(lblSumOfTicketsSoldOnTicketExchange, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(lblClubFeeType, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(lblClubFee, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(lblSumOfTicketsExpiredOnTicketExchange, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(lblSumOfTicketsPendingOnTicketExchange, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(lblSumOfTicketsBooked, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(lblSumOfTicketsAllocatedSold, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(lblValueOfTEFeesExpiredOnTicketExchange, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(lblValueOfTEFeesPendingOnTicketExchange, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);

                MConfigs.Add(lblCustomerRetainsMaximumLimit, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(lblCustomerRetainsPrerequisite, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(lblMinMaxBoundaryType, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);

                MConfigs.Add(StandAreaMaskHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(lblStandAreaMask, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(SumOfTicketsAllocatedSoldHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(SumOfTicketsBookedHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                        
                MConfigs.Add(SumOfTicketsPendingOnTicketExchangeHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(SumOfTicketsExpiredOnTicketExchangeHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(SumOfTicketsSoldOnTicketExchangeHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ValueOfTicketsPendingOnTicketExchangeHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ValueOfTicketsExpiredOnTicketExchangeHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ValueOfTicketsSoldOnTicketExchangeHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ValueOfTEFeesPendingOnTicketExchangeHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ValueOfTEFeesExpiredOnTicketExchangeHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ValueOfTEFeesSoldOnTicketExchangeHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(AllowPurchaseHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(AllowPlaceOnSaleHeader, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                          
                MConfigs.Add(SuccesfulUpdate, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(btnUpdate, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(btnFetchDataForProduct, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);

                MConfigs.Add(YearsOfPastProductsToShow, DataType.DROPDOWN, DisplayTabs.TabHeaderTicketExchangeDefaults);
                MConfigs.Add(ltlAllProducts, DataType.TEXT, DisplayTabs.TabHeaderTicketExchangeDefaults);
           
                Populate();
            }
        }
    }
}