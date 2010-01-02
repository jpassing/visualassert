WIXBINPATH=..\tools\wix

!IF [cscript /nologo buildnum.vbs > __buildnum.inc]
!ENDIF
!INCLUDE __buildnum.inc

VERSION = 1.0.3.$(BUILDNUMBER)

#----------------------------------------------------------------------
#
# MSI.
#
#----------------------------------------------------------------------

VisualAssert.msi: clean VisualAssert.wixobj ui.wixobj dialogs.wixlib versioninfo
	if not exist wixca.dll copy /Y $(WIXBINPATH)\wixca.dll .
	if not exist wixca.wixlib copy /Y $(WIXBINPATH)\wixca.wixlib .
	$(WIXBINPATH)\light -b .. -nologo -w0 -loc dialogs\WixUI_en-us.wxl -loc VisualAssert_en-us.wxl wixca.wixlib dialogs.wixlib VisualAssert.wixobj ui.wixobj -out VisualAssert_$(VERSION).msi

VisualAssert.wixobj: VisualAssert.wxs
	copy /Y $(CFIX_TREE)\installer\cfix_*.wxs .
	$(WIXBINPATH)\candle -dCFIX_TREE=$(CFIX_TREE) -dProductCode=$(PRODUCTCODE) -dPackageCode=$(PACKAGECODE) -dProductVersion=$(VERSION) -nologo VisualAssert.wxs

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
	del VisualAssert.msi
	del examples.zip
	
#----------------------------------------------------------------------
#
# Misc.
#
#----------------------------------------------------------------------

versioninfo:
	cat versioninfo-template.xml | sed s/VERSION/$(VERSION)/ > versioninfo.xml

