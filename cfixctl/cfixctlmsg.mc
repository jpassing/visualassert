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
