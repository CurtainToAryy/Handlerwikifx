@echo off
set /p var=�Ƿ�Ҫж�� WebHandlerService����(Y/N):
if "%var%" == "y" (goto uninstall) else (goto batexit)
 
:uninstall
call sc stop "WebHandlerService"
call sc delete "WebHandlerService"
pause
 
:batexit
exit