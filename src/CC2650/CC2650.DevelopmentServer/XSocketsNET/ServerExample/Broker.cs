//using XSockets.Core.XSocket;
//using XSockets.Core.XSocket.Helpers;
//using XSockets.Core.Common.Socket.Event.Interface;

//namespace CC2650.DevelopmentServer.XSocketsNET.ServerExample
//{
//    /// <summary>
//    /// Implement/Override your custom actionmethods, events etc in this real-time MVC controller
//    /// </summary>
//    public class Broker : XSocketController
//    {
//        public string Location { get; set; }

//        public void Say(string text)
//        {
//            this.InvokeTo(p => p.Location == this.Location, text,"say");
//        }
//    }
//}
