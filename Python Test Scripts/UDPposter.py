import socket
import time

# change Ip address to send to specific clients or general networks
UDP_IP = '192.168.8.1'
UDP_PORT = 9000
MESSAGE = b"Hello, Client!"



sock = socket.socket(socket.AF_INET, # Internet
                     socket.SOCK_DGRAM) # UDP
while 1:
    #print("UDP target IP: %s" % UDP_IP)
    #print("UDP target port: %s" % UDP_PORT)
    #print("message: %s" % MESSAGE)
    sock.sendto(MESSAGE, (UDP_IP, UDP_PORT))
    time.sleep(0.01)
