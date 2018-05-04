/*
* service run
*/

const { elevate, EventLogger } = require('node-windows');

const log = new EventLogger('DScraper.Puppeteer');

elevate('cd .. & start.cmd', {}, () => {
  log.info('Service run callback');
});
