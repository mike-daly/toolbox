// CGCStub.h : Declaration of the CGCStub

#ifndef __CGCSTUB_H_
#define __CGCSTUB_H_

#include "resource.h"       // main symbols

#define SHMEMSIZE 4096 
typedef struct {
	FILETIME	ftLastSet;
	FILETIME	ftAdvance;
	double		dSlewRate;
} T_GODCLOCKBLOB;

extern LPVOID g_lpvShMem; // pointer to shared memory

/////////////////////////////////////////////////////////////////////////////
// CGCStub
class ATL_NO_VTABLE CGCStub : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CGCStub, &CLSID_CGCStub>,
	public IGCStub
{
public:
	CGCStub()
	{
	}

	STDMETHOD( SetClock )(
        long lLastSetHigh, 
        long lLastSetLow, 
        long lAdvanceHigh, 
        long lAdvanceLow, 
        double  dSlewRate );

	STDMETHOD( GetVClock ) (
		long* plVNowHigh,
		long* plVNowLow );


DECLARE_REGISTRY_RESOURCEID(IDR_CGCSTUB)

DECLARE_PROTECT_FINAL_CONSTRUCT()

BEGIN_COM_MAP(CGCStub)
	COM_INTERFACE_ENTRY(IGCStub)
END_COM_MAP()

// IGCStub
public:
};

#endif //__CGCSTUB_H_
