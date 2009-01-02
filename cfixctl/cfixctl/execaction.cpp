/*----------------------------------------------------------------------
 * Purpose:
 *		Execution action. Used to execute one or more fixtures
 *		(depending on the underlying PCFIX_ACTION) of a single module.
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

const GUID IID_ICfixExecutionActionInternal = 
	{ 0x5bcd9d4e, 0xa622, 0x4d60, { 0x86, 0xed, 0x97, 0xd, 0x11, 0xc1, 0x73, 0x8d } };

/*----------------------------------------------------------------------
 * 
 * Class Declaration.
 *
 */

class ExecutionAction : 
	public ICfixExecutionActionInternal
{
	DECLARE_NOT_COPYABLE( ExecutionAction );

private:
	ICfixTestModule *Module;
	PCFIX_ACTION Action;
	
protected:
	ExecutionAction();

public:
	virtual ~ExecutionAction();

	/*------------------------------------------------------------------
	 * IUnknown methods.
	 */

	STDMETHOD_( ULONG, AddRef )() PURE;
	STDMETHOD_( ULONG, Release )() PURE;
	STDMETHOD( QueryInterface )( 
		__in REFIID Iid, 
		__out PVOID* Ptr );

	/*------------------------------------------------------------------
	 * ICfixAction methods.
	 */

	STDMETHOD( Run )( 
		__in ICfixEventSink *Sink
		);

	/*------------------------------------------------------------------
	 * ICfixExecutionActionInternal methods.
	 */

	STDMETHOD( Initialize )(
		__in ICfixTestModule *Module,
		__in PCFIX_ACTION Action
		);
};

/*----------------------------------------------------------------------
 * 
 * Factory.
 *
 */

IClassFactory& CfixctlpGetExecutionActionFactory()
{
	static ComClassFactory< 
		ComMtaObject< ExecutionAction >, CfixctlServerLock > Factory;
	return Factory;
}

/*----------------------------------------------------------------------
 * 
 * Class Implementation.
 *
 */

ExecutionAction::ExecutionAction()
	: Module( NULL )
	, Action( 0 )
{
}

ExecutionAction::~ExecutionAction()
{
	if ( this->Action )
	{
		Action->Dereference( this->Action );
	}

	if ( this->Module )
	{
		this->Module->Release();
	}
}

/*----------------------------------------------------------------------
 * IUnknown methods.
 */

STDMETHODIMP ExecutionAction::QueryInterface( 
	__in REFIID Iid, 
	__out PVOID* Ptr )
{
	HRESULT Hr;

	if ( InlineIsEqualGUID( Iid, IID_IUnknown ) ||
		 InlineIsEqualGUID( Iid, IID_ICfixAction )||
		 InlineIsEqualGUID( Iid, IID_ICfixExecutionActionInternal ) )
	{
		*Ptr = static_cast< ICfixExecutionActionInternal* >( this );
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
 * ICfixAction methods.
 */

STDMETHODIMP ExecutionAction::Run( 
	__in ICfixEventSink *Sink
	)
{
	if ( Sink == NULL )
	{
		return E_POINTER;
	}

	if ( this->Module == NULL )
	{
		return E_UNEXPECTED;
	}

	PCFIX_EXECUTION_CONTEXT Adapter = NULL;
	ICfixProcessEventSink *ProcessSink = NULL;

	//
	// Query process sink - we do not need a ICfixEventSink.
	//
	HRESULT Hr = Sink->QueryInterface( 
		IID_ICfixProcessEventSink, ( PVOID* ) &ProcessSink );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	//
	// Create adapter.
	//
	Hr = CfixctlpCreateExecutionContextAdapter(
		this->Module,
		ProcessSink,
		&Adapter );
	if ( FAILED( Hr ) )
	{
		goto Cleanup;
	}

	//
	// Let it run and tunnel events through the adapter.
	//
	Hr = Action->Run( Action, Adapter );

Cleanup:
	if ( ProcessSink )
	{
		ProcessSink->Release();
	}

	if ( Adapter )
	{
		Adapter->Dereference( Adapter );
	}

	return Hr;
}

/*----------------------------------------------------------------------
 * ICfixExecutionActionInternal methods.
 */

STDMETHODIMP ExecutionAction::Initialize(
	__in ICfixTestModule *Module,
	__in PCFIX_ACTION Action
	)
{
	if ( ! Module || ! Action )
	{
		return E_POINTER;
	}

	if ( this->Module != NULL || this->Action != NULL )
	{
		return E_UNEXPECTED;
	}

	Module->AddRef();
	Action->Reference( Action );

	this->Module			= Module;
	this->Action			= Action;

	return S_OK;
}