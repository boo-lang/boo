"""
MakeItUpper:foo:FOO
MakeItLower:BaR:bar
"""
import NUnit.Framework

def upper(s as string):
	return s.ToUpper()
	
def lower(s as string):
	return s.ToLower()
	
def invoke(fn as ICallable, arg):
	return fn(arg)
	
commandMap = {
				"MakeItUpper" : upper,
				"MakeItLower" : lower
			}
			
text = """
MakeItUpper foo FOO
MakeItLower BaR bar
"""

for line in /\n/.Split(text):
	continue unless len(line.Trim())	
	
	command, arg, expected = /\s+/.Split(line)	
	Assert.AreEqual(expected, invoke(commandMap[command], arg))
	
	print("${command}:${arg}:${expected}")
