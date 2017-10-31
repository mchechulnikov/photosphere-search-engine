using System;
using System.Windows;

namespace Photosphere.SearchEngine.DemoApp.Utils
{
    internal class DispatchService
    {
        public static void Invoke(Action action)
        {
            var dispatchObject = Application.Current.Dispatcher;
            if (dispatchObject == null || dispatchObject.CheckAccess())
            {
                action();
            }
            else
            {
                dispatchObject.Invoke(action);
            }
        }
    }
}