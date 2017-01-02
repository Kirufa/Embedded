#include "wireless/AirClient/SocketHandle.h"
#include "BBB_I2C.h"
#include "MPU6050.h"

#include <iostream>
#include <cstdio>
#include <vector>

#include <pthread.h>

using namespace std;

MPU6050 MPU;
BBB_I2C BBB_I2C;
SOCKET ep;
pthread* thread;

bool run = true;
bool isSend;

void process(int);
void sendI2C();

int main()
{
	if (MPU.testConnection() < 1)
	{
		printf ("Device ID not match!\n");
		exit(1);
	}
		
	if (MPU.initialize() < 1) 
	{
		printf ("MPU initialize fail!\n");
		exit(1);
	}

	if(!InitializeLink(Address("192.168.0.106", "61357"), &ep))
	{
		printf("Wireless network initialize fail!\n");
		exit(1);
	}

	while(run)
	{

	}

	return 0;
}

void sendI2C()
{
	int16_t data[6];

	while(isSend)
	{
		MPU.getMotion6(
			&data[0], &data[1], &data[2], 
			&data[3], &data[4], &data[5]);
		
		DataGram gram;

		gram.Type = 1;
		gram.DataLength = sizeof(int16_t)*6;
		memcpy(gra.Data, data, sizeof(data));

		Send(gram,ep);
	}

}


void process(int inst,vector<int> data)
{
	switch(inst)
	{
		case 2:	//i2c data require
			isSend = true;
			thread = new pthread;
			pthread_create(&thread, NULL, sendI2C, 0);

			break;
		case 4: //pwm set

			break;
		case 6:	//i2c data stop
			isSend = false;
     		pthread_join(thread, NULL);
     		delete thread;
			break;

	}

}