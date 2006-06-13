"""
a!
b!
"""
def invoke(c as callable):
	return c()
	
a = "a!"
b = "b!"

options = {
	"a" : { print(a) },
	"b" : { print(b) }
}

invoke(options["a"])
invoke(options["b"])


