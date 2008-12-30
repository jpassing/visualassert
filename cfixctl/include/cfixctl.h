

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* Compiler settings for cfixctl.idl:
    Oicf, W1, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

/* verify that the <rpcsal.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCSAL_H_VERSION__
#define __REQUIRED_RPCSAL_H_VERSION__ 100
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__


#ifndef __cfixctl_h__
#define __cfixctl_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __ICfixTestItem_FWD_DEFINED__
#define __ICfixTestItem_FWD_DEFINED__
typedef interface ICfixTestItem ICfixTestItem;
#endif 	/* __ICfixTestItem_FWD_DEFINED__ */


#ifndef __ICfixTestModule_FWD_DEFINED__
#define __ICfixTestModule_FWD_DEFINED__
typedef interface ICfixTestModule ICfixTestModule;
#endif 	/* __ICfixTestModule_FWD_DEFINED__ */


#ifndef __ICfixHost_FWD_DEFINED__
#define __ICfixHost_FWD_DEFINED__
typedef interface ICfixHost ICfixHost;
#endif 	/* __ICfixHost_FWD_DEFINED__ */


#ifndef __ICfixAction_FWD_DEFINED__
#define __ICfixAction_FWD_DEFINED__
typedef interface ICfixAction ICfixAction;
#endif 	/* __ICfixAction_FWD_DEFINED__ */


#ifndef __ICfixActionFactory_FWD_DEFINED__
#define __ICfixActionFactory_FWD_DEFINED__
typedef interface ICfixActionFactory ICfixActionFactory;
#endif 	/* __ICfixActionFactory_FWD_DEFINED__ */


#ifndef __Host_FWD_DEFINED__
#define __Host_FWD_DEFINED__

#ifdef __cplusplus
typedef class Host Host;
#else
typedef struct Host Host;
#endif /* __cplusplus */

#endif 	/* __Host_FWD_DEFINED__ */


#ifndef __TestModule_FWD_DEFINED__
#define __TestModule_FWD_DEFINED__

#ifdef __cplusplus
typedef class TestModule TestModule;
#else
typedef struct TestModule TestModule;
#endif /* __cplusplus */

#endif 	/* __TestModule_FWD_DEFINED__ */


#ifndef __TestFixture_FWD_DEFINED__
#define __TestFixture_FWD_DEFINED__

#ifdef __cplusplus
typedef class TestFixture TestFixture;
#else
typedef struct TestFixture TestFixture;
#endif /* __cplusplus */

#endif 	/* __TestFixture_FWD_DEFINED__ */


#ifndef __TestCase_FWD_DEFINED__
#define __TestCase_FWD_DEFINED__

#ifdef __cplusplus
typedef class TestCase TestCase;
#else
typedef struct TestCase TestCase;
#endif /* __cplusplus */

#endif 	/* __TestCase_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 



#ifndef __Cfixctl_LIBRARY_DEFINED__
#define __Cfixctl_LIBRARY_DEFINED__

/* library Cfixctl */
/* [version][helpstring][uuid] */ 



typedef /* [version][v1_enum][uuid] */  DECLSPEC_UUID("57f93e5d-d44f-11dd-99e2-0016d3b98f9b") 
enum CfixTestModuleType
    {	CfixTestModuleTypeUser	= 0,
	CfixTestModuleTypeKernel	= 1,
	CfixTestModuleTypeMax	= 1
    } 	CfixTestModuleType;

typedef /* [version][v1_enum][uuid] */  DECLSPEC_UUID("57f93e5c-d44f-11dd-99e2-0016d3b98f9b") 
enum CfixTestModuleArch
    {	CfixTestModuleArchI386	= 0,
	CfixTestModuleArchAmd64	= 1,
	CfixTestModuleArchMax	= 1
    } 	CfixTestModuleArch;

#define	CFIXCTL_EXECUTE_ALL	( 0xffffffff )


EXTERN_C const IID LIBID_Cfixctl;

#ifndef __ICfixTestItem_INTERFACE_DEFINED__
#define __ICfixTestItem_INTERFACE_DEFINED__

/* interface ICfixTestItem */
/* [oleautomation][version][uuid][object] */ 


EXTERN_C const IID IID_ICfixTestItem;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("57f93e42-d44f-11dd-99e2-0016d3b98f9b")
    ICfixTestItem : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetName( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Name) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateExecutionAction( 
            /* [in] */ ULONG Flags,
            /* [in] */ ULONG Reserved,
            /* [retval][out] */ __RPC__deref_out_opt ICfixAction **Action) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ICfixTestItemVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ICfixTestItem * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ICfixTestItem * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ICfixTestItem * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            ICfixTestItem * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Name);
        
        HRESULT ( STDMETHODCALLTYPE *CreateExecutionAction )( 
            ICfixTestItem * This,
            /* [in] */ ULONG Flags,
            /* [in] */ ULONG Reserved,
            /* [retval][out] */ __RPC__deref_out_opt ICfixAction **Action);
        
        END_INTERFACE
    } ICfixTestItemVtbl;

    interface ICfixTestItem
    {
        CONST_VTBL struct ICfixTestItemVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ICfixTestItem_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ICfixTestItem_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ICfixTestItem_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ICfixTestItem_GetName(This,Name)	\
    ( (This)->lpVtbl -> GetName(This,Name) ) 

#define ICfixTestItem_CreateExecutionAction(This,Flags,Reserved,Action)	\
    ( (This)->lpVtbl -> CreateExecutionAction(This,Flags,Reserved,Action) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ICfixTestItem_INTERFACE_DEFINED__ */


#ifndef __ICfixTestModule_INTERFACE_DEFINED__
#define __ICfixTestModule_INTERFACE_DEFINED__

/* interface ICfixTestModule */
/* [oleautomation][version][uuid][object] */ 


EXTERN_C const IID IID_ICfixTestModule;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("57f93e5b-d44f-11dd-99e2-0016d3b98f9b")
    ICfixTestModule : public ICfixTestItem
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetPath( 
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Path) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetType( 
            /* [out] */ __RPC__out CfixTestModuleType *Type,
            /* [out] */ __RPC__out CfixTestModuleArch *Arch) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ICfixTestModuleVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ICfixTestModule * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ICfixTestModule * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ICfixTestModule * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetName )( 
            ICfixTestModule * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Name);
        
        HRESULT ( STDMETHODCALLTYPE *CreateExecutionAction )( 
            ICfixTestModule * This,
            /* [in] */ ULONG Flags,
            /* [in] */ ULONG Reserved,
            /* [retval][out] */ __RPC__deref_out_opt ICfixAction **Action);
        
        HRESULT ( STDMETHODCALLTYPE *GetPath )( 
            ICfixTestModule * This,
            /* [retval][out] */ __RPC__deref_out_opt BSTR *Path);
        
        HRESULT ( STDMETHODCALLTYPE *GetType )( 
            ICfixTestModule * This,
            /* [out] */ __RPC__out CfixTestModuleType *Type,
            /* [out] */ __RPC__out CfixTestModuleArch *Arch);
        
        END_INTERFACE
    } ICfixTestModuleVtbl;

    interface ICfixTestModule
    {
        CONST_VTBL struct ICfixTestModuleVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ICfixTestModule_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ICfixTestModule_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ICfixTestModule_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ICfixTestModule_GetName(This,Name)	\
    ( (This)->lpVtbl -> GetName(This,Name) ) 

#define ICfixTestModule_CreateExecutionAction(This,Flags,Reserved,Action)	\
    ( (This)->lpVtbl -> CreateExecutionAction(This,Flags,Reserved,Action) ) 


#define ICfixTestModule_GetPath(This,Path)	\
    ( (This)->lpVtbl -> GetPath(This,Path) ) 

#define ICfixTestModule_GetType(This,Type,Arch)	\
    ( (This)->lpVtbl -> GetType(This,Type,Arch) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ICfixTestModule_INTERFACE_DEFINED__ */


#ifndef __ICfixHost_INTERFACE_DEFINED__
#define __ICfixHost_INTERFACE_DEFINED__

/* interface ICfixHost */
/* [oleautomation][version][uuid][object] */ 


EXTERN_C const IID IID_ICfixHost;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("57f93e40-d44f-11dd-99e2-0016d3b98f9b")
    ICfixHost : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LoadModule( 
            /* [in] */ __RPC__in const BSTR Path,
            /* [retval][out] */ __RPC__deref_out_opt ICfixTestModule **Module) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetArchitecture( 
            /* [out] */ __RPC__out CfixTestModuleArch *Arch) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ICfixHostVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ICfixHost * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ICfixHost * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ICfixHost * This);
        
        HRESULT ( STDMETHODCALLTYPE *LoadModule )( 
            ICfixHost * This,
            /* [in] */ __RPC__in const BSTR Path,
            /* [retval][out] */ __RPC__deref_out_opt ICfixTestModule **Module);
        
        HRESULT ( STDMETHODCALLTYPE *GetArchitecture )( 
            ICfixHost * This,
            /* [out] */ __RPC__out CfixTestModuleArch *Arch);
        
        END_INTERFACE
    } ICfixHostVtbl;

    interface ICfixHost
    {
        CONST_VTBL struct ICfixHostVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ICfixHost_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ICfixHost_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ICfixHost_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ICfixHost_LoadModule(This,Path,Module)	\
    ( (This)->lpVtbl -> LoadModule(This,Path,Module) ) 

#define ICfixHost_GetArchitecture(This,Arch)	\
    ( (This)->lpVtbl -> GetArchitecture(This,Arch) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ICfixHost_INTERFACE_DEFINED__ */


#ifndef __ICfixAction_INTERFACE_DEFINED__
#define __ICfixAction_INTERFACE_DEFINED__

/* interface ICfixAction */
/* [oleautomation][version][uuid][object] */ 


EXTERN_C const IID IID_ICfixAction;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("57f93e41-d44f-11dd-99e2-0016d3b98f9b")
    ICfixAction : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Run( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ICfixActionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ICfixAction * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ICfixAction * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ICfixAction * This);
        
        HRESULT ( STDMETHODCALLTYPE *Run )( 
            ICfixAction * This);
        
        END_INTERFACE
    } ICfixActionVtbl;

    interface ICfixAction
    {
        CONST_VTBL struct ICfixActionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ICfixAction_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ICfixAction_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ICfixAction_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ICfixAction_Run(This)	\
    ( (This)->lpVtbl -> Run(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ICfixAction_INTERFACE_DEFINED__ */


#ifndef __ICfixActionFactory_INTERFACE_DEFINED__
#define __ICfixActionFactory_INTERFACE_DEFINED__

/* interface ICfixActionFactory */
/* [oleautomation][version][uuid][object] */ 


EXTERN_C const IID IID_ICfixActionFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("57f93e59-d44f-11dd-99e2-0016d3b98f9b")
    ICfixActionFactory : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateExecutionAction( 
            /* [in] */ ULONG FixtureOrdinal,
            /* [in] */ ULONG TestCaseOrdinal,
            /* [in] */ ULONG Flags,
            /* [in] */ ULONG Reserved,
            /* [retval][out] */ __RPC__deref_out_opt ICfixAction **Action) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ICfixActionFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ICfixActionFactory * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ICfixActionFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ICfixActionFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateExecutionAction )( 
            ICfixActionFactory * This,
            /* [in] */ ULONG FixtureOrdinal,
            /* [in] */ ULONG TestCaseOrdinal,
            /* [in] */ ULONG Flags,
            /* [in] */ ULONG Reserved,
            /* [retval][out] */ __RPC__deref_out_opt ICfixAction **Action);
        
        END_INTERFACE
    } ICfixActionFactoryVtbl;

    interface ICfixActionFactory
    {
        CONST_VTBL struct ICfixActionFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ICfixActionFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ICfixActionFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ICfixActionFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ICfixActionFactory_CreateExecutionAction(This,FixtureOrdinal,TestCaseOrdinal,Flags,Reserved,Action)	\
    ( (This)->lpVtbl -> CreateExecutionAction(This,FixtureOrdinal,TestCaseOrdinal,Flags,Reserved,Action) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ICfixActionFactory_INTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_Host;

#ifdef __cplusplus

class DECLSPEC_UUID("57f93e43-d44f-11dd-99e2-0016d3b98f9b")
Host;
#endif

EXTERN_C const CLSID CLSID_TestModule;

#ifdef __cplusplus

class DECLSPEC_UUID("57f93e55-d44f-11dd-99e2-0016d3b98f9b")
TestModule;
#endif

EXTERN_C const CLSID CLSID_TestFixture;

#ifdef __cplusplus

class DECLSPEC_UUID("57f93e56-d44f-11dd-99e2-0016d3b98f9b")
TestFixture;
#endif

EXTERN_C const CLSID CLSID_TestCase;

#ifdef __cplusplus

class DECLSPEC_UUID("57f93e57-d44f-11dd-99e2-0016d3b98f9b")
TestCase;
#endif
#endif /* __Cfixctl_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


