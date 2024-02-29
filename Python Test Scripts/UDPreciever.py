import socket
import time

lastRecieveTime = 0
timeSinceLastPacket = 0
count = 0
total = 0
outliers = 0
outlier_value = 0
average = 0
outlier_threshold = 4

HOST = '0.0.0.0'
PORT = 9000

sock = socket.socket(socket.AF_INET, # Internet
                     socket.SOCK_DGRAM) # UDP
sock.bind((HOST, PORT))

lastRecieveTime = time.perf_counter() * 1000


# when data is recieved, calculate time since last time this happened to show the interval and calculate average
# also calculate outlier based on a threshold (current 4x average) or use a flat value like 50ms
while True:
   data = sock.recvfrom(1000)
   timeSinceLastPacket = (time.perf_counter()* 1000 - lastRecieveTime)
   lastRecieveTime = time.perf_counter() * 1000
   count += 1
   total = total + timeSinceLastPacket
   average = total/count

   if (timeSinceLastPacket > outlier_threshold * average):
                    outliers = outliers + 1
                    outlier_value = timeSinceLastPacket
   
   print("Interval: " +
          str(timeSinceLastPacket) +
          "ms \nAverage: " +
           str(average) +
          "ms \nOutliers: " +
           str(outliers) +
          "\nLast Outlier: " +
           str(outlier_value) + "ms")
   
