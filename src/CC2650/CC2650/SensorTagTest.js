var SensorTag = require('sensortag');
var tag;
var init = function () {
    // 1 Discover
    SensorTag.discover(function (t) {
        console.log('SensorTag Discovered');
        console.log('Type: ', t.type);
        console.log('UUID: ', t.uuid);
        // 2 Connect
        tag = t;
        tag.connectAndSetUp(enableIrTemp);
    });
};

// 3 Enable Service
function enableIrTemp() {
    console.log('enableIRTemperatureSensor');
    tag.enableIrTemperature(startNotification);
};

// 4 Start Notification
function startNotification() {
    tag.notifyIrTemperature(listenForTempReading);
};

// 5 Listen For Changes
function listenForTempReading() {
    tag.on('irTemperatureChange', function (objectTemp, ambientTemp) {
        console.log('\tObject Temp = %d deg. C', objectTemp.toFixed(1));
        console.log('\tAmbient Temp = %d deg. C', ambientTemp.toFixed(1));
    });
};

//Start
init();
