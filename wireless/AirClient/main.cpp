#include <cstdio>
#include <cstdlib>
#include <string>
#include <iostream>

#include "socketHandle.h"

using namespace std;


int main()
{
	SOCKET s = INVALID_SOCKET;
	
	InitializeLink(Address("192.168.0.106", "61357"), &s);

	CloseLink(s);
	
	getchar();

	return 0;
}
