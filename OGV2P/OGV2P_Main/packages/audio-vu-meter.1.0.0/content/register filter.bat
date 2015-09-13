@echo off
REM Run this script as Administrator to register COM Dll

regsvr32 "%~dp0AudioVUMeterSinkFilter.dll"