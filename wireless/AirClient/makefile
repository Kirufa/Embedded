all: main.o socketHandle.o
	g++ -o client main.o socketHandle.o

socketHandle.o:	socketHandle.cpp socketHandle.h
	g++ -c socketHandle.cpp

main.o: main.cpp
	g++ -c main.cpp


clear:
	rm -f socketHandle.o main.o client
