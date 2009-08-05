#
# 7z a -tzip src.zip cfix-src-1.1.3059 -xr!*.ncb -xr!*.suo
#

WIXBINPATH=..\tools\wix

!IF [cscript /nologo buildnum.vbs > __buildnum.inc]
!ENDIF
!INCLUDE __buildnum.inc

VERSION = 1.0.1.$(BUILDNUMBER)


#----------------------------------------------------------------------
#
# MSI.
#
#----------------------------------------------------------------------

cfixstudio.msi: clean cfixstudio.wixobj ui.wixobj dialogs.wixlib 
	if not exist wixca.dll copy /Y $(WIXBINPATH)\wixca.dll .
	if not exist wixca.wixlib copy /Y $(WIXBINPATH)\wixca.wixlib .
	$(WIXBINPATH)\light -b .. -nologo -w0 -loc dialogs\WixUI_en-us.wxl -loc cfixstudio_en-us.wxl wixca.wixlib dialogs.wixlib cfixstudio.wixobj ui.wixobj -out cfixstudio_$(VERSION).msi

cfixstudio.wixobj: cfixstudio.wxs
	copy /Y $(CFIX_TREE)\installer\cfix_*.wxs .
	$(WIXBINPATH)\candle -dCFIX_TREE=$(CFIX_TREE) -dProductCode=$(PRODUCTCODE) -dPackageCode=$(PACKAGECODE) -dProductVersion=$(VERSION) -nologo cfixstudio.wxs

ui.wixobj: ui.wxi
	$(WIXBINPATH)\candle -nologo ui.wxi
	
dialogs.wixlib:
	cd dialogs
	..\$(WIXBINPATH)\candle -nologo *.wxs
	..\$(WIXBINPATH)\lit -out ..\dialogs.wixlib *.wixobj
	cd ..
	
clean:
	del cfix_*.wxs 
	del dialogs\*.wixobj
	del *.wixobj
	del *.wixlib
	del cfixstudio.msi
	del examples.zip
	
#----------------------------------------------------------------------
#
# Examples.
#
#----------------------------------------------------------------------
