#include "WiFi.h"
#include "AsyncUDP.h"

const char * ssid = "CarRouter2.4G";
const char * password = "viajero2023";
const char * message = "Hello Worm";
int port = 9000;

AsyncUDP udp;
WiFiUDP Udp;

void setup()
{
    Serial.begin(115200);
    WiFi.mode(WIFI_STA);
    WiFi.begin(ssid, password);
    if (WiFi.waitForConnectResult() != WL_CONNECTED) {
        Serial.println("WiFi Failed");
        while(1) {
            delay(1000);
        }
    } else {
      Serial.println("Connected to WiFi");
    }
    //udp.listen(9000);//sets port from which to produce packets

    // set up udp multicast to 
    Udp.beginMulticast(IPAddress(239,1,2,3), 9000);
    Serial.println("\n connected UDP multicast");

}

void loop()
{
    delay(10);
    //Send multicast - works with udp.listen(9000) but cant recieve it, some bug I havent found yet
    //udp.print(message);

    // this works similarly the old board setupm, requires setting a port to udp listen to and is smooth to windows, lags when other devices added
    //udp.broadcastTo("Hello Worm", 9000);

    
    //the ip address here needs to match the headset/recieving device exactly for them to recieve the data smoothly, so this is a 1-1 solution. 
    //If you can find a way of recieving general multicasts (without specifiying an IP) can measure performance on that using multicast Versus broadcast
    Udp.beginPacket(IPAddress(192,168,8,113), 9000);
    Udp.print("Hello Worm");
    Udp.endPacket();

    
    Serial.println("message sent");
}
