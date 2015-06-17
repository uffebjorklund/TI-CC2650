var xsockets = require('xsockets.net');

var c = new xsockets.TcpClient('192.168.254.154', 4502, ['generic']);
var g = c.controller('generic');
g.on('foo', function (d) {
    console.log('foo', d);
    g.send('bar', 'hello from nodejs');
});

g.onopen = function (ci) {
    console.log('connected to generic controller');    
}

c.onconnected = function (d) {
    console.log('connected', d);
}
c.open();
