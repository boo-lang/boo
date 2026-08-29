"""
int: 3
string: foo
"""
import System.Console

def print(value as int):
	Write("int: ")
	WriteLine(value)

	
def print(value as string):
	Write("string: ")
	WriteLine(value)

print(3)
print("foo")
	
