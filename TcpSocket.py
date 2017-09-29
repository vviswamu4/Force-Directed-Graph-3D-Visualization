#MSGLEN = 1024
import socket
import time
import random, string
import DrawGraph


host = "192.168.0.100"
port = 7475

class mysocket:
    def __init__(self, sock=None):
        if sock is None:
            self.sock = socket.socket(
                socket.AF_INET, socket.SOCK_STREAM)
        else:
            self.sock = sock

    def connect(self, host, port):
        print("Connecting")
        self.sock.connect((host, port))
        print("Connection Successfull")

    def mysend(self, msg):
        try:
            print("Sending msg", msg)
            sent = self.sock.send(msg)
        except Exception as e:
            print("Error in sending ",str(e)) 
        

    def close(self):
        self.sock.close()


if __name__ == "__main__":
    ms = mysocket()
    ms.connect(host, port)
    dg = DrawGraph()

    while(True):
        try:
            msg = dg.getNameStream("data/nodes.csv")
            ms.mysend(msg.encode('utf-8'))   
            time.sleep(3)
            msg = dg.getLinkStream("data/links.csv")
            ms.mysend(msg.encode('utf-8'))   
            time.sleep(3)
            msg = dg.getPositionStream()
            ms.mysend(msg.encode('utf-8'))   
            time.sleep(3)
            break
        except KeyboardInterrupt:
            print("Breaking Connection!")
            ms.close()
            exit()
            
            
            
        
