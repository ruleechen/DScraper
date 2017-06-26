@echo off & SETLOCAL ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION

rmdir "%~dp0\output\" /s /q
mkdir "%~dp0\output"

where /q nuget && nuget restore ..\DScraper.sln
del build.log /Q /S
msbuild ..\DScraper.sln /t:rebuild /l:FileLogger,Microsoft.Build.Engine;logfile=build.log; /p:Configuration=Release

@echo Build complete successfully
pause