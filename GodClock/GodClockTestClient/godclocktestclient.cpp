// godclocktestclient.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "godclock.h"
#include "godclocktestclient.h"

int  main(int argc, char** argv, char** argp  )
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
