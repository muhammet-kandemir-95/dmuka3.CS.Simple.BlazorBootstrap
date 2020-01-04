using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dmuka3.CS.Simple.BlazorBootstrap
{
    /// <summary>
    /// Dmuka3Mask component.
    /// </summary>
    public partial class Dmuka3Mask : ComponentBase
    {
        #region Variables
        #region Parameters
        private Dmuka3MaskModel _model = null;
        /// <summary>
        /// <see cref="Dmuka3MaskModel"/> is for communicating.
        /// </summary>
        [Parameter]
        public Dmuka3MaskModel Model
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
                this._model.Mask = this;
            }
        }

        /// <summary>
        /// Mask's css classes on "class" attribute.
        /// </summary>
        [Parameter]
        public string Class { get; set; }

        private Dictionary<string, object> _attributes = new Dictionary<string, object>();
        /// <summary>
        /// Mask's attributes.
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
        public string Value
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
        /// It is for managing masks' ids.
        /// </summary>
        protected static ulong smaskId = 0;
        /// <summary>
        /// Mask's unique id.
        /// </summary>
        internal ulong MaskId = 0;
        /// <summary>
        /// Have component been loaded?
        /// </summary>
        internal bool Loaded = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Dmuka3Mask component.
        /// </summary>
        public Dmuka3Mask()
        {
            this.MaskId = smaskId++;
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
                await this.JSRuntime.InvokeVoidAsync("Dmuka3Mask.Load", new object[] { this.MaskId, this.Model.Pattern, this.Model.RequiredFilling, this.Model.Value, this.Model.PreviousValue });
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
            this.Model.Value = (string)e.Value;

            if (this.Model.OnChangeAsync != null)
                await this.Model.OnChangeAsync(this);

            Dmuka3Helper.StateHasChanged(this.Model.Parent);
        }
        #endregion
    }
}
