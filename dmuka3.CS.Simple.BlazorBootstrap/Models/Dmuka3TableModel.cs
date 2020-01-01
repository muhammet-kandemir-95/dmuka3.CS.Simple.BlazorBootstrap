using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dmuka3.CS.Simple.BlazorBootstrap
{
    public class Dmuka3TableModel
    {
        #region Enums
        public enum SortType
        {
            None,
            Asc,
            Desc
        }
        #endregion

        #region Classes
        public class Column
        {
            #region Variables
            public string Name { get; internal set; }
            public string Description { get; internal set; }
            public SortType SortType { get; internal set; }
            public bool Sortable { get; internal set; }
            #endregion

            #region Constructors
            public Column(string description)
            {
                this.Sortable = false;
                this.Name = null;
                this.Description = description;
            }

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
        public ComponentBase Parent { get; internal set; }
        public Dmuka3Table Table { get; internal set; }

        public string UniqueKey { get; internal set; }
        public Column[] Columns { get; internal set; }
        public int PageIndex { get; set; }
        public int MaxPageIndex
        {
            get
            {
                return Math.Max(0, (this.TotalRowCount / RowCount + (this.TotalRowCount % RowCount == 0 ? 0 : 1)) - 1);
            }
        }
        public int RowCount { get; set; }
        public static string RowCountLabelStatic { get; set; } = "Row Count : ";
        public string RowCountLabel { get; internal set; }
        public int TotalRowCount { get; internal set; }

        #region For Paging
        internal const byte PageRange = 3;
        internal bool PreviousPageEnable
        {
            get
            {
                return this.PageIndex > 0;
            }
        }

        internal bool GoFirstPageEnable
        {
            get
            {
                return this.PageIndex - PageRange > 0;
            }
        }

        internal bool NextPageEnable
        {
            get
            {
                return this.PageIndex < this.MaxPageIndex;
            }
        }

        internal bool GoLastPageEnable
        {
            get
            {
                return this.PageIndex + PageRange < this.MaxPageIndex;
            }
        }

        internal int PageStart
        {
            get
            {
                return this.PageIndex - PageRange - (Math.Max(0, this.PageIndex + PageRange - this.MaxPageIndex)) - (this.GoLastPageEnable ? 0 : 1) - (this.NextPageEnable ? 0 : 1);
            }
        }

        internal int PageEnd
        {
            get
            {
                return this.PageIndex + PageRange + (Math.Max(0, 0 - (this.PageIndex - PageRange))) + (this.GoFirstPageEnable ? 0 : 1) + (this.PreviousPageEnable ? 0 : 1);
            }
        }
        #endregion

        public bool SearchEnable { get; internal set; }
        public static string SearchLabelStatic { get; set; } = "Search : ";
        public string SearchLabel { get; internal set; }
        public string SearchValue { get; internal set; } = "";
        public Dictionary<string, Func<Dmuka3TableModel, string, string, Task>> ColumnEventsAsync { get; internal set; }
        public Dictionary<string, Action<Dmuka3TableModel, string, string>> ColumnEvents { get; internal set; }

        public Func<Dmuka3TableModel, Task<(IEnumerable<object> rows, int totalRowCount)>> OnRefreshAsync { get; set; }
        private Func<Dmuka3TableModel, (IEnumerable<object> rows, int totalRowCount)> onRefresh = null;
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
        public Dmuka3TableModel(
            ComponentBase parent,
            string uniqueKey,
            Column[] columns,
            int pageIndex = 0,
            int rowCount = 10,
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
            this.PageIndex = pageIndex;
            this.RowCount = rowCount;
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
        public async Task RefreshAsync()
        {
            await this.Table.RefreshAsync();
        }

        public void Refresh()
        {
            this.Table.Refresh();
        }
        #endregion
    }
}
