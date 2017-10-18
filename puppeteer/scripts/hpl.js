const fs = require('fs');
const puppeteer = require('puppeteer');

const hpl = async () => {
    const home = 'https://www.hapag-lloyd.com/en/online-business/tracing/tracing-by-booking.html?blno=HLCUXM1170617440';
    const browser = await puppeteer.launch({ headless: false });
    const page = await browser.newPage();
    page.setViewport({
        width: 1800,
        height: 1000
    });
    await page.goto(home);
    await page.waitForSelector('.lowerScrollButton > input');
    await page.waitFor(2000);
    await page.click('.lowerScrollButton > input');
    await page.waitForSelector('table[summary="ScrollTable"]');
    await page.waitFor(2000);
    const html = await page.content();
    await browser.close();
    fs.writeFileSync('./page.html', html);
    console.log(html);
    return html;
};

hpl();
