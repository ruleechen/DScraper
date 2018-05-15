/*
* windows service
*/

const path = require('path');
const { Service, EventLogger } = require('node-windows');

const serviceName = 'DScraper.Puppeteer';
const systemLog = new EventLogger(serviceName);

const svc = new Service({
  name: serviceName,
  description: 'DScraper (puppeteer) Windows Service',
  script: path.resolve(__dirname, './serviceRun.js'),
});

svc.on('install', () => {
  svc.start();
});

svc.on('error', (ex) => {
  systemLog.error(`${ex}`);
});

const args = process.argv.slice(2);
const [action] = args;
switch (action) {
  case '--install': {
    svc.install();
    break;
  }
  case '--uninstall': {
    svc.uninstall();
    break;
  }
  case '--start': {
    svc.start();
    break;
  }
  case '--stop': {
    svc.stop();
    break;
  }
  default: {
    systemLog.warn(`Unsupported action ${action}`);
    break;
  }
}
