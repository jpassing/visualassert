// MfcAppView.cpp : implementation of the CMfcAppView class
//

#include "stdafx.h"
#include "MfcApp.h"

#include "MfcAppDoc.h"
#include "MfcAppView.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CMfcAppView

IMPLEMENT_DYNCREATE(CMfcAppView, CView)

BEGIN_MESSAGE_MAP(CMfcAppView, CView)
	// Standard printing commands
	ON_COMMAND(ID_FILE_PRINT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_DIRECT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_PREVIEW, &CView::OnFilePrintPreview)
END_MESSAGE_MAP()

// CMfcAppView construction/destruction

CMfcAppView::CMfcAppView()
{
	// TODO: add construction code here

}

CMfcAppView::~CMfcAppView()
{
}

BOOL CMfcAppView::PreCreateWindow(CREATESTRUCT& cs)
{
	// TODO: Modify the Window class or styles here by modifying
	//  the CREATESTRUCT cs

	return CView::PreCreateWindow(cs);
}

// CMfcAppView drawing

void CMfcAppView::OnDraw(CDC* /*pDC*/)
{
	CMfcAppDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;

	// TODO: add draw code for native data here
}


// CMfcAppView printing

BOOL CMfcAppView::OnPreparePrinting(CPrintInfo* pInfo)
{
	// default preparation
	return DoPreparePrinting(pInfo);
}

void CMfcAppView::OnBeginPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: add extra initialization before printing
}

void CMfcAppView::OnEndPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: add cleanup after printing
}


// CMfcAppView diagnostics

#ifdef _DEBUG
void CMfcAppView::AssertValid() const
{
	CView::AssertValid();
}

void CMfcAppView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}

CMfcAppDoc* CMfcAppView::GetDocument() const // non-debug version is inline
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(CMfcAppDoc)));
	return (CMfcAppDoc*)m_pDocument;
}
#endif //_DEBUG


// CMfcAppView message handlers
