# coding=utf-8
# using utf-8 code
# version: python 2.*
import socket
import json
import time

playerNumber = 10

# host and port
host = ""
port = 12346

# create TCP socket，TCP
mySocket = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
# bind IP and port
mySocket.bind((host,port))
# start listening
mySocket.listen(playerNumber)
print("waiting for AR device")
# establish the connection
conn,addr = mySocket.accept()

# simulate the slow start up in the real server
print("create connection, sleep 5 sec")
time.sleep(5)

# how to construct message send to AR:
# type(T): (first digit of message reveiced) indicator of the type of sending info
# -2: communication info (cdata)
# others(-1, 0, 1, 2, 3): terminal info (tdata)
# -1: explode
# 0: continue
# 1: diffuse
# 2: first time request for terminal info for each turn
# 3: time out and explode

# Color:


# tdada: terminal info s=type status
tdata=[""]*3
#         T|    left side color    |   right side color    |    btn    |
#         1|1 1|2 2|3 3|4 4|5 5|6 6|1 1|2 2|3 3|4 4|5 5|6 6|1 2 3 4 5 6|
tdata[0]="2$2$3$2$3$2$3$2$2$2$2$2$2$0$1$2$2$1$0$2$1$0$1$1$2$0$0$0$0$0$1"
tdata[1]="0$2$3$2$3$2$3$2$2$2$2$2$2$0$1$2$2$1$0$2$1$0$1$1$2$1$0$0$0$0$1"
tdata[2]="0$2$3$2$3$2$3$2$2$2$2$2$2$0$1$2$2$1$0$2$1$0$1$1$2$1$1$0$0$0$1"

# cdata: communication info
cdata=[""]*3
#          T|L highlight|R Highlight|B|  R text  # L text
#          1|1 2 3 4 5 6|1 2 3 4 5 6|1|
cdata[0]="-2$1$0$1$0$0$1$0$0$0$1$0$0$0$The terminal with red on top, blue middle, green below, is appearing on the left#"
cdata[1]="-2$1$0$1$0$0$1$0$0$0$1$0$0$1$The terminal with red on top, blue middle, green below, is appearing on the left#"
cdata[2]="-2$1$0$1$0$0$1$0$0$0$1$0$0$2$The terminal with red on top, blue middle, green below, is appearing on the left#"

# send the first terminal info
print("send terminal info initialization")
conn.send(str.encode(tdata[0]))

#receive ready
theGetMessage = str(conn.recv(1024), encoding = "utf-8")
print(theGetMessage)

step=0
# receive first digit recived (split by space):
# 1: request for cdata given position message -> "1 x y": x=0 cannot see left, x=1 can see left; y=0 cannot see right, y=1 can see right
# 0: button select confirm message -> "0 x": x is the index of button selected
# 9: time out message -> "9": the bomb time out and boom, AR request for terminal information next turn
while True:
    theGetMessage = str(conn.recv(1024), encoding="utf-8")
    # print("receive: "+theGetMessage)
    if theGetMessage[0]=="1": # request for communication info
        conn.send(str.encode(cdata[step]))
        print("offer communication info at step"+str(step))
    if theGetMessage[0]=="0":
        if step==int(theGetMessage[2]): # correct
            if step == 3: # diffuse
                msg="1"+tdata0[1:]
                conn.send(str.encode(msg))
                print(diffuse)
                print("-----------player diffuse the bomb!----------")
                step=0
            else: #continue
                conn.send(str.encode(tdata[step]))
                print("player choose correct and continue")
                step+=1
        else: # incorrect-> explode
            msg="-1"+tdata0[1:]
            conn.send(str.encode(msg))
            print("-----------player explode the bomb!----------")
            step=0
    if theGetMessage[0]=="9": # timeout
        msg = "3" + tdata0[1:]
        print("-----------player time out, explode the bomb!----------")



