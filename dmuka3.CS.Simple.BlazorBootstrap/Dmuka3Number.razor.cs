using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace dmuka3.CS.Simple.BlazorBootstrap
{
    /// <summary>
    /// Dmuka3Number component.
    /// </summary>
    public partial class Dmuka3Number : ComponentBase
    {
        #region Variables
        #region Parameters
        private Dmuka3NumberModel _model = null;
        /// <summary>
        /// <see cref="Dmuka3NumberModel"/> is for communicating.
        /// </summary>
        [Parameter]
        public Dmuka3NumberModel Model
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
                this._model.Number = this;
            }
        }

        /// <summary>
        /// Number's css classes on "class" attribute.
        /// </summary>
        [Parameter]
        public string Class { get; set; }

        private Dictionary<string, object> _attributes = new Dictionary<string, object>();
        /// <summary>
        /// Number's attributes.
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
        /// Input's value.
        /// </summary>
        public decimal? Value
        {
            get
            {
                return this.Model.Value;
            }
            set
            {
                this.Model.Value = value;
            }
        }

        /// <summary>
        /// Javascript runtime.
        /// </summary>
        [Inject]
        internal IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// It is for managing numbers' ids.
        /// </summary>
        protected static ulong snumberId = 0;
        /// <summary>
        /// Number's unique id.
        /// </summary>
        internal ulong NumberId = 0;
        /// <summary>
        /// Have component been loaded?
        /// </summary>
        internal bool Loaded = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Dmuka3Number component.
        /// </summary>
        public Dmuka3Number()
        {
            this.NumberId = snumberId++;
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
                await this.JSRuntime.InvokeVoidAsync("Dmuka3Number.Load", new object[] { this.NumberId, this.Model.Format, this.Model.FormatCharacters, this.Model.DecimalPlaces, this.Model.ValueAsString });
                this.Loaded = true;

                if (this.Model.OnChangeAsync != null)
                    await this.Model.OnChangeAsync(this);
            }
        }

        /// <summary>
        /// Input's change event.
        /// </summary>
        /// <param name="e">Event parameter.</param>
        /// <returns></returns>
        protected async Task onChange(ChangeEventArgs e)
        {
            var valueStr = ((string)e.Value);
            if (valueStr == "")
                this.Model.Value = null;
            else
            {
                valueStr = valueStr
                                .Replace(this.Model.FormatCharacters[0].ToString(), "")
                                .Replace(this.Model.FormatCharacters[1], '.');

                try
                {
                    this.Value = Convert.ToDecimal(valueStr, CultureInfo.InvariantCulture);
                }
                catch
                {
                    this.Value = null;
                }
            }

            if (this.Model.OnChangeAsync != null)
                await this.Model.OnChangeAsync(this);

            Dmuka3Helper.StateHasChanged(this.Model.Parent);
        }
        #endregion
    }
}
