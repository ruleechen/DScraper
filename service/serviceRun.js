/*
* service run
*/

const username = require('username');
const { elevate, EventLogger } = require('node-windows');
const { log } = require('../src/logger');

const systemLog = new EventLogger('DScraper.Puppeteer');

username().then((name) => {
  log.info(`[serviceRun] user ${name}`);
});

elevate('cd .. & start.cmd', {}, () => {
  systemLog.info('Service run callback');
  log.info('[serviceRun] user Service run callback');
});
