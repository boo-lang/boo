import System
import System.Diagnostics

class Person:
	[property(Name)]
	_name as string
	
def use(p as (Person)):
	Debug.Assert(5 == len(p))
	
def run():
	for i in range(100000):
		a = (
				Person(Name: "a name"),
				Person(Name: "a name"),
				Person(Name: "a name"),
				Person(Name: "a name"),
				Person(Name: "a name"))
	use(a)

	
start = date.Now

for i in range(10):
	run()
	
print("elapsed: ${date.Now-start}")
	
	
