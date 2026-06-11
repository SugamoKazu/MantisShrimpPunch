#include <BluetoothSerial.h>
#include <ESP32Servo.h>
#include <math.h>


#define THRESHOLD 0.5
#define MAX_TIME 3000
#define LOG_SCALER 1.0 / log(10)

#define pres_pin 26
#define LServo_pin 5 
#define RServo_pin 4
#define EtoMrelayR 33
#define EtoMrelayL 32
#define StoMrelayR 13
#define StoMrelayL 14
#define PMAX 1500
#define PMIN 1000

BluetoothSerial SerialBT;
Servo Lservo;
Servo Rservo;

char c = 'D'; //敵に命中したか否か //'H'(Hit), 'M'(Miss), 'D'(Default)
int angleP = 0;
int t = 0;
int sensorValue = 0;
int  correctionValue = 0;
static int preValue;

const int SENSOR_MAX = 4095;
const int SENSOR_MIN = 0;
unsigned long startTime = 0;

void setup() {
  //リレーピン初期化
  pinMode(EtoMrelayR,OUTPUT);
  pinMode(EtoMrelayL,OUTPUT);
  pinMode(StoMrelayR,OUTPUT);
  pinMode(StoMrelayL,OUTPUT);
  digitalWrite(EtoMrelayR,LOW);
  digitalWrite(EtoMrelayL,LOW);
  digitalWrite(StoMrelayR,LOW);
  digitalWrite(StoMrelayL,LOW);

  //モーターピン初期化
  delay(3000);
  Lservo.setPeriodHertz(50);
  Rservo.setPeriodHertz(50);
  Lservo.attach(LServo_pin,PMIN,PMAX);
  Rservo.attach(RServo_pin,PMIN,PMAX);

  //Bluetooth通信の初期化
  Serial.begin(115200);
  SerialBT.begin("Syakote_Left");
  
  //モーターのパルスの初期化
  Calibrate();
}

// void loopの{}で囲われた箇所は、電源オフまたはリセットボタンを押さない限り永久に繰り返されます。
void loop() {
  Lservo.writeMicroseconds(angleP);
  Rservo.writeMicroseconds(angleP);
  sensorValue = analogRead(pres_pin); //0~4095
  correctionValue = calc_correction_presValue(sensorValue);
  SerialBT.println(correctionValue);
  //Serial.println(correctionValue);
  
  recieve_data();


  switch (c){
    case 'H':
      short_brake();
      //motor_stop();
      angleP = PMIN;
      break;
    case 'M':
      motor_stop(correctionValue);
      angleP = PMIN;
      break;
    case 'D':
      digitalWrite(StoMrelayR,LOW);
      digitalWrite(StoMrelayL,LOW);
      digitalWrite(EtoMrelayR,LOW);
      digitalWrite(EtoMrelayL,LOW);
      break;
    case 'S':
    /*
      while(true){
        recieve_data();
        if(c != 'S') break;
        short_brake();
      }
    */
      angleP = PMIN;
      break;
  }  
  preValue = angleP;
  if(sensorValue != 4095){
    c = 'D';
  }

  angleP = map(correctionValue,0,4095,PMAX,PMIN); // 圧力値に基づく回転数
  if(angleP < preValue) angleP = preValue; //常に右肩上がり

  delay(30);
}

void recieve_data(){
  // 命中判定を受け取る
  if (SerialBT.available() > 0) {
    c = SerialBT.read();
    Serial.printf("Received Command: %c\n", c);
    //c = 'H';
  }
}

// モーターを停止
void short_brake(){
  digitalWrite(EtoMrelayR,HIGH);
  digitalWrite(StoMrelayR,HIGH);
  digitalWrite(EtoMrelayL,HIGH);
  digitalWrite(StoMrelayL,HIGH);
  Serial.println("急制動");
}

void motor_stop(int correctionValue){
    Serial.println("停止");
    t = angleP;
    do{
      Serial.println(t);
      t -= 30;
      Lservo.writeMicroseconds(t);
      Rservo.writeMicroseconds(t);
      delay(100);   
      if(correctionValue != 4095){
        break;
      }
    }while(t >= 1000);
    c = 'D';
}

int calc_correction_presValue(int sensorValue){
  int calcValue;
  //4095-0を0-1に変換
  float x_prime = (float)(SENSOR_MAX - sensorValue) / (float)SENSOR_MAX;  

  //0-1の値をy=x^100に代入
  float y = constrain(pow(x_prime,100.0),0.0f,1.0f);

  if(y <= THRESHOLD){
    //yが低圧
    startTime = 0;
    calcValue = (1.0 - y) * 4095;
  }else{
    if(startTime == 0){
      startTime = millis();
    }
    unsigned long durationTime = millis() - startTime;
    float t_prime = constrain((float)durationTime / (float)MAX_TIME,0.0f,1.0f);
    float y_prime = constrain((log(t_prime * 9.0f + 1.0f) * LOG_SCALER),0.0f,1.0f);
    calcValue = (1 - (THRESHOLD + y_prime * (1 - THRESHOLD))) * 4095;
  }

  return (int)calcValue;
}

void Calibrate(){
  // 左のESCだけ
  Lservo.writeMicroseconds(PMAX);
  delay(2000);
  Lservo.writeMicroseconds(PMIN);
  delay(2000);

  // 右のESCだけ
  Rservo.writeMicroseconds(PMAX);
  delay(2000);
  Rservo.writeMicroseconds(PMIN);
  delay(2000);
}
