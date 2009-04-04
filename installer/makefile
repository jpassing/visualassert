#
# 7z a -tzip src.zip cfix-src-1.1.3059 -xr!*.ncb -xr!*.suo
#

WIXBINPATH=..\tools\wix

!IF [cscript /nologo buildnum.vbs > __buildnum.inc]
!ENDIF
!INCLUDE __buildnum.inc

VERSION = 1.0.0.$(BUILDNUMBER)

#----------------------------------------------------------------------
#
# Build.
#
#----------------------------------------------------------------------

cfixstudio.msi: cfixstudio.wixobj ui.wixobj dialogs.wixlib
	if not exist wixca.dll copy /Y $(WIXBINPATH)\wixca.dll .
	light -b .. -nologo -w0 -loc dialogs\WixUI_en-us.wxl -loc cfixstudio_en-us.wxl dialogs.wixlib cfixstudio.wixobj ui.wixobj -out cfixstudio_$(VERSION).msi

cfixstudio.wixobj: cfixstudio.wxs
	copy /Y $(CFIX_TREE)\installer\cfix_*.wxs .
	candle -dCFIX_TREE=$(CFIX_TREE) -dProductCode=$(PRODUCTCODE) -dPackageCode=$(PACKAGECODE) -dProductVersion=$(VERSION) -nologo cfixstudio.wxs

ui.wixobj: ui.wxi
	candle -nologo ui.wxi
	
dialogs.wixlib:
	cd dialogs
	candle -nologo *.wxs
	lit -out ..\dialogs.wixlib *.wixobj
	cd ..
	
clean:
	del dialogs\*.wixobj
	del *.wixobj
	del *.wixlib
	del cfixstudio.msi
	
