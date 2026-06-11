#include <ESP32Servo.h>

#define ESC_PIN 5
#define BTN1 14
#define BTN2 12

Servo esc;

bool flag = true;

void setup() {
  Serial.begin(9600);
  esc.setPeriodHertz(50);
  esc.attach(ESC_PIN,1000,2000); // PWM範囲設定（1000〜2000μs)

  pinMode(BTN1,INPUT_PULLUP);
  pinMode(BTN2,INPUT_PULLUP);
}

void loop() {
  if(digitalRead(BTN1) == LOW){
    flag = true;
    
  }else if(digitalRead(BTN2) == LOW){
    flag = false;
  }

  if(flag){
    esc.writeMicroseconds(2000);
    Serial.println(2000);
  }else{
    esc.writeMicroseconds(1000);
    Serial.println(1000);
  }

}
