# DScraper
A web crawler local server for scraping anything you want. The server based on [Puppeteer](https://github.com/GoogleChrome/puppeteer) by specifying your script.

Environment
------------
- Install [Nodejs](https://nodejs.org/) v8.x.x

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

For puppeteer api please reference to [Puppeteer API](https://github.com/GoogleChrome/puppeteer/blob/master/docs/api.md).
```js
const puppeteer = require('puppeteer');

module.exports = async ({ blno }) => {
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
