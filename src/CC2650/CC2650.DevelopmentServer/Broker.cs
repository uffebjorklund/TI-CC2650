using XSockets.Core.XSocket;
using XSockets.Core.XSocket.Helpers;
using XSockets.Core.Common.Socket.Event.Interface;

namespace CC2650.DevelopmentServer
{
    public class Broker : XSocketController
    {
        public string Location { get; set; }
        public void Say(string text)
        {
            this.InvokeTo(p => p.Location == this.Location, text,"say");
        }       
    }
}
