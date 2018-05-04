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
    value: '7C97242136DD5C983190083F89D45EC526B5B34ED1373252C4B2C49AF791E91B600265092414855DB0DDEA9AD0AA43E2AFD9E8FC8DCE3ED2A249D9FAB6CA4BA450E7DA76F44B28A3D9464D1A26CF829507685B574D5CD6547BD87B67473A8A33FEC5BF1996F494980EEE4BB0E842D408972F01A3',
    domain: 'eportal.penavicoxm.com',
    path: '/',
    expires: new Date('1969-12-31T23:59:59.000Z').getTime(),
    httpOnly: true,
    secure: false,
  },
  {
    name: 'JSESSIONID',
    value: '15B06BE191FB426C65EB9D966A7AA17D',
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
