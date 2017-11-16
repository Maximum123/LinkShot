var fs = require('fs');
var casper = require('casper').create({
    verbose: true,
    viewportSize: {
        width: 1024,
        height: 800
    }
});

var utils = require('utils');
var url = casper.cli.raw.get('url');
var fileName = casper.cli.raw.get('fileName');
casper.start(url);


casper.then(function () {
    this.capture('temp/' + fileName);
});

casper.run();