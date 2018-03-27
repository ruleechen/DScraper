# DScraper
A web crawler local server for scraping anything you want. The server based on [Puppeteer](https://github.com/GoogleChrome/puppeteer) by specifying your script.

Environment
------------
- Install [Nodejs](https://nodejs.org/) v8.x.x

- Install **cnpm**

```bash
$ npm install cnpm -g
```

Start Server
------------
- Open **start.cmd** to start the server, default listen on port **7001**
- Message "**Server is listening on http&#58;//localhost:7001**" will prompt on server started.

Deployment
------------
Put your script (for example **script-name.js**) in **./puppeteer/**

Sub folder is supported.

Call Service
------------
- Send http **GET** request to **http&#58;//localhost:7001/puppeteer/script-relative-path?blno=1234**

- Send http **POST** request with json body to **http&#58;//localhost:7001/puppeteer/json?blno=1234**

```js
{
  "script": "script-relative-path",
  // any other data as you need
  // ...
}
```

Puppeteer script
------------
Please write the scirpt in the following format.

- For puppeteer api please reference to [Puppeteer API](https://github.com/GoogleChrome/puppeteer/blob/master/docs/api.md).

- For es6 programming grammar please reference to [ECMAScript 6](http://es6-features.org)

```js
module.exports = async ({ blno }, { getBrowser }) => {
  const browser = await getBrowser({ headless: true });
  const page = await browser.newPage();
  page.setViewport({
    width: 2000,
    height: 1000,
  });

  // crawler logic here
  // ...

  return {
    // any data as you need
    // ...
  };
};

```

Service Response
------------
The respond data depends on your script result.
