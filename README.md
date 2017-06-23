# DScraper
Crawler engine base on casperjs

Environment Variables Required
------------
1. [python.exe](https://www.python.org/)
2. [casperjs.exe](http://casperjs.org/)
3. [phantomjs.exe](http://phantomjs.org/)

Sample CasperScraper
------------
```c#
var scraper = new CasperScraper();
var scriptFile = "D:\\test.js";
var args = new { };
var result = scraper.Execute(scriptFile, args);
// the echo values from your casper script
Console.WriteLine(result);
```

CasperScraper for Development
------------
```c#
var scraper = new CasperScraper
{
    // enable debugger
    // ensure you have chrome installed,
    // then you can debug casper script in chrome developer tool
    Debugger = true
};
var scriptFile = "D:\\test.js";
var args = new { };
var result = scraper.Execute(scriptFile, args);
// the echo values from your casper script
Console.WriteLine(result);
```
