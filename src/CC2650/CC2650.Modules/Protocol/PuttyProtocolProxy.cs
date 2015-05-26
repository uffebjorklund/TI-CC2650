using System.Collections.Generic;
using System.Linq;
using System.Text;
using XSockets.Core.Common.Globals;
using XSockets.Core.Common.Protocol;
using XSockets.Core.Common.Socket.Event.Arguments;
using XSockets.Core.Common.Socket.Event.Interface;
using XSockets.Core.Common.Utility.Serialization;
using XSockets.Core.XSocket.Model;
using XSockets.Plugin.Framework;

namespace CC2650.Modules.Protocol
{
    /// <summary>
    /// Special proxy for sensor demo... We will expect double,double if the topic is "irtempchange"
    /// Just to make things easier in the demo...
    /// </summary>
    public class PuttyProtocolProxy : IProtocolProxy
    {
        private IXSocketJsonSerializer JsonSerializer { get; set; }

        public PuttyProtocolProxy()
        {
            JsonSerializer = Composable.GetExport<IXSocketJsonSerializer>();
        }
        public IMessage In(IEnumerable<byte> payload, MessageType messageType)
        {
            var data = Encoding.UTF8.GetString(payload.ToArray());
            if (data.Length == 0) return null;
            var d = data.Split('|');
            switch (d[1])
            {
                case "subscribe":
                    return new Message(new XSubscription { Topic = d[2] }, Constants.Events.PubSub.Subscribe, d[0], JsonSerializer);
                case "unsubscribe":
                    return new Message(new XSubscription { Topic = d[2] }, Constants.Events.PubSub.Unsubscribe, d[0], JsonSerializer);
                case "irtempchange":
                    var v = d[2].Split(',');
                    return new Message(new {obj=v[0],amb=v[1]}, d[1], d[0], JsonSerializer);
                default:
                    return new Message(d[2], d[1], d[0], JsonSerializer);

            }
        }

        public byte[] Out(IMessage message)
        {
            return Encoding.UTF8.GetBytes(string.Format("{0}|{1}|{2}\r\n", message.Controller, message.Topic, message.Data));
        }
    }
}