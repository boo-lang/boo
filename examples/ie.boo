import System.Threading

def CreateInstance(progid):
	type = System.Type.GetTypeFromProgID(progid)
	return type()	

ie as duck = CreateInstance("InternetExplorer.Application")
ie.Visible = true
ie.Navigate2("http://www.go-mono.com/monologue/")

Thread.Sleep(50ms) while ie.Busy

document = ie.Document
print("${document.title} is ${document.fileSize} bytes long.")

