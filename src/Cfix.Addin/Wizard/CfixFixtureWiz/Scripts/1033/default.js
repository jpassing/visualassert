
function OnFinish(selProj, selObj)
{
	try
	{
		var strProjectPath = wizard.FindSymbol('PROJECT_PATH');
		var strTemplatePath = wizard.FindSymbol('TEMPLATES_PATH');
		var strApiType = wizard.FindSymbol('FIXTURE_APITYPE');
		var strTemplate = strTemplatePath + '\\' + strApiType + ".tpl";
		var strFile = strProjectPath + '\\' + wizard.FindSymbol('FIXTURE_FILE');

		//
		// Generate template code. If the file exists, the code is appended.
		//
		wizard.RenderTemplate(strTemplate, strFile, false);
		if (selProj.Object.CanAddFile(strFile))
		{
			selProj.Object.AddFile(strFile);
		}
		
		//
		// Add include.
		//
		codeModel = selProj.CodeModel;
		codeModel.Synchronize();
		codeModel.StartTransaction("Add Unit Test");
		if (!DoesIncludeExist(selProj, "<" + strApiType + ".h>", strFile))
		{
			codeModel.AddInclude("<" + strApiType + ".h>", strFile, vsCMAddPositionEnd);
		}

		codeModel.CommitTransaction();
		
		selProj.Object.Save();
	}
	catch(e)
	{
		if (e.description.length != 0)
		{
			SetErrorInfo(e);
		}
		
		return e.number
	}
}