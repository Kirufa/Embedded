#include <cstdio>
#include <cstdlib>
#include <string>

using namespace std;

typedef unsigned char byte;

const int DATA_LENGTH = 1024;


#define WINDOWS
#ifdef WINDOWS

#include<winsock2.h>
#include <ws2tcpip.h>
#pragma comment(lib, "ws2_32.lib")

class WSAHandle
{
private:


public:

	static void Initialize()
	{
		WSAData wsaData;

		if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
		{

			fprintf(stderr, "Startup failed.\n");
			Finialize();
			exit(1);
		}

	}

	static void Finialize()
	{
		WSACleanup();
	}
};

#endif



class Address
{
public:
	string IP;
	string Port;

	Address(string ip, string port) :IP(ip), Port(port) {}
};

typedef struct SocketData
{
	int instruction;
	int length;
	byte data[DATA_LENGTH];
} SocketData;


bool InitializeLink(Address, SOCKET*);
bool Send(string, SOCKET);
bool Recieve(string&, int, SOCKET);
void CloseLink(SOCKET);