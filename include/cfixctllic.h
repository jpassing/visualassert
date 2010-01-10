#pragma once

/*----------------------------------------------------------------------
 * Purpose:
 *		Licensing information. 
 *
 * Copyright:
 *		2009, Johannes Passing. All rights reserved.
 */

#define CFIXCTL_LICENSE_REG_KEYPATH L"Software\\VisualAssert\\1.0"
#define CFIXCTL_LICENSE_REG_KEY_NAME_LICENSE L"License"
#define CFIXCTL_LICENSE_REG_KEY_NAME_LICENSE_DATE L"State"

#define CFIXCTL_LIC_TRIAL_PERIOD				30UL

//
// 2010-02-28.
//
//#define CFIXCTL_LIC_HARD_EXPIRY_DATE			149442

//
// N.B. All versions numbers may not exceed 0xF.
//

#define CFIXCTL_LIC_MIN_ALLOWED_VERSION_MAJOR	1
#define CFIXCTL_LIC_MIN_ALLOWED_VERSION_MINOR	0
#define CFIXCTL_LIC_MAX_ALLOWED_VERSION_MAJOR	1
#define CFIXCTL_LIC_MAX_ALLOWED_VERSION_MINOR	0