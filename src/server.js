/*
* server
*/

const http = require('http');
const Router = require('routes-router');
const jsonBody = require('body/json');
const anyBody = require('body/any');
const fsExtra = require('fs-extra');
const { temporalize, execute } = require('./puppeteer');

// router
const app = Router({
  // global error handler
  errorHandler: (req, res, err) => {
    res.statusCode = 500;
    res.end(err.message);
  },
});

// GET: http://localhost:7001/puppeteer/test.js
// GET: http://localhost:7001/puppeteer/hpl%2Ftest.js
app.addRoute('/puppeteer/:script', async (req, res, opts) => {
  const result = await execute({
    req, res, opts, scriptPath: opts.params.script,
  });
  res.setHeader('content-type', 'application/json');
  res.end(JSON.stringify(result.data));
});

// POST: http://localhost:7001/puppeteer/json
// with body: { script: 'script-content' }
// and header: { Content-Type: 'application/json' }
app.addRoute('/puppeteer/json', (req, res, opts) => {
  jsonBody(req, res, async (err, body) => {
    if (err) {
      throw err;
    }
    let isTemp = false;
    let scriptPath = body.script;
    if (!scriptPath && body.scriptContent) {
      isTemp = true;
      scriptPath = await temporalize({
        req, res, opts, body, scriptContent: body.scriptContent,
      });
    }
    const result = await execute({
      req, res, opts, body, scriptPath,
    });
    if (isTemp) {
      await fsExtra.remove(result.fullPath);
    }
    res.setHeader('content-type', 'application/json');
    res.end(JSON.stringify(result.data));
  });
});

// exports
module.exports = {
  create: () => (
    http.createServer(app)
  ),
};
