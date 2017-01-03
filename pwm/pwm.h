#include <string>
#include <fstream>

using namespace std;

class PWM
{
public:
	PWM();

	bool Initialize();
	void SetValue(int, string, int);
	void GetValue(int, string, int&);

private:
	bool inited;
	bool findString(fstream&, string);
	string itos(int);

	string pwm[4];
	string pin[4];

//         front
//           0                
//        3     1 
//           2
//
//0:P9_22 pwm0
//1:P9_14 pwm3
//2:P8_19 pwm5
//3:P9_42 pwm2



};

