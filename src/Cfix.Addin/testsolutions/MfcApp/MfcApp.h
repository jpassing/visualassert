// MfcApp.h : main header file for the MfcApp application
//
#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"       // main symbols


// CMfcAppApp:
// See MfcApp.cpp for the implementation of this class
//

class CMfcAppApp : public CWinApp
{
public:
	CMfcAppApp();


// Overrides
public:
	virtual BOOL InitInstance();

// Implementation
	afx_msg void OnAppAbout();
	DECLARE_MESSAGE_MAP()
};

extern CMfcAppApp theApp;