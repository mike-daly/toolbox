// CGCStub.cpp : Implementation of CGCStub
#include "stdafx.h"
#include "godclock_stub_h.h"
#include "CGCStub.h"

/////////////////////////////////////////////////////////////////////////////
// CGCStub

STDMETHODIMP CGCStub::SetClock (
        long lLastSetHigh, 
        long lLastSetLow, 
        long lAdvanceHigh, 
        long lAdvanceLow, 
        double  dSlewRate )
{
	T_GODCLOCKBLOB* pGC = (T_GODCLOCKBLOB*) g_lpvShMem;

    if (!pGC) return E_FAIL;

	if (dSlewRate < 0.0) return E_INVALIDARG;
    
	pGC->ftLastSet.dwHighDateTime = lLastSetHigh;
	pGC->ftLastSet.dwLowDateTime = lLastSetLow;
	pGC->ftAdvance.dwHighDateTime = lAdvanceHigh;
	pGC->ftAdvance.dwLowDateTime = lAdvanceLow;
	pGC->dSlewRate = dSlewRate;

	return S_OK;
};

STDMETHODIMP CGCStub::GetVClock(
		long* plVNowHigh,
		long* plVNowLow )
{
	SYSTEMTIME stNow;
	FILETIME ftNow;
	HRESULT hr = S_OK;

	if (!plVNowHigh) hr = E_INVALIDARG;
	if (!plVNowLow) hr = E_INVALIDARG;
	if (hr != S_OK) goto Error;

	GetVSystemTime(&stNow);

	SystemTimeToFileTime( &stNow, &ftNow );
	if (hr != S_OK) goto Error;

	*plVNowHigh = ftNow.dwHighDateTime;
	*plVNowLow = ftNow.dwLowDateTime;

Error:
	return hr;
};