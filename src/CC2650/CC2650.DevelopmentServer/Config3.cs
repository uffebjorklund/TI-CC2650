using XSockets.Core.Configuration;

namespace CC2650.DevelopmentServer
{
    /// <summary>
    /// For showing communication over multiple endpoints
    /// </summary>
    public class Config3 : ConfigurationSetting
    {
        public Config3() : base("ws://127.0.0.1:4502") { }
    }

    public class Config4 : ConfigurationSetting
    {
        public Config4() : base("ws://192.168.254.154:8080") { }
    }
}