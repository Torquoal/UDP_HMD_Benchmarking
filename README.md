# UDP -  VR HMD Benchmarking Notes

## Observed Issues
Byte buffers being sent from the Arduino SparkFun ESP32 ThingPlus (setup tutorial here: https://learn.sparkfun.com/tutorials/esp32-thing-plus-hookup-guide/all) boards were experiencing delays and latency issues when received by the VR HMDs used in the Viajero experiments. 

These packets are sent every 10ms and are used to send current vehicle speed and rotational data via an Arduino (code here on UDP Broadcasting Branch: https://github.com/mark-mcg/Viajero_Arduino) to the VR HMD, which the Unity-built application (https://github.com/mark-mcg/Passenger-VR-AR-In-Car--internal- Branch - UDP Broadcasting) then uses this data to move the user through the Virtual Environment in a matching manner, to reduce the motion sickness caused by mismatched movement between real world and VE. 

The delays and latency issues caused jumps and pauses, creating movement mismatches and undermining this setup.

The current setup used UDP broadcasting from the board to a local wifi network on port 9000. Packets were received by the Unity code listening on port 9000. 

Interestingly, while Windows laptops receive the packets with almost no delays or issues, when a non-Windows laptop is added to the same local network the delays start to occur, even if that device is not actively listening. When those devices do listen, the delay is the same. This holds true for Android phones, iPhones, Macs and HMDs.

## Solutions Tried
Through thorough dummy testing of UDP broadcasting between the board and between computers using both Python UDP clients/servers and the Unity code, we determined that the Unity code is not responsible for the delay, and couldn't find any wifi settings to alter with our routers to fix it. 

I would recommend using a network monitor like **WireShark** to help observe packets and traffic when testing.

We decided to try UDP multicasting, rather than broadcasting, from the Arduino side.


## Current Solution

The current solution is shown in the example Arduino Sketch udp multicast, which can be compiled and uploaded to the boards just using the Arduino IDE. You may need to add packages for the ESP32 boards if you are having issues with compilation. 

Rather than use UDP broadcast, we use UDP multicasting. However, when we used this with multicast IPs (224.0.0.0 - 239.255.255.255) the same delays occurred, which may be due to router bottlenecking. 

When we used UDP multicast but then **created packets specifically addressed to the Headset's IP address**, however, we observed very few delays, which should ensure smooth in-car motion if implemented to the main Arduino codebase. 

This looks like this

    
    [the rest of the code (see Sketch)]
    
    WiFiUDP Udp;
    
    void setup(){

		[the rest of the code (see Sketch)]

	    // set up udp multicast to 
	    Udp.beginMulticast(IPAddress(239,1,2,3), 9000);
	    Serial.println("\n connected UDP multicast");
    }
    
    void loop(){
	    delay(10);
	    // packet address specifically to the HMD
	    Udp.beginPacket(IPAddress(192,168,8,113), 9000);
	    Udp.print("Hello World");
	    Udp.endPacket();
	}

As opposed to multicast examples that are more like this using AsyncUDP: https://github.com/espressif/arduino-esp32/blob/master/libraries/AsyncUDP/examples/AsyncUDPMulticastServer/AsyncUDPMulticastServer.ino)

Because it is addressed to a specific client IP (in this case the HMD's), however, this is currently a 1 - to - 1 solution. You can get the HMD IP from the client list of the router when it is connected. Performance benchmarking can be done using the simple Unity project which deploys a simple VE to a HMD that shows currently received packet intervals to the user and counts outliers (delays over 50ms). The included Python reciever script does the same thing. In future, it would be nice to get true multicasting working so that multiple HMDs can receive car movement data at the same time.

## Future Directions
First, we need to implement the IP-addressed multicast packets to the main Arduino codebase 

We believe the delays are inherent to the board, the router, or a combination of the two, so here are some future directions.

- Investigate how we can improve the performance of the multicasting to general multicast addresses (e.g. 239.1.2.3), as per examples like this (https://github.com/espressif/arduino-esp32/blob/master/libraries/AsyncUDP/examples/AsyncUDPMulticastServer/AsyncUDPMulticastServer.ino).
This may be due to router bottlenecking: 
([https://superuser.com/questions/1287485/udp-broadcasting-not-working-on-some-routers](https://superuser.com/questions/1287485/udp-broadcasting-not-working-on-some-routers), [https://networkengineering.stackexchange.com/questions/36450/can-i-truly-multicast-over-wifi](https://networkengineering.stackexchange.com/questions/36450/can-i-truly-multicast-over-wifi))
In this case, try to find specific routers which are known to support local multicasting without delay. 

- Could also investigate using an ethernet shield with the board to cut out part of the process which might be causing delays.

- Can always keep trying with the ASync udp style example and see if there is more to be explored there too. 

- Given that we only achieved low latency with UDP multicasting when using packets with a single specified address, it would also be interesting to see if the same low latency performance can be achieved by 1-1 TCP. Can try a simple TCP arduino sketch and a TCP receiver script in the Unity project to test this with the headset. This obviously wouldn't solve the eventual need for multicasting though.

Hopefully this is helpful, I took on this exploration without a huge amount of networking experience so there may be more complicated solutions to explore, I stuck primarily to simple approaches so I could narrow down the possible problem.
