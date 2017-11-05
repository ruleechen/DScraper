/*
* hpl
*/

module.exports = async ({ blno }, { getBrowser }) => {
  const browser = await getBrowser({ headless: true });
  const page = await browser.newPage();
  page.setViewport({
    width: 2000,
    height: 1000,
  });

  let radioInputs;
  let containers;
  const response = [];

  const loadPage = async () => {
    const home = `https://www.hapag-lloyd.cn/en/online-business/tracing/tracing-by-booking.html?blno=${blno}`;
    const radioSelector = 'input[type="radio"][class="inputRadio"]';
    await page.goto(home);
    await page.waitForSelector(radioSelector);
    await page.waitFor(2000);
    radioInputs = await page.$$(radioSelector);
    // query containers
    if (!containers) {
      containers = await page.evaluate((selector) => {
        const radios = document.querySelectorAll(selector);
        if (!radios.length) {
          return [];
        }
        const ths = radios[0] // radio
          .parentElement // td
          .parentElement // tr
          .parentElement // tbody
          .parentElement // table
          .firstElementChild // thead
          .firstElementChild // tr
          .children; // th
        const columns = Array.from(ths).map(th => th.innerText);
        return Array.from(radios).map((radio) => {
          const tds = radio.parentElement.parentElement.children;
          const cells = Array.from(tds).map(td => td.innerText);
          const container = {};
          for (let c = 0; c < columns.length; c += 1) {
            const column = columns[c];
            const value = cells[c];
            container[column] = value;
          }
          return container;
        });
      }, radioSelector);
    }
  };

  await loadPage();
  const count = radioInputs.length;
  for (let i = 0; i < count; i += 1) {
    if (i > 0) {
      // console.log(i);
      await loadPage();
    }
    const radio = radioInputs[i];
    await radio.click();

    const tableRowSelector = 'table[summary="ScrollTable"] tbody .hal-table-row';
    await page.click('.lowerScrollButton > input');
    await page.waitFor(2000);
    await page.waitForSelector(tableRowSelector);
    const trackings = await page.evaluate((selector) => {
      const trs = document.querySelectorAll(selector);
      if (!trs.length) {
        return [];
      }
      const ths = trs[0] // tr
        .parentElement // tbody
        .parentElement // table
        .firstElementChild // thead
        .firstElementChild // tr
        .children; // th
      const columns = Array.from(ths).map(th => th.innerText);
      return Array.from(trs).map((tr) => {
        const cells = Array.from(tr.children).map(td => td.innerText);
        const tracking = {};
        for (let c = 0; c < columns.length; c += 1) {
          const column = columns[c];
          const value = cells[c];
          tracking[column] = value;
        }
        return tracking;
      });
    }, tableRowSelector);

    response.push({
      container: containers[i],
      trackings,
    });
  }

  return response;
};
