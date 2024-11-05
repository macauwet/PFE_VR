/* FSR testing sketch. 
 
Connect one end of FSR to power, the other end to Analog 0.
Then connect one end of a 10K resistor from Analog 0 to ground 
 
For more information see www.ladyada.net/learn/sensors/fsr.html */
 
long fsrReading;     // the analog reading from the FSR resistor divider
int fsrReading0;
int fsrReading1;
int fsrVoltage;     // the analog reading converted to voltage
int etat;
unsigned long fsrResistance;  // The voltage converted to resistance, can be very big so make "long"


void setup(void) {
  Serial.begin(115200);   // We'll send debugging information via the Serial monitor
  pinMode(A0, INPUT);
  pinMode(A1, INPUT);
  pinMode(LED_BUILTIN, OUTPUT);
}
 
void loop() {
  handleSerial();
  switch(etat)
  {
    case 0:
      fsrReading0 = analogRead(A0);  
      fsrReading1 = analogRead(A1);

      if(fsrReading0 > fsrReading1)
      {
        fsrReading = fsrReading0;
      }
      else
      {
        fsrReading = fsrReading1;
      }
      // analog voltage reading ranges from about 0 to 1023 which maps to 0V to 5V (= 5000mV)
      fsrVoltage = map(fsrReading, 0, 1023, 0, 5000);
      if(fsrVoltage != 0)
      {
        fsrResistance = 5000 - fsrVoltage;     // fsrVoltage is in millivolts so 5V = 5000mV
        fsrResistance *= 10000;                // 10K resistor
        fsrResistance /= fsrVoltage;           // en Ohms
      }
      else
      {
        fsrResistance = 0;
      }
      
      
      if(fsrReading >= 20)
      {
        digitalWrite(LED_BUILTIN, HIGH);
      }
      else
      {
        digitalWrite(LED_BUILTIN, LOW);    
      }
      
      break;

    case 1:
      Serial.println((int)fsrResistance);
      etat = 0;
      break;
  }
  
}

void handleSerial() {
  if (Serial.available() > 0) {
    char incomingCharacter = Serial.read();
    switch (incomingCharacter)
    {
      case 'a':
        etat = 1;
        break;

      default:
        break;
    }
  }
}
