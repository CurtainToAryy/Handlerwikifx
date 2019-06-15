@echo off
set /p var=是否要卸载 WebHandlerService服务(Y/N):
if "%var%" == "y" (goto uninstall) else (goto batexit)
 
:uninstall
call sc stop "WebHandlerService"
call sc delete "WebHandlerService"
pause
 
:batexit
exit