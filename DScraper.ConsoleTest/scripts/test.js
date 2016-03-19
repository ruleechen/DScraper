
var casper = require('casper').create({
    clientScripts: ['includes/jquery.js'],
    verbose: true
});

function getArguments() {
    var arg = casper.cli.args[0];
    var json = decodeURIComponent(arg);
    return JSON.parse(json);
}

casper.start('http://www.cnblogs.com/', function (response) {

    //this.echo('test:' + args.test)

    //var args = JSON.parse(casper.cli.args[0] || '{}');
    //this.echo(casper.cli.args);

    //this.echo(casper.cli.args.get('foo'));

    //for (var k in casper.cli.args) {
    //    this.echo('k:' + k);
    //}

    var args = getArguments();
    //this.echo(args);

    //require('utils').dump(response);

    //var nameCount = this.evaluate(function () {
    //    return $('img').jquery;
    //});

    //this.echo(nameCount);

    //this.echo(JSON.stringify({
    //    title: this.getTitle()
    //}));

    this.evaluate(function () {
        $('#zzk_q').val('rulee');
    });
});

casper.thenClick('.search_btn', function () {

    this.echo(this.getCurrentUrl());

    var value = this.evaluate(function () {
        return $('A').first().text();
    });

    this.echo(value);
});

casper.run();