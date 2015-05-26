using System;
using System.Linq;
using XSockets.Core.Common.Socket.Event.Attributes;
using XSockets.Core.XSocket;
using XSockets.Core.XSocket.Helpers;

namespace CC2650.Modules.Controller
{
    /// <summary>
    /// Controller that monitoring client will use
    /// 
    /// - The clients can set an individual temp-limit since we have state
    /// - The clients can enable/disable sensors by using "EnableTemp/DisableTemp"
    /// 
    /// </summary>
    public class Monitor : XSocketController
    {
        [NoEvent]
        public double TempLimit { get; set; }

        /// <summary>
        /// When the controller is openend by the client it sends back information about all the connected sensors.
        /// </summary>
        public override void OnOpened()
        {            
            this.TempLimit = 10;
            //Find all sensors and get latest known value and name
            var data =
                this.FindOn<Sensor>()
                    .Select(p => new {id = p.ConnectionId, p.LastValue.obj, p.LastValue.amb, name = p.Name});
            //Send back sensor information
            this.Invoke(data,"sensors");
        }

        /// <summary>
        /// Set an individual templimit... Se SensorController and the method "IrTempChange" to see usage
        /// </summary>
        /// <param name="tempLimit"></param>
        public void SetTempLimit(double tempLimit)
        {
            this.TempLimit = tempLimit;
            this.Invoke(this.TempLimit,"newtemplimit");
        }
        
        public void DisableIrTemp(Guid connectionId)
        {
            this.InvokeTo<Sensor>(p => p.ConnectionId == connectionId,"disableIrTemp");
        }
        
        public void EnableIrTemp(Guid connectionId)
        {
            this.InvokeTo<Sensor>(p => p.ConnectionId == connectionId, "enableIrTemp");
        }
    }
}

