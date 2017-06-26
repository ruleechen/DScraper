
phantom.casperTest = true;

var casper = require('casper').create({
    clientScripts: [],
    verbose: false,
    pageSettings: {
        loadImages: false,
        loadPlugins: true,
        userAgent: 'Mozilla/5.0 (Windows; U; Windows NT 5.1; nl; rv:1.9.1.6) Gecko/20091201 Firefox/3.5.6'
    }
});

function getArguments() {
    var arg = casper.cli.args[0];
    var json = decodeURIComponent(arg);
    try {
        return JSON.parse(json);
    } catch (ex) {
        return json;
    }
}

/*
* { blno: '750313769', company: 'APL' }
*/
var parsedArgs = getArguments();

casper.start('http://www.track-trace.com/bol', function () {
    this.waitForSelector('form[action="/bol"]', function () {
        this.evaluate(function (blno) {
            document.querySelector('input[name="number"]').setAttribute('value', blno);
            document.querySelector('input[name="commit"]').click();
        }, parsedArgs.blno);
    });
});

casper.then(function () {
    var dataId = this.evaluate(function (company) {
        var links = document.querySelectorAll('div[data-id]');
        for (var i = 0; i < links.length; i++) {
            if (links[i].innerText.trim().toLowerCase() === company.toLowerCase()) {
                return links[i].getAttribute('data-id');
            }
        }
    }, parsedArgs.company);

    this.click('[data-id="' + dataId + '"]');
});

casper.waitForSelector('iframe', function () {
    this.withFrame(0, function () {

        // get parent css
        var cssHrefs = this.evaluate(function () {
            var hrefs = [];
            var links = document.querySelectorAll('head>link');
            for (var i = 0; i < links.length; i++) { hrefs.push(links[i].href); }
            return hrefs;
        });

        this.withFrame('bodyframe', function () {

            // inject parent css to child frame
            this.evaluate(function (cssHrefs) {
                var head = document.querySelector('head');
                for (var i = 0; i < cssHrefs.length; i++) {
                    var link = document.createElement('link');
                    link.setAttribute('href', cssHrefs[i]);
                    link.setAttribute('rel', 'stylesheet');
                    link.setAttribute('type', 'text/css');
                    head.appendChild(link);
                }
            }, cssHrefs);

            this.echo(this.getPageContent());
        });
    });
});

casper.run();
