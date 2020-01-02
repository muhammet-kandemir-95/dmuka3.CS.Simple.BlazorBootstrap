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
    public partial class Dmuka3Table : ComponentBase, IDisposable
    {
        #region Variables
        #region Parameters
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public Dmuka3TableModel Model { get; set; }

        [Parameter]
        public string Class { get; set; }

        [Parameter]
        public Dictionary<string, object> Attributes { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }
        #endregion

        protected static ulong stableId = 0;
        protected ulong tableId = 0;
        protected static ConcurrentDictionary<ulong, Dmuka3Table> instances = new ConcurrentDictionary<ulong, Dmuka3Table>();
        #endregion

        #region Methods
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.tableId = stableId++;
                instances.AddOrUpdate(this.tableId, this, (id, val) =>
                {
                    return this;
                });

                this.Model.Table = this;

                await JSRuntime.InvokeAsync<bool>("Dmuka3Table.Load", new object[] { this.tableId });

                await this.RefreshAsync();
            }
        }

        public void Dispose()
        {
            instances.TryRemove(this.tableId, out _);
        }

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

        public async Task RefreshAsync()
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

            await JSRuntime.InvokeAsync<bool>("Dmuka3Table.Fill", new object[] { this.tableId, rows });

            this.StateHasChanged();
        }

        public void Refresh()
        {
            this.RefreshAsync().RunSynchronously();
        }

        protected async Task clickColumn(Dmuka3TableModel.Column column)
        {
            if (column.Sortable == false)
                return;

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