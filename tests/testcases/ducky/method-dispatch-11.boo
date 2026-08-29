"""
func1
func2 with arg: test
func3 param:multiple
func3 param:params
func1
func2 with arg: dispatcher arg test
func3 param:dispatcher
func3 param:multiple
func3 param:args
func4 with arg: the arg
func4 param:42
"""
def func1():
	print 'func1'

def func2(arg1):
	print 'func2 with arg: ' + arg1

def func3(*args):
	for item in args:
		print 'func3 param:' + item
		
def func4(arg1, *rest):
	print 'func4 with arg: ' + arg1
	for item in rest:
		print 'func4 param:' + item

func1()
func2('test')
func3('multiple', 'params')

dispatcher = {'f1': func1, 'f2': func2, 'f3': func3, 'f4': func4 }

dispatcher['f1']()
dispatcher['f2']('dispatcher arg test')
dispatcher['f3']('dispatcher', 'multiple', 'args')
dispatcher['f4']('the arg', 42)

