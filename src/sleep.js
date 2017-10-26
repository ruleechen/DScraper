/*
* sleep
*/

module.exports = ms => (
  new Promise((resolve) => {
    setTimeout(ms, resolve);
  })
);
