# NpediCracker
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
Put your script (for example **script-name.js**) in **./puppeteer/scripts**

Call Service
------------
Send http **GET** request with to **http&#58;//localhost:7001/puppeteer/script-name.js**

Service Response
------------
The respond data depends on your script result.
