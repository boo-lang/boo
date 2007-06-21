import NUnit.Framework
import BooCompiler.Tests

class App:
	
	[getter(Times)]
	_times = 0
	
	def Run():
		button = Clickable()
		button.Click += { ++_times }
			
		button.RaiseClick()
		button.RaiseClick()

app = App()
app.Run()
Assert.AreEqual(2, app.Times)
