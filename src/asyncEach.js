/*
* async each
*/

const sliceExecute = require('./sliceExecute');

module.exports = ({ array, handler }) => sliceExecute({
  array,
  threshold: 1,
  handler: items => handler(items[0]),
});
