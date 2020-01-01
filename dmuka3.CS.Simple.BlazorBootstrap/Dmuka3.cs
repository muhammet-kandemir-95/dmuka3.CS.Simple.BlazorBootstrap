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
        public static async Task<T> RunJS<T>(IJSRuntime jSRuntime, string javascript)
        {
            return await jSRuntime.InvokeAsync<T>("Dmuka3.RunJS", new object[] { javascript });
        }

        public static async Task AlertJS(IJSRuntime jSRuntime, string message)
        {
            message = message.Replace("'", "\\'").Replace("\n", " ");
            await RunJS<bool>(jSRuntime, $"alert('{message}');return true;");
        }

        public static void StateHasChanged(ComponentBase component)
        {
            component.GetType().GetMethod("StateHasChanged", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(component, new object[0]);
        }
        #endregion
    }
}
