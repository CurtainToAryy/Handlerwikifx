@echo off
set /p var=�Ƿ�Ҫ��װWebHandlerService����(Y/N):
if "%var%" == "y" (goto install) else (goto batexit)
 
:install
copy C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe  InstallUtil.exe /Y
call InstallUtil.exe Handlerwikifx.exe
call sc start "WebHandlerService"
pause
 
:batexit
exit