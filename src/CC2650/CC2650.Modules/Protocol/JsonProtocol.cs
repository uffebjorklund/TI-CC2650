using System.Collections.Generic;
using System.Linq;
using System.Text;
using XSockets.Core.Common.Protocol;
using XSockets.Core.Common.Socket.Event.Arguments;
using XSockets.Core.Common.Socket.Event.Interface;
using XSockets.Core.Common.Utility.Serialization;
using XSockets.Plugin.Framework;
using XSockets.Plugin.Framework.Attributes;
using XSockets.Protocol;

namespace CC2650.Modules.Protocol
{
    /// <summary>
    /// This protocol uses JSON and expects a start byte and an end byte in the form
    /// 
    /// 0x00 JSON 0xFF
    /// 
    /// </summary>
    [Export(typeof(IXSocketProtocol), Rewritable = Rewritable.No)]
    public class JsonProtocol : XSocketProtocol
    {
        /// <summary>
        /// Answer with proper JSON
        /// </summary>
        public override string HostResponse
        {
            get
            {
                return Composable.GetExport<IXSocketJsonSerializer>().SerializeToString(new { T = "0x00", D = "Welcome to JsonProtocol" });
            }
        }

        public override bool CanDoHeartbeat()
        {
            return false;
        }

        public override IXSocketProtocol NewInstance()
        {
            return new JsonProtocol();
        }

        /// <summary>
        /// Override the default incoming frame to filter away CRLF
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public override IMessage OnIncomingFrame(IEnumerable<byte> payload, MessageType messageType)
        {
            var data = Encoding.UTF8.GetString(payload.ToArray()).TrimEnd('\r', '\n');
            return string.IsNullOrEmpty(data) ? null : base.OnIncomingFrame(payload, messageType);
        }
    }
}