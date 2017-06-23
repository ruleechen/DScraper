@echo off

echo ---------- Install DScraper.WindowsService Service ----------
%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe -i %~dp0\..\DScraper.WindowsService.exe

echo ---------- Start DScraper.WindowsService Service ----------
net start DScraper.WindowsService

pause