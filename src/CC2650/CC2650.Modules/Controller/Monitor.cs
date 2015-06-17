namespace CC2650.Modules.Controller
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using XSockets.Core.Common.Socket.Event.Attributes;
    using XSockets.Core.XSocket;
    using XSockets.Core.XSocket.Helpers;
    using XSockets.Core.Common.Socket;
    using XSockets.Plugin.Framework;

    /// <summary>
    /// Controller that monitoring client will use
    /// 
    /// - The clients can set an individual temp-limit since we have state
    /// - The clients can enable/disable sensors by using "EnableTemp/DisableTemp"
    /// 
    /// </summary>
    public class Monitor : XSocketController
    {
        #region Individual TempLimit for each monitoring client
        [NoEvent]
        public double TempLimit { get; set; }

        /// <summary>
        /// Set an individual templimit... Se SensorController and the method "IrTempChange" to see usage
        /// </summary>
        /// <param name="tempLimit"></param>
        public void SetTempLimit(double tempLimit)
        {
            this.TempLimit = tempLimit;
            this.Invoke(this.TempLimit, "newtemplimit");
        }
        #endregion
        
        /// <summary>
        /// When the controller is openend by the client it sends back information about all the connected sensors.
        /// </summary>
        public override async Task OnOpened()
        {
            //Send back containerId so see if we are on different servers when scaling
            await this.Invoke(Composable.GetExport<IXSocketServerContainer>().ContainerId, "containerid");

            this.TempLimit = 10;
            //Find all sensors and get latest known value and name
            var sensors = this.FindOn<Sensor>().Select(p => p.SensorInfo);
            //Send back sensor information
            await this.Invoke(sensors,"sensors");
        }        

        #region ENABLE / DISABLE SENSORTAG IR-TEMP
        public void DisableIrTemp(Guid connectionId)
        {
            this.InvokeTo<Sensor>(p => p.ConnectionId == connectionId,"disableIrTemp");
        }
        
        public void EnableIrTemp(Guid connectionId)
        {
            this.InvokeTo<Sensor>(p => p.ConnectionId == connectionId, "enableIrTemp");
        }
        #endregion
    }
}

