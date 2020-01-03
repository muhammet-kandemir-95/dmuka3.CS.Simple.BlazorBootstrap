using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dmuka3.CS.Simple.BlazorBootstrap
{
    /// <summary>
    /// There are many method to help you to do something easily.
    /// </summary>
    public static class Dmuka3Helper
    {
        #region Methods
        #region JS
        /// <summary>
        /// Run javascript on browser.
        /// </summary>
        /// <param name="jSRuntime">
        /// JSRuntime instance.
        /// </param>
        /// <param name="javascript">
        /// What is javascript code?
        /// </param>
        /// <returns></returns>
        public static async Task RunJS(this IJSRuntime jSRuntime, string javascript)
        {
            await RunJS<object>(jSRuntime, javascript);
        }

        /// <summary>
        /// Run javascript on browser.
        /// </summary>
        /// <param name="jSRuntime">
        /// JSRuntime instance.
        /// </param>
        /// <param name="javascript">
        /// What is javascript code?
        /// </param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> RunJS<T>(this IJSRuntime jSRuntime, string javascript)
        {
            return await jSRuntime.InvokeAsync<T>("Dmuka3.RunJS", new object[] { javascript });
        }

        /// <summary>
        /// Show an alert on browser using "alert" command.
        /// </summary>
        /// <param name="jSRuntime">
        /// JSRuntime instance.
        /// </param>
        /// <param name="message">
        /// What is message as string?
        /// </param>
        /// <returns></returns>
        public static async Task AlertJS(this IJSRuntime jSRuntime, string message)
        {
            message = message.Replace("'", "\\'").Replace("\n", " ");
            await RunJS<bool>(jSRuntime, $"alert('{message}');return true;");
        }

        /// <summary>
        /// Change location on browser using setting "location".
        /// </summary>
        /// <param name="jSRuntime">
        /// JSRuntime instance.
        /// </param>
        /// <param name="location">
        /// What is new location as string?
        /// </param>
        /// <returns></returns>
        public static async Task SetLocation(this IJSRuntime jSRuntime, string location)
        {
            await RunJS<bool>(jSRuntime, $"location = '{location}';return true;");
        }
        #endregion

        /// <summary>
        /// To trigger <see cref="ComponentBase.StateHasChanged"/> using reflectoring.
        /// </summary>
        /// <param name="component">
        /// Which component will be used?
        /// </param>
        public static void StateHasChanged(ComponentBase component)
        {
            component.GetType().GetMethod("StateHasChanged", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(component, new object[0]);
        }
        #endregion
    }
}
