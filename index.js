/*
* index
*/

const dotenv = require('dotenv');
const username = require('username');
const { log } = require('./src/logger');
const server = require('./src/server');

dotenv.config({ path: './.env' });

log.info('[index] start');

const instance = server.create();
instance.listen(process.env.port, () => {
  log.info(`[server] listening on http://localhost:${process.env.port}`);
  username().then((name) => {
    log.info(`[server] user ${name}`);
  });
});
