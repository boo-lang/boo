import NUnit.Framework
import BooCompiler.Tests

class Path:
	pass

class AmbiguousSub3(AmbiguousSub2):
	override def ToString():
		return Path

s = AmbiguousSub2()
Assert.AreEqual("Sub1", s.Path)

s = AmbiguousSub3()
Assert.AreEqual("Sub1", s.ToString())
