<?php
// no direct access
defined( '_JEXEC' ) or die( 'Restricted access' );
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="<?php echo $this->language; ?>" lang="<?php echo $this->language; ?>" >
	<head>
		<jdoc:include type="head" />
		
		<link rel="stylesheet" href="<?php echo $this->baseurl ?>/templates/visualassert/assets/master.css" type="text/css">
		<!--[if lt IE 7]>
		<link rel="stylesheet" href="<?php echo $this->baseurl ?>/templates/visualassert/assets/master-ie6patch.css" type="text/css">
		<style>
			#main_content { padding: 10px; }
		</style>
		<![endif]-->
	
		<style>
			.ticks ul 
			{
				list-style-type: none;
				padding: 0em;
				margin: 0em;
			}

			.ticks ul li 
			{
				background-image: url(/unit-testing-framework/images/stories/tick.png);
				background-repeat: no-repeat;
				background-position: 0em .4em;
				padding-left: 25px;
				padding-bottom: 10px;
			}

			ul.newsfeed 
			{
				list-style-type: none;
				padding-left: 0px;
			}

			.newsfeed_item
			{
				padding-top: 10px;
				padding-bottom: 10px;
				padding-left: 10px;
			}
		</style>

	</head>
	<body>
		<div id='tab'>
			<div class='tab_activebegin'><img src='<?php echo $this->baseurl ?>/templates/visualassert/assets/img/tab_activebegin.png' alt=''/></div>
			<div class='tab_active'><a href='http://www.visualassert.com/'>Visual Assert &#x2013; The Unit Testing Add-In for Visual C++</a></div>	
			<div class='tab_active2passive'><img src='<?php echo $this->baseurl ?>/templates/visualassert/assets/img/tab_active2passive.png' alt=''/></div>
			<div class='tab_passive'><a href='http://www.cfix-testing.org/'>cfix &#x2013;  C/C++ unit testing for Win32 and NT</a></div>
			<div class='tab_passiveend'><img src='<?php echo $this->baseurl ?>/templates/visualassert/assets/img/tab_passiveend.png' alt=''/></div>
			<div class='tab_pad'>&#xA0;</div>
			<div class='tab_clear'></div>
	    </div>
		<div id='topborder'>&#xA0;</div>
		<div id='header'>
			<img src='<?php echo $this->baseurl ?>/templates/visualassert/assets/img/logo-visualassert.gif' alt='Visual Assert &#x2013; The Unit Testing Add-In for Visual C++' style="margin-left: 30px"/>
	    </div>
	    <div id='menu'>
	        <div id='menu_box'>
		        <jdoc:include type="modules" name="top" />
		    </div>
	    </div>
		<div id='main'>
	        <?php if( $this->countModules( 'right' ) ) { ?>
			<div id='main_content_half'>
				<jdoc:include type="component" />
			</div>
			<div id='main_content_half'>
		        	<jdoc:include type="modules" name="right" style="custom" />
			</div>
		<?php } else { ?>
	        	<div id='main_content_wide'>
				<jdoc:include type="component" />
			</div>
		<?php } ?>
	        <div id='main_clear'></div>
    	</div>
		<div id='bottomborder'>&#xA0;</div>
	  
		<div id='footer'>
			Visual Assert &#x2013; The Unit Testing Add-In for Visual C++<br />
			<a href='contact.html'>Contact</a> |
			<a href='terms.html'>Terms of Use</a><br /><br />
			
			(C) 2009-2010 <a href='http://jpassing.com/'>Johannes Passing</a>. All rights reserved.
		</div>
	<script type="text/javascript">
		var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
		document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
	</script>
	<script type="text/javascript">
		try {
		var pageTracker = _gat._getTracker("UA-9197378-3");
		pageTracker._trackPageview();
		} catch(err) {}
	</script>
    </body>
</html>
