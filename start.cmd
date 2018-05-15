@echo off

if "%1"=="h" goto begin

mshta vbscript:createobject("wscript.shell").run("""%~nx0"" h",0)(window.close)&&exit

:begin

cd /D %~dp0

cnpm install & npm run start

pause
