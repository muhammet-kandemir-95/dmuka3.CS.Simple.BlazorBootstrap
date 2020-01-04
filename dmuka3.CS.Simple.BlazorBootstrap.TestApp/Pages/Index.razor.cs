using dmuka3.CS.Simple.BlazorBootstrap.TestApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dmuka3.CS.Simple.BlazorBootstrap.TestApp.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        protected string SearchId = "";
        protected string SearchName = "";
        protected string SearchSurname = "";

        protected Dmuka3TableModel _tableModel = null;
        protected Dmuka3TableModel TableModel
        {
            get
            {
                if (this._tableModel == null)
                    this._tableModel = new Dmuka3TableModel(
                        parent: this,
                        uniqueKey: "id",
                        rowCountOptions: new int[] { 5, 10, 20 },
                        columns: new Dmuka3TableModel.Column[]
                        {
                            new Dmuka3TableModel.Column("id", "User Id", Dmuka3TableModel.SortType.None),
                            new Dmuka3TableModel.Column("name", "Name", Dmuka3TableModel.SortType.Asc),
                            new Dmuka3TableModel.Column("surname", "Surname", Dmuka3TableModel.SortType.None),
                            new Dmuka3TableModel.Column("#")
                        },
                        columnEventsAsync: new Dictionary<string, Func<Dmuka3TableModel, string, string, Task>>()
                        {
                            {
                                "test",
                                async (m, id, json) =>
                                {
                                    await Dmuka3Helper.AlertJS(this.JSRuntime, id);
                                }
                            }
                        },
                        onRefreshAsync: async (Dmuka3TableModel m) =>
                        {
                            await Task.Delay(1000);
                            var result = TestDataModel.Rows;

                            result = result
                                        .Where(o => o.id.ToString().Contains(SearchId))
                                        .Where(o => o.name.Contains(SearchName))
                                        .Where(o => o.surname.Contains(SearchSurname))
                                        .Where(o => o.id.ToString().Contains(m.SearchValue) || o.name.Contains(m.SearchValue) || o.surname.Contains(m.SearchValue))
                                        .ToList();
                            var totalRowCount = result.Count;

                            IOrderedEnumerable<object> ieresultOrder = null;
                            foreach (var col in m.Columns)
                            {
                                if (col.Sortable == false)
                                    continue;

                                if (ieresultOrder == null)
                                {
                                    if (col.SortType == Dmuka3TableModel.SortType.Asc)
                                        ieresultOrder = result.OrderBy(o => o.GetType().GetProperty(col.Name).GetValue(o));
                                    else if (col.SortType == Dmuka3TableModel.SortType.Desc)
                                        ieresultOrder = result.OrderByDescending(o => o.GetType().GetProperty(col.Name).GetValue(o));
                                }
                                else
                                {
                                    if (col.SortType == Dmuka3TableModel.SortType.Asc)
                                        ieresultOrder = ieresultOrder.ThenBy(o => o.GetType().GetProperty(col.Name).GetValue(o));
                                    else if (col.SortType == Dmuka3TableModel.SortType.Desc)
                                        ieresultOrder = ieresultOrder.ThenByDescending(o => o.GetType().GetProperty(col.Name).GetValue(o));
                                }
                            }
                            if (ieresultOrder == null)
                                ieresultOrder = result.OrderBy(o => true);

                            var ieresult = ieresultOrder.Skip(m.PageIndex * m.RowCount).Take(m.RowCount);

                            return (rows: ieresult, totalRowCount: totalRowCount);
                        });

                return this._tableModel;
            }
        }

        protected Dmuka3MaskModel _maskModel = null;
        protected Dmuka3MaskModel MaskModel
        {
            get
            {
                if (this._maskModel == null)
                    this._maskModel = new Dmuka3MaskModel(
                        pattern: "99.99.9999 99:99:99",
                        value: "11.08.2019 0",
                        requiredFilling: true,
                        onChange: m =>
                        {
                            this.test = m.Value;
                            this.StateHasChanged();
                        });
                return this._maskModel;
            }
        }

        protected Dmuka3NumberModel _numberModel = null;
        protected Dmuka3NumberModel NumberModel
        {
            get
            {
                if (this._numberModel == null)
                    this._numberModel = new Dmuka3NumberModel(
                        value: 123456.789m,
                        format: true,
                        decimalPlaces: 5,
                        formatCharacters: new char[] { ',', '.' },
                        onChange: m =>
                        {
                            this.test = (m.Value ?? 0).ToString();
                            this.StateHasChanged();
                        });
                return this._numberModel;
            }
        }
    }
}
