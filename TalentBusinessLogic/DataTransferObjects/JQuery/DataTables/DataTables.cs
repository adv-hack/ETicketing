﻿using System.Collections.Generic;

///This view model class has been referred from example created by Marien Monnier at Soft.it. All credits to Marien for this class
namespace TalentBusinessLogic.DataTransferObjects.JQuery.DataTables
{
    /// <summary>
    /// A full result, as understood by jQuery DataTables.
    /// </summary>
    /// <typeparam name="T">The data type of each row.</typeparam>
    public class DTResult<T>
    {
        /// <summary>
        /// The draw counter that this object is a response to - from the draw parameter sent as part of the data request.
        /// Note that it is strongly recommended for security reasons that you cast this parameter to an integer, rather than simply echoing back to the client what it sent in the draw parameter, in order to prevent Cross Site Scripting (XSS) attacks.
        /// </summary>
        public int draw { get; set; }

        /// <summary>
        /// Total records, before filtering (i.e. the total number of records in the database)
        /// </summary>
        public int recordsTotal { get; set; }

        /// <summary>
        /// Total records, after filtering (i.e. the total number of records after filtering has been applied - not just the number of records being returned for this page of data).
        /// </summary>
        public int recordsFiltered { get; set; }

        /// <summary>
        /// The data to be displayed in the table.
        /// This is an array of data source objects, one for each row, which will be used by DataTables.
        /// Note that this parameter's name can be changed using the ajaxDT option's dataSrc property.
        /// </summary>
        public List<T> data { get; set; }
    }

    /// <summary>
    /// The additional columns that you can send to jQuery DataTables for automatic processing.
    /// </summary>
    public abstract class DTRow
    {
        /// <summary>
        /// Set the ID property of the dt-tag tr node to this value
        /// </summary>
        public virtual string DT_RowId
        {
            get { return null; }
        }

        /// <summary>
        /// Add this class to the dt-tag tr node
        /// </summary>
        public virtual string DT_RowClass
        {
            get { return null; }
        }

        /// <summary>
        /// Add this data property to the row's dt-tag tr node allowing abstract data to be added to the node, using the HTML5 data-* attributes.
        /// This uses the jQuery data() method to set the data, which can also then be used for later retrieval (for example on a click event).
        /// </summary>
        public virtual object DT_RowData
        {
            get { return null; }
        }
    }

    /// <summary>
    /// The parameters sent by jQuery DataTables in AJAX queries.
    /// DTParameters has now moved to BaseInputModelForDataTables
    /// </summary>

    /// <summary>
    /// A jQuery DataTables column.
    /// </summary>
    public class DTColumn
    {
        /// <summary>
        /// Column's data source, as defined by columns.data.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Column's name, as defined by columns.name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Flag to indicate if this column is searchable (true) or not (false). This is controlled by columns.searchable.
        /// </summary>
        public bool Searchable { get; set; }

        /// <summary>
        /// Flag to indicate if this column is orderable (true) or not (false). This is controlled by columns.orderable.
        /// </summary>
        public bool Orderable { get; set; }

        /// <summary>
        /// Specific search value.
        /// </summary>
        public DTSearch Search { get; set; }
    }

    /// <summary>
    /// An order, as sent by jQuery DataTables when doing AJAX queries.
    /// </summary>
    public class DTOrder
    {
        /// <summary>
        /// Column to which ordering should be applied.
        /// This is an index reference to the columns array of information that is also submitted to the server.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Ordering direction for this column.
        /// It will be dt-string asc or dt-string desc to indicate ascending ordering or descending ordering, respectively.
        /// </summary>
        public DTOrderDir Dir { get; set; }
    }

    /// <summary>
    /// Sort orders of jQuery DataTables.
    /// </summary>
    public enum DTOrderDir
    {
        ASC,
        DESC
    }

    /// <summary>
    /// A search, as sent by jQuery DataTables when doing AJAX queries.
    /// </summary>
    public class DTSearch
    {
        /// <summary>
        /// Global search value. To be applied to all columns which have searchable as true.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// true if the global filter should be treated as a regular expression for advanced searching, false otherwise.
        /// Note that normally server-side processing scripts will not perform regular expression searching for performance reasons on large data sets, but it is technically possible and at the discretion of your script.
        /// </summary>
        public bool Regex { get; set; }
    }
}