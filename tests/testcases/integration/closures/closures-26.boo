"""
a!
b!
"""
def invoke(c as callable):
	return c()
	
options = {
	"a" : { print("a!") },
	"b" : { print("b!") }
}

invoke(options["a"])
invoke(options["b"])


