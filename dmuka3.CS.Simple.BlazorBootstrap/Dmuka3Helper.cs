using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dmuka3.CS.Simple.BlazorBootstrap
{
    public static class Dmuka3Helper
    {
        #region Methods
        #region JS
        public static async Task RunJS(this IJSRuntime jSRuntime, string javascript)
        {
            await RunJS<object>(jSRuntime, javascript);
        }

        public static async Task<T> RunJS<T>(this IJSRuntime jSRuntime, string javascript)
        {
            return await jSRuntime.InvokeAsync<T>("Dmuka3.RunJS", new object[] { javascript });
        }

        public static async Task AlertJS(this IJSRuntime jSRuntime, string message)
        {
            message = message.Replace("'", "\\'").Replace("\n", " ");
            await RunJS<bool>(jSRuntime, $"alert('{message}');return true;");
        }

        public static async Task SetLocation(this IJSRuntime jSRuntime, string location)
        {
            await RunJS<bool>(jSRuntime, $"location = '{location}';return true;");
        }
        #endregion

        public static void StateHasChanged(ComponentBase component)
        {
            component.GetType().GetMethod("StateHasChanged", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(component, new object[0]);
        }
        #endregion
    }
}
