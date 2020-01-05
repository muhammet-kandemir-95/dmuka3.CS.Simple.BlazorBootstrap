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
    /// This model is used for Dmuka3Number.razor to receive and send datas between Dmuka3Number and other component which uses Dmuka3Number.
    /// </summary>
    public class Dmuka3NumberModel
    {
        #region Variables
        /// <summary>
        /// Which component uses Dmuka3Number?
        /// </summary>
        public ComponentBase Parent { get; internal set; }
        /// <summary>
        /// which Dmuka3Number uses this class?
        /// </summary>
        public Dmuka3Number Number { get; internal set; }

        /// <summary>
        /// Change event.
        /// </summary>
        public Func<Dmuka3Number, Task> OnChangeAsync { get; set; }

        private Action<Dmuka3Number> _onChange = null;
        /// <summary>
        /// Change event.
        /// </summary>
        public Action<Dmuka3Number> OnChange
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
        /// Value as string.
        /// </summary>
        internal string ValueAsString
        {
            get
            {
                if (this.Value == null)
                    return "";
                else if(this.Format)
                    return this.Value.Value.ToString(CultureInfo.InvariantCulture).Replace('.', this.FormatCharacters[1]);
                else
                    return this.Value.Value.ToString(CultureInfo.InvariantCulture);
            }
        }

        private decimal? _value = null;
        /// <summary>
        /// Input's value.
        /// </summary>
        public decimal? Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;

                if (value != null)
                    this._value = Math.Round(this._value.Value, this.DecimalPlaces, MidpointRounding.ToEven);

                if (this.Number != null && this.Number.Loaded)
                    this.Number.JSRuntime.InvokeVoidAsync("Dmuka3Number.Set", new object[] { this.Number.NumberId, this.ValueAsString });
            }
        }

        /// <summary>
        /// Decimal places of input's value.
        /// </summary>
        public ushort DecimalPlaces { get; private set; }

        /// <summary>
        /// Will value be formatted?
        /// </summary>
        public bool Format { get; private set; }

        private static char[] __formatCharactersStatic = new char[] { ',', '.' };
        /// <summary>
        /// You can change default <see cref="FormatCharacters"/> by setting this.
        /// </summary>
        public static char[] FormatCharactersStatic
        {
            get
            {
                return __formatCharactersStatic;
            }
            set
            {
                if (value == null)
                    throw new NullReferenceException();
                if (value.Length != 2)
                    throw new Exception($"{nameof(FormatCharacters)} length must be 2!");
                if (value[0] == value[1])
                    throw new Exception($"{nameof(FormatCharacters)} values must be different!");

                __formatCharactersStatic = value;
            }
        }

        private char[] _formatCharacters = null;
        /// <summary>
        /// '123&lt;item[0]&gt;456&lt;item[0]&gt;789&lt;item[1]&gt;321
        /// <para></para>
        /// Example = 123,456,789.321
        /// </summary>
        public char[] FormatCharacters
        {
            get
            {
                return this._formatCharacters;
            }
            private set
            {
                if (value == null)
                    throw new NullReferenceException();
                if (value.Length != 2)
                    throw new Exception($"{nameof(FormatCharacters)} length must be 2!");
                if (value[0] == value[1])
                    throw new Exception($"{nameof(FormatCharacters)} values must be different!");

                this._formatCharacters = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// This model is used for Dmuka3Number.razor to receive and send datas between Dmuka3Number and other component which uses Dmuka3Number.
        /// </summary>
        /// <param name="parent">
        /// Which component uses Dmuka3Number?
        /// </param>
        /// <param name="value">
        /// Input's value.
        /// </param>
        /// <param name="format">
        /// Will value be formatted?
        /// </param>
        /// <param name="decimalPlaces">
        /// Decimal places of input's value.
        /// </param>
        /// <param name="formatCharacters">
        /// '123&lt;item[0]&gt;456&lt;item[0]&gt;789&lt;item[1]&gt;321
        /// <para></para>
        /// Example = 123,456,789.321
        /// </param>
        /// <param name="onChange">
        /// Change event.
        /// </param>
        /// <param name="onChangeAsync">
        /// Change event.
        /// </param>
        public Dmuka3NumberModel(
            ComponentBase parent,
            decimal? value = null,
            bool format = true,
            ushort decimalPlaces = 2,
            char[] formatCharacters = null,
            Action<Dmuka3Number> onChange = null,
            Func<Dmuka3Number, Task> onChangeAsync = null
            )
        {
            this.Parent = parent;
            this.DecimalPlaces = decimalPlaces;
            this.Format = format;
            if (formatCharacters != null)
                this.FormatCharacters = formatCharacters;
            else
                this.FormatCharacters = FormatCharactersStatic;
            this.Value = value;
            if (onChange != null)
                this.OnChange = onChange;
            if (onChangeAsync != null)
                this.OnChangeAsync = onChangeAsync;
        }
        #endregion
    }
}
