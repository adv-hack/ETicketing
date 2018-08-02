using System.Collections.Generic;
using TalentBusinessLogic.DataTransferObjects.JQuery.DataTables;
using Talent.Common;

namespace TalentBusinessLogic.Models.ApplicationModels.BaseModels
{
    public class BaseViewModelForDataTables<T>: BaseViewModel
    {

        /// <summary>
        /// This is a generic list that will contain the data that will be populated in the JQuery data table.  
        /// We are using generics here so we can reuse this base class for data tables that we need develop
        /// </summary>

        public List<T> DataTableList { get; set; }

        /// <summary>
        /// Draw counter.
        /// This is used by DataTables to ensure that the Ajax returns from server-side processing requests are drawn in sequence by DataTables (Ajax requests are asynchronous and thus can return out of sequence).
        /// This is used as part of the draw return parameter (see below).
        /// </summary>
        public int Draw { get; set; }

        public int RecordsTotal { get; set; }

        public int RecordsFiltered { get; set; }
        /// <summary>
        /// This methiod transforms the DataTableList data into a JQueryDataTable format.  
        /// This is used by DataTables to ensure that the Ajax returns from server-side processing requests are drawn in sequence by DataTables (Ajax requests are asynchronous and thus can return out of sequence).
        /// This is used as part of the draw return parameter (see below).
        /// </summary>
        public JQueryDataTable<T> GetJQueryDataTable()
        {
            JQueryDataTable<T> result = new JQueryDataTable<T>();
            result.draw = Draw;

            //Initialize row count variables if not implicitly set 
            if (DataTableList == null)
            {
                result.recordsTotal = 0;
                result.recordsFiltered = 0;
                result.dataList = new List<T>();
            }
            else
            {
                // Populate the totals
                if (RecordsTotal > 0)
                {
                    result.recordsTotal = RecordsTotal;
                }
                else
                {
                    result.recordsTotal = DataTableList.Count;
                }

                if (RecordsFiltered > 0)
                {
                    result.recordsFiltered = RecordsFiltered;
                }
                else
                {
                    result.recordsFiltered = DataTableList.Count;
                }

                // Populate the list
                result.dataList = DataTableList;
            }
            

            
            return result;
        }

        /// <summary>
        /// Constructors required for the base class
        /// </summary>

        public BaseViewModelForDataTables(WebFormResource resource, bool getContentAndAttributes) : base(resource, getContentAndAttributes) { }

        public BaseViewModelForDataTables(UserControlResource resource, bool getContentAndAttributes, string controlCode) : base(resource, getContentAndAttributes, controlCode) { }
        public BaseViewModelForDataTables() : base() { }
        
    }
}
