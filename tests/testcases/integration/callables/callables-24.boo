import System
import NUnit.Framework

callable BinaryFunction(a, b) as object

def click1():
	return "clicked!"
	
def click2(sender):
	return "${sender} clicked!"
	
def click3(sender as string):
	return "And ${sender} clicked!"
	
handler as BinaryFunction

handler = click1
Assert.AreEqual("clicked!", handler(null, null))

handler = click2
Assert.AreEqual("foo clicked!", handler("foo", null))

handler = click3
Assert.AreEqual("And bar clicked!", handler("bar", null))

