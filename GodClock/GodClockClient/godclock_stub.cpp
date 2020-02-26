// newcontrol.cpp : Implementation of DLL Exports.


// Note: Proxy/Stub Information
//      To build a separate proxy/stub DLL, 
//      run nmake -f newcontrolps.mk in the project directory.

#include "stdafx.h"
#include "resource.h"
#include <initguid.h>
#include "godclock_stub_h.h"

#include "godclock_stub_i.c"
#include "CGCStub.h"

CComModule _Module;
LPVOID g_lpvShMem = NULL; // pointer to shared memory

BEGIN_OBJECT_MAP(ObjectMap)
OBJECT_ENTRY(CLSID_CGCStub, CGCStub)
END_OBJECT_MAP()

/////////////////////////////////////////////////////////////////////////////
// DLL Entry Point

extern "C"
BOOL WINAPI DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID /*lpReserved*/)
{
    HANDLE hMapObject = NULL;  // handle to file mapping
    BOOL fInit, fIgnore; 


    switch (dwReason) 
    { 
        // The DLL is loading due to process 
        // initialization or a call to LoadLibrary. 
 
          case DLL_PROCESS_ATTACH: 
	        _Module.Init(ObjectMap, hInstance, &LIBID_GODCLOCKLib);
	        DisableThreadLibraryCalls(hInstance);
 
            // Create a named file mapping object.
 

            hMapObject = CreateFileMapping( 
                INVALID_HANDLE_VALUE, // use paging file
                NULL,                 // no security attributes
                PAGE_READWRITE,       // read/write access
                0,                    // size: high 32-bits
                SHMEMSIZE,            // size: low 32-bits
                L"GodClockSharedMem");     // name of map object
            if (hMapObject == NULL) 
                return FALSE; 
 
            // The first process to attach initializes memory.
 
            fInit = (GetLastError() != ERROR_ALREADY_EXISTS); 
 
            // Get a pointer to the file-mapped shared memory.
 
            g_lpvShMem = MapViewOfFile( 
                hMapObject,     // object to map view of
                FILE_MAP_WRITE, // read/write access
                0,              // high offset:  map from
                0,              // low offset:   beginning
                0);             // default: map entire file
            if (g_lpvShMem == NULL) 
                return FALSE; 
 
            // Initialize memory if this is the first process.
 
			if (fInit) {
                memset(g_lpvShMem, '\0', SHMEMSIZE); 

				T_GODCLOCKBLOB*	pGodClockData = (T_GODCLOCKBLOB*) g_lpvShMem;
				pGodClockData->dSlewRate = 1.0;

#if 0
				SYSTEMTIME	stNow;
				FILETIME	ftNow;
				GetSystemTime( &stNow);
				SystemTimeToFileTime( &stNow, &ftNow );
				pGodClockData->ftLastSet = ftNow;

				ftNow.dwHighDateTime = 0;
				ftNow.dwLowDateTime = 0;
				pGodClockData->ftAdvance = ftNow;

				pGodClockData->dSlewRate = 2.0;
#endif

			}
 
            break; 
 
        // The attached process creates a new thread. 
 
        case DLL_THREAD_ATTACH: 
            break; 
 
        // The thread of the attached process terminates.
 
        case DLL_THREAD_DETACH: 
            break; 
 
        // The DLL is unloading from a process due to 
        // process termination or a call to FreeLibrary. 
 
        case DLL_PROCESS_DETACH: 
	        _Module.Term();
 
            // Unmap shared memory from the process's address space.
 
            fIgnore = UnmapViewOfFile(g_lpvShMem); 
 
            // Close the process's handle to the file-mapping object.
 
            fIgnore = CloseHandle(hMapObject); 
 
            break; 
 
        default: 
          break; 
     } 
 
	
	
	return TRUE;    // ok
}

/////////////////////////////////////////////////////////////////////////////
// Used to determine whether the DLL can be unloaded by OLE

extern "C" STDAPI DllCanUnloadNow(void)
{
    return (_Module.GetLockCount()==0) ? S_OK : S_FALSE;
}

/////////////////////////////////////////////////////////////////////////////
// Returns a class factory to create an object of the requested type

extern "C" STDAPI DllGetClassObject(REFCLSID rclsid, REFIID riid, LPVOID* ppv)
{
    return _Module.GetClassObject(rclsid, riid, ppv);
}

/////////////////////////////////////////////////////////////////////////////
// DllRegisterServer - Adds entries to the system registry

STDAPI DllRegisterServer(void)
{
    // registers object, typelib and all interfaces in typelib
    return _Module.RegisterServer(TRUE);
}

/////////////////////////////////////////////////////////////////////////////
// DllUnregisterServer - Removes entries from the system registry

STDAPI DllUnregisterServer(void)
{
    return _Module.UnregisterServer(TRUE);
}


//----------------------------------------------------------------------
extern "C" __declspec(dllexport) VOID GetVSystemTime( LPSYSTEMTIME lpSystemTime ) {
	// Get real system time,
	// go to shared memory, 
	// get the current offset, 
	// and add it to real system time

	FILETIME		ftNow;
	LARGE_INTEGER	ulNow;
	LARGE_INTEGER	ulLastSet;
	LARGE_INTEGER	ulAdvance;
	WCHAR			wszDebug[512];

	T_GODCLOCKBLOB*	pGodClockData = (T_GODCLOCKBLOB*) g_lpvShMem;

	GetSystemTime( lpSystemTime );
	SystemTimeToFileTime( lpSystemTime, &ftNow );

	ulNow.HighPart = ftNow.dwHighDateTime;
	ulNow.LowPart = ftNow.dwLowDateTime;

	ulLastSet.HighPart = pGodClockData->ftLastSet.dwHighDateTime;
	ulLastSet.LowPart = pGodClockData->ftLastSet.dwLowDateTime;

	ulAdvance.HighPart = pGodClockData->ftAdvance.dwHighDateTime;
	ulAdvance.LowPart = pGodClockData->ftAdvance.dwLowDateTime;

    ulNow.QuadPart = (ULONGLONG) ((ulNow.QuadPart - ulLastSet.QuadPart) * pGodClockData->dSlewRate)
			+ ulAdvance.QuadPart + ulLastSet.QuadPart;

	ftNow.dwHighDateTime = ulNow.HighPart;
	ftNow.dwLowDateTime = ulNow.LowPart;

	FileTimeToSystemTime( &ftNow, lpSystemTime );

	swprintf( wszDebug, L"GetVSystemTime():  %l.%l.%l %l:%l:%l.%l\n", 
		lpSystemTime->wYear, 
		lpSystemTime->wMonth,
		lpSystemTime->wDay,
		lpSystemTime->wHour,
		lpSystemTime->wMinute,
		lpSystemTime->wSecond,
		lpSystemTime->wMilliseconds );

	OutputDebugString( wszDebug );

	return;
};

