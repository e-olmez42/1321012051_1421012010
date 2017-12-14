#include <LiquidCrystal.h>  //LCD ekran kütüphanesi
#include <dht11.h>   //DHT11 Sıcaklık ve Nem Sensörü Kütüphanesi
#define DHT11PIN 36 // DHT11PIN olarak Dijital 36"yi belirliyoruz.
dht11 DHT11; //DHT11 nesnesi

LiquidCrystal lcd(A0, A1, A2, A3, A4, A5);  //LCD Ekran pin girişleri.

//Sıcaklık Ledleri tanımlandı.
int sicaklikLedGreen = 32;
int sicaklikLedRed = 30;
int sicaklikBuzzer = 28;

//Gaz sensoru için sensor data portunun tanımlanması
int analogGaz = A7;  //Gaz sensörünün sağlıklı ölçümler yapabilmesi için analog olarak ölçüm yapması gerekir.
double okunanGaz = 0;
int gazLedGreen = 40;
int gazLedRed = 38;
int gazBuzzer = 42;

//Motor sürücü kullanabilmek için gereken pin tanımlamaları
const int in1 = 46;     
const int in2 = 48;     
const int in3 = 50;
const int in4 = 52;



void setup()
{
  Serial.begin(9600); //Seri port ekranının aktif edilmesi.
  lcd.begin(16, 2);  //LCD ekranın aktif edilmesi.
  sicaklikNemSetup(); //DHT11 - Sıcaklık ve Nem Sensörüne bağlı olan led ve buzzer'ın kullanıma sunulması.
  gazSetup();
  motorSetup();

}
void loop()
{
  sicaklikNem();
  gazSensoru();  

}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void sicaklikNemSetup()
{
  pinMode(sicaklikLedGreen, OUTPUT);
  pinMode(sicaklikLedRed, OUTPUT);
  pinMode(sicaklikBuzzer, OUTPUT);
  
}
void sicaklikNem()//DHT11 sıcaklık ve nem sensörünün görevleri
{
  //Sensörün okunup okunmadığını konrol ediyoruz.
  // chk 0 ise sorunsuz okunuyor demektir. Sorun yaşarsanız
  // chk değerini serial monitörde yazdırıp kontrol edebilirsiniz.

  int chk = DHT11.read(DHT11PIN);

  //Sensörden gelen veri lcd ekrana basılıyor.
  lcd.clear();
  lcd.print("Nem : %");
  lcd.print((float)DHT11.humidity, 0);
  lcd.setCursor(0, 1);

  lcd.print("Derece : ");
  lcd.print((float)DHT11.temperature, 0);
  lcd.print((char)223); //Derece işaretini yapmak için.
  lcd.print("C"); //Celcius işaretini yapmak için.
  lcd.setCursor(0, 1);
  String sicaklik, nem, ikisi;

  //Eğer belirlenmiş olan sıcaklıkların üstüne çıkarsa ledi yakıyor, altında kalırsa led sönüyor.
  if ((float)DHT11.temperature >= 25 || (float)DHT11.humidity >= 50)
  {
    sicaklik = ((int)DHT11.temperature);
    nem = ((int)DHT11.humidity);
    ikisi = sicaklik+""+nem;
    
    
    Serial.println(ikisi);//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    digitalWrite(sicaklikLedGreen, LOW);
    digitalWrite(sicaklikLedRed, HIGH);
    sicaklikMotorAc();

    lcd.clear();
  lcd.print("Nem : %");
  lcd.print((float)DHT11.humidity, 0);
  lcd.setCursor(0, 1);

  lcd.print("Derece : ");
  lcd.print((float)DHT11.temperature, 0);
  lcd.print((char)223); //Derece işaretini yapmak için.
  lcd.print("C"); //Celcius işaretini yapmak için.
  lcd.setCursor(0, 1);

digitalWrite(sicaklikBuzzer, HIGH);
    delay(250);
    digitalWrite(sicaklikBuzzer, LOW);
    delay(250);
    digitalWrite(sicaklikBuzzer, HIGH);
    delay(250);
    digitalWrite(sicaklikBuzzer, LOW);
    
  }
  else if ((float)DHT11.temperature < 24 || (float)DHT11.humidity < 50)
  {
    //Serial.println((float)DHT11.temperature,0);//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    sicaklik = ((int)DHT11.temperature);
    //Serial.println((float)DHT11.humidity,0);//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    nem = ((int)DHT11.humidity);
    ikisi = sicaklik+""+nem;
    
    
    Serial.println(ikisi);
    digitalWrite(sicaklikLedGreen, HIGH);
    digitalWrite(sicaklikLedRed, LOW);
   sicaklikMotorKapat();

  }

  // Her 5 snde bir ölçüm yapacak ve yeni değerleri lcd ekrana basacak.
  delay(5000);
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////GAZ SENSÖRÜ///////////////////////////////////////////////////////////////////////////////
void gazSetup()
{
  pinMode(gazLedGreen, OUTPUT);
  pinMode(gazLedRed, OUTPUT);
  pinMode(gazBuzzer, OUTPUT);
  
}
void gazSensoru()
{
  okunanGaz = analogRead(analogGaz);  //Sensörden gaz değeri okunuyor.
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Gaz :");
  lcd.print(" ");
  lcd.print("%");
  lcd.print(okunanGaz / 10000); //Gaz çözünürlüğü hesaplanıp ekrana basılıyor-ppm değer-
  lcd.setCursor(0, 1);
  lcd.print("PPM :");
  lcd.print(" ");
  lcd.print(okunanGaz, 0);
  lcd.print(" ");
  lcd.print("ppm");


  if (okunanGaz >= 500) //Okunan gaz değerinin tehlike arz ettiği değerden fazla olması
  {
    
    Serial.println(okunanGaz,0);////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    gazMotorAc();
    digitalWrite(gazLedRed, HIGH);
    digitalWrite(gazLedGreen, LOW);
    digitalWrite(gazBuzzer, HIGH);
    delay(350);
    digitalWrite(gazBuzzer, LOW);
    delay(50);
    digitalWrite(gazBuzzer, HIGH);
    delay(350);
    digitalWrite(gazBuzzer, LOW);
    delay(50);
digitalWrite(gazBuzzer, HIGH);
    delay(350);
    digitalWrite(gazBuzzer, LOW);
    delay(50);
    digitalWrite(gazBuzzer, HIGH);
    delay(350);
    digitalWrite(gazBuzzer, LOW);
    
lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Gaz :");
  lcd.print(" ");
  lcd.print("%");
  lcd.print(okunanGaz / 10000); //Gaz çözünürlüğü hesaplanıp ekrana basılıyor-ppm değer-
  lcd.setCursor(0, 1);
  lcd.print("PPM :");
  lcd.print(" ");
  lcd.print(okunanGaz, 0);
  lcd.print(" ");
  lcd.print("ppm");

//    lcd.clear();
//    lcd.setCursor(1, 0);
//    lcd.print("GAZ YOGUNLUGU");
//    lcd.setCursor(3, 1);
//    lcd.print("YUKSELDI !");
    
    
  }

  else  //Gaz durumu normalse
  {
    
    Serial.println(okunanGaz,0);////////////////////////////////////////////////////////////////////////////////////////////////////////
    gazMotorKapat();
    digitalWrite(gazLedGreen, HIGH);
    digitalWrite(gazLedRed, LOW);
    


  }

  delay(4000);

}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////MOTOR SÜRÜCÜ/////////////////////////////////////////////////////////////////////////////////////
void motorSetup()
{
  pinMode(in1, OUTPUT);  
pinMode(in2, OUTPUT);  
pinMode(in3, OUTPUT);
pinMode(in4, OUTPUT);
}
void sicaklikMotorAc()
{
  digitalWrite(in1, HIGH);
  digitalWrite(in2,  LOW);
}
void sicaklikMotorKapat()
{
  digitalWrite(in1, LOW);
  digitalWrite(in2, LOW);
}
void gazMotorAc()
{
  digitalWrite(in3, HIGH);
  digitalWrite(in4,  LOW);
}
void gazMotorKapat()
{
  digitalWrite(in3, LOW);
  digitalWrite(in4, LOW);
}


