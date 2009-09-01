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

MessageId		= 0xB00a
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_REG_CORRUPT
Language		= English
Corrupt registry settings detected - please reinstall.
.

MessageId		= 0xB00b
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_LIC_TAMPERED
Language		= English
The licensing data has been tampered with.
.

MessageId		= 0xB00c
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_LIC_INVALID
Language		= English
The license key is invalid or not applicable for this product version.
.

MessageId		= 0xB00d
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_NO_LIC_INSTALLED
Language		= English
No license key installed.
.

MessageId		= 0xB00e
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_LIC_TRIAL_EXPIRED
Language		= English
The evaluation period has expired.
.

MessageId		= 0xB00f
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_MISSING_AGENT_MK
Language		= English
The agent moniker was not found in the process environment.
.

MessageId		= 0xB010
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_CUSTOM_HOST_EXITED_PREMATURELY
Language		= English
The process exited without registering with the agent - this may be due to the module not containing any test fixtures.
.

MessageId		= 0xB011
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_HOST_EXITED_PREMATURELY
Language		= English
The custom host process exited prematurely without setting a proper exit code.
.

MessageId		= 0xB012
Severity		= Warning
Facility		= Interface
SymbolicName	= CFIXCTL_E_HOST_IMAGE_HAS_NO_FIXTURES
Language		= English
The custom host process contains no fixture and therefore was not started.
.
