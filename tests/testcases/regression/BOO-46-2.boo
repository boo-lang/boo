import NUnit.Framework

class Man:
	Speak:
		get:
			return run
			
	private def run():
		return "albatross!"
		
Assert.AreEqual("albatross!", Man().Speak())
