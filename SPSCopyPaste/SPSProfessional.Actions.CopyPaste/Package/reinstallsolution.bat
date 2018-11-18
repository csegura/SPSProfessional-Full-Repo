
@echo Retracting Solution %1
@stsadm.exe -o retractsolution -local -allcontenturls -name %1.wsp
@echo Uninstalling Solution %1
@stsadm.exe -o deletesolution -name %1.wsp

@echo off
echo Adding... %1
stsadm.exe -o addsolution -filename %1.wsp
IF ERRORLEVEL   1 GOTO Error
echo Depoying... %1
stsadm.exe -o deploysolution -name %1.wsp -local -allowgacdeployment -allcontenturls
IF ERRORLEVEL   1 GOTO Error
echo Reseting web server
rem @iisreset
%systemroot%\system32\iisapp.vbs /a "SharePoint - 80" /r
GOTO End
:Error
@echo Install Error (%ERRORLEVEL%)

:End