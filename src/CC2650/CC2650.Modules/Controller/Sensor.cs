using CC2650.Modules.Model;
using XSockets.Core.XSocket;
using XSockets.Core.XSocket.Helpers;

namespace CC2650.Modules.Controller
{
    /// <summary>
    /// Controller that sensor clients will use    
    /// </summary>
    public class Sensor : XSocketController
    {
        /// <summary>
        /// The name of the sensor
        /// Should be set from the sensor when it is connected
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Last known value for this sensor
        /// </summary>
        public TempModel LastValue { get; set; }

        public Sensor()
        {
            this.Name = "N/A - Waiting for sensor input";
            this.LastValue = new TempModel();
        }

        /// <summary>
        /// A sensor connected, tell all monitoring clients about it.
        /// </summary>
        public override void OnOpened()
        {
            this.InvokeToAll<Monitor>(new { id = this.ConnectionId, this.LastValue.obj, this.LastValue.amb, name = this.Name }, "sensorConnected");
        }
        
        /// <summary>
        /// A sensor disconnected, tell all the monitoring clients about it.
        /// </summary>
        public override void OnClosed()
        {
            this.InvokeToAll<Monitor>(this.ConnectionId, "sensorDisconnected");
        }

        /// <summary>
        /// Will send the infomation to clients on the monitor controller that has templimit set to a value lower than obj/amb
        /// </summary>
        /// <param name="model"></param>
        public void IrTempChange(TempModel model)
        {
            //set last know value so that new monitoring clients can get the information
            this.LastValue = model;
            //Send only the clients mathching the expression
            this.InvokeTo<Monitor>(p => p.TempLimit <= model.obj || p.TempLimit <= model.amb, new {id = this.ConnectionId, model.obj, model.amb, name = Name},"irTempChange");
        }

        /// <summary>
        /// Tell all monitoring client that IR-temp was disabled for this sensor
        /// </summary>
        public void IrTempEnabled()
        {
            this.InvokeToAll<Monitor>(this.ConnectionId, "irTempEnabled");
        }

        /// <summary>
        /// Tell all monitoring client that IR-temp was enavled for this sensor
        /// </summary>
        public void IrTempDisabled()
        {         
            this.InvokeToAll<Monitor>(this.ConnectionId, "irTempDisabled");
        }
    }
}
