@echo off

echo ---------- Uninstall [DScraper.Puppeteer] Service ----------
cd /D %~dp0
cd .. & cnpm install & npm run service-uninstall

pause
