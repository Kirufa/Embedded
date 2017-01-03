#include <fstream>
#include <cstdlib>
#include <string>
#include <sstream>

#include "pwm.h"


bool PWM::Initialize()
{
	fstream fs("/sys/devices/bone_capemgr.9/slots",fstream::in | fstream::out);
	
	if(!findString(fs, "am33xx_pwm"))
		system("sudo echo am33xx_pwm > /sys/devices/bone_capemgr.9/slots");
	else
		return false;

	string fileName = "bone_pwm_";

	for(int i = 0; i != 4; ++i)
	{
		if(!findString(fs ,fileName + pin[i]))
			system(("sudo echo " + fileName + pin[i] + " > /sys/devices/bone_capemgr.9/slots").c_str());
		else
			return false;
	}

	inited = true;

	return true;
}

string PWM::itos(int i)
{
	stringstream ss;
	string s;

	ss<<i;
	ss>>s;

	return s;
}

void PWM::SetValue(int num, string var, int value)
{
	if(!inited) return;

	string path = "/sys/class/pwm/";
	
	system(("echo " + itos(value) + " > "path + '/' + pwm[num] + '/' + var).c_str());

	return;
}


void PWM::GetValue(int num, string var, int& value)
{
	if(!inited) return;

	string path = "/sys/class/pwm/";
	
	fstream fs(path + '/' + pwm[num] + '/' + var, fstream::in | fstream::out);

	fs>>value;

	fs.close();

	return;
}