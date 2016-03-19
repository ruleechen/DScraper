
var casper = require('casper').create({
    clientScripts: ['includes/jquery.js']
});

casper.start('http://www.cnblogs.com/', function () {

    var nameCount = this.evaluate(function () {
        return $('img').jquery;
    });

    this.echo(nameCount);

    this.echo(this.getTitle());
});

casper.run();