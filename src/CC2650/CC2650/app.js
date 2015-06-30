/*NOTE: You cant test this script on windows since there is no BLE support.
 * Deploy to the raspbery pi and install xsockets.net and sensortag to test! */

var sensorTagLocation = 'sensortag';
var xsocketsLocation = 'xsockets.net';
var targetUUID = undefined;
var server = 'localhost';
var port = 4502;
//var server = 'cc2650.cloudapp.net';
//var port = 8080;

var TexasInstrument = {} || TexasInstrument;
TexasInstrument.CC2650 = (function (sensortag, xsockets) {
    var sensor = function () {
        var self = this;
        var tagInstance = undefined;
        //Create a new connection to the Azure worker role
        var conn = new xsockets.TcpClient(server, port, ['sensor']);
        conn.onopen = function () {
            console.log('conn open');
        };
        var sensorcontroller = conn.controller('sensor');
        //When the controller is open on the server set the name of the unit and start scanning for SensorTags
        sensorcontroller.onopen = function () {
            //Set Name
            sensorcontroller.setProperty('SetName', 'CC2650@NDCOslo');
            if(targetUUID) {
                console.log('connecting to ', targetUUID);
                sensortag.discoverByUuid(targetUUID, onDiscovered);
            }
            else {
                console.log('connecting to ONE tag'); 
                sensortag.discover(onDiscovered);
            }
        };
        sensorcontroller.onclose = function () {
            console.log('close');
        };
        sensorcontroller.onerror = function (e) {
            console.log('error', e);
        };
        //Temp changed.. Notify the server on Azure
        var onChangeTemp = function(ot, at) {            
            sensorcontroller.send('irtempchange', { obj: ot, amb: at });
        };
        //When a tag is discovered, connect to it and enable the service(s) to use
        var onDiscovered = function (tag) {
            tag.connect(function () {
                console.log('tag connected');
                tagInstance = tag;
                tagInstance.discoverServicesAndCharacteristics(function () {
                    console.log('characteristics discovered');
                    //Start IrTemp                                    
                    tagInstance.enableIrTemperature(function () {
                        console.log('temp enabled');                        
                        self.onconnected();    
                    });                    
                    sensorcontroller.on('enableirtemp', self.enableIrTemperature);
                    sensorcontroller.on('disableirtemp', self.disableIrTemperature);                                                           
                });
            });
        };
        this.onconnected = function () {
            console.warn('implement onconnected');
        };
        //Enable irtemp
        this.enableIrTemperature = function (cb) {
            tagInstance.notifyIrTemperature(function () {
                console.log('notification ON');
                tagInstance.on('irTemperatureChange', onChangeTemp);
                console.log('send notification about irtemp ENABLED');
                sensorcontroller.send('irtempenabled');
                if (cb) cb();
            });           
        };
        //Disable irtemp
        this.disableIrTemperature = function (cb) {            
            tagInstance.unnotifyIrTemperature(function () {
                console.log('notification OFF');
                tagInstance.removeAllListeners('irTemperatureChange');
                sensorcontroller.send('irtempdisabled');
                console.log('send notification about irtemp DISABLED');
                if (cb) cb();
            });
        };
        //Open the connection!
        conn.open();
    }
    return sensor;
})(require(sensorTagLocation), require(xsocketsLocation));

//Start the script and enable irtemp
var cc2650 = new TexasInstrument.CC2650();
cc2650.onconnected = function () {
    cc2650.enableIrTemperature();
}