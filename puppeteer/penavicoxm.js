/*
* http://eportal.penavicoxm.com/eportal/index.aspx
*/

module.exports = async ({ cookies }, { getBrowser }) => {
  const browser = await getBrowser({ headless: false });
  const page = await browser.newPage();
  await page.setViewport({
    width: 2000,
    height: 1000,
  });

  const cookieSet = cookies || [{
    name: 'ASP.NET_SessionId',
    value: 'tfn4s52va2dau2y5g0lvvdpd',
    domain: 'eportal.penavicoxm.com',
    path: '/',
    expires: new Date('1969-12-31T23:59:59.000Z').getTime(),
    httpOnly: true,
    secure: false,
  },
  {
    name: 'EPORTAL.AUTH',
    value: 'B68433D5E0F5130678FF8A3735E82E4776D6F166F3F24F86EEFA9C05E9BAB930F190F4F60C377F856934FDF27CFDEF617FD5D75770A1626FE5602BE1B75C62286B2A0867579EE68A04632F761B37012336F98D46EA4AEF521B3C13E837EBFA2F0AC96FF44C6F95A4641D82840AF47EDF6445DA2D',
    domain: 'eportal.penavicoxm.com',
    path: '/',
    expires: new Date('1969-12-31T23:59:59.000Z').getTime(),
    httpOnly: true,
    secure: false,
  },
  {
    name: 'JSESSIONID',
    value: '899DA5144374A0491677E507F76EC1F0',
    domain: 'www.qihuatong.com',
    path: '/',
    expires: new Date('1969-12-31T23:59:59.000Z').getTime(),
    httpOnly: false,
    secure: false,
  }];

  await page.setCookie(...cookieSet);

  const manageUrl = 'http://eportal.penavicoxm.com/eportal/manage.aspx';

  await page.goto(manageUrl);

  await new Promise((resolve) => {
    setTimeout(resolve, 5 * 1000);
  });
};
