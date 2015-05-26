using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XSockets.Core.Common.Protocol;
using XSockets.Core.Common.Socket.Event.Arguments;
using XSockets.Core.Common.Socket.Event.Interface;
using XSockets.Plugin.Framework;
using XSockets.Plugin.Framework.Attributes;
using XSockets.Protocol;

namespace CC2650.Modules.Protocol
{
    /// <summary>
    /// A really simple/stupid protocol for Putty.
    /// </summary>
    [Export(typeof(IXSocketProtocol), Rewritable = Rewritable.No)]
    public class PuttyProtocol : XSocketProtocol
    {
        public PuttyProtocol()
        {
            this.ProtocolProxy = new PuttyProtocolProxy();
        }

        /// <summary>
        /// The string to return after handshake
        /// </summary>
        public override string HostResponse
        {
            get { return "Welcome to PuttyProtocol"; }
        }

        /// <summary>
        /// Returns the host response of the putty protocol and adds a CRLF
        /// </summary>
        /// <returns></returns>
        public override byte[] GetHostResponse()
        {
            return Encoding.UTF8.GetBytes(string.Format("{0}\r\n", HostResponse));
        }

        /// <summary>
        /// Set to true if your clients connected to this protocol will return pong on ping.
        /// </summary>
        /// <returns></returns>
        public override bool CanDoHeartbeat()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="readState"></param>
        /// <param name="processFrame"></param>
        public override void ReceiveData(ArraySegment<byte> data, IReadState readState, Action<FrameType, IEnumerable<byte>> processFrame)
        {
            lock (_locker)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    readState.Data.Add(data.Array[i]);
                    //if the frame is completed we will find \r\n at the end
                    if (readState.Data.Count >= 2 && readState.Data[readState.Data.Count - 1] == 10 &&
                        readState.Data[readState.Data.Count - 2] == 13)
                    {
                        processFrame(FrameType.Text, readState.Data.Take(readState.Data.Count - 2));
                        readState.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IXSocketProtocol NewInstance()
        {
            return new PuttyProtocol();
        }

        /// <summary>
        /// Converts the incomming data from putty into a IMessage
        /// The data is expected to be in the format "controller|topic|data"
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public override IMessage OnIncomingFrame(IEnumerable<byte> payload, MessageType messageType)
        {
            return this.ProtocolProxy.In(payload, messageType);
        }

        /// <summary>
        /// Converts a IMessage into a string to send to putty.
        /// Putty will receive the data in the format "controller|topic|data"        
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override byte[] OnOutgoingFrame(IMessage message)
        {
            return this.ProtocolProxy.Out(message);
        }
    }
}