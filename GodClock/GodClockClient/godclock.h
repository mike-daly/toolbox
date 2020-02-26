#include <windows.h>

// Virtualized system time -- "The God Clock"

extern "C" __declspec(dllexport) 
VOID GetVSystemTime(
  LPSYSTEMTIME lpSystemTime   // system time
);

