HH_HOME=c:\Program Files (x86)\HTML Help Workshop\

!IF [cscript /nologo buildnum.vbs > __buildnum.inc]
!ENDIF
!INCLUDE __buildnum.inc

all: chmdoc webdoc quickstartdoc

__cfix.xml: $(CFIX_TREE)\doc\docbook\book.xml
	xsltproc docbook\identity.xsl $(CFIX_TREE)\doc\docbook\book.xml > __cfix.xml

quickstartdoc:
	cd quickstart
	-3 robocopy $(CFIX_TREE)\doc\assets assets /MIR /XD .svn /XX /NJH /NJS
	copy ..\assets\img\logo-visualassert.gif assets\img\logo-visualassert.gif
	
	cd ..
	
chmdoc: __cfix.xml
	cd docbook\chm
	copy $(CFIX_TREE)\doc\docbook\stylechm.css .
	
	-3 robocopy ..\images images /MIR /XD .svn /XX /NJH /NJS
	copy /Y $(CFIX_TREE)\doc\docbook\chm\*.gif .
	copy /Y $(CFIX_TREE)\doc\docbook\chm\*.png .
	
	xsltproc --xinclude --param buildnumber $(BUILDNUMBER) ..\stylechm.xsl ..\book.xml	
	-1 "$(HH_HOME)\hhc" htmlhelp.hhp	
	move cfix.chm ..\VisualAssert.chm

	cd ..\..
	
	
webdoc: __cfix.xml
	if not exist docbook\web\doc md docbook\web\doc
	cd docbook\web\doc
	copy ..\..\styleweb.css ..
	-3 robocopy $(CFIX_TREE)\doc\assets ..\assets /MIR /XD .svn /XX /NJH /NJS
	-3 robocopy ..\..\..\assets ..\assets /MIR /XD .svn /XX /NJH /NJS
	-3 robocopy ..\..\images images /MIR /XD .svn /XX /NJH /NJS
	-3 robocopy ..\..\chm\images images /MIR /XD .svn /XX /NJH /NJS
	copy /Y $(CFIX_TREE)\doc\docbook\chm\*.gif .
	copy /Y $(CFIX_TREE)\doc\docbook\chm\*.png .

	xsltproc --xinclude --param buildnumber $(BUILDNUMBER) ..\..\styleweb.xsl ..\..\book.xml
	
	cd ../../..
	
joomla-templatedoc:
	cd joomla-template
	-3 robocopy $(CFIX_TREE)\doc\assets assets /MIR /XD .svn /XX /NJH /NJS
	-3 robocopy ..\assets assets /MIR /XD .svn /XX /NJH /NJS
	
	cd ..
