using XSockets.Core.Configuration;

namespace CC2650.DevelopmentServer
{
    /// <summary>
    /// For showing communication over multiple endpoints
    /// </summary>
    public class Config1 : ConfigurationSetting
    {
        public Config1() : base("ws://127.0.0.1:4502") { }
    }
}