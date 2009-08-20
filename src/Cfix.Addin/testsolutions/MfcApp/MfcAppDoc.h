// MfcAppDoc.h : interface of the CMfcAppDoc class
//


#pragma once


class CMfcAppDoc : public CDocument
{
protected: // create from serialization only
	CMfcAppDoc();
	DECLARE_DYNCREATE(CMfcAppDoc)

// Attributes
public:

// Operations
public:

// Overrides
public:
	virtual BOOL OnNewDocument();
	virtual void Serialize(CArchive& ar);

// Implementation
public:
	virtual ~CMfcAppDoc();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// Generated message map functions
protected:
	DECLARE_MESSAGE_MAP()
};


