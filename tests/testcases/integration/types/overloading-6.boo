
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

class Path:
	pass

class AmbiguousSub3(AmbiguousSub2):
	override def ToString():
		return Path

s = AmbiguousSub2()
assert "Sub1" == s.Path

s = AmbiguousSub3()
assert "Sub1" == s.ToString()
