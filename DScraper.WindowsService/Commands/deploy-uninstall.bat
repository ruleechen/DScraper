@echo off

echo ---------- Uninstall DScraper.WindowsService Service ----------
%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe -u %~dp0\..\DScraper.WindowsService.exe

pause