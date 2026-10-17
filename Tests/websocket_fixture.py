import socketserver, socket, hashlib, base64, struct, threading, ssl
class H(socketserver.StreamRequestHandler):
 def handle(self):
  try:
   headers={}
   self.rfile.readline()
   while True:
    line=self.rfile.readline().strip()
    if not line: break
    k,v=line.decode().split(':',1);headers[k.lower()]=v.strip()
   if headers.get('x-ip-test')!='native':
    self.wfile.write(b'HTTP/1.1 403 Forbidden\r\nContent-Length: 0\r\n\r\n');return
   accept=base64.b64encode(hashlib.sha1((headers['sec-websocket-key']+'258EAFA5-E914-47DA-95CA-C5AB0DC85B11').encode()).digest())
   self.wfile.write(b'HTTP/1.1 101 Switching Protocols\r\nUpgrade: websocket\r\nConnection: Upgrade\r\nSec-WebSocket-Accept: '+accept+b'\r\n\r\n')
   def frame(op,p,fin=True):
    n=len(p); self.wfile.write(bytes([(128 if fin else 0)|op])+(bytes([n]) if n<126 else b'\x7e'+struct.pack('!H',n) if n<65536 else b'\x7f'+struct.pack('!Q',n))+p)
   while True:
    h=self.rfile.read(2)
    if len(h)!=2:return
    op=h[0]&15;n=h[1]&127
    if n==126:n=struct.unpack('!H',self.rfile.read(2))[0]
    if n==127:n=struct.unpack('!Q',self.rfile.read(8))[0]
    mask=self.rfile.read(4) if h[1]&128 else None
    p=self.rfile.read(n)
    if mask:p=bytes(x^mask[i%4] for i,x in enumerate(p))
    if op==9:frame(10,p);continue
    if op==8:frame(8,p);return
    if op==10:continue
    if p==b'abort':
     self.connection.shutdown(socket.SHUT_RDWR);return
    if p==b'hold':continue
    if p==b'close':frame(8,b'\x03\xe8');return
    frame(op,p[:1],False);frame(9,b'check');frame(0,p[1:])
  except (BrokenPipeError,ConnectionResetError,ssl.SSLError):pass
class S(socketserver.ThreadingTCPServer):allow_reuse_address=True;daemon_threads=True
import argparse
a=argparse.ArgumentParser()
a.add_argument('--cert');a.add_argument('--key')
a=a.parse_args()
s=S(('127.0.0.1',18766 if a.cert else 18765),H)
if a.cert:
 c=ssl.SSLContext(ssl.PROTOCOL_TLS_SERVER)
 c.load_cert_chain(a.cert,a.key)
 s.socket=c.wrap_socket(s.socket,server_side=True)
print('ready',flush=True);s.serve_forever()
