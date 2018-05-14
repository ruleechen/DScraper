/*
* index
*/

const dotenv = require('dotenv');
const username = require('username');
const { log } = require('./src/logger');
const server = require('./src/server');

dotenv.config({ path: './.env' });

log.info('[index] start');

username().then((name) => {
  log.info(`[index] user ${name}`);
});

const instance = server.create();
instance.listen(process.env.port, () => {
  log.info(`[index] server is listening on http://localhost:${process.env.port}`);
});
