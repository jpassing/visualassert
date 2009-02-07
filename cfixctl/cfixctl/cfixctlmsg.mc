;// Copyright:
;//		2008, Johannes Passing (passing at users.sourceforge.net)
;//
;// This file is part of cfix.
;//
;// cfix is free software: you can redistribute it and/or modify
;// it under the terms of the GNU Lesser General Public License as published by
;// the Free Software Foundation, either version 3 of the License, or
;// (at your option) any later version.
;//
;// cfix is distributed in the hope that it will be useful,
;// but WITHOUT ANY WARRANTY; without even the implied warranty of
;// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
;// GNU Lesser General Public License for more details.
;// 
;// You should have received a copy of the GNU Lesser General Public License
;// along with cfix.  If not, see <http://www.gnu.org/licenses/>.
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
