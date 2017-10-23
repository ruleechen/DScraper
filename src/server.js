/*
* server
*/

const http = require('http');
const Router = require('routes-router');
const jsonBody = require('body/json');
const anyBody = require('body/any');
const fsExtra = require('fs-extra');
const url = require('url');
const querystring = require('querystring');
const { temporalize, execute } = require('./puppeteer');

// router
const app = Router({
  // global error handlers
  errorHandler: (req, res, err) => {
    res.statusCode = 500;
    res.end(err.message);
  },
  notFound: function (req, res) {
    res.statusCode = 404
    res.end('Can not find the resource.');
  }
});

// POST: http://localhost:7001/puppeteer/json
// with body: { script: 'script-content' }
// and header: { Content-Type: 'application/json' }
app.addRoute('/puppeteer/json', (req, res, opts) => {
  jsonBody(req, res, async (err, body) => {
    let result = {};
    let isTemp = false;
    try {
      if (err) {
        throw err;
      }
      let scriptPath = body.script;
      if (!scriptPath && body.scriptContent) {
        isTemp = true;
        scriptPath = await temporalize({
          req, res, opts, body, scriptContent: body.scriptContent,
        });
      }
      const query = querystring.parse(url.parse(req.url).query);
      const { data, fullPath } = await execute({
        req, res, opts, body, query, scriptPath,
      });
      if (isTemp) {
        await fsExtra.remove(fullPath);
      }
      result.data = data;
    } catch (ex) {
      result.error = ex.message;
    }
    res.setHeader('content-type', 'application/json');
    res.end(JSON.stringify(result.error || result.data));
  });
});

// GET: http://localhost:7001/puppeteer/test.js
// GET: http://localhost:7001/puppeteer/hpl%2Ftest.js
app.addRoute('/puppeteer/:script', async (req, res, opts) => {
  let result = {};
  try {
    const query = querystring.parse(url.parse(req.url).query);
    const { data, fullPath } = await execute({
      req, res, opts, body: {}, query, scriptPath: opts.params.script,
    });
    result.data = data;
  } catch (ex) {
    result.error = ex.message;
  }
  res.setHeader('content-type', 'application/json');
  res.end(JSON.stringify(result.error || result.data));
});

// exports
module.exports = {
  create: () => (
    http.createServer(app)
  ),
};
