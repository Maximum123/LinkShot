var fs = require('fs');
var casper = require('casper').create({
    viewportSize: {
        width: 1024,
        height: 768
    }
});

var utils = require('utils');
var url = casper.cli.raw.get('url');
casper.start(url);


casper.then(function () {
    this.capture('temp/temp.png');
});

casper.run();