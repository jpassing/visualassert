/*----------------------------------------------------------------------
 * Purpose:
 *		TestModule class.
 *
 * Copyright:
 *		2008, Johannes Passing (passing at users.sourceforge.net)
 *
 * This file is part of cfix.
 *
 * cfix is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * cfix is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with cfix.  If not, see <http://www.gnu.org/licenses/>.
 */

#include "cfixctlp.h"

const GUID IID_ICfixTestModuleInternal = 
	{ 0x2e8f211, 0xe29b, 0x40e0, { 0xb2, 0xba, 0x39, 0xba, 0x4c, 0x56, 0xb8, 0x39 } };

/*------------------------------------------------------------------
 * 
 * Class Declaration.
 *
 */

class TestModuleEumerator;

class TestModule : 
	public ICfixTestModuleInternal,
	public ICfixTestContainer,
	public ICfixActionFactory,
	public IOleItemContainer
{
	DECLARE_NOT_COPYABLE( TestModule );

private:
	//
	// N.B. Members are effectively immutable.
	//
	PCFIX_TEST_MODULE Module;
	BSTR Path;
	CfixTestModuleType Type;
	CfixTestModuleArch Architecture;

	//
	// Allow enumerator to access members.
	//
	friend class TestModuleEnumerator;

	STDMETHOD( GetFixture )(
		__in ULONG Index,
		__in REFIID Iid,
		__out PVOID *Object
		);

protected:
	TestModule();

public:
	virtual ~TestModule();

	/*------------------------------------------------------------------
	 * IUnknown methods.
	 */

	STDMETHOD_( ULONG, AddRef )() PURE;
	STDMETHOD_( ULONG, Release )() PURE;
	STDMETHOD( QueryInterface )( 
		__in REFIID Iid, 
		__out PVOID* Ptr );

	/*------------------------------------------------------------------
	 * IParseDisplayName methods.
	 */

	STDMETHOD( ParseDisplayName )( 
		__in IBindCtx *Context,
		__in LPOLESTR DisplayName,
		__out ULONG *Eaten,
		__out IMoniker **Moniker
		);

	/*------------------------------------------------------------------
	 * IOleContainer methods.
	 */

	STDMETHOD( EnumObjects )( 
		__in DWORD Flags,
		__out IEnumUnknown **Enum
		);

	STDMETHOD( LockContainer )( 
		__in BOOL Lock
		);

	/*------------------------------------------------------------------
	 * IOleItemContainer methods.
	 */

	STDMETHOD( GetObject )( 
		__in LPOLESTR Item,
		__in DWORD SpeedNeeded,
		__in IBindCtx *Context,
		__in REFIID Iid,
		__out PVOID *Object
		);

	STDMETHOD( GetObjectStorage )( 
		__in LPOLESTR Item,
		__in IBindCtx *Context,
		__in REFIID Iid,
		__out PVOID *Storage
		);

	STDMETHOD( IsRunning )( 
		__in LPOLESTR Item 
		);

	/*------------------------------------------------------------------
	 * ICfixTestContainer methods.
	 */
	
	STDMETHOD( GetItemCount )(
		__out ULONG *Count
		);

	STDMETHOD( GetItem )(
		__in ULONG Ordinal,
		__out ICfixTestItem **Item
		);

	/*------------------------------------------------------------------
	 * ICfixTestModule methods.
	 */

	STDMETHOD( GetName )(
        __out BSTR *Name
		);

	STDMETHOD( CreateExecutionAction )(
		__in ULONG SchedulingFlags,
		__in ULONG Reserved,
		__out ICfixAction **Action 
		);

	STDMETHOD( GetPath )(
		__out BSTR *Path
		);

	STDMETHOD( GetType )(
		__out CfixTestModuleType *Type,
		__out CfixTestModuleArch *Arch
		);

	/*------------------------------------------------------------------
	 * ICfixActionFactory methods.
	 */

	STDMETHOD( CreateExecutionAction )( 
        __in ULONG FixtureOrdinal,
        __in ULONG TestCaseOrdinal,
        __in ULONG SchedulingFlags,
        __in ULONG Reserved,
        __out ICfixAction **Action
		);

	/*------------------------------------------------------------------
	 * ICfixTestModuleInternal methods.
	 */

	STDMETHOD( Initialize )(
		__in PCWSTR Path,
		__in CfixTestModuleType Type,
		__in CfixTestModuleArch Architecture,
		__in PCFIX_TEST_MODULE Module
		);
};

class TestModuleEnumerator : public ComAbstractEnumerator< IUnknown >
{
	DECLARE_NOT_COPYABLE( TestModuleEnumerator );

private:
	TestModule *Module;

public:
	TestModuleEnumerator( TestModule *Parent ) ;
	virtual ~TestModuleEnumerator();

	STDMETHOD( GetElement )( 
		__in ULONG Index,
		__out IUnknown** Object
		);
	STDMETHOD_( ULONG, GetCount )();
	STDMETHOD( Clone )( 
		__out IEnumUnknown **Enum
		);
};

/*------------------------------------------------------------------
 * 
 * Factory.
 *
 */
IClassFactory& CfixctlpGetTestModuleFactory()
{
	static ComClassFactory< ComMtaObject< TestModule >, CfixctlServerLock > Factory;
	return Factory;
}

/*------------------------------------------------------------------
 * 
 * Helpers.
 *
 */

static HRESULT CfixctlsCreateModuleExecutionAction(
	__in PCFIX_TEST_MODULE Module,
	__in ULONG Flags,
	__out PCFIX_ACTION *Action
	)
{
	ASSERT( Module );
	ASSERT( Action );

	PCFIX_ACTION SeqAction = NULL;
	HRESULT Hr;

	Hr = CfixCreateSequenceAction( &SeqAction );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	for ( ULONG Index = 0; Index < Module->FixtureCount; Index++ )
	{
		PCFIX_ACTION FixtureAction;
		Hr = CfixCreateFixtureExecutionAction(
			Module->Fixtures[ Index ],
			Flags,
			&FixtureAction );
		if ( FAILED( Hr ) )
		{
			break;
		}

		Hr = CfixAddEntrySequenceAction( SeqAction, FixtureAction );

		FixtureAction->Dereference( FixtureAction );

		if ( FAILED( Hr ) )
		{
			break;
		}
	}

	if ( SUCCEEDED( Hr ) )
	{
		*Action = SeqAction;
		return S_OK;
	}
	else
	{
		SeqAction->Dereference( SeqAction );
		return Hr;
	}
}

/*------------------------------------------------------------------
 * 
 * TestModule Class Implementation.
 *
 */
TestModule::TestModule() : Path( NULL ), Module( NULL )
{
}

TestModule::~TestModule()
{
	if ( this->Path != NULL )
	{
		SysFreeString( this->Path );
	}

	if ( this->Module != NULL )
	{
		this->Module->Routines.Dereference( this->Module );
	}
}

STDMETHODIMP TestModule::GetFixture(
	__in ULONG Index,
	__in REFIID Iid,
	__out PVOID *Object 
	)
{
	if ( ! Object )
	{
		return E_POINTER;
	}
	else
	{
		*Object = NULL;
	}

	if ( Index >= this->Module->FixtureCount )
	{
		return E_INVALIDARG;
	}

	IClassFactory& Factory = CfixctlpGetTestFixtureFactory();
	
	ICfixTestFixtureInternal *Fixture;
	HRESULT Hr = Factory.CreateInstance(
		NULL,
		IID_ICfixTestFixtureInternal,
		( PVOID* ) &Fixture );
	if ( FAILED( Hr ) )
	{
		return Hr;
	}

	Hr = Fixture->Initialize( 
		Module->Fixtures[ Index ],
		Index,
		static_cast< ICfixActionFactory* >( this ) );
	if ( FAILED( Hr ) )
	{
		Fixture->Release();
		return Hr;
	}

	Hr = Fixture->QueryInterface( Iid, Object );
	Fixture->Release();
	return Hr;
}

/*------------------------------------------------------------------
 * IUnknown methods.
 */

STDMETHODIMP TestModule::QueryInterface( 
	__in REFIID Iid, 
	__out PVOID* Ptr )
{
	HRESULT Hr;

	if ( InlineIsEqualGUID( Iid, IID_IUnknown ) ||
		 InlineIsEqualGUID( Iid, IID_ICfixTestItem ) ||
		 InlineIsEqualGUID( Iid, IID_ICfixTestModule ) )
	{
		*Ptr = static_cast< ICfixTestModule* >( this );
		Hr = S_OK;

	}
	else if ( InlineIsEqualGUID( Iid, IID_ICfixActionFactory ) )
	{
		*Ptr = static_cast< ICfixActionFactory* >( this );
		Hr = S_OK;
	}
	else if ( InlineIsEqualGUID( Iid, IID_ICfixTestModuleInternal ) )
	{
		*Ptr = static_cast< ICfixTestModuleInternal* >( this );
		Hr = S_OK;
	}
	else if ( InlineIsEqualGUID( Iid, IID_ICfixTestContainer ) )
	{
		*Ptr = static_cast< ICfixTestContainer* >( this );
		Hr = S_OK;
	}
	else if ( InlineIsEqualGUID( Iid, IID_IOleContainer ) )
	{
		*Ptr = static_cast< IOleContainer* >( this );
		Hr = S_OK;
	}
	else if ( InlineIsEqualGUID( Iid, IID_IOleItemContainer ) )
	{
		*Ptr = static_cast< IOleItemContainer* >( this );
		Hr = S_OK;
	}
	else if ( InlineIsEqualGUID( Iid, IID_IParseDisplayName ) )
	{
		*Ptr = static_cast< IParseDisplayName* >( this );
		Hr = S_OK;
	}
	else
	{
		*Ptr = NULL;
		Hr = E_NOINTERFACE;
	}

	if ( SUCCEEDED( Hr ) )
	{
		this->AddRef();
	}

	return Hr;
}

/*------------------------------------------------------------------
 * IParseDisplayName methods.
 */

STDMETHODIMP TestModule::ParseDisplayName( 
	__in IBindCtx *Context,
	__in LPOLESTR DisplayName,
	__out ULONG *Eaten,
	__out IMoniker **Moniker
	)
{
	UNREFERENCED_PARAMETER( Context );
	UNREFERENCED_PARAMETER( DisplayName );
	UNREFERENCED_PARAMETER( Eaten );
	UNREFERENCED_PARAMETER( Moniker );
	return E_NOTIMPL;
}

/*------------------------------------------------------------------
 * IOleContainer methods.
 */

STDMETHODIMP TestModule::EnumObjects( 
	__in DWORD Flags,
	__out IEnumUnknown **Enum
	)
{
	UNREFERENCED_PARAMETER( Flags );

	*Enum = new TestModuleEnumerator( this );

	if ( *Enum == NULL )
	{
		return E_OUTOFMEMORY;
	}
	else
	{
		return S_OK;
	}
}

STDMETHODIMP TestModule::LockContainer( 
	__in BOOL Lock
	)
{
	UNREFERENCED_PARAMETER( Lock );
	ASSERT( !"Not applicable" );
	return E_FAIL;
}

/*------------------------------------------------------------------
 * IOleItemContainer methods.
 */

STDMETHODIMP TestModule::GetObject( 
	__in LPOLESTR Item,
	__in DWORD SpeedNeeded,
	__in IBindCtx *Context,
	__in REFIID Iid,
	__out PVOID *Object
	)
{
	if ( ! Object )
	{
		return E_POINTER;
	}
	else
	{
		*Object = NULL;
	}

	if ( Item == NULL )
	{
		return MK_E_NOOBJECT;
	}

	UNREFERENCED_PARAMETER( SpeedNeeded );
	UNREFERENCED_PARAMETER( Context );

	for ( ULONG Index = 0; Index < this->Module->FixtureCount; Index++ )
	{
		if ( 0 == wcscmp( Item, this->Module->Fixtures[ Index ]->Name ) )
		{
			//
			// Create object for this.
			//
			return GetFixture( Index, Iid, Object );
		}
	}

	return MK_E_NOOBJECT;
}

STDMETHODIMP TestModule::GetObjectStorage( 
	__in LPOLESTR Item,
	__in IBindCtx *Context,
	__in REFIID Iid,
	__out PVOID *Storage
	)
{
	UNREFERENCED_PARAMETER( Item );
	UNREFERENCED_PARAMETER( Context );
	UNREFERENCED_PARAMETER( Iid );
	UNREFERENCED_PARAMETER( Storage );
	ASSERT( !"Not applicable" );
	
	if ( Storage )
	{
		*Storage = NULL;
	}

	return MK_E_NOSTORAGE;
}

STDMETHODIMP TestModule::IsRunning( 
	__in LPOLESTR Item 
	)
{
	if ( Item == NULL )
	{
		return MK_E_NOOBJECT;
	}

	//
	// Check if we know this item.
	//
	BOOL Found = FALSE;
	for ( ULONG Index = 0; Index < this->Module->FixtureCount; Index++ )
	{
		if ( 0 == wcscmp( Item, this->Module->Fixtures[ Index ]->Name ) )
		{
			Found = TRUE;
			break;
		}
	}

	return Found ? S_OK : MK_E_NOOBJECT;
}

/*------------------------------------------------------------------
 * ICfixTestContainer methods.
 */

STDMETHODIMP TestModule::GetItemCount(
	__out ULONG *Count
	)
{
	if ( ! Count )
	{
		return E_POINTER;
	}
	else
	{
		*Count = this->Module->FixtureCount;
		return S_OK;
	}
}

STDMETHODIMP TestModule::GetItem(
	__in ULONG Ordinal,
	__out ICfixTestItem **Item
	)
{
	return GetFixture( Ordinal, IID_ICfixTestItem, ( PVOID* ) Item );
}

/*------------------------------------------------------------------
 * ICfixTestModule methods.
 */

STDMETHODIMP TestModule::GetName(
    __out BSTR *Name
	)
{
	return ComGetProperty( this->Module->Name, Name );
}

STDMETHODIMP TestModule::CreateExecutionAction(
	__in ULONG SchedulingFlags,
	__in ULONG Reserved,
	__out ICfixAction **Action 
	)
{
	return CreateExecutionAction(
		CFIXCTL_EXECUTE_ALL,
		CFIXCTL_EXECUTE_ALL,
		SchedulingFlags,
		Reserved,
		Action );
}

STDMETHODIMP TestModule::GetPath(
    __out BSTR *Path
	)
{
	return ComGetProperty( this->Path, Path );
}

STDMETHODIMP TestModule::GetType(
	__out CfixTestModuleType *Type,
	__out CfixTestModuleArch *Arch
	)
{
	*Type = this->Type;
	*Arch = this->Architecture;
	return S_OK;
}

/*------------------------------------------------------------------
 * ICfixActionFactory methods.
 */

STDMETHODIMP TestModule::CreateExecutionAction( 
    __in ULONG FixtureOrdinal,
    __in ULONG TestCaseOrdinal,
    __in ULONG SchedulingFlags,
    __in ULONG Reserved,
    __out ICfixAction **Action
	)
{
	if ( ! Action )
	{
		return E_POINTER;
	}
	else
	{
		*Action = NULL;
	}

	if ( Reserved != 0 ||
		 ( FixtureOrdinal == CFIXCTL_EXECUTE_ALL && 
		   TestCaseOrdinal != CFIXCTL_EXECUTE_ALL ) ||
		 ( FixtureOrdinal != CFIXCTL_EXECUTE_ALL && 
		   FixtureOrdinal >= this->Module->FixtureCount ) ||
		 ( TestCaseOrdinal != CFIXCTL_EXECUTE_ALL && 
		   TestCaseOrdinal >= this->Module->Fixtures[ FixtureOrdinal ]->TestCaseCount ) )
	{
		return E_INVALIDARG;
	}

	if ( TestCaseOrdinal != CFIXCTL_EXECUTE_ALL )
	{
		//
		// Single-testcase runs are currently not supported.
		//
		return E_NOTIMPL;
	}

	//
	// Create cfix action.
	//
	PCFIX_ACTION ExecAction = NULL;
	ICfixExecutionActionInternal *ActionObject = NULL;
	HRESULT Hr;

	if ( FixtureOrdinal == CFIXCTL_EXECUTE_ALL )
	{
		Hr = CfixctlsCreateModuleExecutionAction(
			this->Module,
			SchedulingFlags,
			&ExecAction );
	}
	else
	{
		Hr = CfixCreateFixtureExecutionAction(
			this->Module->Fixtures[ FixtureOrdinal ],
			SchedulingFlags,
			&ExecAction );
	}

	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	//
	// Wrap action.
	//
	Hr = CfixctlpGetExecutionActionFactory().CreateInstance(
		NULL,
		IID_ICfixExecutionActionInternal, 
		( PVOID* ) &ActionObject );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	Hr = ActionObject->Initialize(
		static_cast< ICfixTestModule* >( this ),
		ExecAction );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	*Action = ActionObject;
	Hr = S_OK;

Cleanup:
	if ( ExecAction )
	{
		ExecAction->Dereference( ExecAction );
	}

	if ( FAILED( Hr ) )
	{
		if ( ActionObject )
		{
			ActionObject->Release();
		}
	}

	return Hr;
}

/*------------------------------------------------------------------
 * ICfixTestModuleInternal methods.
 */

STDMETHODIMP TestModule::Initialize(
	__in PCWSTR Path,
	__in CfixTestModuleType Type,
	__in CfixTestModuleArch Architecture,
	__in PCFIX_TEST_MODULE Module
	)
{
	if ( this->Path != NULL ||
		 this->Module != NULL )
	{
		return E_UNEXPECTED;
	}

	this->Path = SysAllocString( Path );
	if ( this->Path == NULL )
	{
		return E_OUTOFMEMORY;
	}

	Module->Routines.Reference( Module );
	this->Module = Module;

	this->Architecture = Architecture;
	this->Type = Type;

	return S_OK;
}

/*------------------------------------------------------------------
 * 
 * TestModuleEnumerator Class Implementation.
 *
 */

TestModuleEnumerator::TestModuleEnumerator( 
	__in TestModule *Parent 
	) 
	: ComAbstractEnumerator( static_cast< ICfixTestModule* >( Parent ) )
{
	//
	// N.B. The base class addref's the parent for us - no need to
	// addref it again.
	//
	this->Module = Parent;
}

TestModuleEnumerator::~TestModuleEnumerator()
{
}

STDMETHODIMP TestModuleEnumerator::GetElement( 
	__in ULONG Index,
	__out IUnknown** Element
	)
{
	ASSERT( Element );
		
	return this->Module->GetFixture( 
		Index, 
		IID_IUnknown, 
		( PVOID* ) Element );	
}

ULONG TestModuleEnumerator::GetCount()
{
	return this->Module->Module->FixtureCount;
}

STDMETHODIMP TestModuleEnumerator::Clone( 
	__out IEnumUnknown **Enum
	)
{
	*Enum = new TestModuleEnumerator( this->Module );
	if ( *Enum == NULL )
	{
		return E_OUTOFMEMORY;
	}
	else
	{
		return S_OK;
	}
}