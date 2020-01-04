using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace dmuka3.CS.Simple.BlazorBootstrap
{
    /// <summary>
    /// Dmuka3Table component.
    /// </summary>
    public partial class Dmuka3Table : ComponentBase, IDisposable
    {
        #region Variables
        #region Parameters
        /// <summary>
        /// Child content.
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        private Dmuka3TableModel _model = null;
        /// <summary>
        /// <see cref="Dmuka3TableModel"/> is for communicating.
        /// </summary>
        [Parameter]
        public Dmuka3TableModel Model
        {
            get
            {
                return this._model;
            }
            set
            {
                if (value == null)
                    throw new NullReferenceException();

                this._model = value;
                this._model.Table = this;
            }
        }

        /// <summary>
        /// Table's css classes on "class" attribute.
        /// </summary>
        [Parameter]
        public string Class { get; set; }

        private Dictionary<string, object> _attributes = new Dictionary<string, object>();
        /// <summary>
        /// Table's attributes.
        /// </summary>
        [Parameter]
        public Dictionary<string, object> Attributes
        {
            get
            {
                return this._attributes;
            }
            set
            {
                if (value == null)
                    throw new NullReferenceException();

                this._attributes = value;
            }
        }
        #endregion

        /// <summary>
        /// Javascript runtime.
        /// </summary>
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// It is for managing tables' ids.
        /// </summary>
        protected static ulong stableId = 0;
        /// <summary>
        /// Table's unique id.
        /// </summary>
        protected ulong tableId = 0;
        /// <summary>
        /// This stores all instances to manage callbacks.
        /// If an instance destroys, it will be deleted from here automatically.
        /// </summary>
        protected static ConcurrentDictionary<ulong, Dmuka3Table> instances = new ConcurrentDictionary<ulong, Dmuka3Table>();
        #endregion

        #region Constructors
        /// <summary>
        /// Dmuka3Table component.
        /// </summary>
        public Dmuka3Table()
        {
            this.tableId = stableId++;
            instances.AddOrUpdate(this.tableId, this, (id, val) =>
            {
                return this;
            });
        }
        #endregion

        #region Methods
        /// <summary>
        /// OnAfterRenderAsync method.
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("Dmuka3Table.Load", new object[] { this.tableId });
                await this.RefreshAsync();
            }
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        public void Dispose()
        {
            instances.TryRemove(this.tableId, out _);
        }

        /// <summary>
        /// This method is triggered just while callbacks from client.
        /// </summary>
        /// <param name="tableId">
        /// Instance unique id.
        /// </param>
        /// <param name="type">
        /// Callback's type.
        /// </param>
        /// <param name="name">
        /// Callback's process name.
        /// </param>
        /// <param name="id">
        /// Callback's process id.
        /// </param>
        /// <param name="json">
        /// Callback's json.
        /// </param>
        /// <returns></returns>
        [JSInvokable("Dmuka3Table.JSEvent")]
        public static async Task JSEvent(ulong tableId, string type, string name, string id, string json)
        {
            var instance = instances[tableId];
            switch (type)
            {
                case "dmuka3-table-dom-events":
                    {
                        Func<Dmuka3TableModel, string, string, Task> actionAsync;
                        Action<Dmuka3TableModel, string, string> action;
                        if (instance.Model.ColumnEvents != null && instance.Model.ColumnEvents.TryGetValue(name, out action))
                        {
                            action(instance.Model, id, json);

                            Dmuka3Helper.StateHasChanged(instance.Model.Table);
                            Dmuka3Helper.StateHasChanged(instance.Model.Parent);
                        }
                        else if (instance.Model.ColumnEventsAsync != null && instance.Model.ColumnEventsAsync.TryGetValue(name, out actionAsync))
                        {
                            await actionAsync(instance.Model, id, json);
                            Dmuka3Helper.StateHasChanged(instance.Model.Table);
                            Dmuka3Helper.StateHasChanged(instance.Model.Parent);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// This function provides to fill table by datas which are sended by developer who is using Dmuka3.
        /// </summary>
        /// <returns></returns>
        public async Task RefreshAsync()
        {
            this.Model.Loading = true;
            this.StateHasChanged();

            try
            {
                var result = await this.Model.OnRefreshAsync(this.Model);

                this.Model.TotalRowCount = result.totalRowCount;

                var rows = new List<Dictionary<string, string>>();

                foreach (var item in result.rows)
                {
                    var type = item.GetType();
                    var props = type.GetProperties();

                    Dictionary<string, string> row = new Dictionary<string, string>();
                    foreach (var prop in props)
                        row.Add(prop.Name, (prop.GetValue(item) ?? "").ToString());

                    rows.Add(row);
                }

                await JSRuntime.InvokeVoidAsync("Dmuka3Table.Fill", new object[] { this.tableId, rows });
                this.Model.Loading = false;
                this.StateHasChanged();
            }
            catch
            {
                this.Model.Loading = false;
                this.StateHasChanged();
                throw;
            }
        }

        /// <summary>
        /// To set next sort type to column.
        /// </summary>
        /// <param name="column">
        /// Which column will be used?
        /// </param>
        /// <returns></returns>
        protected async Task clickColumn(Dmuka3TableModel.Column column)
        {
            if (column.Sortable == false)
                return;

            if (this.Model.SingleSort)
                foreach (var col in this.Model.Columns)
                    if (col != column)
                        col.SortType = Dmuka3TableModel.SortType.None;

            if (column.SortType == Dmuka3TableModel.SortType.None)
                column.SortType = Dmuka3TableModel.SortType.Asc;
            else if (column.SortType == Dmuka3TableModel.SortType.Asc)
                column.SortType = Dmuka3TableModel.SortType.Desc;
            else if (column.SortType == Dmuka3TableModel.SortType.Desc)
                column.SortType = Dmuka3TableModel.SortType.None;

            await this.RefreshAsync();
        }
        #endregion
    }
}