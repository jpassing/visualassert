;//----------------------------------------------------------------------
;// Purpose:
;//		Auxiliary header file. 
;//
;// Copyright:
;//		2009, Johannes Passing. All rights reserved.
;//
;//--------------------------------------------------------------------
;// Definitions
;//--------------------------------------------------------------------

MessageIdTypedef=HRESULT

SeverityNames=(
  Success=0x0
  Informational=0x1
  Warning=0x2
  Error=0x3
)

FacilityNames=(
  Interface=4
)

LanguageNames=(English=0x409:MSG00409)


;//--------------------------------------------------------------------
MessageId		= 0xB000
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_UNRECOGNIZED_MODULE_TYPE
Language		= English
Unrecognized module type.
.

MessageId		= 0xB001
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_QUERY_DEF_DISP_FAILED
Language		= English
Failed to obtain default disposition decision.
.

MessageId		= 0xB002
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_REPORT_EVENT_FAILED
Language		= English
An event could not be reported.
.

MessageId		= 0xB003
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_NO_FIXTURE_SINK
Language		= English
Obtaining fixture sink failed.
.

MessageId		= 0xB004
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_NO_TESTCASE_SINK
Language		= English
Obtaining testcase sink failed.
.

MessageId		= 0xB005
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_HOST_NOT_FOUND
Language		= English
No host has been registered with this cookie.
.

MessageId		= 0xB006
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_USER_ABORT
Language		= English
Run aborted.
.

MessageId		= 0xB007
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_HOST_IMAGE_NOT_FOUND
Language		= English
The host image could not be located.
.

MessageId		= 0xB008
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_HOST_DIED_PREMATURELY
Language		= English
The host image terminated prematurely and its exit code could not be determined.
.

MessageId		= 0xB009
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_TESTMODULE_NOT_FOUND
Language		= English
Test module not found.
.
