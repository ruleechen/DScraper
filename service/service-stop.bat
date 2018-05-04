@echo off

echo ---------- Stop [DScraper.Puppeteer] Service ----------
cd /D %~dp0
cd .. & cnpm install & npm run service-stop

pause
