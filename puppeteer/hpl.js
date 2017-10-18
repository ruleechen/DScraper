const fs = require('fs');
const puppeteer = require('puppeteer');

module.exports = async ({ blno }) => {
    const browser = await puppeteer.launch({ headless: false });
    const page = await browser.newPage();
    page.setViewport({
        width: 1800,
        height: 1000
    });

    const home = `https://www.hapag-lloyd.com/en/online-business/tracing/tracing-by-booking.html?blno=${blno}`;
    await page.goto(home);
    await page.waitForSelector('.lowerScrollButton > input');
    await page.waitFor(2000);
    await page.click('.lowerScrollButton > input');
    await page.waitForSelector('table[summary="ScrollTable"]');
    await page.waitFor(2000);
    const html = await page.content();
    await browser.close();

    return {
        html,
    };
};
