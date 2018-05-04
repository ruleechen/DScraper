@echo off

echo ---------- Install [DScraper.Puppeteer] Service ----------
cd /D %~dp0
cd .. & cnpm install & npm run service-install

pause
