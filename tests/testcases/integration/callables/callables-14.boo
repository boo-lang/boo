import NUnit.Framework

class Computer:
	_prefix as string
	
	def constructor(prefix):
		_prefix = prefix
		
	def Compute():
		return "${_prefix} 42"
	

compute = Computer("The answer is").Compute
handle = compute.BeginInvoke(null, null)

Assert.AreEqual("The answer is 42", compute.EndInvoke(handle))
	
	
