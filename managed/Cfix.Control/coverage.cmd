@echo off
if "x%VSINSTALLDIR%x" == "xx" (
	echo Run vcvars32 first!
	goto Exit
)

echo on
set "PATH=%PATH%;%VSINSTALLDIR%\Team Tools\Performance Tools"
vsinstr ..\..\bin\chk\i386\cfix.control.dll -coverage -verbose 
start vsperfmon -coverage -output:cfix.control.coverage
sleep 2
pushd ..\..\bin\chk\i386
"C:\Program Files (x86)\NUnit 2.4.8\bin\nunit-console-x86.exe" /noshadow cfix.control.test.dll
popd
vsperfcmd -shutdown

:Exit