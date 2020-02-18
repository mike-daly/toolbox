
#pragma warning( disable: 4049 )  /* more than 64k source lines */

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 6.00.0347 */
/* at Wed Jul 17 08:59:29 2002
 */
/* Compiler settings for godclock_stub.idl:
    Os, W1, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 440
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __godclock_stub_h_h__
#define __godclock_stub_h_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IGCStub_FWD_DEFINED__
#define __IGCStub_FWD_DEFINED__
typedef interface IGCStub IGCStub;
#endif 	/* __IGCStub_FWD_DEFINED__ */


#ifndef __CGCStub_FWD_DEFINED__
#define __CGCStub_FWD_DEFINED__

#ifdef __cplusplus
typedef class CGCStub CGCStub;
#else
typedef struct CGCStub CGCStub;
#endif /* __cplusplus */

#endif 	/* __CGCStub_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 

void * __RPC_USER MIDL_user_allocate(size_t);
void __RPC_USER MIDL_user_free( void * ); 

#ifndef __IGCStub_INTERFACE_DEFINED__
#define __IGCStub_INTERFACE_DEFINED__

/* interface IGCStub */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IGCStub;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3EDF9FAA-FE8B-43C0-A462-1930299E98DA")
    IGCStub : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetClock( 
            /* [in] */ long lLastSetHigh,
            /* [in] */ long lLastSetLow,
            /* [in] */ long lAdvanceHigh,
            /* [in] */ long lAdvanceLow,
            /* [in] */ double fSlewRate) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetVClock( 
            /* [out] */ long *plVNowHigh,
            /* [out] */ long *plVNowLow) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IGCStubVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IGCStub * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IGCStub * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IGCStub * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetClock )( 
            IGCStub * This,
            /* [in] */ long lLastSetHigh,
            /* [in] */ long lLastSetLow,
            /* [in] */ long lAdvanceHigh,
            /* [in] */ long lAdvanceLow,
            /* [in] */ double fSlewRate);
        
        HRESULT ( STDMETHODCALLTYPE *GetVClock )( 
            IGCStub * This,
            /* [out] */ long *plVNowHigh,
            /* [out] */ long *plVNowLow);
        
        END_INTERFACE
    } IGCStubVtbl;

    interface IGCStub
    {
        CONST_VTBL struct IGCStubVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IGCStub_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define IGCStub_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define IGCStub_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define IGCStub_SetClock(This,lLastSetHigh,lLastSetLow,lAdvanceHigh,lAdvanceLow,fSlewRate)	\
    (This)->lpVtbl -> SetClock(This,lLastSetHigh,lLastSetLow,lAdvanceHigh,lAdvanceLow,fSlewRate)

#define IGCStub_GetVClock(This,plVNowHigh,plVNowLow)	\
    (This)->lpVtbl -> GetVClock(This,plVNowHigh,plVNowLow)

#endif /* COBJMACROS */


#endif 	/* C style interface */



HRESULT STDMETHODCALLTYPE IGCStub_SetClock_Proxy( 
    IGCStub * This,
    /* [in] */ long lLastSetHigh,
    /* [in] */ long lLastSetLow,
    /* [in] */ long lAdvanceHigh,
    /* [in] */ long lAdvanceLow,
    /* [in] */ double fSlewRate);


void __RPC_STUB IGCStub_SetClock_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IGCStub_GetVClock_Proxy( 
    IGCStub * This,
    /* [out] */ long *plVNowHigh,
    /* [out] */ long *plVNowLow);


void __RPC_STUB IGCStub_GetVClock_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __IGCStub_INTERFACE_DEFINED__ */



#ifndef __GODCLOCKLib_LIBRARY_DEFINED__
#define __GODCLOCKLib_LIBRARY_DEFINED__

/* library GODCLOCKLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_GODCLOCKLib;

EXTERN_C const CLSID CLSID_CGCStub;

#ifdef __cplusplus

class DECLSPEC_UUID("A76F4742-E169-410F-BD25-E562FC00628D")
CGCStub;
#endif
#endif /* __GODCLOCKLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


