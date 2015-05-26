using XSockets.Core.Common.Socket.Event.Interface;
using XSockets.Core.XSocket;
using XSockets.Core.XSocket.Helpers;

namespace CC2650.DevelopmentServer
{
    /// <summary>
    /// Demo controller for bad broadcasting.. Just a POC
    /// </summary>
    public class DevSum : XSocketController
    {
        public override void OnMessage(IMessage message)
        {
            this.InvokeToAll(message);
        }
    }
}