import NUnit.Framework

class Computer:
	_prefix as string
	
	def constructor(prefix):
		_prefix = prefix
		
	def GimmeTheAnswer():
		return "${_prefix} 42"
	

c = Computer("The answer is")
handle = c.GimmeTheAnswer.BeginInvoke(null, null)

Assert.AreEqual("The answer is 42", c.GimmeTheAnswer.EndInvoke(handle))
	
	
