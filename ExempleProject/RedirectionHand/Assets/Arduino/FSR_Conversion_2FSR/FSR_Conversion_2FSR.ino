int fsrReading;     // the analog reading from the FSR resistor divider
int fsrReading0;
int fsrReading1;
int fsrVoltage;     // the analog reading converted to voltage
unsigned long fsrResistance;  // The voltage converted to resistance, can be very big so make "long"
unsigned long fsrConductance; 
long fsrForce;       // Finally, the resistance converted to force
int etat;

void setup() {
  Serial.begin(115200);   // We'll send debugging information via the Serial monitor
  pinMode(A0, INPUT);
  pinMode(A1, INPUT);
  pinMode(LED_BUILTIN, OUTPUT);

}
void loop() {
  // put your main code here, to run repeatedly:
  handleSerial();
  switch(etat)
  {
    case 0:
      fsrReading0 =  analogRead(A0);
      fsrReading1 = analogRead(A1);
      
      if(fsrReading0 >= 20)
      {
        digitalWrite(LED_BUILTIN, HIGH);
        fsrReading = fsrReading0;
      }
      else if(fsrReading1 > 20)
      {
        digitalWrite(LED_BUILTIN, HIGH);
        fsrReading = fsrReading1;
      }
      else
      {
        digitalWrite(LED_BUILTIN, LOW);
      }
      
      
        // analog voltage reading ranges from about 0 to 1023 which maps to 0V to 5V (= 5000mV)
      fsrVoltage = map(fsrReading, 0, 1023, 0, 5000);

      if (fsrVoltage == 0) {
      
      }
      else
      {
        // The voltage = Vcc * R / (R + FSR) where R = 10K and Vcc = 5V
        // so FSR = ((Vcc - V) * R) / V        yay math!
        fsrResistance = 5000 - fsrVoltage;     // fsrVoltage is in millivolts so 5V = 5000mV
        fsrResistance *= 10000;                // 10K resistor
        fsrResistance /= fsrVoltage;
        
        fsrConductance = 1000000;           // we measure in micromhos so 
        fsrConductance /= fsrResistance;
        
        // Use the two FSR guide graphs to approximate the force
        if (fsrConductance <= 1000) {
          fsrForce = fsrConductance / 80;
        } else {
          fsrForce = fsrConductance - 1000;
          fsrForce /= 30;
        }
      }
      delay(100);

      break;

    case 1:
      Serial.println((int)fsrForce);
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
