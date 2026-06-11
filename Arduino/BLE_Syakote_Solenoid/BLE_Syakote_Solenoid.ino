#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLEUtils.h>
#include <BLE2902.h>


// ★★★ 1. あなた専用のUUIDに置き換えてください (3台とも同じUUIDを使用) ★★★
#define SERVICE_UUID        "9012ef44-2588-4b6a-98ad-fd0f60f3d141"
#define CHARACTERISTIC_UUID "0041c94c-fe80-4009-8d95-8bf60c1b01e0"
#define COMMAND_CHARACTERISTIC_UUID "026186d9-4f21-42ef-9b94-2ffb2ead7a58" // Unity -> ESP32 (コマンド)
// ★★★ 2. このESP32の固有の名前に設定してください ("Sensor-A", "Sensor-B", "Sensor-C") ★★★
const char* DEVICE_NAME = "Syakote_Solenoid";
// --- グローバル変数 ---
BLECharacteristic *pCharacteristic;
bool deviceConnected = false;

char receivedChar;
// リレーを接続するピン番号
const int RELAY_PIN_1 = 19;
const int RELAY_PIN_2 = 5;
const int RELAY_PIN_3 = 4;
const int RELAY_PIN_4 = 15;

const int RELAY_PIN_5 = 33;
const int RELAY_PIN_6 = 26;
const int RELAY_PIN_7 = 14;
const int RELAY_PIN_8 = 13;

// 座位シャコ用ピン
const int RELAY_PIN_9 = 22;
const int RELAY_PIN_10 = 21;

int t;

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
        receivedChar = value[0]; // 最初の1バイトをコマンドとして受け取る
        Serial.printf("Received Command: %c\n", receivedChar);
        // ★★★ 4. ここで受信したコマンドに応じた処理を記述します ★★★
      }
    }
};


// --- 初期設定 ---
// プログラム起動時に一度だけ実行される関数
void setup() {
  // Bluetoothシリアル通信を開始 (通信速度: 115200 bps)
  Serial.begin(115200);
  // BLEデバイスを初期化 (デバイス名を設定)

  // 各ピンを出力モードに設定
  pinMode(RELAY_PIN_1, OUTPUT);
  pinMode(RELAY_PIN_2, OUTPUT);
  pinMode(RELAY_PIN_3, OUTPUT);
  pinMode(RELAY_PIN_4, OUTPUT);
  pinMode(RELAY_PIN_5, OUTPUT);
  pinMode(RELAY_PIN_6, OUTPUT);
  pinMode(RELAY_PIN_7, OUTPUT);
  pinMode(RELAY_PIN_8, OUTPUT);
  pinMode(RELAY_PIN_9, OUTPUT);
  pinMode(RELAY_PIN_10, OUTPUT);

  // リレー初期状態
  digitalWrite(RELAY_PIN_1, LOW);
  digitalWrite(RELAY_PIN_2, LOW);
  digitalWrite(RELAY_PIN_3, LOW);
  digitalWrite(RELAY_PIN_4, LOW);
  digitalWrite(RELAY_PIN_5, LOW);
  digitalWrite(RELAY_PIN_6, LOW);
  digitalWrite(RELAY_PIN_7, LOW);
  digitalWrite(RELAY_PIN_8, LOW);
  digitalWrite(RELAY_PIN_9, LOW);
  digitalWrite(RELAY_PIN_10, LOW);
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

}

// --- メインループ ---
// setup()の後、繰り返し実行される関数
void loop() {
  if(deviceConnected){
    if (receivedChar == 'R') {
      Serial.println("Rを検出。リレーシーケンスを開始します。");
      R_activateRelaySequence(40); // 左手用リレー制御の関数を呼び出す

    // 読み込んだ文字がスペース'L'だったら  
    }else if (receivedChar == 'L') {
      Serial.println("Lを検出。リレーシーケンスを開始します。");
      L_activateRelaySequence(40); // 右手用リレー制御の関数を呼び出す

    // 読み込んだ文字がスペース'M'だったら  
    }else if(receivedChar == 'M'){
      Serial.println("Mを検出。リレーシーケンスを開始します。");
      M_activateRelaySequence(40); // 右手座位用リレー制御の関数を呼び出す

    // 読み込んだ文字がスペース'H'だったら  
    }else if(receivedChar == 'H'){
      Serial.println("Hを検出。リレーシーケンスを開始します。");
      H_activateRelaySequence(40); // 左手座位用リレー制御の関数を呼び出す
    }
  } else {
    // 切断されている場合は、アドバタイズを再開させる
    delay(500);
    BLEDevice::startAdvertising();
  }
}

// --- 右手用リレー制御シーケンスを実行する関数 ---
void R_activateRelaySequence(int t) {
  // --- リレーを順番にONにする ---
  digitalWrite(RELAY_PIN_4, HIGH); // 1つ目のリレーON
  delay(180);
  digitalWrite(RELAY_PIN_4, LOW); // リレー1をOFF (通電時間: 30+30 = 60msec)
  delay(70);
  digitalWrite(RELAY_PIN_1, HIGH); // 2つ目のリレーON
  delay(t);                     // 30ms待機
  digitalWrite(RELAY_PIN_2, HIGH); // 3つ目のリレーON
  delay(t);                     // 30ms待機
  digitalWrite(RELAY_PIN_3, HIGH); // 4つ目のリレーON
  // --- 各リレーの通電時間が200msecになるように、順番にOFFにする ---
  delay(140);
  digitalWrite(RELAY_PIN_1, LOW); // リレー2をOFF (通電時間: 30+30+140 = 200msec)
  delay(t);
  digitalWrite(RELAY_PIN_2, LOW); // リレー3をOFF (通電時間: 30+140+30 = 200msec)
  delay(t);
  digitalWrite(RELAY_PIN_3, LOW); // リレー4をOFF (通電時間: 140+30+30 = 200msec)
  Serial.println("リレーシーケンスが完了しました。");
  receivedChar = 'N';
}

// --- 左手用リレー制御シーケンスを実行する関数 ---
void L_activateRelaySequence(int t) {
  // --- リレーを順番にONにする ---
  digitalWrite(RELAY_PIN_8, HIGH); // 1つ目のリレーON
  delay(180);
  digitalWrite(RELAY_PIN_8, LOW); // リレー1をOFF (通電時間: 30+30 = 60msec)
  delay(70);                                                                                                                                                                           
  digitalWrite(RELAY_PIN_5, HIGH); // 2つ目のリレーON
  delay(t);                     // 30ms待機
  digitalWrite(RELAY_PIN_6, HIGH); // 3つ目のリレーON
  delay(t);                     // 30ms待機
  digitalWrite(RELAY_PIN_7, HIGH); // 4つ目のリレーON
  // --- 各リレーの通電時間が200msecになるように、順番にOFFにする ---
  delay(140);
  digitalWrite(RELAY_PIN_5, LOW); // リレー2をOFF (通電時間: 30+30+140 = 200msec)
  delay(t);
  digitalWrite(RELAY_PIN_6, LOW); // リレー3をOFF (通電時間: 30+140+30 = 200msec)
  delay(t);
  digitalWrite(RELAY_PIN_7, LOW); // リレー4をOFF (通電時間: 140+30+30 = 200msec)
  Serial.println("リレーシーケンスが完了しました。");
  receivedChar = 'N';
}

// --- 右手座位用リレー制御シーケンスを実行する関数 ---
void M_activateRelaySequence(int t) {
  // --- リレーを順番にONにする ---
  digitalWrite(RELAY_PIN_9, HIGH); // 1つ目のリレーON
  delay(180);
  digitalWrite(RELAY_PIN_9, LOW); // リレー1をOFF (通電時間: 30+30 = 60msec)
  delay(70);
  digitalWrite(RELAY_PIN_1, HIGH); // 2つ目のリレーON
  delay(t);                     // 30ms待機
  digitalWrite(RELAY_PIN_2, HIGH); // 3つ目のリレーON
  delay(t);                     // 30ms待機
  digitalWrite(RELAY_PIN_3, HIGH); // 4つ目のリレーON
  // --- 各リレーの通電時間が200msecになるように、順番にOFFにする ---
  delay(140);
  digitalWrite(RELAY_PIN_1, LOW); // リレー2をOFF (通電時間: 30+30+140 = 200msec)
  delay(t);
  digitalWrite(RELAY_PIN_2, LOW); // リレー3をOFF (通電時間: 30+140+30 = 200msec)
  delay(t);
  digitalWrite(RELAY_PIN_3, LOW); // リレー4をOFF (通電時間: 140+30+30 = 200msec)
  Serial.println("リレーシーケンスが完了しました。");
  receivedChar = 'N';
}

// --- 左手座位用リレー制御シーケンスを実行する関数 ---
void H_activateRelaySequence(int t) {
  // --- リレーを順番にONにする ---
  digitalWrite(RELAY_PIN_10, HIGH); // 1つ目のリレーON
  delay(180);
  digitalWrite(RELAY_PIN_10, LOW); // リレー1をOFF (通電時間: 30+30 = 60msec)
  delay(70);
  digitalWrite(RELAY_PIN_5, HIGH); // 2つ目のリレーON
  delay(t);                     // 30ms待機
  digitalWrite(RELAY_PIN_6, HIGH); // 3つ目のリレーON
  delay(t);                     // 30ms待機
  digitalWrite(RELAY_PIN_7, HIGH); // 4つ目のリレーON
  // --- 各リレーの通電時間が200msecになるように、順番にOFFにする ---
  delay(140);
  digitalWrite(RELAY_PIN_5, LOW); // リレー2をOFF (通電時間: 30+30+140 = 200msec)
  delay(t);
  digitalWrite(RELAY_PIN_6, LOW); // リレー3をOFF (通電時間: 30+140+30 = 200msec)
  delay(t);
  digitalWrite(RELAY_PIN_7, LOW); // リレー4をOFF (通電時間: 140+30+30 = 200msec)
  Serial.println("リレーシーケンスが完了しました。");
  receivedChar = 'N';
}
