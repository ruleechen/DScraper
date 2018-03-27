/*
* hpl http://localhost:7001/puppeteer/hpl.js?blno=HLCUTS1161242297&ctnrno=TCLU9224118
*/
module.exports = async ({ blno, ctnrno }, { getBrowser }) => {
    const browser = await getBrowser({ headless: false });
    //获取url参数如`${blno}`，反单引号
  var html="",billno=`${blno}`,ctnrno=`${ctnrno}`; 
  const page = await browser.newPage();
  page.setViewport({
    width: 2000,
    height: 1000,
  });

  let radioInputs;
  let containers;
  const response = [];
  const radioSelector = 'input[type="radio"][class="inputRadio"]';

  const loadPage = async () => {
    const home = `https://www.hapag-lloyd.cn/en/online-business/tracing/tracing-by-booking.html?blno=${blno}`;
    
    await page.goto(home);
    await page.waitForSelector(radioSelector);
    
    await page.waitFor('#tracing_by_booking_f\\:hl27\\:1\\:hl30');
    radioInputs = await page.$$(radioSelector);
  
    // query containers
    if (!containers) {
      containers = await page.evaluate((selector) => {
        const radios = document.querySelectorAll(selector);
        if (!radios.length) {
          return [];
        }

        const ths = radios[0] // radio
        .parentElement   //div
          .parentElement // td
          .parentElement // tr
          .parentElement // tbody
          .parentElement // table
          .firstElementChild // thead
          .firstElementChild // tr
          .children; // th
        const columns = Array.from(ths).map(th => th.innerText);
        return Array.from(radios).map((radio) => {
          const tds = radio.parentElement.parentElement.parentElement.children;
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
      if(ctnrno.trim() != ""){
          if(containers[i]['Container No.'].toString().replace(" ", "")!=ctnrno.trim())
          {
              response.push({
                  container: containers[i],
                  no:containers[i]['Container No.'].toString().replace(" ", ""),
              });
              continue;
             
          }
      }else{
          if (i > 0){
              response.push({
                  container: containers[i],
              });
              continue;
          }
      }
      
    if (i > 0) {
      // console.log(i);
      await loadPage();
    }

    // const radio = radioInputs[i];
    // await radio.click();

    //page.click无效情况下使用evaluate
    await page.evaluate((selector,radionum) => {
      const radios = document.querySelectorAll(selector);
      radios[radionum].click();
    }, radioSelector,i);

    const tableRowSelector = 'table[summary="ScrollTable"] tbody tr';// hal-table-row
    await page.click('.hl-tbl-lower-scroll-button > input');// lowerScrollButton

    
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
