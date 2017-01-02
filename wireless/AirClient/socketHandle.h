#include <cstdio>
#include <cstdlib>
#include <string>
#include <cstring>

#define LINUX
#ifdef LINUX

#define INVALID_SOCKET (SOCKET)(~0)
#define SOCKET_ERROR (-1)

#include <unistd.h>
#include <errno.h>
#include <netdb.h>
#include <sys/types.h>
#include <netinet/in.h>
#include <sys/socket.h>
#include <arpa/inet.h>

typedef int SOCKET;

#endif

using namespace std;

typedef unsigned char byte;

const int DATA_LENGTH = 192;


//#define WINDOWS
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


typedef struct DataGram
{	
	int Type;
	int DataLength;
	byte Data[DATA_LENGTH];

	//1 client->server i2c data
	//2 server->client i2c data require
	//3 ---
	//4 server->client pwm set
	//5 ---
	//6 server->client i2c data stop

	//i2c data: (ax ay az)(gx gy gz)
	//pwm data: (num period duty)

} DataGram;


bool InitializeLink(Address, SOCKET*);
bool Send(DataGram, SOCKET);
bool Recieve(DataGram&, SOCKET);
void CloseLink(SOCKET);
