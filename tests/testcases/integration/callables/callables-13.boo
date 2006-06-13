import NUnit.Framework

class Computer:
	_prefix as string
	
	def constructor(prefix):
		_prefix = prefix
		
	def Compute():
		return "${_prefix} 42"
	

c = Computer("The answer is")
handle = c.Compute.BeginInvoke(null, null)

Assert.AreEqual("The answer is 42", c.Compute.EndInvoke(handle))
	
	
