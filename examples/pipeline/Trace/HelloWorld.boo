class Knight:
	def Speak():
		print("ni!")
		
def foo():
	print("inside foo.")
	
def bar():
	print("inside bar.")
	raise "a exception!"
	
def main():
	foo()
	try:
		bar()
	except x:
		print("caught '${x.Message}'.")
	Knight().Speak()
	
main()