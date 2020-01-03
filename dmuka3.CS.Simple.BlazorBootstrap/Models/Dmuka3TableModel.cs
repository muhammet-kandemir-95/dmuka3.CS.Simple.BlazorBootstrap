using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dmuka3.CS.Simple.BlazorBootstrap
{
    /// <summary>
    /// This model is used for Dmuka3Table.razor to receive and send datas between Dmuka3Table and other component which uses Dmuka3Table.
    /// </summary>
    public class Dmuka3TableModel
    {
        #region Enums
        /// <summary>
        /// Column sort type.
        /// </summary>
        public enum SortType
        {
            /// <summary>
            /// There is no sorting.
            /// </summary>
            None,
            /// <summary>
            /// Ascending.
            /// </summary>
            Asc,
            /// <summary>
            /// Descending.
            /// </summary>
            Desc
        }
        #endregion

        #region Classes
        /// <summary>
        /// Table column.
        /// </summary>
        public class Column
        {
            #region Variables
            /// <summary>
            /// What is property's name which is coming from source.
            /// This won't be seen by users.
            /// But developer can see using devtool.
            /// </summary>
            public string Name { get; internal set; }
            /// <summary>
            /// Description of property which is shown on frontend.
            /// </summary>
            public string Description { get; internal set; }
            /// <summary>
            /// What is sorting of column?
            /// </summary>
            public SortType SortType { get; internal set; }
            /// <summary>
            /// Can column be sorted?
            /// </summary>
            public bool Sortable { get; internal set; }
            #endregion

            #region Constructors
            /// <summary>
            /// If you give just a description, it means this column is not sortable and doesn't come from a property.
            /// </summary>
            /// <param name="description">
            /// Description of property which is shown on frontend.
            /// </param>
            public Column(string description)
            {
                this.Sortable = false;
                this.Name = null;
                this.Description = description;
            }

            /// <summary>
            /// Create a column which uses source's datas.
            /// </summary>
            /// <param name="name">
            /// What is property's name which is coming from source.
            /// This won't be seen by users.
            /// But developer can see using devtool.
            /// </param>
            /// <param name="description">
            /// Description of property which is shown on frontend.
            /// </param>
            /// <param name="sortType">
            /// What is sorting of column?
            /// </param>
            public Column(string name, string description, SortType sortType)
            {
                this.Sortable = true;
                this.Name = name;
                this.Description = description;
                this.SortType = sortType;
            }
            #endregion
        }
        #endregion

        #region Variables
        /// <summary>
        /// Which component uses Dmuka3Table?
        /// </summary>
        public ComponentBase Parent { get; internal set; }
        /// <summary>
        /// which Dmuka3Table uses this class?
        /// </summary>
        public Dmuka3Table Table { get; internal set; }

        /// <summary>
        /// What is the unique key(like ID) in a row?
        /// This is a property name which must be in row.
        /// </summary>
        public string UniqueKey { get; internal set; }
        /// <summary>
        /// Table's columns.
        /// </summary>
        public Column[] Columns { get; internal set; }
        /// <summary>
        /// This means if you set value to true, it avoid multiple sorting.
        /// This decides what is going to happen when user clicks a column.
        /// </summary>
        public bool SingleSort { get; internal set; }
        /// <summary>
        /// Which page is browser user on?
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// PageIndex limit.
        /// </summary>
        public int MaxPageIndex
        {
            get
            {
                return Math.Max(0, (this.TotalRowCount / RowCount + (this.TotalRowCount % RowCount == 0 ? 0 : 1)) - 1);
            }
        }
        /// <summary>
        /// Max row count for each page.
        /// This is filled by user or while instance.
        /// </summary>
        public int RowCount { get; internal set; }
        /// <summary>
        /// You can change default <see cref="RowCountOptions"/> Options by setting this.
        /// </summary>
        public static int[] RowCountOptionsStatic { get; set; } = new int[] { 10, 20, 50, 100 };
        /// <summary>
        /// How many options are there for user to change row count.
        /// </summary>
        public int[] RowCountOptions { get; internal set; }
        /// <summary>
        /// You can change default <see cref="RowCountLabel"/> Label by setting this.
        /// </summary>
        public static string RowCountLabelStatic { get; set; } = "Row Count : ";
        /// <summary>
        /// Label which is near to Row Count Select.
        /// </summary>
        public string RowCountLabel { get; internal set; }
        /// <summary>
        /// This is calculated according to data which came from Refresh event.
        /// </summary>
        public int TotalRowCount { get; internal set; }
        /// <summary>
        /// Is loading enable?
        /// </summary>
        internal bool Loading { get; set; } = true;
        /// <summary>
        /// Loading style by loading enable.
        /// </summary>
        internal string LoadingStyle
        {
            get
            {
                return this.Loading ? "opacity:0.5;pointer-events:none;" : "";
            }
        }

        #region For Paging
        /// <summary>
        /// How many number should be in paging section?
        /// It's calculated according to <see cref="TotalRowCount"/>, <see cref="PageIndex"/> and <see cref="RowCount"/>.
        /// Also, this value will multiply 2 while calculating.
        /// </summary>
        internal const byte PageRange = 3;
        /// <summary>
        /// Is previous page button enable?
        /// </summary>
        internal bool PreviousPageEnable
        {
            get
            {
                return this.PageIndex > 0;
            }
        }
        /// <summary>
        /// Is go first page button enable?
        /// </summary>
        internal bool GoFirstPageEnable
        {
            get
            {
                return this.PageIndex - PageRange > 0;
            }
        }
        /// <summary>
        /// Is next page button enable?
        /// </summary>
        internal bool NextPageEnable
        {
            get
            {
                return this.PageIndex < this.MaxPageIndex;
            }
        }
        /// <summary>
        /// Is go last page button enable?
        /// </summary>
        internal bool GoLastPageEnable
        {
            get
            {
                return this.PageIndex + PageRange < this.MaxPageIndex;
            }
        }
        /// <summary>
        /// Which number does paging start from?
        /// </summary>
        internal int PageStart
        {
            get
            {
                return this.PageIndex - PageRange - (Math.Max(0, this.PageIndex + PageRange - this.MaxPageIndex)) - (this.GoLastPageEnable ? 0 : 1) - (this.NextPageEnable ? 0 : 1);
            }
        }
        /// <summary>
        /// Which number is paging's last number?
        /// </summary>
        internal int PageEnd
        {
            get
            {
                return this.PageIndex + PageRange + (Math.Max(0, 0 - (this.PageIndex - PageRange))) + (this.GoFirstPageEnable ? 0 : 1) + (this.PreviousPageEnable ? 0 : 1);
            }
        }
        #endregion

        /// <summary>
        /// Is search bar enable?
        /// </summary>
        public bool SearchEnable { get; internal set; }
        /// <summary>
        /// You can change default <see cref="SearchLabel"/> by setting this.
        /// </summary>
        public static string SearchLabelStatic { get; set; } = "Search : ";
        /// <summary>
        /// What is search input's label?
        /// </summary>
        public string SearchLabel { get; internal set; }
        /// <summary>
        /// Search value which is used on refreshing.
        /// It should be used to filter by developer who is using Dmuka3.
        /// </summary>
        public string SearchValue { get; internal set; } = "";
        /// <summary>
        /// This is triggered by "dom-event" if you use a string name.
        /// </summary>
        public Dictionary<string, Func<Dmuka3TableModel, string, string, Task>> ColumnEventsAsync { get; internal set; }
        /// <summary>
        /// This is triggered by "dom-event" if you use a string name.
        /// </summary>
        public Dictionary<string, Action<Dmuka3TableModel, string, string>> ColumnEvents { get; internal set; }

        /// <summary>
        /// This function provides you to get datas by paging and filtering.
        /// </summary>
        public Func<Dmuka3TableModel, Task<(IEnumerable<object> rows, int totalRowCount)>> OnRefreshAsync { get; set; }
        private Func<Dmuka3TableModel, (IEnumerable<object> rows, int totalRowCount)> onRefresh = null;
        /// <summary>
        /// This function provides you to get datas by paging and filtering.
        /// </summary>
        public Func<Dmuka3TableModel, (IEnumerable<object> rows, int totalRowCount)> OnRefresh
        {
            get
            {
                return this.onRefresh;
            }
            set
            {
                this.onRefresh = value;
                if (this.onRefresh == null)
                    return;

                this.OnRefreshAsync = async (model) =>
                {
                    var result = await Task.Run<(IEnumerable<object> rows, int totalRowCount)>(() =>
                    {
                        return this.onRefresh(model);
                    });

                    return result;
                };
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// This model is used for Dmuka3Table.razor to receive and send datas between Dmuka3Table and other component which uses Dmuka3Table.
        /// </summary>
        /// <param name="parent">
        /// Which component uses Dmuka3Table?
        /// </param>
        /// <param name="uniqueKey">
        /// What is the unique key(like ID) in a row?
        /// This is a property name which must be in row.
        /// </param>
        /// <param name="columns">
        /// Table's columns.
        /// </param>
        /// <param name="pageIndex">
        /// Which page is browser user on?
        /// </param>
        /// <param name="singleSort">
        /// This means if you set value to true, it avoid multiple sorting.
        /// This decides what is going to happen when user clicks a column.
        /// </param>
        /// <param name="rowCountOptions">
        /// How many options are there for user to change row count.
        /// </param>
        /// <param name="rowCountLabel">
        /// Label which is near to Row Count Select.
        /// </param>
        /// <param name="searchLabel">
        /// What is search input's label?
        /// </param>
        /// <param name="searchEnable">
        /// Is search bar enable?
        /// </param>
        /// <param name="columnEventsAsync">
        /// This is triggered by "dom-event" if you use a string name.
        /// </param>
        /// <param name="columnEvents">
        /// This is triggered by "dom-event" if you use a string name.
        /// </param>
        /// <param name="onRefreshAsync">
        /// This function provides you to get datas by paging and filtering.
        /// </param>
        /// <param name="onRefresh">
        /// This function provides you to get datas by paging and filtering.
        /// </param>
        public Dmuka3TableModel(
            ComponentBase parent,
            string uniqueKey,
            Column[] columns,
            int pageIndex = 0,
            bool singleSort = true,
            int[] rowCountOptions = null,
            string rowCountLabel = null,
            string searchLabel = null,
            bool searchEnable = true,
            Dictionary<string, Func<Dmuka3TableModel, string, string, Task>> columnEventsAsync = null,
            Dictionary<string, Action<Dmuka3TableModel, string, string>> columnEvents = null,
            Func<Dmuka3TableModel, Task<(IEnumerable<object> rows, int totalRowCount)>> onRefreshAsync = null,
            Func<Dmuka3TableModel, (IEnumerable<object> rows, int totalRowCount)> onRefresh = null
            )
        {
            this.Parent = parent;
            this.UniqueKey = uniqueKey;
            this.Columns = columns;
            this.SingleSort = singleSort;
            this.PageIndex = pageIndex;
            this.RowCountOptions = rowCountOptions ?? RowCountOptionsStatic;
            this.RowCount = this.RowCountOptions[0];
            if (string.IsNullOrEmpty(rowCountLabel))
                this.RowCountLabel = RowCountLabelStatic;
            else
                this.RowCountLabel = rowCountLabel;
            if (string.IsNullOrEmpty(searchLabel))
                this.SearchLabel = SearchLabelStatic;
            else
                this.SearchLabel = searchLabel;
            this.SearchEnable = searchEnable;
            this.ColumnEventsAsync = columnEventsAsync;
            this.ColumnEvents = columnEvents;
            this.OnRefreshAsync = onRefreshAsync;
            this.OnRefresh = onRefresh;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Refresh table rows.
        /// </summary>
        /// <returns></returns>
        public async Task RefreshAsync()
        {
            await this.Table.RefreshAsync();
        }

        /// <summary>
        /// Refresh table rows.
        /// </summary>
        public void Refresh()
        {
            this.Table.Refresh();
        }
        #endregion
    }
}
