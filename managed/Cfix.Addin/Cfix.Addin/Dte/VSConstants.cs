using System;
using System.Collections.Generic;
using System.Text;

//
// From: http://www.koders.com/csharp/fidEF11D49B5E8F93DBC9C21BA53CD2D54E7AA4F8A8.aspx.
//
namespace Cfix.Addin.Dte
{
	public enum VSStd97CmdID
	{
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AlignBottom"]/*' />
		AlignBottom = 1,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AlignHorizontalCenters"]/*' />
		AlignHorizontalCenters = 2,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AlignLeft"]/*' />
		AlignLeft = 3,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AlignRight"]/*' />
		AlignRight = 4,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AlignToGrid"]/*' />
		AlignToGrid = 5,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AlignTop"]/*' />
		AlignTop = 6,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AlignVerticalCenters"]/*' />
		AlignVerticalCenters = 7,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ArrangeBottom"]/*' />
		ArrangeBottom = 8,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ArrangeRight"]/*' />
		ArrangeRight = 9,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BringForward"]/*' />
		BringForward = 10,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BringToFront"]/*' />
		BringToFront = 11,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CenterHorizontally"]/*' />
		CenterHorizontally = 12,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CenterVertically"]/*' />
		CenterVertically = 13,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Code"]/*' />
		Code = 14,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Copy"]/*' />
		Copy = 15,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Cut"]/*' />
		Cut = 16,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Delete"]/*' />
		Delete = 17,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FontName"]/*' />
		FontName = 18,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FontNameGetList"]/*' />
		FontNameGetList = 500,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FontSize"]/*' />
		FontSize = 19,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FontSizeGetList"]/*' />
		FontSizeGetList = 501,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Group"]/*' />
		Group = 20,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.HorizSpaceConcatenate"]/*' />
		HorizSpaceConcatenate = 21,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.HorizSpaceDecrease"]/*' />
		HorizSpaceDecrease = 22,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.HorizSpaceIncrease"]/*' />
		HorizSpaceIncrease = 23,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.HorizSpaceMakeEqual"]/*' />
		HorizSpaceMakeEqual = 24,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.LockControls"]/*' />
		LockControls = 369,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.InsertObject"]/*' />
		InsertObject = 25,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Paste"]/*' />
		Paste = 26,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Print"]/*' />
		Print = 27,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Properties"]/*' />
		Properties = 28,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Redo"]/*' />
		Redo = 29,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MultiLevelRedo"]/*' />
		MultiLevelRedo = 30,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SelectAll"]/*' />
		SelectAll = 31,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SendBackward"]/*' />
		SendBackward = 32,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SendToBack"]/*' />
		SendToBack = 33,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowTable"]/*' />
		ShowTable = 34,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SizeToControl"]/*' />
		SizeToControl = 35,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SizeToControlHeight"]/*' />
		SizeToControlHeight = 36,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SizeToControlWidth"]/*' />
		SizeToControlWidth = 37,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SizeToFit"]/*' />
		SizeToFit = 38,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SizeToGrid"]/*' />
		SizeToGrid = 39,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SnapToGrid"]/*' />
		SnapToGrid = 40,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TabOrder"]/*' />
		TabOrder = 41,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Toolbox"]/*' />
		Toolbox = 42,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Undo"]/*' />
		Undo = 43,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MultiLevelUndo"]/*' />
		MultiLevelUndo = 44,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Ungroup"]/*' />
		Ungroup = 45,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.VertSpaceConcatenate"]/*' />
		VertSpaceConcatenate = 46,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.VertSpaceDecrease"]/*' />
		VertSpaceDecrease = 47,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.VertSpaceIncrease"]/*' />
		VertSpaceIncrease = 48,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.VertSpaceMakeEqual"]/*' />
		VertSpaceMakeEqual = 49,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ZoomPercent"]/*' />
		ZoomPercent = 50,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BackColor"]/*' />
		BackColor = 51,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Bold"]/*' />
		Bold = 52,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BorderColor"]/*' />
		BorderColor = 53,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BorderDashDot"]/*' />
		BorderDashDot = 54,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BorderDashDotDot"]/*' />
		BorderDashDotDot = 55,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BorderDashes"]/*' />
		BorderDashes = 56,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BorderDots"]/*' />
		BorderDots = 57,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BorderShortDashes"]/*' />
		BorderShortDashes = 58,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BorderSolid"]/*' />
		BorderSolid = 59,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BorderSparseDots"]/*' />
		BorderSparseDots = 60,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BorderWidth1"]/*' />
		BorderWidth1 = 61,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BorderWidth2"]/*' />
		BorderWidth2 = 62,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BorderWidth3"]/*' />
		BorderWidth3 = 63,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BorderWidth4"]/*' />
		BorderWidth4 = 64,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BorderWidth5"]/*' />
		BorderWidth5 = 65,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BorderWidth6"]/*' />
		BorderWidth6 = 66,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BorderWidthHairline"]/*' />
		BorderWidthHairline = 67,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Flat"]/*' />
		Flat = 68,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ForeColor"]/*' />
		ForeColor = 69,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Italic"]/*' />
		Italic = 70,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.JustifyCenter"]/*' />
		JustifyCenter = 71,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.JustifyGeneral"]/*' />
		JustifyGeneral = 72,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.JustifyLeft"]/*' />
		JustifyLeft = 73,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.JustifyRight"]/*' />
		JustifyRight = 74,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Raised"]/*' />
		Raised = 75,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Sunken"]/*' />
		Sunken = 76,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Underline"]/*' />
		Underline = 77,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Chiseled"]/*' />
		Chiseled = 78,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Etched"]/*' />
		Etched = 79,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Shadowed"]/*' />
		Shadowed = 80,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CompDebug1"]/*' />
		CompDebug1 = 81,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CompDebug2"]/*' />
		CompDebug2 = 82,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CompDebug3"]/*' />
		CompDebug3 = 83,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CompDebug4"]/*' />
		CompDebug4 = 84,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CompDebug5"]/*' />
		CompDebug5 = 85,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CompDebug6"]/*' />
		CompDebug6 = 86,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CompDebug7"]/*' />
		CompDebug7 = 87,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CompDebug8"]/*' />
		CompDebug8 = 88,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CompDebug9"]/*' />
		CompDebug9 = 89,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CompDebug10"]/*' />
		CompDebug10 = 90,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CompDebug11"]/*' />
		CompDebug11 = 91,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CompDebug12"]/*' />
		CompDebug12 = 92,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CompDebug13"]/*' />
		CompDebug13 = 93,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CompDebug14"]/*' />
		CompDebug14 = 94,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CompDebug15"]/*' />
		CompDebug15 = 95,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExistingSchemaEdit"]/*' />
		ExistingSchemaEdit = 96,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Find"]/*' />
		Find = 97,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.GetZoom"]/*' />
		GetZoom = 98,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.QueryOpenDesign"]/*' />
		QueryOpenDesign = 99,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.QueryOpenNew"]/*' />
		QueryOpenNew = 100,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SingleTableDesign"]/*' />
		SingleTableDesign = 101,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SingleTableNew"]/*' />
		SingleTableNew = 102,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowGrid"]/*' />
		ShowGrid = 103,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.NewTable"]/*' />
		NewTable = 104,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CollapsedView"]/*' />
		CollapsedView = 105,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FieldView"]/*' />
		FieldView = 106,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.VerifySQL"]/*' />
		VerifySQL = 107,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.HideTable"]/*' />
		HideTable = 108,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PrimaryKey"]/*' />
		PrimaryKey = 109,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Save"]/*' />
		Save = 110,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SaveAs"]/*' />
		SaveAs = 111,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SortAscending"]/*' />
		SortAscending = 112,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SortDescending"]/*' />
		SortDescending = 113,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AppendQuery"]/*' />
		AppendQuery = 114,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CrosstabQuery"]/*' />
		CrosstabQuery = 115,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DeleteQuery"]/*' />
		DeleteQuery = 116,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MakeTableQuery"]/*' />
		MakeTableQuery = 117,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SelectQuery"]/*' />
		SelectQuery = 118,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.UpdateQuery"]/*' />
		UpdateQuery = 119,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Parameters"]/*' />
		Parameters = 120,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Totals"]/*' />
		Totals = 121,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewCollapsed"]/*' />
		ViewCollapsed = 122,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewFieldList"]/*' />
		ViewFieldList = 123,


		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewKeys"]/*' />
		ViewKeys = 124,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewGrid"]/*' />
		ViewGrid = 125,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.InnerJoin"]/*' />
		InnerJoin = 126,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RightOuterJoin"]/*' />
		RightOuterJoin = 127,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.LeftOuterJoin"]/*' />
		LeftOuterJoin = 128,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FullOuterJoin"]/*' />
		FullOuterJoin = 129,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.UnionJoin"]/*' />
		UnionJoin = 130,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowSQLPane"]/*' />
		ShowSQLPane = 131,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowGraphicalPane"]/*' />
		ShowGraphicalPane = 132,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowDataPane"]/*' />
		ShowDataPane = 133,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowQBEPane"]/*' />
		ShowQBEPane = 134,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SelectAllFields"]/*' />
		SelectAllFields = 135,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OLEObjectMenuButton"]/*' />
		OLEObjectMenuButton = 136,

		// ids on the ole verbs menu - these must be sequential ie verblist0-verblist9
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ObjectVerbList0"]/*' />
		ObjectVerbList0 = 137,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ObjectVerbList1"]/*' />
		ObjectVerbList1 = 138,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ObjectVerbList2"]/*' />
		ObjectVerbList2 = 139,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ObjectVerbList3"]/*' />
		ObjectVerbList3 = 140,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ObjectVerbList4"]/*' />
		ObjectVerbList4 = 141,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ObjectVerbList5"]/*' />
		ObjectVerbList5 = 142,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ObjectVerbList6"]/*' />
		ObjectVerbList6 = 143,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ObjectVerbList7"]/*' />
		ObjectVerbList7 = 144,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ObjectVerbList8"]/*' />
		ObjectVerbList8 = 145,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ObjectVerbList9"]/*' />
		ObjectVerbList9 = 146,  // Unused on purpose!

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ConvertObject"]/*' />
		ConvertObject = 147,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CustomControl"]/*' />
		CustomControl = 148,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CustomizeItem"]/*' />
		CustomizeItem = 149,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Rename"]/*' />
		Rename = 150,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Import"]/*' />
		Import = 151,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.NewPage"]/*' />
		NewPage = 152,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Move"]/*' />
		Move = 153,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Cancel"]/*' />
		Cancel = 154,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Font"]/*' />
		Font = 155,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExpandLinks"]/*' />
		ExpandLinks = 156,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExpandImages"]/*' />
		ExpandImages = 157,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExpandPages"]/*' />
		ExpandPages = 158,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RefocusDiagram"]/*' />
		RefocusDiagram = 159,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TransitiveClosure"]/*' />
		TransitiveClosure = 160,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CenterDiagram"]/*' />
		CenterDiagram = 161,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ZoomIn"]/*' />
		ZoomIn = 162,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ZoomOut"]/*' />
		ZoomOut = 163,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RemoveFilter"]/*' />
		RemoveFilter = 164,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.HidePane"]/*' />
		HidePane = 165,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DeleteTable"]/*' />
		DeleteTable = 166,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DeleteRelationship"]/*' />
		DeleteRelationship = 167,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Remove"]/*' />
		Remove = 168,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.JoinLeftAll"]/*' />
		JoinLeftAll = 169,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.JoinRightAll"]/*' />
		JoinRightAll = 170,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AddToOutput"]/*' />
		AddToOutput = 171,      // Add selected fields to query output
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OtherQuery"]/*' />
		OtherQuery = 172,      // change query type to 'other'
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.GenerateChangeScript"]/*' />
		GenerateChangeScript = 173,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SaveSelection"]/*' />
		SaveSelection = 174,     // Save current selection
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutojoinCurrent"]/*' />
		AutojoinCurrent = 175,     // Autojoin current tables
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutojoinAlways"]/*' />
		AutojoinAlways = 176,     // Toggle Autojoin state
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.EditPage"]/*' />
		EditPage = 177,     // Launch editor for url
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewLinks"]/*' />
		ViewLinks = 178,     // Launch new webscope for url
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Stop"]/*' />
		Stop = 179,     // Stope webscope rendering
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Pause"]/*' />
		Pause = 180,     // Pause webscope rendering
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Resume"]/*' />
		Resume = 181,     // Resume webscope rendering
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FilterDiagram"]/*' />
		FilterDiagram = 182,     // Filter webscope diagram
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowAllObjects"]/*' />
		ShowAllObjects = 183,     // Show All objects in webscope diagram
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowApplications"]/*' />
		ShowApplications = 184,     // Show Application objects in webscope diagram
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowOtherObjects"]/*' />
		ShowOtherObjects = 185,     // Show other objects in webscope diagram
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowPrimRelationships"]/*' />
		ShowPrimRelationships = 186,     // Show primary relationships
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Expand"]/*' />
		Expand = 187,     // Expand links
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Collapse"]/*' />
		Collapse = 188,     // Collapse links
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Refresh"]/*' />
		Refresh = 189,     // Refresh Webscope diagram
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Layout"]/*' />
		Layout = 190,     // Layout websope diagram
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowResources"]/*' />
		ShowResources = 191,     // Show resouce objects in webscope diagram
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.InsertHTMLWizard"]/*' />
		InsertHTMLWizard = 192,     // Insert HTML using a Wizard
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowDownloads"]/*' />
		ShowDownloads = 193,     // Show download objects in webscope diagram
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowExternals"]/*' />
		ShowExternals = 194,     // Show external objects in webscope diagram
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowInBoundLinks"]/*' />
		ShowInBoundLinks = 195,     // Show inbound links in webscope diagram
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowOutBoundLinks"]/*' />
		ShowOutBoundLinks = 196,     // Show out bound links in webscope diagram
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowInAndOutBoundLinks"]/*' />
		ShowInAndOutBoundLinks = 197,     // Show in and out bound links in webscope diagram
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Preview"]/*' />
		Preview = 198,     // Preview page
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Open"]/*' />
		Open = 261,     // Open
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OpenWith"]/*' />
		OpenWith = 199,     // Open with
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowPages"]/*' />
		ShowPages = 200,     // Show HTML pages
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RunQuery"]/*' />
		RunQuery = 201,      // Runs a query
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ClearQuery"]/*' />
		ClearQuery = 202,      // Clears the query's associated cursor
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RecordFirst"]/*' />
		RecordFirst = 203,      // Go to first record in set
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RecordLast"]/*' />
		RecordLast = 204,      // Go to last record in set
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RecordNext"]/*' />
		RecordNext = 205,      // Go to next record in set
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RecordPrevious"]/*' />
		RecordPrevious = 206,      // Go to previous record in set
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RecordGoto"]/*' />
		RecordGoto = 207,      // Go to record via dialog
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RecordNew"]/*' />
		RecordNew = 208,      // Add a record to set

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.InsertNewMenu"]/*' />
		InsertNewMenu = 209,     // menu designer
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.InsertSeparator"]/*' />
		InsertSeparator = 210,     // menu designer
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.EditMenuNames"]/*' />
		EditMenuNames = 211,     // menu designer

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DebugExplorer"]/*' />
		DebugExplorer = 212,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DebugProcesses"]/*' />
		DebugProcesses = 213,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewThreadsWindow"]/*' />
		ViewThreadsWindow = 214,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.WindowUIList"]/*' />
		WindowUIList = 215,

		// ids on the file menu
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.NewProject"]/*' />
		NewProject = 216,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OpenProject"]/*' />
		OpenProject = 217,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OpenProjectFromWeb"]/*' />
		OpenProjectFromWeb = 450,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OpenSolution"]/*' />
		OpenSolution = 218,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CloseSolution"]/*' />
		CloseSolution = 219,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FileNew"]/*' />
		FileNew = 221,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.NewProjectFromExisting"]/*' />
		NewProjectFromExisting = 385,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FileOpen"]/*' />
		FileOpen = 222,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FileOpenFromWeb"]/*' />
		FileOpenFromWeb = 451,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FileClose"]/*' />
		FileClose = 223,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SaveSolution"]/*' />
		SaveSolution = 224,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SaveSolutionAs"]/*' />
		SaveSolutionAs = 225,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SaveProjectItemAs"]/*' />
		SaveProjectItemAs = 226,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PageSetup"]/*' />
		PageSetup = 227,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PrintPreview"]/*' />
		PrintPreview = 228,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Exit"]/*' />
		Exit = 229,

		// ids on the edit menu
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Replace"]/*' />
		Replace = 230,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Goto"]/*' />
		Goto = 231,

		// ids on the view menu
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PropertyPages"]/*' />
		PropertyPages = 232,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FullScreen"]/*' />
		FullScreen = 233,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ProjectExplorer"]/*' />
		ProjectExplorer = 234,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PropertiesWindow"]/*' />
		PropertiesWindow = 235,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListWindow"]/*' />
		TaskListWindow = 236,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OutputWindow"]/*' />
		OutputWindow = 237,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ObjectBrowser"]/*' />
		ObjectBrowser = 238,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DocOutlineWindow"]/*' />
		DocOutlineWindow = 239,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ImmediateWindow"]/*' />
		ImmediateWindow = 240,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.WatchWindow"]/*' />
		WatchWindow = 241,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.LocalsWindow"]/*' />
		LocalsWindow = 242,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CallStack"]/*' />
		CallStack = 243,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutosWindow"]/*' />
		AutosWindow = DebugReserved1,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ThisWindow"]/*' />
		ThisWindow = DebugReserved2,

		// ids on project menu
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AddNewItem"]/*' />
		AddNewItem = 220,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AddExistingItem"]/*' />
		AddExistingItem = 244,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.NewFolder"]/*' />
		NewFolder = 245,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SetStartupProject"]/*' />
		SetStartupProject = 246,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ProjectSettings"]/*' />
		ProjectSettings = 247,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ProjectReferences"]/*' />
		ProjectReferences = 367,

		// ids on the debug menu
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.StepInto"]/*' />
		StepInto = 248,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.StepOver"]/*' />
		StepOver = 249,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.StepOut"]/*' />
		StepOut = 250,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RunToCursor"]/*' />
		RunToCursor = 251,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AddWatch"]/*' />
		AddWatch = 252,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.EditWatch"]/*' />
		EditWatch = 253,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.QuickWatch"]/*' />
		QuickWatch = 254,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ToggleBreakpoint"]/*' />
		ToggleBreakpoint = 255,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ClearBreakpoints"]/*' />
		ClearBreakpoints = 256,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowBreakpoints"]/*' />
		ShowBreakpoints = 257,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SetNextStatement"]/*' />
		SetNextStatement = 258,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowNextStatement"]/*' />
		ShowNextStatement = 259,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.EditBreakpoint"]/*' />
		EditBreakpoint = 260,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DetachDebugger"]/*' />
		DetachDebugger = 262,

		// ids on the tools menu
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CustomizeKeyboard"]/*' />
		CustomizeKeyboard = 263,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ToolsOptions"]/*' />
		ToolsOptions = 264,

		// ids on the windows menu
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.NewWindow"]/*' />
		NewWindow = 265,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Split"]/*' />
		Split = 266,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Cascade"]/*' />
		Cascade = 267,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TileHorz"]/*' />
		TileHorz = 268,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TileVert"]/*' />
		TileVert = 269,

		// ids on the help menu
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TechSupport"]/*' />
		TechSupport = 270,

		// NOTE cmdidAbout and cmdidDebugOptions must be consecutive
		//      cmd after cmdidDebugOptions (ie 273) must not be used
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.About"]/*' />
		About = 271,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DebugOptions"]/*' />
		DebugOptions = 272,

		// ids on the watch context menu
		// CollapseWatch appears as 'Collapse Parent', on any
		// non-top-level item
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DeleteWatch"]/*' />
		DeleteWatch = 274,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CollapseWatch"]/*' />
		CollapseWatch = 275,
		// ids 276, 277, 278, 279, 280 are in use
		// below 
		// ids on the property browser context menu
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PbrsToggleStatus"]/*' />
		PbrsToggleStatus = 282,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PropbrsHide"]/*' />
		PropbrsHide = 283,

		// ids on the docking context menu
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DockingView"]/*' />
		DockingView = 284,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.HideActivePane"]/*' />
		HideActivePane = 285,
		// ids for window selection via keyboard
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PaneNextPane"]/*' />
		PaneNextPane = 316,  //(listed below in order)
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PanePrevPane"]/*' />
		PanePrevPane = 317,  //(listed below in order)
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PaneNextTab"]/*' />
		PaneNextTab = 286,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PanePrevTab"]/*' />
		PanePrevTab = 287,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PaneCloseToolWindow"]/*' />
		PaneCloseToolWindow = 288,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PaneActivateDocWindow"]/*' />
		PaneActivateDocWindow = 289,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DockingViewMDI"]/*' />
		DockingViewMDI = 290,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DockingViewFloater"]/*' />
		DockingViewFloater = 291,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideWindow"]/*' />
		AutoHideWindow = 292,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MoveToDropdownBar"]/*' />
		MoveToDropdownBar = 293,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FindCmd"]/*' />
		FindCmd = 294,  // internal Find commands
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Start"]/*' />
		Start = 295,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Restart"]/*' />
		Restart = 296,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AddinManager"]/*' />
		AddinManager = 297,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MultiLevelUndoList"]/*' />
		MultiLevelUndoList = 298,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MultiLevelRedoList"]/*' />
		MultiLevelRedoList = 299,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ToolboxAddTab"]/*' />
		ToolboxAddTab = 300,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ToolboxDeleteTab"]/*' />
		ToolboxDeleteTab = 301,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ToolboxRenameTab"]/*' />
		ToolboxRenameTab = 302,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ToolboxTabMoveUp"]/*' />
		ToolboxTabMoveUp = 303,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ToolboxTabMoveDown"]/*' />
		ToolboxTabMoveDown = 304,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ToolboxRenameItem"]/*' />
		ToolboxRenameItem = 305,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ToolboxListView"]/*' />
		ToolboxListView = 306,
		//(below) cmdidSearchSetCombo        307

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.WindowUIGetList"]/*' />
		WindowUIGetList = 308,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.InsertValuesQuery"]/*' />
		InsertValuesQuery = 309,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowProperties"]/*' />
		ShowProperties = 310,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ThreadSuspend"]/*' />
		ThreadSuspend = 311,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ThreadResume"]/*' />
		ThreadResume = 312,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ThreadSetFocus"]/*' />
		ThreadSetFocus = 313,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DisplayRadix"]/*' />
		DisplayRadix = 314,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OpenProjectItem"]/*' />
		OpenProjectItem = 315,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ClearPane"]/*' />
		ClearPane = 318,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.GotoErrorTag"]/*' />
		GotoErrorTag = 319,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListSortByCategory"]/*' />
		TaskListSortByCategory = 320,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListSortByFileLine"]/*' />
		TaskListSortByFileLine = 321,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListSortByPriority"]/*' />
		TaskListSortByPriority = 322,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListSortByDefaultSort"]/*' />
		TaskListSortByDefaultSort = 323,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListShowTooltip"]/*' />
		TaskListShowTooltip = 324,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListFilterByNothing"]/*' />
		TaskListFilterByNothing = 325,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CancelEZDrag"]/*' />
		CancelEZDrag = 326,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListFilterByCategoryCompiler"]/*' />
		TaskListFilterByCategoryCompiler = 327,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListFilterByCategoryComment"]/*' />
		TaskListFilterByCategoryComment = 328,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ToolboxAddItem"]/*' />
		ToolboxAddItem = 329,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ToolboxReset"]/*' />
		ToolboxReset = 330,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SaveProjectItem"]/*' />
		SaveProjectItem = 331,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SaveOptions"]/*' />
		SaveOptions = 959,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewForm"]/*' />
		ViewForm = 332,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewCode"]/*' />
		ViewCode = 333,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PreviewInBrowser"]/*' />
		PreviewInBrowser = 334,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BrowseWith"]/*' />
		BrowseWith = 336,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SearchSetCombo"]/*' />
		SearchSetCombo = 307,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SearchCombo"]/*' />
		SearchCombo = 337,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.EditLabel"]/*' />
		EditLabel = 338,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Exceptions"]/*' />
		Exceptions = 339,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DefineViews"]/*' />
		DefineViews = 340,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ToggleSelMode"]/*' />
		ToggleSelMode = 341,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ToggleInsMode"]/*' />
		ToggleInsMode = 342,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.LoadUnloadedProject"]/*' />
		LoadUnloadedProject = 343,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.UnloadLoadedProject"]/*' />
		UnloadLoadedProject = 344,

		// ids on the treegrids (watch/local/threads/stack)
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ElasticColumn"]/*' />
		ElasticColumn = 345,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.HideColumn"]/*' />
		HideColumn = 346,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListPreviousView"]/*' />
		TaskListPreviousView = 347,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ZoomDialog"]/*' />
		ZoomDialog = 348,

		// find/replace options
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FindHiddenText"]/*' />
		FindHiddenText = 349,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FindMatchCase"]/*' />
		FindMatchCase = 350,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FindWholeWord"]/*' />
		FindWholeWord = 351,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FindSimplePattern"]/*' />
		FindSimplePattern = 276,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FindRegularExpression"]/*' />
		FindRegularExpression = 352,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FindBackwards"]/*' />
		FindBackwards = 353,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FindInSelection"]/*' />
		FindInSelection = 354,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FindStop"]/*' />
		FindStop = 355,
		// UNUSED                               356
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FindInFiles"]/*' />
		FindInFiles = 277,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ReplaceInFiles"]/*' />
		ReplaceInFiles = 278,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.NextLocation"]/*' />
		NextLocation = 279,  // next item in task list, find in files results, etc.
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PreviousLocation"]/*' />
		PreviousLocation = 280,  // prev item "
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.GotoQuick"]/*' />
		GotoQuick = 281,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListNextError"]/*' />
		TaskListNextError = 357,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListPrevError"]/*' />
		TaskListPrevError = 358,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListFilterByCategoryUser"]/*' />
		TaskListFilterByCategoryUser = 359,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListFilterByCategoryShortcut"]/*' />
		TaskListFilterByCategoryShortcut = 360,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListFilterByCategoryHTML"]/*' />
		TaskListFilterByCategoryHTML = 361,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListFilterByCurrentFile"]/*' />
		TaskListFilterByCurrentFile = 362,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListFilterByChecked"]/*' />
		TaskListFilterByChecked = 363,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListFilterByUnchecked"]/*' />
		TaskListFilterByUnchecked = 364,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListSortByDescription"]/*' />
		TaskListSortByDescription = 365,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListSortByChecked"]/*' />
		TaskListSortByChecked = 366,

		// 367 is used above in cmdidProjectReferences
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.StartNoDebug"]/*' />
		StartNoDebug = 368,
		// 369 is used above in cmdidLockControls

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FindNext"]/*' />
		FindNext = 370,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FindPrev"]/*' />
		FindPrev = 371,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FindSelectedNext"]/*' />
		FindSelectedNext = 372,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FindSelectedPrev"]/*' />
		FindSelectedPrev = 373,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SearchGetList"]/*' />
		SearchGetList = 374,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.InsertBreakpoint"]/*' />
		InsertBreakpoint = 375,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.EnableBreakpoint"]/*' />
		EnableBreakpoint = 376,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.F1Help"]/*' />
		F1Help = 377,

		//UNUSED 378-396

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MoveToNextEZCntr"]/*' />
		MoveToNextEZCntr = 384,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.UpdateMarkerSpans"]/*' />
		UpdateMarkerSpans = 386,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MoveToPreviousEZCntr"]/*' />
		MoveToPreviousEZCntr = 393,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ProjectProperties"]/*' />
		ProjectProperties = 396,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PropSheetOrProperties"]/*' />
		PropSheetOrProperties = 397,

		// NOTE - the next items are debug only !!
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TshellStep"]/*' />
		TshellStep = 398,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TshellRun"]/*' />
		TshellRun = 399,

		// marker commands on the codewin menu
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MarkerCmd0"]/*' />
		MarkerCmd0 = 400,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MarkerCmd1"]/*' />
		MarkerCmd1 = 401,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MarkerCmd2"]/*' />
		MarkerCmd2 = 402,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MarkerCmd3"]/*' />
		MarkerCmd3 = 403,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MarkerCmd4"]/*' />
		MarkerCmd4 = 404,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MarkerCmd5"]/*' />
		MarkerCmd5 = 405,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MarkerCmd6"]/*' />
		MarkerCmd6 = 406,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MarkerCmd7"]/*' />
		MarkerCmd7 = 407,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MarkerCmd8"]/*' />
		MarkerCmd8 = 408,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MarkerCmd9"]/*' />
		MarkerCmd9 = 409,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MarkerLast"]/*' />
		MarkerLast = 409,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MarkerEnd"]/*' />
		MarkerEnd = 410,  // list terminator reserved

		// user-invoked project reload and unload
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ReloadProject"]/*' />
		ReloadProject = 412,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.UnloadProject"]/*' />
		UnloadProject = 413,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.NewBlankSolution"]/*' />
		NewBlankSolution = 414,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SelectProjectTemplate"]/*' />
		SelectProjectTemplate = 415,

		// document outline commands
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DetachAttachOutline"]/*' />
		DetachAttachOutline = 420,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowHideOutline"]/*' />
		ShowHideOutline = 421,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SyncOutline"]/*' />
		SyncOutline = 422,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RunToCallstCursor"]/*' />
		RunToCallstCursor = 423,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.NoCmdsAvailable"]/*' />
		NoCmdsAvailable = 424,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ContextWindow"]/*' />
		ContextWindow = 427,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Alias"]/*' />
		Alias = 428,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.GotoCommandLine"]/*' />
		GotoCommandLine = 429,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.EvaluateExpression"]/*' />
		EvaluateExpression = 430,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ImmediateMode"]/*' />
		ImmediateMode = 431,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.EvaluateStatement"]/*' />
		EvaluateStatement = 432,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FindResultWindow1"]/*' />
		FindResultWindow1 = 433,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FindResultWindow2"]/*' />
		FindResultWindow2 = 434,

		// 500 is used above in cmdidFontNameGetList
		// 501 is used above in cmdidFontSizeGetList

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RenameBookmark"]/*' />
		RenameBookmark = 559,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ToggleBookmark"]/*' />
		ToggleBookmark = 560,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DeleteBookmark"]/*' />
		DeleteBookmark = 561,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BookmarkWindowGoToBookmark"]/*' />
		BookmarkWindowGoToBookmark = 562,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.EnableBookmark"]/*' />
		EnableBookmark = 564,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.NewBookmarkFolder"]/*' />
		NewBookmarkFolder = 565,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.NextBookmarkFolder"]/*' />
		NextBookmarkFolder = 568,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PrevBookmarkFolder"]/*' />
		PrevBookmarkFolder = 569,

		// ids on the window menu - these must be sequential ie window1-morewind
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window1"]/*' />
		Window1 = 570,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window2"]/*' />
		Window2 = 571,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window3"]/*' />
		Window3 = 572,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window4"]/*' />
		Window4 = 573,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window5"]/*' />
		Window5 = 574,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window6"]/*' />
		Window6 = 575,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window7"]/*' />
		Window7 = 576,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window8"]/*' />
		Window8 = 577,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window9"]/*' />
		Window9 = 578,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window10"]/*' />
		Window10 = 579,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window11"]/*' />
		Window11 = 580,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window12"]/*' />
		Window12 = 581,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window13"]/*' />
		Window13 = 582,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window14"]/*' />
		Window14 = 583,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window15"]/*' />
		Window15 = 584,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window16"]/*' />
		Window16 = 585,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window17"]/*' />
		Window17 = 586,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window18"]/*' />
		Window18 = 587,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window19"]/*' />
		Window19 = 588,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window20"]/*' />
		Window20 = 589,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window21"]/*' />
		Window21 = 590,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window22"]/*' />
		Window22 = 591,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window23"]/*' />
		Window23 = 592,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window24"]/*' />
		Window24 = 593,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Window25"]/*' />
		Window25 = 594,    // note cmdidWindow25 is unused on purpose!
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MoreWindows"]/*' />
		MoreWindows = 595,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideAllWindows"]/*' />
		AutoHideAllWindows = 597,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListTaskHelp"]/*' />
		TaskListTaskHelp = 598,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ClassView"]/*' />
		ClassView = 599,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj1"]/*' />
		MRUProj1 = 600,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj2"]/*' />
		MRUProj2 = 601,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj3"]/*' />
		MRUProj3 = 602,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj4"]/*' />
		MRUProj4 = 603,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj5"]/*' />
		MRUProj5 = 604,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj6"]/*' />
		MRUProj6 = 605,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj7"]/*' />
		MRUProj7 = 606,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj8"]/*' />
		MRUProj8 = 607,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj9"]/*' />
		MRUProj9 = 608,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj10"]/*' />
		MRUProj10 = 609,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj11"]/*' />
		MRUProj11 = 610,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj12"]/*' />
		MRUProj12 = 611,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj13"]/*' />
		MRUProj13 = 612,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj14"]/*' />
		MRUProj14 = 613,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj15"]/*' />
		MRUProj15 = 614,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj16"]/*' />
		MRUProj16 = 615,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj17"]/*' />
		MRUProj17 = 616,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj18"]/*' />
		MRUProj18 = 617,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj19"]/*' />
		MRUProj19 = 618,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj20"]/*' />
		MRUProj20 = 619,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj21"]/*' />
		MRUProj21 = 620,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj22"]/*' />
		MRUProj22 = 621,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj23"]/*' />
		MRUProj23 = 622,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj24"]/*' />
		MRUProj24 = 623,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUProj25"]/*' />
		MRUProj25 = 624,   // note cmdidMRUProj25 is unused on purpose!

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SplitNext"]/*' />
		SplitNext = 625,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SplitPrev"]/*' />
		SplitPrev = 626,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CloseAllDocuments"]/*' />
		CloseAllDocuments = 627,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.NextDocument"]/*' />
		NextDocument = 628,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PrevDocument"]/*' />
		PrevDocument = 629,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool1"]/*' />
		Tool1 = 630,   // note cmdidTool1 - cmdidTool24 must be
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool2"]/*' />
		Tool2 = 631,   // consecutive
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool3"]/*' />
		Tool3 = 632,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool4"]/*' />
		Tool4 = 633,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool5"]/*' />
		Tool5 = 634,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool6"]/*' />
		Tool6 = 635,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool7"]/*' />
		Tool7 = 636,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool8"]/*' />
		Tool8 = 637,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool9"]/*' />
		Tool9 = 638,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool10"]/*' />
		Tool10 = 639,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool11"]/*' />
		Tool11 = 640,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool12"]/*' />
		Tool12 = 641,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool13"]/*' />
		Tool13 = 642,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool14"]/*' />
		Tool14 = 643,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool15"]/*' />
		Tool15 = 644,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool16"]/*' />
		Tool16 = 645,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool17"]/*' />
		Tool17 = 646,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool18"]/*' />
		Tool18 = 647,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool19"]/*' />
		Tool19 = 648,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool20"]/*' />
		Tool20 = 649,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool21"]/*' />
		Tool21 = 650,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool22"]/*' />
		Tool22 = 651,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool23"]/*' />
		Tool23 = 652,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Tool24"]/*' />
		Tool24 = 653,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExternalCommands"]/*' />
		ExternalCommands = 654,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PasteNextTBXCBItem"]/*' />
		PasteNextTBXCBItem = 655,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ToolboxShowAllTabs"]/*' />
		ToolboxShowAllTabs = 656,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ProjectDependencies"]/*' />
		ProjectDependencies = 657,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CloseDocument"]/*' />
		CloseDocument = 658,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ToolboxSortItems"]/*' />
		ToolboxSortItems = 659,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView1"]/*' />
		ViewBarView1 = 660,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView2"]/*' />
		ViewBarView2 = 661,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView3"]/*' />
		ViewBarView3 = 662,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView4"]/*' />
		ViewBarView4 = 663,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView5"]/*' />
		ViewBarView5 = 664,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView6"]/*' />
		ViewBarView6 = 665,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView7"]/*' />
		ViewBarView7 = 666,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView8"]/*' />
		ViewBarView8 = 667,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView9"]/*' />
		ViewBarView9 = 668,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView10"]/*' />
		ViewBarView10 = 669,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView11"]/*' />
		ViewBarView11 = 670,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView12"]/*' />
		ViewBarView12 = 671,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView13"]/*' />
		ViewBarView13 = 672,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView14"]/*' />
		ViewBarView14 = 673,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView15"]/*' />
		ViewBarView15 = 674,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView16"]/*' />
		ViewBarView16 = 675,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView17"]/*' />
		ViewBarView17 = 676,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView18"]/*' />
		ViewBarView18 = 677,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView19"]/*' />
		ViewBarView19 = 678,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView20"]/*' />
		ViewBarView20 = 679,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView21"]/*' />
		ViewBarView21 = 680,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView22"]/*' />
		ViewBarView22 = 681,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView23"]/*' />
		ViewBarView23 = 682,    //UNUSED
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewBarView24"]/*' />
		ViewBarView24 = 683,    //UNUSED

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SolutionCfg"]/*' />
		SolutionCfg = 684,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SolutionCfgGetList"]/*' />
		SolutionCfgGetList = 685,

		//
		// Schema table commands:
		// All invoke table property dialog and select appropriate page.
		//
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ManageIndexes"]/*' />
		ManageIndexes = 675,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ManageRelationships"]/*' />
		ManageRelationships = 676,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ManageConstraints"]/*' />
		ManageConstraints = 677,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView1"]/*' />
		TaskListCustomView1 = 678,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView2"]/*' />
		TaskListCustomView2 = 679,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView3"]/*' />
		TaskListCustomView3 = 680,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView4"]/*' />
		TaskListCustomView4 = 681,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView5"]/*' />
		TaskListCustomView5 = 682,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView6"]/*' />
		TaskListCustomView6 = 683,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView7"]/*' />
		TaskListCustomView7 = 684,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView8"]/*' />
		TaskListCustomView8 = 685,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView9"]/*' />
		TaskListCustomView9 = 686,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView10"]/*' />
		TaskListCustomView10 = 687,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView11"]/*' />
		TaskListCustomView11 = 688,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView12"]/*' />
		TaskListCustomView12 = 689,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView13"]/*' />
		TaskListCustomView13 = 690,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView14"]/*' />
		TaskListCustomView14 = 691,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView15"]/*' />
		TaskListCustomView15 = 692,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView16"]/*' />
		TaskListCustomView16 = 693,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView17"]/*' />
		TaskListCustomView17 = 694,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView18"]/*' />
		TaskListCustomView18 = 695,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView19"]/*' />
		TaskListCustomView19 = 696,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView20"]/*' />
		TaskListCustomView20 = 697,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView21"]/*' />
		TaskListCustomView21 = 698,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView22"]/*' />
		TaskListCustomView22 = 699,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView23"]/*' />
		TaskListCustomView23 = 700,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView24"]/*' />
		TaskListCustomView24 = 701,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView25"]/*' />
		TaskListCustomView25 = 702,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView26"]/*' />
		TaskListCustomView26 = 703,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView27"]/*' />
		TaskListCustomView27 = 704,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView28"]/*' />
		TaskListCustomView28 = 705,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView29"]/*' />
		TaskListCustomView29 = 706,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView30"]/*' />
		TaskListCustomView30 = 707,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView31"]/*' />
		TaskListCustomView31 = 708,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView32"]/*' />
		TaskListCustomView32 = 709,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView33"]/*' />
		TaskListCustomView33 = 710,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView34"]/*' />
		TaskListCustomView34 = 711,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView35"]/*' />
		TaskListCustomView35 = 712,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView36"]/*' />
		TaskListCustomView36 = 713,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView37"]/*' />
		TaskListCustomView37 = 714,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView38"]/*' />
		TaskListCustomView38 = 715,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView39"]/*' />
		TaskListCustomView39 = 716,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView40"]/*' />
		TaskListCustomView40 = 717,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView41"]/*' />
		TaskListCustomView41 = 718,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView42"]/*' />
		TaskListCustomView42 = 719,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView43"]/*' />
		TaskListCustomView43 = 720,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView44"]/*' />
		TaskListCustomView44 = 721,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView45"]/*' />
		TaskListCustomView45 = 722,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView46"]/*' />
		TaskListCustomView46 = 723,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView47"]/*' />
		TaskListCustomView47 = 724,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView48"]/*' />
		TaskListCustomView48 = 725,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView49"]/*' />
		TaskListCustomView49 = 726,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaskListCustomView50"]/*' />
		TaskListCustomView50 = 727,  //not used on purpose, ends the list

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.WhiteSpace"]/*' />
		WhiteSpace = 728,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CommandWindow"]/*' />
		CommandWindow = 729,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CommandWindowMarkMode"]/*' />
		CommandWindowMarkMode = 730,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.LogCommandWindow"]/*' />
		LogCommandWindow = 731,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Shell"]/*' />
		Shell = 732,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SingleChar"]/*' />
		SingleChar = 733,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ZeroOrMore"]/*' />
		ZeroOrMore = 734,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OneOrMore"]/*' />
		OneOrMore = 735,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BeginLine"]/*' />
		BeginLine = 736,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.EndLine"]/*' />
		EndLine = 737,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BeginWord"]/*' />
		BeginWord = 738,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.EndWord"]/*' />
		EndWord = 739,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CharInSet"]/*' />
		CharInSet = 740,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CharNotInSet"]/*' />
		CharNotInSet = 741,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Or"]/*' />
		Or = 742,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Escape"]/*' />
		Escape = 743,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TagExp"]/*' />
		TagExp = 744,

		// Regex builder context help menu commands
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PatternMatchHelp"]/*' />
		PatternMatchHelp = 745,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RegExList"]/*' />
		RegExList = 746,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DebugReserved1"]/*' />
		DebugReserved1 = 747,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DebugReserved2"]/*' />
		DebugReserved2 = 748,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DebugReserved3"]/*' />
		DebugReserved3 = 749,
		//USED ABOVE                        750
		//USED ABOVE                        751
		//USED ABOVE                        752
		//USED ABOVE                        753

		//Regex builder wildcard menu commands
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.WildZeroOrMore"]/*' />
		WildZeroOrMore = 754,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.WildSingleChar"]/*' />
		WildSingleChar = 755,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.WildSingleDigit"]/*' />
		WildSingleDigit = 756,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.WildCharInSet"]/*' />
		WildCharInSet = 757,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.WildCharNotInSet"]/*' />
		WildCharNotInSet = 758,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FindWhatText"]/*' />
		FindWhatText = 759,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaggedExp1"]/*' />
		TaggedExp1 = 760,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaggedExp2"]/*' />
		TaggedExp2 = 761,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaggedExp3"]/*' />
		TaggedExp3 = 762,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaggedExp4"]/*' />
		TaggedExp4 = 763,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaggedExp5"]/*' />
		TaggedExp5 = 764,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaggedExp6"]/*' />
		TaggedExp6 = 765,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaggedExp7"]/*' />
		TaggedExp7 = 766,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaggedExp8"]/*' />
		TaggedExp8 = 767,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.TaggedExp9"]/*' />
		TaggedExp9 = 768,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.EditorWidgetClick"]/*' />
		EditorWidgetClick = 769,  // param 0 is the moniker as VT_BSTR, param 1 is the buffer line as VT_I4, and param 2 is the buffer index as VT_I4
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CmdWinUpdateAC"]/*' />
		CmdWinUpdateAC = 770,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SlnCfgMgr"]/*' />
		SlnCfgMgr = 771,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AddNewProject"]/*' />
		AddNewProject = 772,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AddExistingProject"]/*' />
		AddExistingProject = 773,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AddExistingProjFromWeb"]/*' />
		AddExistingProjFromWeb = 774,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext1"]/*' />
		AutoHideContext1 = 776,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext2"]/*' />
		AutoHideContext2 = 777,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext3"]/*' />
		AutoHideContext3 = 778,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext4"]/*' />
		AutoHideContext4 = 779,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext5"]/*' />
		AutoHideContext5 = 780,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext6"]/*' />
		AutoHideContext6 = 781,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext7"]/*' />
		AutoHideContext7 = 782,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext8"]/*' />
		AutoHideContext8 = 783,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext9"]/*' />
		AutoHideContext9 = 784,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext10"]/*' />
		AutoHideContext10 = 785,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext11"]/*' />
		AutoHideContext11 = 786,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext12"]/*' />
		AutoHideContext12 = 787,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext13"]/*' />
		AutoHideContext13 = 788,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext14"]/*' />
		AutoHideContext14 = 789,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext15"]/*' />
		AutoHideContext15 = 790,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext16"]/*' />
		AutoHideContext16 = 791,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext17"]/*' />
		AutoHideContext17 = 792,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext18"]/*' />
		AutoHideContext18 = 793,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext19"]/*' />
		AutoHideContext19 = 794,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext20"]/*' />
		AutoHideContext20 = 795,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext21"]/*' />
		AutoHideContext21 = 796,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext22"]/*' />
		AutoHideContext22 = 797,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext23"]/*' />
		AutoHideContext23 = 798,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext24"]/*' />
		AutoHideContext24 = 799,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext25"]/*' />
		AutoHideContext25 = 800,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext26"]/*' />
		AutoHideContext26 = 801,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext27"]/*' />
		AutoHideContext27 = 802,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext28"]/*' />
		AutoHideContext28 = 803,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext29"]/*' />
		AutoHideContext29 = 804,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext30"]/*' />
		AutoHideContext30 = 805,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext31"]/*' />
		AutoHideContext31 = 806,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext32"]/*' />
		AutoHideContext32 = 807,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AutoHideContext33"]/*' />
		AutoHideContext33 = 808,   // must remain unused

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavBackward"]/*' />
		ShellNavBackward = 809,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavForward"]/*' />
		ShellNavForward = 810,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate1"]/*' />
		ShellNavigate1 = 811,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate2"]/*' />
		ShellNavigate2 = 812,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate3"]/*' />
		ShellNavigate3 = 813,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate4"]/*' />
		ShellNavigate4 = 814,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate5"]/*' />
		ShellNavigate5 = 815,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate6"]/*' />
		ShellNavigate6 = 816,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate7"]/*' />
		ShellNavigate7 = 817,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate8"]/*' />
		ShellNavigate8 = 818,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate9"]/*' />
		ShellNavigate9 = 819,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate10"]/*' />
		ShellNavigate10 = 820,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate11"]/*' />
		ShellNavigate11 = 821,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate12"]/*' />
		ShellNavigate12 = 822,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate13"]/*' />
		ShellNavigate13 = 823,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate14"]/*' />
		ShellNavigate14 = 824,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate15"]/*' />
		ShellNavigate15 = 825,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate16"]/*' />
		ShellNavigate16 = 826,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate17"]/*' />
		ShellNavigate17 = 827,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate18"]/*' />
		ShellNavigate18 = 828,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate19"]/*' />
		ShellNavigate19 = 829,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate20"]/*' />
		ShellNavigate20 = 830,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate21"]/*' />
		ShellNavigate21 = 831,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate22"]/*' />
		ShellNavigate22 = 832,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate23"]/*' />
		ShellNavigate23 = 833,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate24"]/*' />
		ShellNavigate24 = 834,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate25"]/*' />
		ShellNavigate25 = 835,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate26"]/*' />
		ShellNavigate26 = 836,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate27"]/*' />
		ShellNavigate27 = 837,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate28"]/*' />
		ShellNavigate28 = 838,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate29"]/*' />
		ShellNavigate29 = 839,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate30"]/*' />
		ShellNavigate30 = 840,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate31"]/*' />
		ShellNavigate31 = 841,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate32"]/*' />
		ShellNavigate32 = 842,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellNavigate33"]/*' />
		ShellNavigate33 = 843,   // must remain unused

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate1"]/*' />
		ShellWindowNavigate1 = 844,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate2"]/*' />
		ShellWindowNavigate2 = 845,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate3"]/*' />
		ShellWindowNavigate3 = 846,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate4"]/*' />
		ShellWindowNavigate4 = 847,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate5"]/*' />
		ShellWindowNavigate5 = 848,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate6"]/*' />
		ShellWindowNavigate6 = 849,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate7"]/*' />
		ShellWindowNavigate7 = 850,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate8"]/*' />
		ShellWindowNavigate8 = 851,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate9"]/*' />
		ShellWindowNavigate9 = 852,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate10"]/*' />
		ShellWindowNavigate10 = 853,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate11"]/*' />
		ShellWindowNavigate11 = 854,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate12"]/*' />
		ShellWindowNavigate12 = 855,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate13"]/*' />
		ShellWindowNavigate13 = 856,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate14"]/*' />
		ShellWindowNavigate14 = 857,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate15"]/*' />
		ShellWindowNavigate15 = 858,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate16"]/*' />
		ShellWindowNavigate16 = 859,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate17"]/*' />
		ShellWindowNavigate17 = 860,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate18"]/*' />
		ShellWindowNavigate18 = 861,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate19"]/*' />
		ShellWindowNavigate19 = 862,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate20"]/*' />
		ShellWindowNavigate20 = 863,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate21"]/*' />
		ShellWindowNavigate21 = 864,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate22"]/*' />
		ShellWindowNavigate22 = 865,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate23"]/*' />
		ShellWindowNavigate23 = 866,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate24"]/*' />
		ShellWindowNavigate24 = 867,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate25"]/*' />
		ShellWindowNavigate25 = 868,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate26"]/*' />
		ShellWindowNavigate26 = 869,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate27"]/*' />
		ShellWindowNavigate27 = 870,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate28"]/*' />
		ShellWindowNavigate28 = 871,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate29"]/*' />
		ShellWindowNavigate29 = 872,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate30"]/*' />
		ShellWindowNavigate30 = 873,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate31"]/*' />
		ShellWindowNavigate31 = 874,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate32"]/*' />
		ShellWindowNavigate32 = 875,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShellWindowNavigate33"]/*' />
		ShellWindowNavigate33 = 876,   // must remain unused

		// ObjectSearch cmds
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBSDoFind"]/*' />
		OBSDoFind = 877,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBSMatchCase"]/*' />
		OBSMatchCase = 878,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBSMatchSubString"]/*' />
		OBSMatchSubString = 879,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBSMatchWholeWord"]/*' />
		OBSMatchWholeWord = 880,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBSMatchPrefix"]/*' />
		OBSMatchPrefix = 881,

		// build cmds
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BuildSln"]/*' />
		BuildSln = 882,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RebuildSln"]/*' />
		RebuildSln = 883,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DeploySln"]/*' />
		DeploySln = 884,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CleanSln"]/*' />
		CleanSln = 885,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BuildSel"]/*' />
		BuildSel = 886,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RebuildSel"]/*' />
		RebuildSel = 887,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DeploySel"]/*' />
		DeploySel = 888,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CleanSel"]/*' />
		CleanSel = 889,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CancelBuild"]/*' />
		CancelBuild = 890,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BatchBuildDlg"]/*' />
		BatchBuildDlg = 891,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BuildCtx"]/*' />
		BuildCtx = 892,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RebuildCtx"]/*' />
		RebuildCtx = 893,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DeployCtx"]/*' />
		DeployCtx = 894,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CleanCtx"]/*' />
		CleanCtx = 895,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.QryManageIndexes"]/*' />
		QryManageIndexes = 896,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PrintDefault"]/*' />
		PrintDefault = 897,         // quick print
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BrowseDoc"]/*' />
		BrowseDoc = 898,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowStartPage"]/*' />
		ShowStartPage = 899,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile1"]/*' />
		MRUFile1 = 900,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile2"]/*' />
		MRUFile2 = 901,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile3"]/*' />
		MRUFile3 = 902,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile4"]/*' />
		MRUFile4 = 903,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile5"]/*' />
		MRUFile5 = 904,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile6"]/*' />
		MRUFile6 = 905,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile7"]/*' />
		MRUFile7 = 906,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile8"]/*' />
		MRUFile8 = 907,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile9"]/*' />
		MRUFile9 = 908,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile10"]/*' />
		MRUFile10 = 909,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile11"]/*' />
		MRUFile11 = 910,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile12"]/*' />
		MRUFile12 = 911,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile13"]/*' />
		MRUFile13 = 912,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile14"]/*' />
		MRUFile14 = 913,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile15"]/*' />
		MRUFile15 = 914,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile16"]/*' />
		MRUFile16 = 915,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile17"]/*' />
		MRUFile17 = 916,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile18"]/*' />
		MRUFile18 = 917,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile19"]/*' />
		MRUFile19 = 918,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile20"]/*' />
		MRUFile20 = 919,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile21"]/*' />
		MRUFile21 = 920,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile22"]/*' />
		MRUFile22 = 921,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile23"]/*' />
		MRUFile23 = 922,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile24"]/*' />
		MRUFile24 = 923,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MRUFile25"]/*' />
		MRUFile25 = 924,   // note cmdidMRUFile25 is unused on purpose!

		//External Tools Context Menu Commands
		// continued at 1109
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExtToolsCurPath"]/*' />
		ExtToolsCurPath = 925,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExtToolsCurDir"]/*' />
		ExtToolsCurDir = 926,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExtToolsCurFileName"]/*' />
		ExtToolsCurFileName = 927,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExtToolsCurExtension"]/*' />
		ExtToolsCurExtension = 928,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExtToolsProjDir"]/*' />
		ExtToolsProjDir = 929,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExtToolsProjFileName"]/*' />
		ExtToolsProjFileName = 930,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExtToolsSlnDir"]/*' />
		ExtToolsSlnDir = 931,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExtToolsSlnFileName"]/*' />
		ExtToolsSlnFileName = 932,


		// Object Browsing & ClassView cmds
		// Shared shell cmds (for accessing Object Browsing functionality)
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.GotoDefn"]/*' />
		GotoDefn = 935,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.GotoDecl"]/*' />
		GotoDecl = 936,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BrowseDefn"]/*' />
		BrowseDefn = 937,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SyncClassView"]/*' />
		SyncClassView = 938,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowMembers"]/*' />
		ShowMembers = 939,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowBases"]/*' />
		ShowBases = 940,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowDerived"]/*' />
		ShowDerived = 941,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowDefns"]/*' />
		ShowDefns = 942,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowRefs"]/*' />
		ShowRefs = 943,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowCallers"]/*' />
		ShowCallers = 944,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowCallees"]/*' />
		ShowCallees = 945,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AddClass"]/*' />
		AddClass = 946,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AddNestedClass"]/*' />
		AddNestedClass = 947,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AddInterface"]/*' />
		AddInterface = 948,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AddMethod"]/*' />
		AddMethod = 949,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AddProperty"]/*' />
		AddProperty = 950,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AddEvent"]/*' />
		AddEvent = 951,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AddVariable"]/*' />
		AddVariable = 952,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ImplementInterface"]/*' />
		ImplementInterface = 953,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Override"]/*' />
		Override = 954,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AddFunction"]/*' />
		AddFunction = 955,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AddConnectionPoint"]/*' />
		AddConnectionPoint = 956,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.AddIndexer"]/*' />
		AddIndexer = 957,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BuildOrder"]/*' />
		BuildOrder = 958,
		//959 used above for cmdidSaveOptions

		// Object Browser Tool Specific cmds
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBShowHidden"]/*' />
		OBShowHidden = 960,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBEnableGrouping"]/*' />
		OBEnableGrouping = 961,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBSetGroupingCriteria"]/*' />
		OBSetGroupingCriteria = 962,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBBack"]/*' />
		OBBack = 963,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBForward"]/*' />
		OBForward = 964,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBShowPackages"]/*' />
		OBShowPackages = 965,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBSearchCombo"]/*' />
		OBSearchCombo = 966,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBSearchOptWholeWord"]/*' />
		OBSearchOptWholeWord = 967,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBSearchOptSubstring"]/*' />
		OBSearchOptSubstring = 968,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBSearchOptPrefix"]/*' />
		OBSearchOptPrefix = 969,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBSearchOptCaseSensitive"]/*' />
		OBSearchOptCaseSensitive = 970,

		// ClassView Tool Specific cmds
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CVGroupingNone"]/*' />
		CVGroupingNone = 971,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CVGroupingSortOnly"]/*' />
		CVGroupingSortOnly = 972,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CVGroupingGrouped"]/*' />
		CVGroupingGrouped = 973,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CVShowPackages"]/*' />
		CVShowPackages = 974,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CVNewFolder"]/*' />
		CVNewFolder = 975,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CVGroupingSortAccess"]/*' />
		CVGroupingSortAccess = 976,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ObjectSearch"]/*' />
		ObjectSearch = 977,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ObjectSearchResults"]/*' />
		ObjectSearchResults = 978,

		// Further Obj Browsing cmds at 1095

		// build cascade menus
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Build1"]/*' />
		Build1 = 979,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Build2"]/*' />
		Build2 = 980,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Build3"]/*' />
		Build3 = 981,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Build4"]/*' />
		Build4 = 982,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Build5"]/*' />
		Build5 = 983,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Build6"]/*' />
		Build6 = 984,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Build7"]/*' />
		Build7 = 985,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Build8"]/*' />
		Build8 = 986,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Build9"]/*' />
		Build9 = 987,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BuildLast"]/*' />
		BuildLast = 988,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Rebuild1"]/*' />
		Rebuild1 = 989,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Rebuild2"]/*' />
		Rebuild2 = 990,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Rebuild3"]/*' />
		Rebuild3 = 991,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Rebuild4"]/*' />
		Rebuild4 = 992,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Rebuild5"]/*' />
		Rebuild5 = 993,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Rebuild6"]/*' />
		Rebuild6 = 994,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Rebuild7"]/*' />
		Rebuild7 = 995,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Rebuild8"]/*' />
		Rebuild8 = 996,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Rebuild9"]/*' />
		Rebuild9 = 997,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RebuildLast"]/*' />
		RebuildLast = 998,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Clean1"]/*' />
		Clean1 = 999,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Clean2"]/*' />
		Clean2 = 1000,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Clean3"]/*' />
		Clean3 = 1001,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Clean4"]/*' />
		Clean4 = 1002,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Clean5"]/*' />
		Clean5 = 1003,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Clean6"]/*' />
		Clean6 = 1004,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Clean7"]/*' />
		Clean7 = 1005,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Clean8"]/*' />
		Clean8 = 1006,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Clean9"]/*' />
		Clean9 = 1007,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CleanLast"]/*' />
		CleanLast = 1008,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Deploy1"]/*' />
		Deploy1 = 1009,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Deploy2"]/*' />
		Deploy2 = 1010,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Deploy3"]/*' />
		Deploy3 = 1011,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Deploy4"]/*' />
		Deploy4 = 1012,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Deploy5"]/*' />
		Deploy5 = 1013,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Deploy6"]/*' />
		Deploy6 = 1014,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Deploy7"]/*' />
		Deploy7 = 1015,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Deploy8"]/*' />
		Deploy8 = 1016,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Deploy9"]/*' />
		Deploy9 = 1017,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DeployLast"]/*' />
		DeployLast = 1018,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BuildProjPicker"]/*' />
		BuildProjPicker = 1019,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.RebuildProjPicker"]/*' />
		RebuildProjPicker = 1020,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CleanProjPicker"]/*' />
		CleanProjPicker = 1021,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DeployProjPicker"]/*' />
		DeployProjPicker = 1022,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ResourceView"]/*' />
		ResourceView = 1023,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ShowHomePage"]/*' />
		ShowHomePage = 1024,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.EditMenuIDs"]/*' />
		EditMenuIDs = 1025,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.LineBreak"]/*' />
		LineBreak = 1026,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CPPIdentifier"]/*' />
		CPPIdentifier = 1027,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.QuotedString"]/*' />
		QuotedString = 1028,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SpaceOrTab"]/*' />
		SpaceOrTab = 1029,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Integer"]/*' />
		Integer = 1030,
		//unused 1031-1035

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CustomizeToolbars"]/*' />
		CustomizeToolbars = 1036,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MoveToTop"]/*' />
		MoveToTop = 1037,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.WindowHelp"]/*' />
		WindowHelp = 1038,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewPopup"]/*' />
		ViewPopup = 1039,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CheckMnemonics"]/*' />
		CheckMnemonics = 1040,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PRSortAlphabeticaly"]/*' />
		PRSortAlphabeticaly = 1041,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PRSortByCategory"]/*' />
		PRSortByCategory = 1042,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ViewNextTab"]/*' />
		ViewNextTab = 1043,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CheckForUpdates"]/*' />
		CheckForUpdates = 1044,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Browser1"]/*' />
		Browser1 = 1045,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Browser2"]/*' />
		Browser2 = 1046,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Browser3"]/*' />
		Browser3 = 1047,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Browser4"]/*' />
		Browser4 = 1048,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Browser5"]/*' />
		Browser5 = 1049,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Browser6"]/*' />
		Browser6 = 1050,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Browser7"]/*' />
		Browser7 = 1051,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Browser8"]/*' />
		Browser8 = 1052,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Browser9"]/*' />
		Browser9 = 1053,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Browser10"]/*' />
		Browser10 = 1054,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Browser11"]/*' />
		Browser11 = 1055,  //note unused on purpose to end list

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OpenDropDownOpen"]/*' />
		OpenDropDownOpen = 1058,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OpenDropDownOpenWith"]/*' />
		OpenDropDownOpenWith = 1059,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ToolsDebugProcesses"]/*' />
		ToolsDebugProcesses = 1060,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PaneNextSubPane"]/*' />
		PaneNextSubPane = 1062,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PanePrevSubPane"]/*' />
		PanePrevSubPane = 1063,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MoveFileToProject1"]/*' />
		MoveFileToProject1 = 1070,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MoveFileToProject2"]/*' />
		MoveFileToProject2 = 1071,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MoveFileToProject3"]/*' />
		MoveFileToProject3 = 1072,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MoveFileToProject4"]/*' />
		MoveFileToProject4 = 1073,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MoveFileToProject5"]/*' />
		MoveFileToProject5 = 1074,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MoveFileToProject6"]/*' />
		MoveFileToProject6 = 1075,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MoveFileToProject7"]/*' />
		MoveFileToProject7 = 1076,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MoveFileToProject8"]/*' />
		MoveFileToProject8 = 1077,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MoveFileToProject9"]/*' />
		MoveFileToProject9 = 1078,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MoveFileToProjectLast"]/*' />
		MoveFileToProjectLast = 1079,  // unused in order to end list
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.MoveFileToProjectPick"]/*' />
		MoveFileToProjectPick = 1081,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.DefineSubset"]/*' />
		DefineSubset = 1095,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SubsetCombo"]/*' />
		SubsetCombo = 1096,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SubsetGetList"]/*' />
		SubsetGetList = 1097,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBSortObjectsAlpha"]/*' />
		OBSortObjectsAlpha = 1098,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBSortObjectsType"]/*' />
		OBSortObjectsType = 1099,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBSortObjectsAccess"]/*' />
		OBSortObjectsAccess = 1100,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBGroupObjectsType"]/*' />
		OBGroupObjectsType = 1101,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBGroupObjectsAccess"]/*' />
		OBGroupObjectsAccess = 1102,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBSortMembersAlpha"]/*' />
		OBSortMembersAlpha = 1103,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBSortMembersType"]/*' />
		OBSortMembersType = 1104,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBSortMembersAccess"]/*' />
		OBSortMembersAccess = 1105,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PopBrowseContext"]/*' />
		PopBrowseContext = 1106,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.GotoRef"]/*' />
		GotoRef = 1107,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.OBSLookInReferences"]/*' />
		OBSLookInReferences = 1108,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExtToolsTargetPath"]/*' />
		ExtToolsTargetPath = 1109,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExtToolsTargetDir"]/*' />
		ExtToolsTargetDir = 1110,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExtToolsTargetFileName"]/*' />
		ExtToolsTargetFileName = 1111,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExtToolsTargetExtension"]/*' />
		ExtToolsTargetExtension = 1112,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExtToolsCurLine"]/*' />
		ExtToolsCurLine = 1113,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExtToolsCurCol"]/*' />
		ExtToolsCurCol = 1114,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExtToolsCurText"]/*' />
		ExtToolsCurText = 1115,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BrowseNext"]/*' />
		BrowseNext = 1116,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BrowsePrev"]/*' />
		BrowsePrev = 1117,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BrowseUnload"]/*' />
		BrowseUnload = 1118,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.QuickObjectSearch"]/*' />
		QuickObjectSearch = 1119,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.ExpandAll"]/*' />
		ExpandAll = 1120,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.BookmarkWindow"]/*' />
		BookmarkWindow = 1122,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.CodeExpansionWindow"]/*' />
		CodeExpansionWindow = 1123,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.NextDocumentNav"]/*' />
		NextDocumentNav = 1124,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.PrevDocumentNav"]/*' />
		PrevDocumentNav = 1125,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.StandardMax"]/*' />
		StandardMax = 1500,

		///////////////////////////////////////////
		//
		// cmdidStandardMax is now thought to be
		// obsolete. Any new shell commands should
		// be added to the end of StandardCommandSet2K
		// which appears below.
		//
		// If you are not adding shell commands,
		// you shouldn't be doing it in this file! 
		//
		///////////////////////////////////////////


		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FormsFirst"]/*' />
		FormsFirst = 0x00006000,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.FormsLast"]/*' />
		FormsLast = 0x00006FFF,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.VBEFirst"]/*' />
		VBEFirst = 0x00008000,


		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Zoom200"]/*' />
		Zoom200 = 0x00008002,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Zoom150"]/*' />
		Zoom150 = 0x00008003,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Zoom100"]/*' />
		Zoom100 = 0x00008004,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Zoom75"]/*' />
		Zoom75 = 0x00008005,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Zoom50"]/*' />
		Zoom50 = 0x00008006,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Zoom25"]/*' />
		Zoom25 = 0x00008007,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.Zoom10"]/*' />
		Zoom10 = 0x00008010,


		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.VBELast"]/*' />
		VBELast = 0x00009FFF,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SterlingFirst"]/*' />
		SterlingFirst = 0x0000A000,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.SterlingLast"]/*' />
		SterlingLast = 0x0000BFFF,

		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.uieventidFirst"]/*' />
		uieventidFirst = 0xC000,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.uieventidSelectRegion"]/*' />
		uieventidSelectRegion = 0xC001,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.uieventidDrop"]/*' />
		uieventidDrop = 0xC002,
		/// <include file='doc\VSConstants.uex' path='docs/doc[@for="VSStd97CmdID.uieventidLast"]/*' />
		uieventidLast = 0xDFFF,
	}

}
