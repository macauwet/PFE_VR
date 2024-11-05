/* FSR testing sketch. 
 
Connect one end of FSR to 5V, the other end to Analog 0.
Then connect one end of a 10K resistor from Analog 0 to ground
Connect LED from pin 11 through a resistor to ground 
 
For more information see www.ladyada.net/learn/sensors/fsr.html */
 
int fsrAnalogPin = 0; // FSR is connected to analog 0
int fsrReading;      // the analog reading from the FSR resistor divider
int etat;


void setup() {
  Serial.begin(115200);   // We'll send debugging information via the Serial monitor
  pinMode(A0, INPUT);
  pinMode(LED_BUILTIN, OUTPUT);

}

void loop() {
  // put your main code here, to run repeatedly:
  handleSerial();
  switch(etat)
  {
    case 0:
      fsrReading =  analogRead(A0);
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
      Serial.println((int)fsrReading);
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
