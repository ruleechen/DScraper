/*
* server
*/

const http = require('http');
const Router = require('routes-router');
const jsonBody = require('body/json');
const anyBody = require('body/any');
const puppeteer = require('./puppeteer');

const app = Router({
  errorHandler: (req, res, err) => {
    res.statusCode = 500;
    res.end(err.message);
  },
});

app.addRoute('/puppeteer/json', (req, res, opts) => {
  jsonBody(req, res, async (err, body) => {
    if (err) {
      throw err;
    }
    const result = await puppeteer({
      req, res, opts, body,
    });
    res.setHeader('content-type', 'application/json');
    res.end(JSON.stringify(result));
  });
});

app.addRoute('/puppeteer/*?/:script', async (req, res, opts) => {
  const result = await puppeteer({
    req, res, opts,
  });
  res.setHeader('content-type', 'application/json');
  res.end(JSON.stringify(result));
});

module.exports = {
  create: () => (
    http.createServer(app)
  ),
};
