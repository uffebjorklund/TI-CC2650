using XSockets.Protocol.Mqtt.Modules.Controller;

namespace CC2650.Modules.Controller
{
    using XSockets.Core.Common.Socket.Event.Attributes;    
    using System.Threading.Tasks;
    using Model;
    using XSockets.Core.XSocket;
    using XSockets.Core.XSocket.Helpers;
    using XSockets.Protocol.Mqtt.Modules.Extensions;

    /// <summary>
    /// Controller that sensor clients will use    
    /// </summary>
    public class Sensor : XSocketController
    {
        /// <summary>
        /// The current sensor properties
        /// </summary>
        [NoEvent]
        public SensorInfo SensorInfo { get; set; }

        public Sensor()
        {
            this.SensorInfo = new SensorInfo("N/A - Waiting for sensor input");                     
        }

        public void SetName(string name)
        {
            this.SensorInfo.name = name;
            this.InvokeToAll<Monitor>(this.SensorInfo, "sensorConnected");
        }        

        /// <summary>
        /// A sensor connected, tell all monitoring clients about it.
        /// </summary>
        public override Task OnOpened()
        {
            this.SensorInfo.id = this.ConnectionId;            
            this.InvokeToAll<Monitor>(this.SensorInfo, "sensorConnected");            
            return base.OnOpened();
        }
        
        /// <summary>
        /// A sensor disconnected, tell all the monitoring clients about it.
        /// </summary>
        public override Task OnClosed()
        {
            this.InvokeToAll<Monitor>(this.ConnectionId, "sensorDisconnected");                     
            return base.OnClosed();
        }        

        /// <summary>
        /// Will send the infomation to clients on the monitor controller that has templimit set to a value lower than obj/amb
        /// </summary>
        /// <param name="model"></param>
        public void IrTempChange(TempModel model)
        {
            //set last know value so that new monitoring clients can get the information
            this.SensorInfo.lastValue = model;
            
            //Send only the clients mathching the expression
            this.InvokeTo<Monitor>(p => p.TempLimit <= model.obj || p.TempLimit <= model.amb, this.SensorInfo,"irtempchange");     
            //POC - Notify MQTT clients that subscribe for "irtempchange"...
            this.InvokeToAll<MqttController>(this.SensorInfo, "irtempchange");
        }

        public void IrTempNotify(SensorInfo sensorInfo)
        {
            this.InvokeTo<Monitor>(p => p.TempLimit <= sensorInfo.lastValue.obj || p.TempLimit <= sensorInfo.lastValue.amb, 
                sensorInfo, "irtempchange");
        }

        /// <summary>
        /// Tell all monitoring client that IR-temp was enabled for this sensor
        /// </summary>
        public void IrTempEnabled()
        {
            this.InvokeToAll<Monitor>(this.ConnectionId, "irtempenabled");
        }

        /// <summary>
        /// Tell all monitoring client that IR-temp was disabled for this sensor
        /// </summary>
        public void IrTempDisabled()
        {         
            this.InvokeToAll<Monitor>(this.ConnectionId, "irTempDisabled");            
        }
    }
}
