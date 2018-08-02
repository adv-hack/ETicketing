using System.Collections.Generic;
namespace TalentSystemDefaults
{
    namespace TalentModules
    {
        public class DMProductSearch : DMBase
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
            static private string _ModuleTitle = "Product Search";
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

            public const string displayingLabelT = "BjS4aVLZ-tP2eXiWB-arWve28-arXVCm";
            public const string displayingLabelB = "BjS4aVLZ-tP2eXiWB-arWve28-arXVC6";
            public const string toLabelT = "BjS4aVLZ-B72eQVWB-arWvef3-arXVCp";
            public const string toLabelB = "BjS4aVLZ-B72eQVWB-arWvef3-arXVCf";
            public const string ofLabelT = "BjS4aVLZ-y72eQVWB-arWvef3-arXVC2";
            public const string ofLabelB = "BjS4aVLZ-y72eQVWB-arWvef3-arXVC9";
            public const string LnkFirstT = "BjS4aVLZ-C8a9WdeM-arWvefy-arXVCa";
            public const string LnkFirstB = "BjS4aVLZ-C8a9WdeM-arWvefy-arXVC0";
            public const string LnkPrevT = "BjS4aVLZ-CQsitG4B-arWvefd-arXCeQ";
            public const string LnkPrevB = "BjS4aVLZ-CQsitG4B-arWvefd-arXCew";
            public const string LnkNextT = "BjS4aVLZ-Cjs3igou-arWvefd-arXCem";
            public const string LnkNextB = "BjS4aVLZ-Cjs3igou-arWvefd-arXCe6";
            public const string LnkLastT = "BjS4aVLZ-CjshJgxa-arWvefd-arXCep";
            public const string LnkLastB = "BjS4aVLZ-CjshJgxa-arWvefd-arXCef";
            public const string ltlStartDateLabel = "BjS4aVLZ-cvE4UOoB-arWve2r-arXCe2";
            public const string ltlEndDateLabel = "BjS4aVLZ-cX2eWOo9-arWve2u-arXCe9";
            public const string StadiumSelectDescription = "BjS4aVLZ-clxdXDgL-arWve9r-arXCea";
            public const string NoProductsFoundMessageText = "BjS4aVLZ-BDkuQ7j4-arWveau-arXCe0";
            public const string KeywordLabel = "BjS4aVLZ-QLxkJIzv-arWve2h-arXCRQ";
            public const string StadiumLabel = "BjS4aVLZ-clxkJIgv-arWve2u-arXCRw";
            public const string DateLabel = "BjS4aVLZ-rnv6QYxP-arWve2O-arXCRm";
            public const string SearchButtonText = "BjS4aVLZ-QpprOzSB-arWve2a-arXCR6";
            public const string ClearButtonText = "BjS4aVLZ-FDalOgpO-arWve9y-arXCRp";
            public const string CategoryLabel = "BjS4aVLZ-rvC4OOLG-arWve2r-arXCRf";
            public const string CategoryPleaseSelectText = "BjS4aVLZ-rvxedAYB-arWveaq-arXCR2";
            public const string SaveAndSearchButtonText = "BjS4aVLZ-rLedFgeK-arWveay-arXCR9";
            public const string ProductTypeDescriptionH = "BjS4aVLZ-Wjx16NfQ-arWve9y-arXCRa";
            public const string ProductTypeDescriptionA = "BjS4aVLZ-Wjx16NfQ-arWve9y-arXCR0";
            public const string ProductTypeDescriptionS = "BjS4aVLZ-Wjx16NfQ-arWveau-arXCMQ";
            public const string ProductTypeDescriptionE = "BjS4aVLZ-Wjx16NfQ-arWve9h-arXCMw";
            public const string ProductTypeDescriptionT = "BjS4aVLZ-Wjx16NfQ-arWve98-arXCMm";
            public const string ProductTypeDescriptionC = "BjS4aVLZ-Wjx16NfQ-arWvea3-arXCM6";
            public const string LocationLabel = "BjS4aVLZ-BAC45OKX-arWve2y-arXCMp";
            public const string ProductTypeLabel = "BjS4aVLZ-Wjxh2gxv-arWve9y-arXCMf";
            public const string ProductTypePleaseSelectText = "BjS4aVLZ-WjxiUg4B-arWve0d-arXCM2";
            public const string LocationPleaseSelectText = "BjS4aVLZ-BAxedALB-arWveah-arXCM9";
            public const string SearchTypeS = "BjS4aVLZ-Qm1s3NKB-arWve9u-arXCMa";
            public const string SearchTypeP = "BjS4aVLZ-Qm1i3NKB-arWve9u-arXCM0";
            public const string SortByDescription = "BjS4aVLZ-BH6Q5zKa-arWve9h-arXCEQ";
            public const string SortByProductDate = "BjS4aVLZ-BHKX5zJX-arWve2a-arXCEw";
            public const string SortByLocation = "BjS4aVLZ-BHv6DYz#-arWve9O-arXCEm";
            public const string SortByProductType = "BjS4aVLZ-BHKX5zJX-arWve98-arXCE6";
            public const string SortByCategory = "BjS4aVLZ-BHx2De4#-arWve9O-arXCEp";
            public const string SortByStadium = "BjS4aVLZ-BHKE3gbK-arWve2r-arXCEf";
            public const string SortByLabel = "BjS4aVLZ-BZSsa8TX-arWve2y-arXCE2";
            public const string SearchTypeLabel = "BjS4aVLZ-Qm2egio#-arWve9d-arXCE9";
            public const string ViewTypeLabel = "BjS4aVLZ-t5C4XOof-arWve2a-arXCEa";
            public const string ViewTypeL = "BjS4aVLZ-t58EJIPc-arWve23-arXCE0";
            public const string ViewTypeT = "BjS4aVLZ-t58EFIPc-arWve23-arXCAQ";
            public const string StadiumPleaseSelectText = "BjS4aVLZ-clv#FgfK-arWveau-arXCAw";
            public const string txtDateRegExErrorText = "BjS4aVLZ-4OgoBAEP-arWvead-arXCAm";
            public const string SavedSearchLabel = "BjS4aVLZ-rOThGIxv-arWve9h-arXCA6";
            public const string LastSearchLabel = "BjS4aVLZ-rW2e9xoM-arWve9q-arXCAp";
            public const string SavedSearchMaskP = "BjS4aVLZ-rOTF48kU-arWvM28-arXCAf";
            public const string SavedSearchMaskS = "BjS4aVLZ-rOTF48kU-arWvRar-arXCA2";

            public const string Cacheing = "BjS6aVxZ-r06MmvoL-arWvefd-arXCA9";
            public const string CacheTimeMinutes = "BjS6aVxZ-rcjrmdfK-arWvefa-arXCAa";
            public const string numberOfLinks = "BjS6aVxZ-54a9odUp-arWvefh-arXCA0";
            public const string numberOfProductsVariableL = "BjS6aVxZ-54pZ6VUL-arWve2a-arXC1Q";
            public const string numberOfProductsVariableT = "BjS6aVxZ-54pZ6VUL-arWve2r-arXC1w";
            public const string showEndDate = "BjS6aVxZ-JLx#QCJF-arWvefh-arXC1m";
            public const string showStartDate = "BjS6aVxZ-JWv6Qyz4-arWvefr-arXC16";
            public const string dateFormat = "BjS6aVxZ-rvKedgxX-arWve28-arXC1p";
            public const string txtDateRegEx = "BjS6aVxZ-4OgocZxT-arWvRm3-arXC1f";
            public const string SavedAgentSearchCaching = "BjS6aVxZ-rKV4mJHT-arWve9d-arXC12";
            public const string SavedAgentSearchCachingTimeInMins = "BjS6aVxZ-rKV4mJe4-arWvea3-arXC19";
            public const string ViewShadowBoxProperty = "BjS6aVxZ-tWuuddJd-arWve08-arXC1a";

            public DMProductSearch(DESettings settings, bool initialiseData)
                : base(ref settings, initialiseData)
            {
            }
            public override void SetModuleConfiguration()
            {
                //Search Options Panel 
                MConfigs.Add(SearchTypeLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SearchTypeS, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SearchTypeP, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SortByLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SortByDescription, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SortByProductDate, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                //MConfigs.Add(SortByLocation, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                //MConfigs.Add(SortByProductType, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                //MConfigs.Add(SortByCategory, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                //MConfigs.Add(SortByStadium, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ViewTypeLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ViewTypeL, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ViewTypeT, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SearchButtonText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ClearButtonText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SaveAndSearchButtonText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);

                //Search Filters Panel
                MConfigs.Add(KeywordLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(DateLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(txtDateRegExErrorText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(StadiumLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(StadiumSelectDescription, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(StadiumPleaseSelectText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(LocationLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(LocationPleaseSelectText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ProductTypeLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ProductTypePleaseSelectText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ProductTypeDescriptionH, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ProductTypeDescriptionA, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ProductTypeDescriptionS, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ProductTypeDescriptionE, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ProductTypeDescriptionT, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ProductTypeDescriptionC, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CategoryLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CategoryPleaseSelectText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);

                MConfigs.Add(NoProductsFoundMessageText, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(numberOfLinks, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(numberOfProductsVariableL, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(numberOfProductsVariableT, DataType.TEXT, DisplayTabs.TabHeaderGeneral);

                MConfigs.Add(dateFormat, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ltlStartDateLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ltlEndDateLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(showStartDate, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(showEndDate, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(displayingLabelT, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(toLabelT, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ofLabelT, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(LnkFirstT, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(LnkPrevT, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(LnkNextT, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(LnkLastT, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(displayingLabelB, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(toLabelB, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ofLabelB, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(LnkFirstB, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(LnkPrevB, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(LnkNextB, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(LnkLastB, DataType.TEXT, DisplayTabs.TabHeaderGeneral);

                MConfigs.Add(SavedSearchLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(LastSearchLabel, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SavedSearchMaskP, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SavedSearchMaskS, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(Cacheing, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(CacheTimeMinutes, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SavedAgentSearchCaching, DataType.BOOL, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(SavedAgentSearchCachingTimeInMins, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(txtDateRegEx, DataType.TEXT, DisplayTabs.TabHeaderGeneral);
                MConfigs.Add(ViewShadowBoxProperty, DataType.BOOL, DisplayTabs.TabHeaderGeneral);

                Populate();
            }
        }
    }
}