// Object model (pode ser qualquer tipo) fornecido pela
// aplicação para algo como:
//		// compile(code as string, globals as Type, baseType as Type)
//		macro = compile(code, ObjectModel, GameObject)
//      gameObject as GameObject = macro() 
class ObjectModel:

	[getter(Foo)]
	_myFoo as Foo
			
	def constructor(foo as Foo):
		_myFoo = foo


// código em uma macro (macro.boo)
def printFoo():
	print(Foo.Name)

	
printFoo()

// classe final gerada pela compilação
//
class FooMacro(IBooMacro):
	def printFoo():
		print(_context.Foo.Name)
		
	def Run() as object:
		printFoo()
		
	def FooMacro(context as ObjectModel):
		_context = context
		
	_context as ObjectModel
	
	
// código da aplicação para executar uma macro
execute(macro, ObjectModel(applicationFoo)) 



		

