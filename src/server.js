/*
* server
*/

const http = require('http');
const Router = require('routes-router');
const jsonBody = require('body/json');
const puppeteer = require('./puppeteer');

const app = Router({
  errorHandler: (req, res, err) => {
    res.statusCode = 500;
    res.end(err.message);
  },
});

app.addRoute('/puppeteer', (req, res) => {
  jsonBody(req, res, async (err, body) => {
    if (err) {
      throw err;
    }
    const result = await puppeteer({
      body,
      query: req.query,
    });
    res.setHeader('content-type', 'application/json');
    res.end(JSON.stringify(result));
  });
});

module.exports = {
  create: () => (
    http.createServer(app)
  ),
};
