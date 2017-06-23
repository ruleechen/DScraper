

phantom.casperTest = true;

var fs = require('fs');

var casper = require('casper').create({
    clientScripts: ['D:\\yun\\git\\DScraper\\DScraper.ConsoleTest\\includes\\jquery.js'],
    verbose: true,
    pageSettings: {
        loadImages: false,
        loadPlugins: true,
        userAgent: 'Mozilla/5.0 (Windows; U; Windows NT 5.1; nl; rv:1.9.1.6) Gecko/20091201 Firefox/3.5.6'
    }
});

debugger;

function getArguments() {
    var arg = casper.cli.args[0];
    var json = decodeURIComponent(arg);
    try {
        return JSON.parse(json);
    } catch (ex) {
        return json;
    }
}

casper.start('http://www.cnblogs.com/');

casper.then(function (response) {

    debugger;

    fs.write('page.html', this.getPageContent(), 'w');

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

    this.evaluate(function (v) {
        $('#zzk_q').val(v);
    }, args.test);

    //this.echo('rulee');
});

casper.thenClick('.search_btn', function () {

    this.echo(this.getCurrentUrl());

    var value = this.evaluate(function () {
        return $('.searchItemTitle').text();
    });

    this.echo(value);
});

casper.run();
