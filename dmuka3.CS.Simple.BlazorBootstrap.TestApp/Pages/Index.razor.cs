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

        protected Dmuka3TableModel _model = null;
        protected Dmuka3TableModel Model
        {
            get
            {
                if (this._model == null)
                    this._model = new Dmuka3TableModel(
                        parent: this,
                        uniqueKey: "id",
                        columns: new Dmuka3TableModel.Column[]
                        {
                            new Dmuka3TableModel.Column("id", "User Id", Dmuka3TableModel.SortType.None),
                            new Dmuka3TableModel.Column("name", "Name", Dmuka3TableModel.SortType.Asc),
                            new Dmuka3TableModel.Column("surname", "Surname", Dmuka3TableModel.SortType.None),
                            new Dmuka3TableModel.Column("#")
                        },
                        onRefresh: (Dmuka3TableModel m) =>
                        {
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

                return this._model;
            }
        }
    }
}
