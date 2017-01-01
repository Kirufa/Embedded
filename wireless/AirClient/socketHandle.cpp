
#include "socketHandle.h"





bool InitializeLink(Address addr, SOCKET* connectSocket)
{
	//initialize winsock
	WSAHandle::Initialize();


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
			printf("socket failed with error: %ld\n", WSAGetLastError());

			return false;
		}

		// Connect to server.
		iResult = connect(*connectSocket, ptr->ai_addr, (int)ptr->ai_addrlen);

		if (iResult == SOCKET_ERROR) {
			closesocket(*connectSocket);
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

bool Send(string message, SOCKET connectSocket)
{

	// Send an initial buffer
	int iResult = send(connectSocket, message.c_str(), message.size(), 0);

	if (iResult == SOCKET_ERROR) {
		//printf("send failed with error: %d\n", WSAGetLastError());	
		return false;
	}
	else
		return true;

}

bool Recieve(string& message, int size, SOCKET connectSocket)
{
	byte* recvbuf = new byte[size];

	int iResult = recv(connectSocket, (char*)recvbuf, size, 0);


	if (iResult > 0)
	{
		message = string((char*)recvbuf);
		return true;
	}
	else
		return false;

}

void CloseLink(SOCKET connectSocket)
{
	// cleanup
	closesocket(connectSocket);
	WSAHandle::Finialize();
}
