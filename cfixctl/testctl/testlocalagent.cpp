/*----------------------------------------------------------------------
 * Purpose:
 *		Test LocalAgent class.
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

#include <testctlp.h>

class TestLocalAgent : public cfixcc::TestFixture
{
private:
	static COM_EXPORTS Exports;
	IClassFactory *AgentFactory;;
	
public:
	TestLocalAgent() : AgentFactory( NULL )
	{
	}

	static void SetUp()
	{
		CoInitializeEx( NULL, COINIT_MULTITHREADED );
		GetComExports( L"cfixctl.dll", &Exports );
	}

	static void TearDown()
	{
		CoUninitialize();
	}

	virtual void Before()
	{
		CFIXCC_ASSERT_OK( Exports.GetClassObject( 
			CLSID_LocalAgent, IID_IClassFactory, ( PVOID* ) &this->AgentFactory ) );
		CFIXCC_ASSERT( this->AgentFactory );
	}

	virtual void After()
	{
		if ( this->AgentFactory )
		{
			 this->AgentFactory->Release();
		}
	}


	void RegisterAndObtainWithoutWaiting()
	{
		ICfixAgent *Agent;
		CFIXCC_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIXCC_ASSERT( Agent );
		__assume( Agent );

		ICfixHost *LocalHost;
		CFIXCC_ASSERT_OK( Agent->CreateHost( 
			TESTCTLP_OWN_ARCHITECTURE,
			CLSCTX_INPROC_SERVER,
			0,
			0,
			&LocalHost ) );

		//
		// Query invalid host.
		//
		ICfixHost *ObtainedHost;
		CFIXCC_ASSERT_EQUALS( CFIXCTL_E_HOST_NOT_FOUND,
			Agent->WaitForHostConnection(
				0xF00, 0, &ObtainedHost ) );
		CFIXCC_ASSERT( ObtainedHost == NULL );

		CFIXCC_ASSERT_EQUALS( HRESULT_FROM_WIN32( ERROR_TIMEOUT ),
			Agent->WaitForHostConnection(
				0xF00, 5, &ObtainedHost ) );
		CFIXCC_ASSERT( ObtainedHost == NULL );

		//
		// Register & Query.
		//
		CFIXCC_ASSERT_OK( Agent->RegisterHost( 0xB00, LocalHost ) );
		LocalHost->Release();

		CFIXCC_ASSERT_EQUALS( CFIXCTL_E_HOST_NOT_FOUND,
			Agent->WaitForHostConnection(
				0xF00, 0, &ObtainedHost ) );
		CFIXCC_ASSERT( ObtainedHost == NULL );

		CFIXCC_ASSERT_OK( 
			Agent->WaitForHostConnection(
				0xB00, 0, &ObtainedHost ) );
		CFIX_ASSUME( ObtainedHost );
		ObtainedHost->Release();

		Agent->Release();
	}

	static DWORD RegisterThreadProc( __in PVOID PvAgent )
	{
		ICfixAgent *Agent = static_cast< ICfixAgent* >( PvAgent );

		Sleep( 200 );

		ICfixHost *LocalHost;
		CFIXCC_ASSERT_OK( Agent->CreateHost( 
			TESTCTLP_OWN_ARCHITECTURE,
			CLSCTX_INPROC_SERVER,
			0,
			0,
			&LocalHost ) );

		CFIXCC_ASSERT_OK( Agent->RegisterHost( 0xB00, LocalHost ) );
		LocalHost->Release();

		return 0;
	}

	void RegisterAndObtainWithWaiting()
	{
		ICfixAgent *Agent;
		CFIXCC_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIXCC_ASSERT( Agent );
		__assume( Agent );


		HANDLE Thread = CfixCreateThread(
			NULL, 0, RegisterThreadProc, Agent, 0, NULL );
		CFIXCC_ASSERT( Thread != NULL );

		//
		// Query.
		//
		ICfixHost *ObtainedHost;
		CFIXCC_ASSERT_OK( 
			Agent->WaitForHostConnection(
				0xB00, 500, &ObtainedHost ) );
		CFIX_ASSUME( ObtainedHost );
		ObtainedHost->Release();
		
		//
		// Query again.
		//
		CFIXCC_ASSERT_EQUALS( CFIXCTL_E_HOST_NOT_FOUND,
			Agent->WaitForHostConnection(
				0xF00, 0, &ObtainedHost ) );
		CFIXCC_ASSERT( ObtainedHost == NULL );

		Agent->Release();

		WaitForSingleObject( Thread, INFINITE );
		CloseHandle( Thread );
	}

	void SpawnSameArch()
	{
		ICfixAgent *Agent;
		CFIXCC_ASSERT_OK( AgentFactory->CreateInstance( 
			NULL, IID_ICfixAgent, ( PVOID* ) &Agent ) );
		CFIX_ASSUME( Agent );

		ULONG FlagSets[] = { 0, CFIXCTL_AGENT_FLAG_USE_JOB };

		for ( ULONG Flags = 0; Flags < _countof( FlagSets ); Flags++ )
		{
			ICfixHost *Host;
			CFIXCC_ASSERT_OK( Agent->CreateHost(
				CFIXCTL_OWN_ARCHITECTURE,
				CLSCTX_LOCAL_SERVER,
				FlagSets[ Flags ],
				INFINITE,
				&Host ) );

			CFIX_ASSUME( Host );
			Host->Release();
		}

		Agent->Release();
	}
};

COM_EXPORTS TestLocalAgent::Exports;

CFIXCC_BEGIN_CLASS( TestLocalAgent )
	CFIXCC_METHOD( RegisterAndObtainWithoutWaiting )
	CFIXCC_METHOD( RegisterAndObtainWithWaiting )
	CFIXCC_METHOD( SpawnSameArch )
CFIXCC_END_CLASS()