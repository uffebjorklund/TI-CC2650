//using System;
//using System.Configuration;
//using System.Runtime.InteropServices;
//using XSockets.Core.Common.Socket;
//using XSockets.Core.XSocket.Helpers;

//namespace CC2650.Modules.ScaleOut
//{
//    public static class ScaleOutExtensions
//    {
//        public static bool ScaleOutEnabled = bool.Parse(ConfigurationManager.AppSettings.Get("XSockets.Scaleout.Enabled"));
//        public static void InvokeTo<T>(this T controller, Func<T, bool> expression, object obj, string topic)
//            where T : class, IXSocketController
//        {
//            foreach (var c in controller.FindOn<T>(expression))
//            {
//                c.Invoke(obj, topic);
//            }
//            if(ScaleOutEnabled)
//               controller.ScaleOut(obj, topic);
//        }

//        public static void InvokeToAll<T>(this T controller, object obj, string topic)
//            where T : class, IXSocketController
//        {
//            foreach (var c in controller.FindOn<T>())
//            {
//                c.Invoke(obj, topic);
//            }
//            if (ScaleOutEnabled)
//                controller.ScaleOut(obj, topic);
//        } 
//    }
//}
