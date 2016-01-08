#Controlling ultra low power CC2650 wireless MCU from anywhere in the world
![CC2650](http://www.mouser.de/images/microsites/TI_CC2650STK.png)

by using Bluetooth® low energy in combination with full-duplex real-time communication

#About the author
Uffe Björklund is one of the founders and the CEO of XSockets.NET. He has been working with development around real-time communication since 2009.

#Introduction
The SimpleLink™ multi-standard CC2650 wireless MCU from Texas Instruments is an amazing piece of hardware that enables communication over Bluetooth low energy as a peripheral device to a central unit. The SimpleLink SensorTag, based on the CC2650, has a lot of services, and it would be great to be able to access them from anywhere in the world.

##The mission
In this post we will take a look at how we can extend communication with CC2650 by adding a full-duplex communication layer behind the Bluetooth low energy central, so that we can read and write in to CC2650 from anywhere in the world. Since this might feel a bit abstract the image below might help to visualize what we are going to do.

There are many ways to connect to a peripheral Bluetooth Smart device, but in this post we will be using a Raspberry Pi 2 as the BLE central device.
![Communication](http://xsockets.net/$2/file/article-cc2650-1.png)

The image illustrates that the CC2650 wireless MCU communicates with the RaspberryPi over Bluetooth low energy. Then the Pi has a TCP/IP  connection with XSockets (in this case using NodeJS) to be able to send/receive data in full-duplex. XSockets will then be able to send/receive data from any TCP/IP connection so that we can read/write to the CC2650 from anywhere. In the image above the clients is represented by a few selected ones, but it can be anything that has TCP/IP.

## IoT & Real-Time Communication
In the world of the Internet of Things (IoT), real-time communication is almost a requirement. The most popular IoT protocols use full-duplex communication, and for a good reason. The IoT is often about sending data at a high frequency or receiving data when something happens. Solving this with a request response driven architecture is often a bad idea. With the half duplex technique you risk to get a very chatty solution with large overhead and messages that are sent when not needed.

#Pre-requirements
You may be able to use another setup, but this proof of concept is built using the hardware and software specified below.

We will install some additional software later on, but the post assume that you have the things below installed.
##Hardware

 - Raspberry Pi 2 Model B
 - Bluetooth 4.0 USB adapter
 - WiFi USB adapter
 - 8 GB Micro SD card
 - TI-CC2650 SensorTag

##Software
 - The NOOBS image from http://www.raspberrypi.org/downloads/ on the Raspberry Pi
 - Windows 8 (dev machine)
 - Visual Studio 2013 (dev machine)
 - NodeJS tool for Visual Studio https://nodejstools.codeplex.com/ 
 - Putty (dev machine, for connecting to the Pi) http://www.putty.org/ 

#Setting up the Raspberry Pi for BLE
We could have built apps for Android/iOS to accomplish the same thing since XSockets.NET has many different clients. However, regardless what client we choose it will still just be a proxy to our final destination. In this case the destination is Azure, but the server can of course be any hosted anywhere. 

##Installing required software
There are many great resources on how to get started with the Raspberry Pi. One of the best is of course http://www.raspberrypi.org/help/noobs-setup/ . However, that will only get you up and running with the Pi, we also need to install some additional packages to be able to use our Pi as a BLE central device.

If you are not going to setup this on your own Pi you can skip this part. It is only technical details around setting things up.

###Installing Bluez
***Note: If you already have Bluez installed you can skip this step.***
Bluez provides support for the core Bluetooth layers and protocols, so we need to install bluez and the dependencies to get our BLE adapter working.

####Dependencies
Installing the required dependencies.

    sudo apt-get install libusb-dev libdbus-1-dev libglib2.0-dev libudev-dev libical-dev libreadline-dev libbluetooth-dev

####Bluez
We need to download, compile and install Bluez.

#####Download
	
	sudo mkdir bluez
    cd bluez
    sudo wget www.kernel.org/pub/linux/bluetooth/bluez-5.28.tar.xz

#####Unzip, Compile, Install
	
	sudo unxz bluez-5.28.tar.xz
	sudo tar xvf bluez-5.28.tar
	cd bluez-5.28
	sudo ./configure --disable-systemd
	sudo make
	sudo make install

Now, reboot with the BLE adapter inserted.

##Testing BLE manually
When performing read/write to the CC2650 sensortag we will be using a nodejs library called “[sensortag](https://github.com/sandeepmistry/node-sensortag)” written by [Sandeep Mistry](https://github.com/sandeepmistry). The sensortag library is wrapping another nodejs library for BLE called “[noble](https://github.com/sandeepmistry/noble)”, this is also written by [Sandeep Mistry](https://github.com/sandeepmistry)

Before we use a library to read/write data we will do this manually to get some basic understanding about BLE in general and CC2650 in specific.

###Make sure BLE is up
After the installation of Bluez above we rebooted. Now the BLE adapter should be discovered by our operating system. Now we will make sure that the service is up and running.
Navigate to the folder where Bluez was installed (in my case ***/usr/lib/bluez-5.28/***)

Run: 
    
    tools/hciconfig

This should show something like

    hci0:   Type: BR/EDR  Bus: USB
        BD Address: 00:1A:7D:DA:71:0A  ACL MTU: 310:10  SCO MTU: 64:8
        DOWN
        RX bytes:48436 acl:121 sco:0 events:1615 errors:0
        TX bytes:2371 acl:98 sco:0 commands:84 errors:0

As you can see above the the BLE is DOWN, so to enable it we run.

    tools/hciconfig hci0 up

Now, when we run the hcifonfig the BLE will be UP.

    root@raspberrypi:/usr/lib/bluez-5.28# tools/hciconfig
    hci0:   Type: BR/EDR  Bus: USB
        BD Address: 00:1A:7D:DA:71:0A  ACL MTU: 310:10  SCO MTU: 64:8
        UP RUNNING
        RX bytes:49000 acl:121 sco:0 events:1644 errors:0
        TX bytes:2729 acl:98 sco:0 commands:113 errors:0

###Scan for BLE devices
To scan for BLE devices you can use the hcitool.  

    hcitool lescan

This will continuously provide all devices in range. Make sure that your SensorTag is on!

Example putput

    00:12:4B:00:53:11 CC2650 SensorTag

The first part ***[00:12:4B:00:53:11]*** is the uuid that we can use to connect to a specific sensortag.

###Connect with GATTTOOL
The gatttool (included in Bluez) can help us connect and read/write data to the sensortag. To be able to use the gatttool from anywhere we add it to ***/usr/local/bin*** with the command

    cp path/to/gatttool /usr/local/bin

Note: On my machine the ***path/to/gatttool*** is ***/usr/lib/bluez-5.28/attrib/gatttool***

Now we can connect to the BLE device (sensortag) with the gatttool using the uuid of sensortag.

    gatttool -b 00:12:4B:00:53:11 -I

This will return something like

    [00:12:4B:00:53:11][LE]>

Then just write 	"connect"

The complete operation would look like
    
    root@raspberrypi:/# gatttool -b 00:12:4B:00:53:11 -I
    [00:12:4B:00:53:11][LE]> connect
    Attempting to connect to 00:12:4B:00:53:11
    Connection successful
    [00:12:4B:00:53:11][LE]>

###Read/Write data
In the previous section we used the gatttool to connect to the sensortag. Now lets continue with the gatttool to read and write data to the sensortag. To know what to read and write from the sensortag we need to take a look at the GATT services of the sensortag CC2650. Texas Instruments provides GATT-tables for all services. There is a lot of information, but lets focus on the IR-Temperature service. The table looks like this.
![enter image description here](http://xsockets.net/$2/file/gatt-table-irtemp.png)

By using gatttool we can now read data with “***char-read-hnd &lt;hex handle&gt;***”, and write data with “***char-write-cmd &lt;hex handle&gt; &lt;new value&gt;***”

First we enable the IR-Temperature service by writing “01” to handle “0x24”.

    char-write-cmd 0x24 01

Then we enable read-notification for the IR-Temperature by reading from handle 0x21

    char-read-hnd 0x21

Next we enable notification for the IR-Temperature by writing “01:00” to handle “0x22”

    char-write-cmd 0x22 01:00

We will now start to see IR-Temperature data like
    
    Notification handle = 0x0021 value: 4c 09 24 0b
    Notification handle = 0x0021 value: 7c 09 28 0b
    Notification handle = 0x0021 value: 6c 09 28 0b
    Notification handle = 0x0021 value: 64 09 28 0b
    Notification handle = 0x0021 value: 7c 09 28 0b
    Notification handle = 0x0021 value: 6c 09 28 0b
    Notification handle = 0x0021 value: 88 09 28 0b
    Notification handle = 0x0021 value: 70 09 28 0b
    Notification handle = 0x0021 value: 70 09 28 0b
    Notification handle = 0x0021 value: 84 09 28 0b

And if we disable notifications by writing “00:00” to handle 0x22 the information will stop

    char-write-cmd 0x22 00:00

##Summary for manual reading
The gatttool is great, but to build a wrapper around it to use from Python or NodeJS is a lot of work. So lets be grateful for people like [Sandeep Mistry](https://github.com/sandeepmistry) and his [sensortag](https://github.com/sandeepmistry) library that provide functionality so that it becomes very easy to do the things we did manually above. It actually only takes a few lines of code.

#Setting up the Raspberry Pi for Real-Time communication
Since the library we use to communicate with the SensorTag from the Raspberrry Pi is based on NodeJS, we will use NodeJS for real-time communication as well.

##Installing NodeJS
Installing NodeJS on a Raspberry Pi is very easy.

    sudo wget http://node-arm.herokuapp.com/node_latest_armhf.deb 
    sudo dpkg -i node_latest_armhf.deb

Then you can verify the version by running (and this will probably output v0.12.0)

    node -v

---
#The solution
The mission with this article is to show how to read/write in full-duplex to CC2650 from anywhere in the world. To be able to do so we need three parts.

 1. A sensor client on the Raspberry Pi that communicates with the Bluetooth low energy device and also has a full-duplex connection to our real-time server.
 2. A real-time server that can dispatch messages to the clients monitoring the sensors as well as dispatching messages to the sensor client when the monitoring clients want to write data to the sensor.
 3. A monitoring client (can be several types) that display sensor data and send command to the sensor client via the real-time server.

These three implementations will be covered below.

##Sensor Client
The sensor client (NodeJS) on the Raspberry Pi is pretty easy to build.

### Setup
Create a folder called CC2650 and navigate to it.

#### Install SensorTag library
    
    npm install sensortag
    
#### Install xsockets.net library

	npm install xsockets.net

### Code
The complete code for the client (~70 lines) can be found in the github repository, but the important parts is covered here. Just place the *app.js* file in the folder where you installed the packages above.

Connection to server, note that the ip and port here is used for development only. When deployed to Azure the IP and port will be replaced with the public endpoint.

	//Connecting to XSockets
	var conn = new xsockets.TcpClient('192.168.1.3', 4502, ['sensor']);
	//Getting the sensorcontroller
	//The controller is used to listen for data as well as sending data
	var sensorcontroller = conn.controller('sensor');

When temperature changes on the sensortag

    tagInstance.on('irTemperatureChange', function (ot, at) {
	    //call server method 'irTempChange' and pass new value
	   sensorcontroller.send('irtempchange', { obj: ot, amb: at });
    });
       
When a monitoring client enables IR-Temperature 

	sensorcontroller.on('enableirtemp', self.enableIrTemperature);
                    
When a monitoring client some where in the world disables the IR-Temperature
             
    sensorcontroller.on('disableirtemp', self.disableIrTemperature);                                                           
       

##Real-Time Server
Since XSockets.NET has state, you can connect anything and it allows you to talk cross-protocol etc. it will be very easy to build the server-side communication.

### Sensor Controller
This is the controller that the sensor client will use to send data to. The concept is simple yet efficient. When a sensor client send a message to the sensor controller, the message is dispatched to all clients having an instance of the monitor controller. This way all clients monitoring will get notifications about. 

 - Sensors that connect/disconnect
 - Sensors that get IR-Temperature enabled/disabled
 - Changed in temperature on the sensor

![enter image description here](http://xsockets.net/$2/file/sensorcontroller.png)

### Monitor Controller
The monitor controller is even simpler than the sensor controller. This only has three methods.

 - First one to be able to get information about all sensors being online (OnOpened).
 - Second one for disabling the IR-Temperature notifications on the sensor.
 - Third one for enabling the IR-Temperature on the sensor.

By passing in the connection id that we know from the sensor client the server can target the correct sensor to disable/enable.

![enter image description here](http://xsockets.net/$2/file/monitorcontroller.png)

##Monitoring Client
Since you can connect anything to the real-time server (XSockets) you can control the sensor from pretty much anything. Your imagination sets the limits! In this sample I will only use a basic webpage and JavaScript to read/write data from the sensor.

### Code
The complete code for the client can be found in the github repository, but the important parts is covered here.

Connection to server, note that the ip and port here is used for development only.

	//Connecting to XSockets
	var conn = new XSockets.WebSocket('ws://192.168.1.3:4502', ['monitor']);        
	//Getting the monitorcontroller
	//The controller is used to listen for data as well as sending data
	var monitor = conn.controller('monitor');

When the server send notificaiton about temperature changes

    monitor.on('irTempChange', function(d) {
        console.log('irtempchange', d);
        vm.update(d);
    });
       
Enable the IR-Temperature from the webpage 

	monitor.invoke('enableIrTemp', vm.id());
    
Disable the IR-Temperature from the webpage 

	monitor.invoke('disableIrTemp', vm.id());
    
When a monitoring client some where in the world disables the IR-Temperature
             
    monitor.on('irTempDisabled', function(id) {
        vm.disable(id);
    });

When a monitoring client some where in the world enables the IR-Temperature
             
    monitor.on('irTempEnabled', function(id) {
        vm.enable(id);
    });

### Up & Running
An image showing the result from my development machine. We see the sensor tag connected over BLE to a Raspberry Pi that uses NodeJS to communicate with XSockets. Then XSockets sends data to all clients, in this case just a webpage. We can also enable/disable the sensors services directly from the webpage (or any other client).

![enter image description here](http://xsockets.net/$2/file/cc2650communicationdiagram.png)

#Deploying to Azure
To host this solution on Azure we need to create a Worker Role that will host the XSockets server. Since we already have our XSockets modules in a separate class library we just need to reference that assembly.

##Configuration
To be able to connect to the server on Azure we need to start the server on atleast one public endpoint.
###Service Configuration
Since we want to be able to run on localhost we configure both the "ServiceConfiguration.Local.cscfg" as well as the "ServiceConfiguration.Cloud.cscfg"

Local
![enter image description here](http://xsockets.net/$2/file/sesnor-localconfig.png)

Cloud
*Note: you should of course replace "cc2650.cloudapp.net" with the uri you deploy to.*
![enter image description here](http://xsockets.net/$2/file/snesor-cloudconfig.png)

###Service Endpoint
We can run the server on multiple endpoints, but here we only setup port 8080. 
![enter image description here](http://xsockets.net/$2/file/azure-config.png)
So when the local config is used we run on 127.0.0.1:8080 and when the cloud config is used we run on cc2650.cloudapp.net:8080
##Starting
We will start the server in the "OnStart" method of the Worker Role, the extension method called "StartOnAzure" will create configurations based on the configuration combined with the service endpoint.
    
    public override bool OnStart()
    {
        bool result = base.OnStart();
        _container = Composable.GetExport<IXSocketServerContainer>();                       
        _container.StartOnAzure();            
        return result;
    }

#Summary
The biggest challenge (for me) when building this was to setup BLE on the Raspberry Pi, but the reason for that is probably my limited skills in Linux and BLE. The Raspberry Pi 2 is extremely great to work with and the Texas Instruments SensorTag is very stable and easy to use. I also want to give some credit to Azure since deploying XSockets on Azure was extremely easy.

## What's next?
This post only uses the IR-Temperature service from the CC2650. I will continue to improve the solution and add support for more services as well as support for multiple SensorTags so that people around the world can register their own tags to be shown on Azure.

##Video
A short video showing how to deploy and test the solution.
[link here]

##GitHub Repository
The complete solution is available on [GitHub](https://github.com/uffebjorklund/TI-CC2650)

#Resources
 - The sensortag lib on-top of Noble https://github.com/sandeepmistry/node-sensortag
 - Noble https://github.com/sandeepmistry/noble 
 - Raspberry PI resources http://www.raspberrypi.org/downloads/
 - Raspberry PI - Noobs setup http://www.raspberrypi.org/help/noobs-setup/ 
 - The nodejs client for XSockets.NET https://github.com/XSockets/XSockets.Clients/tree/master/src/XSockets.Clients/XSockets4NodeJS
 - About GATT at the Bluetooth Developer Portal https://developer.bluetooth.org/TechnologyOverview/Pages/GATT.aspx 
 - NodeJS tool for Visual Studio https://nodejstools.codeplex.com/ 
 - Texas Instruments CC2650 http://go-dsp.com/sensorTag/tearDown.html
 - Michael Saunby http://mike.saunby.net/2013/04/raspberry-pi-and-ti-cc2541-sensortag.html

