#include "windows.h"
#include "stdio.h"
#include "stdafx.h"

#include "..\godclockclient\godclock.h"

int __cdecl main()
{
	SYSTEMTIME stVNow;
	GetSystemTime( &stVNow );

	printf( "GetSystemTime():  %02d:%02d:%02d,%02d:%02d:%02d\n",
           stVNow.wMonth, stVNow.wDay, (stVNow.wYear % 100),
           stVNow.wHour, stVNow.wMinute, stVNow.wSecond );

	GetVSystemTime( &stVNow );
	printf( "GetVSystemTime():  %02d:%02d:%02d,%02d:%02d:%02d\n",
           stVNow.wMonth, stVNow.wDay, (stVNow.wYear % 100),
           stVNow.wHour, stVNow.wMinute, stVNow.wSecond );

	return 0;
}
