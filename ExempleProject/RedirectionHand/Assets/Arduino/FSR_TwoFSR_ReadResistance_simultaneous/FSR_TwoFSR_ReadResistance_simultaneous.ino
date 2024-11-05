/* FSR testing sketch. 
 
Connect one end of FSR to power, the other end to Analog 0.
Then connect one end of a 10K resistor from Analog 0 to ground 
 
For more information see www.ladyada.net/learn/sensors/fsr.html */
 
long fsrReading;     // the analog reading from the FSR resistor divider
int fsrReading0;
int fsrReading1;
int fsrVoltage;     // the analog reading converted to voltage

int fsrVoltage2;
float force0, force1;
int etat;
unsigned long fsrResistance, fsrResistance2, fsrResistance3;  // The voltage converted to resistance, can be very big so make "long"


void setup(void) {
  Serial.begin(115200);   // We'll send debugging information via the Serial monitor
  pinMode(A0, INPUT);
  pinMode(A1, INPUT);
  pinMode(LED_BUILTIN, OUTPUT);
}
 
void loop() {

      fsrReading0 = analogRead(A0);  
      fsrReading1 = analogRead(A3);


      // analog voltage reading ranges from about 0 to 1023 which maps to 0V to 5V (= 5000mV)
      fsrVoltage = map(fsrReading0, 0, 1023, 0, 5000);
      if(fsrVoltage != 0)
      {
        fsrResistance = 5000 - fsrVoltage;     // fsrVoltage is in millivolts so 5V = 5000mV
        fsrResistance *= 10000;                // 10K resistor
        fsrResistance /= fsrVoltage;           // en Ohms
        force0 = 6570.2*pow(fsrResistance,-0.742);
      }
      else
      {
        fsrResistance = 0;
        force0 = 0;
      }


      fsrVoltage2 = map(fsrReading1, 0, 1023, 0, 5000);
      if(fsrVoltage2 != 0)
      {
        fsrResistance2 = 5000 - fsrVoltage2;     // fsrVoltage is in millivolts so 5V = 5000mV
        fsrResistance2 *= 10000;                // 10K resistor
        fsrResistance2 /= fsrVoltage2;           // en Ohms
//        fsrResistance3 = 2033*exp(9*fsrResistance2*pow(10, -5));
//        force1 = 6570.2*pow(fsrResistance3,-0.742);
//        if(fsrResistance2 < 20000)
//        {
//          force1 = 156.3*exp(fsrResistance2 * (-3) * pow(10,-4));
          force1 = 0.1388*exp(3.77*(19.77*pow(fsrResistance2, -0.301)));

//        }
//        else
//        {
//          force1 = 19.77*pow(fsrResistance2, -0.301);
//        }
//        
      }
      else
      {
        fsrResistance2 = 0;
        fsrResistance3 = 0;
        force1 = 0;
      }

      
      
      if(fsrReading >= 20)
      {
        digitalWrite(LED_BUILTIN, HIGH);
      }
      else
      {
        digitalWrite(LED_BUILTIN, LOW);
      }
      
//      Serial.print("Resistance1 : ");
//      Serial.print((int)fsrResistance);
//////      Serial.print(" ; Resistance2 : ");
//      Serial.print(" ; ");
//////
//      Serial.println((int)fsrResistance2);
////      Serial.print(" ; ");
//
//      Serial.println((int)fsrResistance3);

//      Serial.print("Force1 : ");

      Serial.print(force0);
      Serial.print(" ; ");
//      Serial.print(" ; Force2 : ");
      Serial.println(force1);
//      Serial.print(" ; ");
//      Serial.println((int)fsrResistance2);
delay(20);
  
  
}
