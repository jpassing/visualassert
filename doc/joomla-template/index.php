<?php
// no direct access
defined( '_JEXEC' ) or die( 'Restricted access' );
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="<?php echo $this->language; ?>" lang="<?php echo $this->language; ?>" >
	<head>
		<jdoc:include type="head" />
		
		<link rel="stylesheet" href="<?php echo $this->baseurl ?>/templates/cfixstudio/assets/master.css" type="text/css">
		<!--[if lt IE 7]>
		<link rel="stylesheet" href="<?php echo $this->baseurl ?>/templates/cfixstudio/assets/master-ie6patch.css" type="text/css">
		<style>
			#main_content { padding: 10px; }
		</style>
		<![endif]-->
	</head>
	<body>
		<div id='tab'>
			<div class='tab_active'><a href='http://www.cfix-studio.org/'>cfix studio &#x2013; C/C++ unit testing for Visual Studio</a></div>
			<div class='tab_active2passive'><img src='<?php echo $this->baseurl ?>/templates/cfixstudio/assets/img/tab_active2passive.png' alt=''/></div>
			<div class='tab_passive'><a href='http://www.cfix-testing.org/'>cfix &#x2013;  C/C++ unit testing for Win32 and NT</a></div>
			<div class='tab_passiveend'><img src='<?php echo $this->baseurl ?>/templates/cfixstudio/assets/img/tab_passiveend.png' alt=''/></div>
			<div class='tab_pad'>&#xA0;</div>
			<div class='tab_clear'></div>
	    </div>
		<div id='header'>
			<img src='<?php echo $this->baseurl ?>/templates/cfixstudio/assets/img/logo-cfixstudio.gif' alt='cfix studio &#x2013; C/C++ unit testing for Visual Studio' style="margin-left: 10px"/>
	    </div>
	    <div id='menu'>
	        <div id='menu_box'>
		        <jdoc:include type="modules" name="top" />
		    </div>
	    </div>
		<div id='main'>
	        <div id='main_sidebar'>
				<div class='submenu'>
				    <div class='submenu_box'>
					    Documentation				
				    </div>
				    <div class='submenu_content'>
				        <jdoc:include type="modules" name="left" />
				    </div> 
			    </div>
	        </div>
    	    
	        <div id='main_content'>
				<jdoc:include type="modules" name="right" />
			</div>
	        <div id='main_clear'></div>
    	</div>
	  
		<div id='footer'>
			(C) 2009 Johannes Passing
		</div>
    </body>
</html>
