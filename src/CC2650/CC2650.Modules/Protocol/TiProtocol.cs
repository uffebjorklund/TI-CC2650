
namespace CC2650.Modules.Protocol
{
    using XSockets.Core.Common.Protocol;
    using XSockets.Plugin.Framework;
    using XSockets.Plugin.Framework.Attributes;
    using XSockets.Protocol.Putty;

    /// <summary>
    /// A really simple/stupid protocol for testing from putty over RAW sockets.
    /// </summary>
    [Export(typeof(IXSocketProtocol), Rewritable = Rewritable.No)]
    public class TiProtocol : PuttyProtocol
    {
        public TiProtocol()
        {
            this.ProtocolProxy = new TiProtocolProxy();
        }

        /// <summary>
        /// The string to return after handshake
        /// </summary>
        public override string HostResponse
        {
            get { return "Welcome to TiProtocol"; }
        }

        public override IXSocketProtocol NewInstance()
        {
            return new TiProtocol();
        }
    }
}