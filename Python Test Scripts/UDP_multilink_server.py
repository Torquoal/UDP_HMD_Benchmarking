import time
import socket

# replace with multicast IP, e.g. 239.1.2.3
group = '0.0.0.0'
port = 9000
# 2-hop restriction in network
ttl = 2
sock = socket.socket(socket.AF_INET,
                     socket.SOCK_DGRAM,
                     socket.IPPROTO_UDP)
sock.setsockopt(socket.IPPROTO_IP,
                socket.IP_MULTICAST_TTL,
                ttl)


while 1:
    sock.sendto(b"hello world", (group, port))
    time.sleep(0.01)

