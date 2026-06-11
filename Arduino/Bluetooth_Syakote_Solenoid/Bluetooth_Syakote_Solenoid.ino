#include <BluetoothSerial.h>

// --- 定数定義 ---
BluetoothSerial SerialBT;

// リレーを接続するピン番号
const int RELAY_PIN_1 = 19;
const int RELAY_PIN_2 = 5;
const int RELAY_PIN_3 = 4;
const int RELAY_PIN_4 = 15;

const int RELAY_PIN_5 = 33;
const int RELAY_PIN_6 = 26;
const int RELAY_PIN_7 = 14;
const int RELAY_PIN_8 = 13;

int t;

// --- 初期設定 ---
// プログラム起動時に一度だけ実行される関数
void setup() {
  // Bluetoothシリアル通信を開始 (通信速度: 115200 bps)
  Serial.begin(115200);
  SerialBT.begin("SolenoidManager");

  // シリアルモニタに準備完了の文字を表示
  SerialBT.println("準備ができました。");

  // 各ピンを出力モードに設定
  pinMode(RELAY_PIN_1, OUTPUT);
  pinMode(RELAY_PIN_2, OUTPUT);
  pinMode(RELAY_PIN_3, OUTPUT);
  pinMode(RELAY_PIN_4, OUTPUT);
  pinMode(RELAY_PIN_5, OUTPUT);
  pinMode(RELAY_PIN_6, OUTPUT);
  pinMode(RELAY_PIN_7, OUTPUT);
  pinMode(RELAY_PIN_8, OUTPUT);

  // リレー初期状態
  digitalWrite(RELAY_PIN_1, LOW);
  digitalWrite(RELAY_PIN_2, LOW);
  digitalWrite(RELAY_PIN_3, LOW);
  digitalWrite(RELAY_PIN_4, LOW);
  digitalWrite(RELAY_PIN_5, LOW);
  digitalWrite(RELAY_PIN_6, LOW);
  digitalWrite(RELAY_PIN_7, LOW);
  digitalWrite(RELAY_PIN_8, LOW);
}

// --- メインループ ---
// setup()の後、繰り返し実行される関数
void loop() {
  // PCからシリアルデータが送信されてきたか確認
  if (SerialBT.available() > 0) {
    // 送られてきたデータを1文字読み込む
    char receivedChar = SerialBT.read();

    // 読み込んだ文字が'R'だったら
    if (receivedChar == 'R') {
      Serial.println("Rを検出。リレーシーケンスを開始します。");
      R_activateRelaySequence(30); // 左手用リレー制御の関数を呼び出す

    // 読み込んだ文字がスペース'L'だったら  
    }else if (receivedChar == 'L') {
      Serial.println("Lを検出。リレーシーケンスを開始します。");
      L_activateRelaySequence(30); // 右手用リレー制御の関数を呼び出す
    // 読み込んだ文字が'M'だったら
    }else if (receivedChar == 'M') {
      Serial.println("Mを検出。リレーシーケンスを開始します。");
      M_activateRelaySequence(30); // 左手座位用リレー制御の関数を呼び出す

    // 読み込んだ文字が'H'だったら
    }else if (receivedChar == 'H') {
      Serial.println("Hを検出。リレーシーケンスを開始します。");
      H_activateRelaySequence(30); // 右手座位用リレー制御の関数を呼び出す
    }
  }
}

// --- 右手用リレー制御シーケンスを実行する関数 ---
void R_activateRelaySequence(int t) {
  // --- リレーを順番にONにする ---
  digitalWrite(RELAY_PIN_4, HIGH); // 1つ目のリレーON
  delay(100);
  digitalWrite(RELAY_PIN_4, LOW); // リレー1をOFF (通電時間: 30+30 = 60msec)

  delay(150);
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
}

// --- 左手用リレー制御シーケンスを実行する関数 ---
void L_activateRelaySequence(int t) {
  // --- リレーを順番にONにする ---
  digitalWrite(RELAY_PIN_8, HIGH); // 1つ目のリレーON
  delay(100);
  digitalWrite(RELAY_PIN_8, LOW); // リレー1をOFF (通電時間: 30+30 = 60msec)
  
  delay(150);                                                                                                                                                                           
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
  digitalWrite(RELAY_PIN_7, LOW); // リレー4をOFF (通電時間: 140+30+30 = 200msec
  Serial.println("リレーシーケンスが完了しました。");
}

void M_activateRelaySequence(int t) {
  // --- リレーを順番にONにする ---
  digitalWrite(RELAY_PIN_4, HIGH); // 1つ目のリレーON
  delay(100);
  digitalWrite(RELAY_PIN_4, LOW); // リレー1をOFF (通電時間: 30+30 = 60msec)

  delay(150);
  digitalWrite(RELAY_PIN_3, HIGH); // 4つ目のリレーON
  delay(t);                     // 30ms待機
  digitalWrite(RELAY_PIN_2, HIGH); // 3つ目のリレーON
  delay(t);                     // 30ms待機
  digitalWrite(RELAY_PIN_1, HIGH); // 2つ目のリレーON

  // --- 各リレーの通電時間が200msecになるように、順番にOFFにする ---  
  delay(140);
  digitalWrite(RELAY_PIN_3, LOW); // リレー4をOFF (通電時間: 30+30+140 = 200msec)
  delay(t);
  digitalWrite(RELAY_PIN_2, LOW); // リレー3をOFF (通電時間: 30+140+30 = 200msec)
  delay(t);
  digitalWrite(RELAY_PIN_1, LOW); // リレー2をOFF (通電時間: 140+30+30 = 200msec)
  Serial.println("リレーシーケンスが完了しました。");
}



void H_activateRelaySequence(int t) {
  // --- リレーを順番にONにする ---
  digitalWrite(RELAY_PIN_8, HIGH); // 1つ目のリレーON
  delay(100);
  digitalWrite(RELAY_PIN_8, LOW); // リレー1をOFF (通電時間: 30+30 = 60msec)
  
  delay(150);                                                                                                                                                                           
  digitalWrite(RELAY_PIN_7, HIGH); // 4つ目のリレーON
  delay(t);                     // 30ms待機
  digitalWrite(RELAY_PIN_6, HIGH); // 3つ目のリレーON
  delay(t);                     // 30ms待機
  digitalWrite(RELAY_PIN_5, HIGH); // 2つ目のリレーON

  // --- 各リレーの通電時間が200msecになるように、順番にOFFにする ---  
  delay(140);
  digitalWrite(RELAY_PIN_7, LOW); // リレー4をOFF (通電時間: 30+30+140 = 200msec)
  delay(t);
  digitalWrite(RELAY_PIN_6, LOW); // リレー3をOFF (通電時間: 30+140+30 = 200msec)
  delay(t);
  digitalWrite(RELAY_PIN_5, LOW); // リレー2をOFF (通電時間: 140+30+30 = 200msec)
  Serial.println("リレーシーケンスが完了しました。");
}
