/*
* index
*/

const dotenv = require('dotenv');
const server = require('./src/server');

dotenv.config({ path: './.env' });

const instance = server.create();
instance.listen(process.env.port, () => {
  console.log(`Server is listening on http://localhost:${process.env.port}`);
});
