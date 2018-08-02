using System.Collections.Generic;

namespace TalentBusinessLogic.DataTransferObjects.JQuery.DataTables
{
    public class JQueryDataTable <T>
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<T> dataList { get; set; }

    }
}
