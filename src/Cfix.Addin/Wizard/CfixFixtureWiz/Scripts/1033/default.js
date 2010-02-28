/*++
	This file is part of Visual Assert.
	
	Copyright 2009, Johannes Passing.
--*/

vsProjectKindSolutionFolder = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";
icProjectVcProject = "{EAF909A5-FA59-4C3D-9431-0FCC20D5BCF9}";

function FindProject( wizard, collection, strProjectName )
{
    for ( index = 1; index <= collection.Count; index++ )
	{
	    var project = collection.Item( index );
	    
		if ( project.Name == strProjectName )
		{
			return project;
		}
		else if ( project.kind == vsProjectKindSolutionFolder )
		{
		    //
		    // SolutionFolder -- ProjectItems collection contains a ProjectItem
		    // for each sub-project.
		    //
		    
		    //
		    // This is totally spooky -- JScript does not save the
		    // variable 'index' when recusrively calling this function.
		    //
		    var savedIndex = index;
		    var subproject = FindProject( wizard, project.ProjectItems, strProjectName );
		    index = savedIndex;
		    
		    if ( subproject != null )
		    {
		        return subproject.Object;
            }
		}
	}
	
	return null;
}

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
			selProj = FindProject( wizard, wizard.dte.Solution.Projects, strProjectName );
			
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
		
		//
		// N.B. The IC automation model is incomplete -- DoesIncludeExist
		// fails for IC projects. Skip the check for these kinds of projects
		// at the risk of getting duplicate includes.
		//
		if ( selProj.Kind == icProjectVcProject ||
		     ! DoesIncludeExist( selProj, "<" + strApiType + ".h>", strFile ) )
		{
			codeModel.AddInclude( "<" + strApiType + ".h>", strFile, vsCMAddPositionDefault );
		}
		
		//
		// Get path to stdafx.h, may be empty.
		//
		var strSTDAFX = ""
		try
		{
			strSTDAFX = GetProjectFile( selProj, "STDAFX", false, true );
		}
		catch (e)
		{
			//
			// Fails for makefile projects.
			//
		}
				
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
