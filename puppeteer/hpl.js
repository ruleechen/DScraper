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

  const home = `https://www.hapag-lloyd.com/en/online-business/tracing/tracing-by-booking.html?blno=${blno}`;
  await page.goto(home);
  await page.waitForSelector('.lowerScrollButton > input');
  await page.waitFor(2000);
  await page.click('.lowerScrollButton > input');
  await page.waitForSelector('table[summary="ScrollTable"]');
  await page.waitFor(2000);

  const html = await page.content();

  return {
    html,
  };
};
