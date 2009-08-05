/*++
	This file is part of cfix studio.
	
	Copyright 2009, Johannes Passing.
--*/

function OnFinish( selProj, selObj )
{
	try
	{
		var strProjectName = wizard.FindSymbol( "PROJECT_NAME" );
		var strProjectPath = wizard.FindSymbol( 'PROJECT_PATH' );
		var strTemplatePath = wizard.FindSymbol( 'TEMPLATES_PATH' );
		var strApiType = wizard.FindSymbol( 'FIXTURE_APITYPE' );
		var strTemplate = strTemplatePath + '\\' + strApiType + ".tpl";
		var strFile = strProjectPath + '\\' + wizard.FindSymbol( 'FIXTURE_FILE' );
		
		if ( selProj == null )
		{
			//
			// If launched via LaunchWizard, selProj is null. So we
			// have to look it up in the solution.
			//
			var projects = wizard.dte.Solution.Projects;
			for ( i = 1; i <= projects.Count; i++ )
			{
				var project = projects.Item( i );
				if ( project.Name == strProjectName )
				{
					selProj = project;
				}
			}
			
			if ( selProj == null )
			{
				wizard.ReportError( "Failed to locate project. Please use the 'Add Class' wizard." );
				return 0;
			}
		}
		
		//
		// Generate template code. If the file exists, the code is appended.
		//
		wizard.RenderTemplate( strTemplate, strFile, false );
		if ( selProj.Object.CanAddFile( strFile ) )
		{
			selProj.Object.AddFile( strFile );
		}
		
		//
		// Add include.
		//
		codeModel = selProj.CodeModel;
		codeModel.Synchronize();
		codeModel.StartTransaction("Add Unit Test");
		if ( ! DoesIncludeExist( selProj, "<" + strApiType + ".h>", strFile ) )
		{
			codeModel.AddInclude( "<" + strApiType + ".h>", strFile, vsCMAddPositionEnd );
		}
		
		//
		// Get path to stdafx.h, may be empty.
		//
		var strSTDAFX = GetProjectFile( selProj, "STDAFX", false, true );
				
		if ( strSTDAFX != "" )
		{
		    //
		    // Make sure that we include this, too.
		    //
		    if ( ! DoesIncludeExist(selProj, "\"" + strSTDAFX + "\"", strFile ) )
		    {
			    codeModel.AddInclude( "\"" + strSTDAFX + "\"", strFile, vsCMAddPositionStart );
		    }
		}

		codeModel.CommitTransaction();
		
		selProj.Object.Save();
		
		try
		{
			wizard.dte.ItemOperations.OpenFile( strFile );
		}
		catch (e)
		{}
	}
	catch(e)
	{
		if (e.description.length != 0)
		{
			SetErrorInfo( e );
		}
		
		return e.number
	}
}
