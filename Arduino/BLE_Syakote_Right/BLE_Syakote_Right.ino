#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLEUtils.h>
#include <BLE2902.h>
#include <ESP32Servo.h>
#include <math.h>


#define THRESHOLD 0.3
#define MAX_TIME 2000
#define LOG_SCALER 1.0 / log(10)

//モータピンの定義
#define pres_pin 26
#define LServo_pin 4 
#define RServo_pin 5
#define EtoMrelayR 32
#define EtoMrelayL 33
#define StoMrelayR 14
#define StoMrelayL 13
#define PMAX 1300
#define PMIN 1000
Servo Lservo;
Servo Rservo;


// ★★★ 1. あなた専用のUUIDに置き換えてください (3台とも同じUUIDを使用) ★★★
#define SERVICE_UUID        "9012ef44-2588-4b6a-98ad-fd0f60f3d141"
#define CHARACTERISTIC_UUID "0041c94c-fe80-4009-8d95-8bf60c1b01e0"
#define COMMAND_CHARACTERISTIC_UUID "026186d9-4f21-42ef-9b94-2ffb2ead7a58" // Unity -> ESP32 (コマンド)

// ★★★ 2. このESP32の固有の名前に設定してください ("Sensor-A", "Sensor-B", "Sensor-C") ★★★
const char* DEVICE_NAME = "Syakote_Right";

BLECharacteristic *pCharacteristic;
bool deviceConnected = false;
bool deviceActive = false;

//モーター制御変数の初期化
char c = 'P'; //敵に命中したか否か //'H'(Hit), 'M'(Miss), 'D'(Default)
int angleP = 0;
int t = 0;
int sensorValue = 0;
int  correctionValue = 0;
static int preValue;

const int SENSOR_MAX = 4095;
const int SENSOR_MIN = 0;
unsigned long startTime = 0;

// 接続/切断イベントを監視するクラス
class MyServerCallbacks: public BLEServerCallbacks {
    void onConnect(BLEServer* pServer) {
      deviceConnected = true;
      Serial.println("Device Connected");
    }

    void onDisconnect(BLEServer* pServer) {
      deviceConnected = false;
      Serial.println("Device Disconnected");
    }
};

// Unityからのコマンド受信を処理するクラス
class MyCharacteristicCallbacks: public BLECharacteristicCallbacks {
    void onWrite(BLECharacteristic *pCharacteristic) {
      // 1. データへのポインタ(getData)と長さ(getLength)を直接取得する
      uint8_t* dataPtr = pCharacteristic->getData();
      size_t length = pCharacteristic->getLength();

      // 2. 取得したポインタと長さから、安全にstd::stringを生成する
      std::string value((char*)dataPtr, length);

      if (value.length() > 0) {
        c = value[0]; // 最初の1バイトをコマンドとして受け取る
        Serial.printf("Received Command: %c\n", c);
      }
    }
};





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

void setup() {
  Serial.begin(115200);
  
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

  //モーターのパルスの初期化
  Calibrate();


  // BLEデバイスを初期化 (デバイス名を設定)
  BLEDevice::init(DEVICE_NAME);

  // BLEサーバーを作成
  BLEServer *pServer = BLEDevice::createServer();
  pServer->setCallbacks(new MyServerCallbacks());

  // サービスを作成
  BLEService *pService = pServer->createService(SERVICE_UUID);

  // (1) データ送信用キャラクタリスティック (ESP32 -> Unity)
  pCharacteristic = pService->createCharacteristic(
                      CHARACTERISTIC_UUID,
                      BLECharacteristic::PROPERTY_NOTIFY
                    );  
  pCharacteristic->addDescriptor(new BLE2902());

  // (2) コマンド受信用キャラクタリスティック (Unity -> ESP32)
  BLECharacteristic *pCommandCharacteristic = pService->createCharacteristic(
                                         COMMAND_CHARACTERISTIC_UUID,
                                         BLECharacteristic::PROPERTY_WRITE
                                       );
  pCommandCharacteristic->setCallbacks(new MyCharacteristicCallbacks());

  // サービスを開始
  pService->start();

  // アドバタイズ（宣伝）を開始
  BLEAdvertising *pAdvertising = BLEDevice::getAdvertising();
  pAdvertising->addServiceUUID(SERVICE_UUID);
  BLEDevice::startAdvertising();
  
  Serial.printf("'%s' is ready. Now advertising...\n", DEVICE_NAME);
  deviceActive = false;
}

void loop() {
  if(deviceConnected) {
    
    switch(c){
      case 'A':
        deviceActive = true;
        break;
      case 'P':
        deviceActive = false;
        pCharacteristic->setValue((uint8_t*)&SENSOR_MAX, sizeof(SENSOR_MAX));
        // 接続しているセントラル（Quest 3）に値を通知
        pCharacteristic->notify();
        break;
      default:
        break;

    }
    
    if (deviceActive) {
      Lservo.writeMicroseconds(angleP);
      Rservo.writeMicroseconds(angleP);
      sensorValue = analogRead(pres_pin); //0~4095
      correctionValue = calc_correction_presValue(sensorValue);
      // キャラクタリスティックの値を更新
      pCharacteristic->setValue((uint8_t*)&correctionValue, sizeof(correctionValue));
      // 接続しているセントラル（Quest 3）に値を通知
      pCharacteristic->notify();
      // シリアルモニタで送信値を確認 (デバッグ用)
      Serial.printf("Sent value: %d\n", correctionValue);
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
          angleP = PMIN;
          break;
      }
      if(sensorValue != 4095){
        c = 'D';
      }

      preValue = angleP;
      angleP = map(correctionValue,0,4095,PMAX,PMIN); // 圧力値に基づく回転数
      if(angleP < preValue) angleP = preValue; //常に右肩上がり

    } else {
      // 何もしない
      Lservo.writeMicroseconds(PMIN);
      Rservo.writeMicroseconds(PMIN);
    }  
  
  } else {
    Lservo.writeMicroseconds(PMIN);
    Rservo.writeMicroseconds(PMIN);
    // 切断されている場合は、アドバタイズを再開させる
    delay(500);
    BLEDevice::startAdvertising();
  }
  // 送信間隔を調整
  delay(30);
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