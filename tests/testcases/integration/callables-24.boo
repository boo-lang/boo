import System
import NUnit.Framework

def click1():
	return "clicked!"
	
def click2(sender):
	return "${sender} clicked!"
	
def click3(sender as string):
	return "And ${sender} clicked!"
	
handler as EventHandler

handler = click1
Assert.AreEqual("clicked!", handler(null, EventArgs.Empty))

handler = click2
Assert.AreEqual("foo clicked!", handler("foo", EventArgs.Empty))

handler = click3
Assert.AreEqual("And bar clicked!", handler("bar", EventArgs.Empty))

