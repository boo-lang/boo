import System
import System.Net
import System.IO

def GetFileName(url as string):
	uri = Uri(url)
	return Path.GetFileName(uri.AbsolutePath)

def DownloadTo(url as string, fname as string):
	using response=WebRequest.Create(url).GetResponse():
		reader=response.GetResponseStream()
		buffer=array(byte, 1024)
		using writer=File.OpenWrite(fname):
			while read=reader.Read(buffer, 0, len(buffer)):
				Console.Write(".")
				writer.Write(buffer, 0, read)

url = argv[0]
fname = GetFileName(url)
print("${url} => ${fname}")
DownloadTo(url, fname)
