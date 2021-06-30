const sleep = require('../src/sleep');

module.exports = async ({ billno }, { getBrowser }) => {
  const browser = await getBrowser({
    headless: false,
    timeout: 120000,
    args: [
      '--incognito',
      '--start-maximized',
    ],
  });

  const context = await browser.createIncognitoBrowserContext();
  const page = await context.newPage();
  // const page = await browser.newPage();

  page.setViewport({
    width: 2000,
    height: 1000,
  });

  const loadPage = async () => {
    const home = 'https://www.zimchina.com/tools/track-a-shipment';
    await page.goto(home, { timeout: 90000 });

    await page.waitForSelector('button[id="onetrust-accept-btn-handler"]', {
      timeout: 15000,
    });

    await sleep(1000);

    await page.evaluate(() => {
      document.querySelector('#onetrust-accept-btn-handler').click();
    });

    await sleep(3000);

    await page.evaluate((num) => {
      document.querySelector('#ConsNumber').value = num;
    }, billno);

    await sleep(3000);

    await page.evaluate(() => {
      document.querySelector('.track-shipment-button').click();
    });

    await page.waitForSelector('div[class="progress-block"]', {
      timeout: 90000,
    });

    await sleep(30000);
  };

  await loadPage();

  return 'Success';
};
