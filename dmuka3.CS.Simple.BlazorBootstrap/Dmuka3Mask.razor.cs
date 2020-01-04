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

        /// <summary>
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
        /// </summary>
        [Parameter]
        public string Pattern { get; set; }

        /// <summary>
        /// Required completely filling.
        /// </summary>
        [Parameter]
        public bool RequiredFilling { get; set; }

        private string _defaultValue = "";
        /// <summary>
        /// Input's default value for first instance.
        /// </summary>
        [Parameter]
        public string DefaultValue
        {
            get
            {
                return this._defaultValue;
            }
            set
            {
                this._defaultValue = value;
            }
        }

        /// <summary>
        /// Change event.
        /// </summary>
        [Parameter]
        public Func<Dmuka3Mask, Task> OnChangeAsync { get; set; }
        private Action<Dmuka3Mask> _onChange = null;

        /// <summary>
        /// Change event.
        /// </summary>
        [Parameter]
        public Action<Dmuka3Mask> OnChange
        {
            get
            {
                return this._onChange;
            }
            set
            {
                this._onChange = value;
                if (this._onChange == null)
                    this.OnChangeAsync = null;
                else
                    this.OnChangeAsync = async (m) =>
                    {
                        this._onChange(m);
                    };
            }
        }
        #endregion

        /// <summary>
        /// Javascript runtime.
        /// </summary>
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        private string _value = "";
        /// <summary>
        /// Input's value.
        /// </summary>
        public string Value
        {
            get
            {
                return this._value;
            }
            private set
            {
                this._value = value;
                if (this.RequiredFilling && this._value.Length != this.Pattern.Length)
                    this._value = "";
            }
        }

        /// <summary>
        /// It is for managing masks' ids.
        /// </summary>
        protected static ulong smaskId = 0;
        /// <summary>
        /// Mask's unique id.
        /// </summary>
        protected ulong maskId = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Dmuka3Table component.
        /// </summary>
        public Dmuka3Mask()
        {
            this.maskId = smaskId++;
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
            await JSRuntime.InvokeVoidAsync("Dmuka3Mask.Load", new object[] { this.maskId, this.Pattern, this.RequiredFilling, this.DefaultValue });
        }

        /// <summary>
        /// To validate last value by pattern.
        /// </summary>
        /// <returns></returns>
        public async Task ValidateAsync()
        {
            await JSRuntime.InvokeVoidAsync("Dmuka3Mask.Validate", new object[] { this.maskId });
        }

        /// <summary>
        /// Input's change event.
        /// </summary>
        /// <param name="e">Event parameter.</param>
        /// <returns></returns>
        protected async Task onChange(ChangeEventArgs e)
        {
            this.Value = (string)e.Value;

            if (this.OnChangeAsync != null)
                await this.OnChangeAsync(this);
        }

        /// <summary>
        /// Set a value.
        /// </summary>
        /// <param name="value">
        /// Input's value.
        /// </param>
        /// <returns></returns>
        public async Task SetValueAsync(string value)
        {
            this.Value = value;
            await JSRuntime.InvokeVoidAsync("Dmuka3Mask.Set", new object[] { this.maskId, this.Value });
        }
        #endregion
    }
}
