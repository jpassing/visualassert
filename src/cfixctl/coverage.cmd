@echo off
if "x%VSINSTALLDIR%x" == "xx" (
	echo Run vcvars32 first!
	goto Exit
)

echo on
set "PATH=%PATH%;%VSINSTALLDIR%\Team Tools\Performance Tools"
vsinstr ..\bin\chk\i386\cfixctl.dll -coverage -verbose 
start vsperfmon -coverage -output:cfixctl.coverage
sleep 2
..\..\..\cfix-cfixctl\bin\chk\i386\cfix32.exe -z ..\bin\chk\i386\testctl.dll
vsperfcmd -shutdown

:Exit