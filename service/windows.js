/*
* windows service
*/

const path = require('path');
const { Service } = require('node-windows');

const svc = new Service({
  name: 'DScraper.Puppeteer',
  description: 'DScraper (puppeteer) Windows Service',
  script: path.resolve(__dirname, '../index.js'),
});

svc.on('install', () => {
  svc.start();
});

svc.on('error', (ex) => {
  console.error(`${ex}`);
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
    console.warn(`Unsupported action ${action}`);
    break;
  }
}
