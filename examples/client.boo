import System.IO
import System.Net
import System.Net.Sockets

s = Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
s.Connect(IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080))

using stream=NetworkStream(s, true):
	writer=StreamWriter(stream)
	writer.WriteLine("ping!")
	writer.Flush()
			
	print(StreamReader(stream).ReadLine())