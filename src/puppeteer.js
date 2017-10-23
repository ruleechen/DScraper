/*
* puppeteer provider
*/

const mainFolder = 'puppeteer';
const fsExtra = require('fs-extra');
const path = require('path');
const uuidv4 = require('uuid/v4');

const temporalize = async ({ scriptContent }) => {
  const uuid = uuidv4();
  const scriptPath = `temp/${uuid}.js`;
  const fullPath = path.resolve(mainFolder, scriptPath);
  await fsExtra.writeFile(fullPath, scriptContent, { encoding: 'utf8' });
  return scriptPath;
};

const execute = async ({ req, res, opts, body, query, scriptPath }) => {
  const fullPath = path.resolve(mainFolder, scriptPath);

  const exists = await fsExtra.exists(fullPath);
  if (!exists) {
    throw new Error('Can not find the specified script.');
  }

  const script = require(fullPath);
  if (typeof script !== 'function') {
    throw new Error('The specified script does not exports a function.');
  }

  const data = await script({
    req, res, opts,
    ...body,
    ...query,
  });

  return {
    fullPath,
    data,
  };
};

module.exports = {
  temporalize,
  execute,
};
