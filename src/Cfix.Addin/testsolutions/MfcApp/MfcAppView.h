// MfcAppView.h : interface of the CMfcAppView class
//


#pragma once


class CMfcAppView : public CView
{
protected: // create from serialization only
	CMfcAppView();
	DECLARE_DYNCREATE(CMfcAppView)

// Attributes
public:
	CMfcAppDoc* GetDocument() const;

// Operations
public:

// Overrides
public:
	virtual void OnDraw(CDC* pDC);  // overridden to draw this view
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
protected:
	virtual BOOL OnPreparePrinting(CPrintInfo* pInfo);
	virtual void OnBeginPrinting(CDC* pDC, CPrintInfo* pInfo);
	virtual void OnEndPrinting(CDC* pDC, CPrintInfo* pInfo);

// Implementation
public:
	virtual ~CMfcAppView();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// Generated message map functions
protected:
	DECLARE_MESSAGE_MAP()
};

#ifndef _DEBUG  // debug version in MfcAppView.cpp
inline CMfcAppDoc* CMfcAppView::GetDocument() const
   { return reinterpret_cast<CMfcAppDoc*>(m_pDocument); }
#endif

