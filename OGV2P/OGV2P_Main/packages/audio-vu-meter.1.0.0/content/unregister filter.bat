@echo off
REM Run this script as Administrator to un-register COM Dll

regsvr32 /u "%~dp0AudioVUMeterSinkFilter.dll"