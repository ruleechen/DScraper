/*
* puppeteer provider
*/

const mainFolder = 'puppeteer';
const fsExtra = require('fs-extra');
const path = require('path');
const uuid = require('uuid');
// const puppeteer = require('puppeteer');
const puppeteer = require('puppeteer-extra');
const StealthPlugin = require('puppeteer-extra-plugin-stealth');

puppeteer.use(StealthPlugin());

const temporalize = async ({ scriptContent }) => {
  const randomId = uuid.v4();
  const scriptPath = `temp/${randomId}.js`;
  const fullPath = path.resolve(mainFolder, scriptPath);
  await fsExtra.writeFile(fullPath, scriptContent, { encoding: 'utf8' });
  return scriptPath;
};

const execute = async ({
  req,
  res,
  opts,
  body,
  query,
  scriptPath,
}) => {
  const fullPath = path.resolve(mainFolder, scriptPath);

  const exists = await fsExtra.exists(fullPath);
  if (!exists) {
    throw new Error('Can not find the specified script.');
  }

  let data;
  let error;
  let browser;

  try {
    const script = require(fullPath);
    if (typeof script !== 'function') {
      throw new Error('The specified script does not exports a function.');
    }

    const context = {
      getBrowser: async (options) => {
        if (!browser) {
          browser = await puppeteer.launch(options || { headless: true });
        }
        return browser;
      },
    };

    data = await script({
      req,
      res,
      opts,
      ...body,
      ...query,
    }, context);
  } catch (ex) {
    error = ex;
  } finally {
    delete require.cache[require.resolve(fullPath)];
    if (browser) {
      try {
        await browser.close();
      } catch (ex) {
        console.error(ex);
      }
    }
  }

  if (error) {
    throw error;
  }

  return {
    fullPath,
    data,
  };
};

module.exports = {
  temporalize,
  execute,
};
