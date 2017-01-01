#include <iostream>
#include <cstdio>
#include <sstream>
#include <string>
#include <fstream>
#include <cstdlib>

using namespace std;


bool findString(fstream& fs,string str)
{

		string tmp;

		while(fs>>tmp)
		{	

				//		puts(tmp.c_str());

				if(tmp.find(str) != string::npos)
						return true;

		}

		return false;

}


int main(int argc, char* argv[])
{   
		fstream fs("/sys/devices/bone_capemgr.9/slots",fstream::in | fstream::out);


		if(argc != 3)
		{

				if(!findString(fs, "am33xx_pwm"))
						system("sudo echo am33xx_pwm > /sys/devices/bone_capemgr.9/slots");
		}
		else
		{   

				//		printf("else\n");

				string n1(argv[1]);
				string n2(argv[2]);
				string fileName = "bone_pwm_P" + n1 + "_" + n2;

				if(!findString(fs , fileName))
						system(("sudo echo " + fileName + " > /sys/devices/bone_capemgr.9/slots").c_str());
		}



		fs.close();

		return 0;
}


