using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dmuka3.CS.Simple.BlazorBootstrap
{
    /// <summary>
    /// This model is used for Dmuka3Mask.razor to receive and send datas between Dmuka3Mask and other component which uses Dmuka3Mask.
    /// </summary>
    public class Dmuka3MaskModel
    {
        #region Variables
        /// <summary>
        /// Which component uses Dmuka3Mask?
        /// </summary>
        public ComponentBase Parent { get; internal set; }
        /// <summary>
        /// which Dmuka3Mask uses this class?
        /// </summary>
        public Dmuka3Mask Mask { get; internal set; }

        private string _pattern = "";
        /// <summary>
        /// ?    = All Characters.
        /// <para></para>
        /// 9    = Only Number.
        /// <para></para>
        /// a, A = Only Letter Insensitive.
        /// <para></para>
        /// l    = Only Lower Letter.
        /// <para></para>
        /// L    = Only Upper Letter.
        /// <para></para>
        /// Examples = ["99.99.9999 99:99", "L-99", "??LL99-AAA", ...].
        /// </summary>
        public string Pattern
        {
            get
            {
                return this._pattern;
            }
            private set
            {
                this._pattern = value ?? "";
            }
        }

        /// <summary>
        /// Required completely filling.
        /// </summary>
        public bool RequiredFilling { get; private set; }

        /// <summary>
        /// Change event.
        /// </summary>
        public Func<Dmuka3Mask, Task> OnChangeAsync { get; set; }

        private Action<Dmuka3Mask> _onChange = null;
        /// <summary>
        /// Change event.
        /// </summary>
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

        /// <summary>
        /// Store previous value before formatting.
        /// </summary>
        internal string PreviousValue { get; set; }
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
            set
            {
                this.PreviousValue = value ?? "";
                this._value = this.PreviousValue;
                if (this.RequiredFilling && this._value.Length != this.Pattern.Length)
                    this._value = "";

                if (this.Mask != null && this.Mask.Loaded)
                    this.Mask.JSRuntime.InvokeVoidAsync("Dmuka3Mask.Set", new object[] { this.Mask.MaskId, this._value, this.PreviousValue });
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// This model is used for Dmuka3Mask.razor to receive and send datas between Dmuka3Mask and other component which uses Dmuka3Mask.
        /// </summary>
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
            string pattern,
            string value = "",
            bool requiredFilling = false,
            Action<Dmuka3Mask> onChange = null,
            Func<Dmuka3Mask, Task> onChangeAsync = null
            )
        {
            this.Pattern = pattern;
            this.RequiredFilling = requiredFilling;
            this.Value = value;
            if (onChange != null)
                this.OnChange = onChange;
            if (onChangeAsync != null)
                this.OnChangeAsync = onChangeAsync;
        }
        #endregion
    }
}
