#include <socketHandle.h>
#include <BBB_I2C.h>
#include <MPU6050.h>
#include <pwm.h>

#include <iostream>
#include <cstdio>
#include <vector>

#include <pthread.h>

using namespace std;

MPU6050 MPU;
BBB_I2C BBB_I2C;
PWM pwm;
SOCKET ep;
pthread_t* thread;

bool run = true;
bool isSend;

void process(DataGram);
void* sendI2C(void*);

int main()
{
	if (MPU.testConnection() < 1)
	{
		printf ("Device ID not match!\n");
		exit(1);
	}
	puts("MPU6050 connect successful.");
		
	if (MPU.initialize() < 1) 
	{
		printf ("MPU initialize fail!\n");
		exit(1);
	}
	puts("MPU6050 initialize successful.");

	if(!pwm.Initialize())
	{
		puts("pwm initialize fail!\n");
		exit(1);
	}
	puts("PWM initialize successful");

	if(!InitializeLink(Address("192.168.1.105", "61357"), &ep))
	{
		printf("Wireless network initialize fail!\n");
		exit(1);
	}
	puts("Wireless network initialize successful.");

	while(run)
	{
		DataGram gram;
		Recieve(gram, ep);
		process(gram);

	}

	return 0;
}

void* sendI2C(void* par)
{
	int16_t data[6];

	while(isSend)
	{
		MPU.getMotion6(
			&data[0], &data[1], &data[2], 
			&data[3], &data[4], &data[5]);

		printf("ax = %d,ay = %d,az = %d,gx = %d,gy = %d,gz = %d\n",
			data[0],data[1],data[2],data[3],data[4],data[5]);	


		DataGram gram;

		gram.Type = 1;
		gram.DataLength = sizeof(int16_t)*6;
		memcpy(gram.Data, data, sizeof(data));

		Send(gram,ep);

		usleep(500000);
	}

}


void process(DataGram gram)
{
	int inst = gram.Type;

	//printf("type = %d\n",gram.Type);

	switch(inst)
	{
		case 2:	//i2c data require

			puts("i2c data required.");

			isSend = true;
			thread = new pthread_t;
			pthread_create(thread, NULL, sendI2C, NULL);

			break;
		case 4: //pwm set
			int num, var, value, index;
			index = 0;
			
			memcpy(&var, &gram.Data[index], sizeof(int));
			index += sizeof(int);

			memcpy(&num, &gram.Data[index], sizeof(int));
			index += sizeof(int);

			memcpy(&value, &gram.Data[index], sizeof(int));
			index += sizeof(int);

			printf("var=%d num=%d value=%d\n", var, num, value);

			if(var == 0)
				pwm.SetValue(num, "run", value);
			if(var == 1)
				pwm.SetValue(num, "duty_ns", value);
			if(var == 2)
				pwm.SetValue(num, "period_ns", value);
			

			break;
		case 6:	//i2c data stop
			puts("i2c data stopped.");

			isSend = false;
     		pthread_join(*thread, NULL);
     		delete thread;
			break;
		default:
			break;

	}

}
