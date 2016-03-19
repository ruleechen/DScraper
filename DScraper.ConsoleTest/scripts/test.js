
var casper = require('casper').create({
    clientScripts: ['includes/jquery.js'],
    verbose: true
});

casper.start('http://www.cnblogs.com/', function (response) {

    require('utils').dump(response);

    var nameCount = this.evaluate(function () {
        return $('img').jquery;
    });

    this.echo(nameCount);

    this.echo(JSON.stringify({
        title: this.getTitle()
    }));
});

casper.run();