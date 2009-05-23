<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  version="1.0">
  <xsl:import href="D:/prog/docbook-xsl-1.73.2/html/chunk.xsl"/>
  <xsl:param name="admon.graphics" select="1"/>
  <xsl:param name="html.stylesheet" select="'../styleweb.css'"/>
  <xsl:param name="chunk.section.depth" select="4"></xsl:param>
  <xsl:param name="chunk.first.sections" select="1"></xsl:param>
  <xsl:param name="use.id.as.filename" select="1"></xsl:param>
  <xsl:param name="highlight.source" select="0"></xsl:param>
  <xsl:param name="toc.section.depth" select="4"></xsl:param>
  <xsl:param name="chunker.output.doctype-public">-//W3C//DTD XHTML 1.0 Transitional//EN</xsl:param>
  <xsl:param name="generate.toc">
	appendix  toc,title
	article/appendix  nop
	article   toc,title
	book      toc,title
	chapter   toc,title
	part      toc,title
	preface   toc,title
	qandadiv  toc
	qandaset  toc
	reference toc,title
	sect1     toc
	sect2     toc
	sect3     toc
	sect4     toc
	sect5     toc
	section   toc,title
	set       toc,title
  </xsl:param>
  
  <xsl:template name="user.header.navigation">
  </xsl:template>
  <xsl:template name="user.footer.navigation">
  </xsl:template>
  
  <xsl:template name="user.head.content">
		<!-- Mind the whitespace! -->
		<xsl:comment>[if lt IE 7]&gt;
			&lt;link rel="stylesheet" href="../assets/master-ie6patch.css" type="text/css" /&gt;
		&lt;![endif]</xsl:comment>
  </xsl:template>
  
  <xsl:template name="chunk-element-content">
  <xsl:param name="prev"/>
  <xsl:param name="next"/>
  <xsl:param name="nav.context"/>
  <xsl:param name="content">
    <xsl:apply-imports/>
  </xsl:param>

  <xsl:call-template name="user.preroot"/>

  <html>
    <xsl:call-template name="html.head">
      <xsl:with-param name="prev" select="$prev"/>
      <xsl:with-param name="next" select="$next"/>
    </xsl:call-template>

	<body>
		<div id='tab'>
			<div class='tab_active'><a href='http://www.cfix-studio.org/'>cfix studio &#x2013; C/C++ unit testing for Visual Studio</a></div>
			<div class='tab_active2passive'><img src='../assets/img/tab_active2passive.png' alt=''/></div>
			<div class='tab_passive'><a href='http://www.cfix-testing.org/'>cfix &#x2013;  C/C++ unit testing for Win32 and NT</a></div>
			<div class='tab_passiveend'><img src='../assets/img/tab_passiveend.png' alt=''/></div>
			<div class='tab_pad'>&#xA0;</div>
			<div class='tab_clear'></div>
	    </div>
		<div id='header'>
			<img src='../assets/img/logo-cfixstudio.gif' alt='cfix studio &#x2013; C/C++ unit testing for Visual Studio' style="margin-left: 10px"/>
	    </div>
	    <div id='menu'>
	        <div id='menu_box'>
		        <ul id='mainmenu'>
		            <li><a href='index.html'>Documentation</a></li>
		        </ul>
		    </div>
	    </div>
	    <div id='main'>
	        <div id='main_sidebar'>
				<div class='submenu'>
				    <div class='submenu_box'>
					    Documentation				
				    </div>
				    <div class='submenu_content'>
				        <ul>
					        <li><a href='index.html'>Table of Contents</a></li>
					        <li><a href='KeyboardShortcuts.html'>Keyboard Shortcuts</a></li>
					        <li><a href='API.html'>API Reference</a></li>
					        <li><a href='WinUnitAPI.html'>WinUnit Compatibility</a></li>
				        </ul>  
				    </div> 
			    </div>
	        </div>
    	    
	        <div id='main_content'>
	
	
	
			<xsl:call-template name="body.attributes"/>
			<xsl:call-template name="user.header.navigation"/>

			<xsl:call-template name="header.navigation">
				<xsl:with-param name="prev" select="$prev"/>
				<xsl:with-param name="next" select="$next"/>
				<xsl:with-param name="nav.context" select="$nav.context"/>
			</xsl:call-template>

			<xsl:call-template name="user.header.content"/>

			<xsl:copy-of select="$content"/>

			<xsl:call-template name="user.footer.content"/>

			<xsl:call-template name="footer.navigation">
				<xsl:with-param name="prev" select="$prev"/>
				<xsl:with-param name="next" select="$next"/>
				<xsl:with-param name="nav.context" select="$nav.context"/>
			</xsl:call-template>

			<xsl:call-template name="user.footer.navigation"/>
	  
	  
			</div>
	        <div id='main_clear'></div>
    	</div>
	  
		<div id='footer'>
			cfix studio &#x2013; C/C++ unit testing for Visual Studio<br />
			Build <xsl:value-of select="$buildnumber" /><br />
			(C) 2009 Johannes Passing, all righs reserved.
			<br />
		</div>
    </body>
  </html>
  <xsl:value-of select="$chunk.append"/>
</xsl:template>

  
</xsl:stylesheet>