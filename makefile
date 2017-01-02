WL=/home/debian/final/Imbedded/wireless/AirClient
I2C=/home/debian/final/Imbedded/I2C


airplane:socketHandle.o main.o BBB_I2C.o 
	g++ -o airplane BBB_I2C.o socketHandle.o main.o MPU6050.o -lpthread

main.o:main.cpp
	g++ -I $(WL) -I $(I2C) -lpthread  -c main.cpp


BBB_I2C.o:$(I2C)/BBB_I2C.cpp $(I2C)/BBB_I2C.h $(I2C)/MPU6050.h $(I2C)/MPU6050.cpp
	g++ -lrt -c $(I2C)/BBB_I2C.cpp $(I2C)/MPU6050.cpp

socketHandle.o:$(WL)/socketHandle.cpp $(WL)/socketHandle.h
	g++ -c $(WL)/socketHandle.cpp

