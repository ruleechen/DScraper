@echo off

echo ---------- Start [DScraper.Puppeteer] Service ----------
cd /D %~dp0
cd .. & cnpm install & npm run service-start

pause
