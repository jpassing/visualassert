// MfcAppDoc.cpp : implementation of the CMfcAppDoc class
//

#include "stdafx.h"
#include "MfcApp.h"

#include "MfcAppDoc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CMfcAppDoc

IMPLEMENT_DYNCREATE(CMfcAppDoc, CDocument)

BEGIN_MESSAGE_MAP(CMfcAppDoc, CDocument)
END_MESSAGE_MAP()


// CMfcAppDoc construction/destruction

CMfcAppDoc::CMfcAppDoc()
{
	// TODO: add one-time construction code here

}

CMfcAppDoc::~CMfcAppDoc()
{
}

BOOL CMfcAppDoc::OnNewDocument()
{
	if (!CDocument::OnNewDocument())
		return FALSE;

	// TODO: add reinitialization code here
	// (SDI documents will reuse this document)

	return TRUE;
}




// CMfcAppDoc serialization

void CMfcAppDoc::Serialize(CArchive& ar)
{
	if (ar.IsStoring())
	{
		// TODO: add storing code here
	}
	else
	{
		// TODO: add loading code here
	}
}


// CMfcAppDoc diagnostics

#ifdef _DEBUG
void CMfcAppDoc::AssertValid() const
{
	CDocument::AssertValid();
}

void CMfcAppDoc::Dump(CDumpContext& dc) const
{
	CDocument::Dump(dc);
}
#endif //_DEBUG


// CMfcAppDoc commands
