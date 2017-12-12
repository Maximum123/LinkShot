var fs = require('fs');
var casper = require('casper').create({
    verbose: true,
    //logLevel: 'info',
    viewportSize: {
        width: 1024,
        height: 768
    },
    waitTimeout: 5000,
    stepTimeout: 10000,
    onError: function (self, m) {   // Any "error" level message will be written
        console.log('FATAL:' + m); // on the console output and PhantomJS will
        self.exit();               // terminate
    },
    //onStepTimeout: function(self, m) {
    //    console.log('timeout: step' + m);
    //    this.capture('temp/' + fileName, {
    //        top: 0,
    //        left: 0,
    //        width: 1024,
    //        height: 768
    //    });
    //}
});

var utils = require('utils');
var url = casper.cli.raw.get('url');
var fileName = casper.cli.raw.get('fileName');
casper.start(url);


casper.then(function () {
    this.capture('temp/' + fileName, {
        top: 0,
        left: 0,
        width: 1024,
        height: 768
    });
});

casper.run();