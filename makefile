WL=wireless/AirClient
I2C=I2C
PWM=pwm

airplane:socketHandle.o main.o BBB_I2C.o pwm.o
	g++ -o airplane BBB_I2C.o pwm.o socketHandle.o main.o MPU6050.o -lpthread

main.o:main.cpp
	g++ -I $(WL) -I $(I2C) -I $(PWM) -lpthread  -c main.cpp

BBB_I2C.o:$(I2C)/BBB_I2C.cpp $(I2C)/BBB_I2C.h $(I2C)/MPU6050.h $(I2C)/MPU6050.cpp
	g++ -lrt -c $(I2C)/BBB_I2C.cpp $(I2C)/MPU6050.cpp

socketHandle.o:$(WL)/socketHandle.cpp $(WL)/socketHandle.h
	g++ -c $(WL)/socketHandle.cpp

pwm.o:$(PWM)/pwm.cpp $(PWM)/pwm.h
	g++ -c $(PWM)/pwm.cpp
