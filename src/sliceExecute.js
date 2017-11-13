/*
* slice execute
*/

module.exports = ({ array, threshold, handler }) => (
  new Promise((resolve, reject) => {
    let index = 0;
    const loop = () => {
      const items = array.slice(index, index + threshold);
      index += threshold;
      if (items.length) {
        handler(items).then((result) => {
          if (result !== false) {
            loop();
          } else {
            resolve();
          }
        }).catch((err) => {
          reject(err);
        });
      } else {
        resolve();
      }
    };
    loop();
  })
);
