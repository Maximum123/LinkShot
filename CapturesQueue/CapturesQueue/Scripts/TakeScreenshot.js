var fs = require('fs');
var casper = require('casper').create({
    verbose: true,
    viewportSize: {
        width: 1024,
        height: 768
    }
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