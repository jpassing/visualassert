HH_HOME=c:\Program Files (x86)\HTML Help Workshop\

!IF [cscript /nologo buildnum.vbs > __buildnum.inc]
!ENDIF
!INCLUDE __buildnum.inc

__cfix.xml: $(CFIX_TREE)\doc\docbook\book.xml
	xsltproc docbook\identity.xsl $(CFIX_TREE)\doc\docbook\book.xml > __cfix.xml

all: chmdoc webdoc quickstart

quickstartdoc:
	cd quickstart
	-3 robocopy $(CFIX_TREE)\doc\assets assets /MIR /XD .svn /XX /NJH /NJS
	copy ..\assets\img\logo-cfixstudio.gif assets\img\logo-cfixstudio.gif
	
	cd ..
	
chmdoc: __cfix.xml
	cd docbook\chm
	copy $(CFIX_TREE)\doc\docbook\stylechm.css .
	
	xsltproc --xinclude --param buildnumber $(BUILDNUMBER) ..\stylechm.xsl ..\book.xml	
	-1 "$(HH_HOME)\hhc" htmlhelp.hhp	
	move cfix.chm ..\cfixstudio.chm

	cd ..\..
	
	
webdoc: __cfix.xml
	if not exist docbook\web\doc md docbook\web\doc
	cd docbook\web\doc
	copy ..\..\styleweb.css ..
	-3 robocopy $(CFIX_TREE)\doc\assets ..\assets /MIR /XD .svn /XX /NJH /NJS
	-3 robocopy ..\..\..\assets ..\assets /MIR /XD .svn /XX /NJH /NJS
	
	xsltproc --xinclude --param buildnumber $(BUILDNUMBER) ..\..\styleweb.xsl ..\..\book.xml
	
	if not exist images md images
	copy /Y ..\..\chm\images images

	cd ..
	

