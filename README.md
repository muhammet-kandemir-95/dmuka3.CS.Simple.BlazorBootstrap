# dmuka3.CS.Simple.BlazorBootstrap

 This library provides you to make somethings(Making a table, a mask input, a number input, ...) easily on Bootstrap 4+.
 
 ## Nuget
 
 **Link** : https://www.nuget.org/packages/dmuka3.CS.Simple.BlazorBootstrap
 ```nuget
 Install-Package dmuka3.CS.Simple.BlazorBootstrap
 ```
 
## Script

 You must add this link in end of body.

```html
<script src="_content/dmuka3.CS.Simple.BlazorBootstrap/general.js"></script>
```
 
## dmuka3.CS.Simple.BlazorBootstrap.Dmuka3Table

 This component creates a HTML Table by bootstrap. Let's look at how to use;
 
 First of all, we have to create a list to show on table.
 
```csharp
public class TestDataModel
{
  public int id { get; set; }
  public string name { get; set; }
  public string surname { get; set; }

  public static List<TestDataModel> Rows = new List<TestDataModel>();

  static TestDataModel()
  {
    for (int i = 0; i < 41; i++)
    {
      Rows.Add(new TestDataModel()
      {
        id = i,
        name = "User Name " + i,
        surname = "User Surname " + i
      });
    }
  }
}
```

 Secondly, we need to create a **Dmuka3TableModel** for communicating between Dmuka3TableComponent and other component which uses this model.
 
```csharp
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
}
```

 You may say it's confusing. Don't worry! You just need to more information to understand. To begin with, you can look at comments by moving your cursor to over what you want to learn. If you don't understand still, let's check it together;
 
```csharp
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
/// <param name="rowCount">
/// Max row count for each page.
/// This is filled by user or while instance.
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
    int rowCount = -1,
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
```

 There are many descriptions above. You can also change default values on some fields.
 
|Field|Where is default value at?|Default Value|
|:--:|:--:|:--:|
|rowCountOptions|Dmuka3TableModel.RowCountOptionsStatic|new int[] { 10, 20, 50, 100 }|
|rowCountLabel|Dmuka3TableModel.RowCountLabelStatic|"Row Count : "|
|searchLabel|Dmuka3TableModel.SearchLabelStatic|"Search : "|
 
 Let's move on example. Now, we will use our model on razor;
 
```razor
<Dmuka3Table Model="this.TableModel" Class="my-table-class" Attributes="@(new Dictionary<string, object> { { "width", "100%" } })">
    <tr data-body data-id="{{id}}">
        <td class="test">{{id}}</td>
        <td>{{name}}</td>
        <td>{{surname}}</td>
        <td>
            <button dom-events="{
                    'click' : 'test'
                }">
                Show
            </button>
        </td>
    </tr>
    <tr data-foot>
        <td>
            <input @oninput="(async e =>
                             {
                                 SearchId = (string)e.Value;
                                 this.TableModel.PageIndex = 0;
                                 await this.TableModel.RefreshAsync();
                             })" />
        </td>
        <td>
            <input @oninput="(async e =>
                             {
                                 SearchName = (string)e.Value;
                                 this.TableModel.PageIndex = 0;
                                 await this.TableModel.RefreshAsync();
                             })" />
        </td>
        <td>
            <input @oninput="(async e =>
                             {
                                 SearchSurname = (string)e.Value;
                                 this.TableModel.PageIndex = 0;
                                 await this.TableModel.RefreshAsync();
                             })" />
        </td>
        <td></td>
    </tr>
</Dmuka3Table>
```
 There are two difference table row.
 
### data-body

 This is used for each row to show them on browser. You have to decide how it will be shown. As you can see, there are many "{{variable}}" usages. We can't use blazor commands here. Because of this, we couldn't manage this rows dinamically on blazor. We have to do it on a another way. You don't need to worry about that. It is so easy to use. Just write your property name to where you want.
 
  But you may need to use DOM events like click. So you have to use "**dom-events**" attribute. There are two ways to use this.
  
#### 'event-name': function (id, row, e)

 You have to declare an event on C#. You can run a javascript code using a function.
 
#### 'event-name': 'Dmuka3Table.ColumnEvents.Name' / 'Dmuka3Table.ColumnEventsAsync.Name'

 If you use that, when an event which is declared here is triggered automaticaly on c# by name. You can look at examples to understand easier.
 
### data-foot

 It is usually used for searching. It is added to footer of table after rendered. You can use all of blazor commands here.

 There are also two new attributes.

### Class

 It is for changing table class.
 
### Attributes

 It is for adding new attributes to table.

## dmuka3.CS.Simple.BlazorBootstrap.Dmuka3Mask

 This component creates a HTML Mask Input by bootstrap. Let's look at how to use;
 
 First of all, we need to create a **Dmuka3TableMask** for communicating between Dmuka3MaskComponent and other component which uses this model.
 
```csharp
public partial class Index : ComponentBase
{
    [Inject]
    IJSRuntime JSRuntime { get; set; }

    protected Dmuka3MaskModel _maskModel = null;
    protected Dmuka3MaskModel MaskModel
    {
        get
        {
            if (this._maskModel == null)
                this._maskModel = new Dmuka3MaskModel(
                    parent: this,
                    pattern: "99.99.9999 99:99:99",
                    value: "11.08.1995 13:00:00",
                    requiredFilling: true);

            return this._maskModel;
        }
    }
}
```

 To begin with, you can look at comments by moving your cursor to over what you want to learn. If you don't understand still, let's check it together;

```csharp
/// <summary>
/// This model is used for Dmuka3Mask.razor to receive and send datas between Dmuka3Mask and other component which uses Dmuka3Mask.
/// </summary>
/// <param name="parent">
/// Which component uses Dmuka3Mask?
/// </param>
/// <param name="pattern">
/// ?    = All Characters
/// <para></para>
/// 9    = Only Number
/// <para></para>
/// a, A = Only Letter Insensitive
/// <para></para>
/// l    = Only Lower Letter
/// <para></para>
/// L    = Only Upper Letter
/// <para></para>
/// Examples = ["99.99.9999 99:99", "L-99", "??LL99-AAA", ...]
/// </param>
/// <param name="value">
/// Input's value.
/// </param>
/// <param name="requiredFilling">
/// Required completely filling.
/// </param>
/// <param name="onChange">
/// Change event.
/// </param>
/// <param name="onChangeAsync">
/// Change event.
/// </param>
public Dmuka3MaskModel(
    ComponentBase parent,
    string pattern,
    string value = "",
    bool requiredFilling = false,
    Action<Dmuka3Mask> onChange = null,
    Func<Dmuka3Mask, Task> onChangeAsync = null
    )
```

 There are many descriptions above. Let's move on example. Now, we will use our model on razor;
 
```razor
<Dmuka3Mask Model="this.MaskModel" Class="my-mask-class" Attributes="@(new Dictionary<string, object> { { "width", "100%" } })"></Dmuka3Mask>
```

 Here is a question. How can I get the value? You just need to use "**Model.Value**".
 
```csharp
var maskValue = this.MaskModel.Value;
```
