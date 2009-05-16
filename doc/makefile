HH_HOME=c:\Program Files (x86)\HTML Help Workshop\

!IF [cscript /nologo buildnum.vbs > __buildnum.inc]
!ENDIF
!INCLUDE __buildnum.inc

__cfix.xml: $(CFIX_TREE)\doc\docbook\book.xml
	xsltproc docbook\identity.xsl $(CFIX_TREE)\doc\docbook\book.xml > __cfix.xml

all: chmdoc webdoc

chmdoc: __cfix.xml
	cd docbook\chm
	copy $(CFIX_TREE)\doc\docbook\stylechm.css .
	
	xsltproc --xinclude --param buildnumber $(BUILDNUMBER) ..\stylechm.xsl ..\book.xml	
	-1 "$(HH_HOME)\hhc" htmlhelp.hhp	
	move cfix.chm ..\cfixstudio.chm

	cd ..\..
	
	
webdoc: __cfix.xml
	if not exist web\doc md web\doc
	
	cd web\doc
	copy ..\..\styleweb.css ..
	
	xsltproc --xinclude --param buildnumber $(BUILDNUMBER) ..\..\styleweb.xsl ..\..\book.xml

	
	if not exist images md images
	copy /Y ..\..\chm\images images

	cd ..
	

