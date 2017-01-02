
#include "socketHandle.h"





bool InitializeLink(Address addr, SOCKET* connectSocket)
{
	//initialize winsock
	#ifdef WINDOWS
	WSAHandle::Initialize();
	#endif

	struct addrinfo *result = NULL,
		*ptr = NULL,
		hints;
	int iResult;

	//initialize hits
	memset(&hints, 0, sizeof(hints));
	hints.ai_family = AF_UNSPEC;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;


	// Resolve the server address and port
	iResult = getaddrinfo(addr.IP.c_str(), addr.Port.c_str(), &hints, &result);

	if (iResult != 0) {
		printf("getaddrinfo failed with error: %d\n", iResult);

		return false;
	}

	// Attempt to connect to an address until one succeeds
	for (ptr = result; ptr != NULL; ptr = ptr->ai_next) {

		// Create a SOCKET for connecting to server
		*connectSocket = socket(ptr->ai_family, ptr->ai_socktype,
			ptr->ai_protocol);

		if (*connectSocket == INVALID_SOCKET) {
			//printf("socket failed with error: %ld\n", WSAGetLastError());

			return false;
		}

		// Connect to server.
		iResult = connect(*connectSocket, ptr->ai_addr, (int)ptr->ai_addrlen);

		if (iResult == SOCKET_ERROR) {
			#ifdef WINDOWS
			closesocket(*connectSocket);
			#endif

			#ifdef LINUX
			close(*connectSocket);
			#endif
			
			*connectSocket = INVALID_SOCKET;
			continue;
		}
		break;
	}

	freeaddrinfo(result);

	if (*connectSocket == INVALID_SOCKET) {
		printf("Unable to connect to server!\n");

		return false;
	}

	return true;
}

bool Send(DataGram gram, SOCKET connectSocket)
{
	byte* tmp = new byte[sizeof(gram)];
	memcpy(tmp, &gram, sizeof(gram));

	// Send an initial buffer
	int iResult = send(connectSocket, (char*)tmp, sizeof(gram), 0);

	delete [] tmp;

	if (iResult == SOCKET_ERROR) {
		//printf("send failed with error: %d\n", WSAGetLastError());	
		return false;
	}
	else
		return true;

}

bool Recieve(DataGram& gram, SOCKET connectSocket)
{
	puts("recv start.");

	int size = sizeof(DataGram);
	byte* recvbuf = new byte[size];

	int iResult = recv(connectSocket, (char*)recvbuf, size, 0);		

	puts("recv end.");

	if (iResult > 0)
		memcpy(&gram, recvbuf, sizeof(DataGram));

	delete [] recvbuf;

	return iResult > 0;
}

void CloseLink(SOCKET connectSocket)
{
	// cleanup

	#ifdef WINDOWS
	closesocket(connectSocket);
	#endif

	#ifdef LINUX
	close(connectSocket);
	#endif
	
	#ifdef WINDOWS
	WSAHandle::Finialize();
	#endif
}
