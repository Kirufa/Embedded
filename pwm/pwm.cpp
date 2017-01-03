#include <fstream>
#include <cstdlib>
#include <string>
#include <sstream>
#include <vector>

#include "pwm.h"

PWM::PWM()
{
	inited = false;

	pwm[0] = string("pwm0");
	pwm[1] = string("pwm3");
	pwm[2] = string("pwm5");
	pwm[3] = string("pwm2");

	pin[0] = string("P9_22");
	pin[1] = string("P9_14");
	pin[2] = string("P8_19");
	pin[3] = string("P9_42");
}




bool PWM::findString(vector<string> vs,string str)
{
	for(int i = 0; i != vs.size(); ++i)
	{
		//puts(tmp.c_str());

		if(vs[i].find(str) != string::npos)
			return true;
	}

	return false;

}



bool PWM::Initialize()
{
	inited = true;

	fstream fs("/sys/devices/bone_capemgr.9/slots",fstream::in | fstream::out);
	
	string tmp;
	vector<string> vs;

	while(fs>>tmp)
		vs.push_back(tmp);
	
	fs.close();

	if(!findString(vs, "am33xx_pwm"))
		system("sudo echo am33xx_pwm > /sys/devices/bone_capemgr.9/slots");
	
	string fileName = "bone_pwm_";

	for(int i = 0; i != 4; ++i)
	{
		if(!findString(vs ,fileName + pin[i]))
			system(("sudo echo " + fileName + pin[i] + " > /sys/devices/bone_capemgr.9/slots").c_str());
	
	}

	fs.open("/sys/devices/bone_capemgr.9/slots",fstream::in | fstream::out);

	vs.clear();

	while(fs>>tmp)
		vs.push_back(tmp);

	if(!findString(vs, "am33xx_pwm"))
		inited = false;

	for(int i = 0; i != 4; ++i)
	{
		if(!findString(vs ,fileName + pin[i]))
		{
			inited = false;
			break;
		}
	}

	return inited;
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
	
	system(("echo " + itos(value) + " > " + path + '/' + pwm[num] + '/' + var).c_str());

	return;
}


void PWM::GetValue(int num, string var, int& value)
{
	if(!inited) return;

	string path = "/sys/class/pwm/";
	
	fstream fs((path + '/' + pwm[num] + '/' + var).c_str(), fstream::in | fstream::out);

	fs>>value;

	fs.close();

	return;
}
