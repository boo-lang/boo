namespace booish

import Boo.Lang
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.IO

class InterpreterEntity(ITypedEntity):

	[getter(Name)]
	[getter(FullName)]
	_name as string

	[getter(Type)]
	_type as IType

	def constructor(name, type):
		_name = name
		_type = type

	EntityType:
		get:
			return TypeSystem.EntityType.Custom

	static def IsInterpreterEntity(node as Node):
		return node.Entity is not null and TypeSystem.EntityType.Custom == node.Entity.EntityType

class InterpreterNamespace(INamespace):

	[getter(ParentNamespace)]
	_parent as INamespace

	_tss as TypeSystemServices

	_interpreter as InteractiveInterpreter

	_declarations = {}

	def constructor(interpreter, tss, parent):
		_interpreter = interpreter
		_tss = tss
		_parent = parent

	def Declare(name as string, type as IType):
		_declarations.Add(name, entity=InterpreterEntity(name, type))
		return entity

	def Resolve(targetList as List, name as string, flags as EntityType) as bool:
		return false unless flags == EntityType.Any

		entity as IEntity = _declarations[name]
		if entity is null:
			value = _interpreter.GetValue(name)
			if value is not null:
				entity = Declare(name, _tss.Map(value.GetType()))

		if entity is not null:
			targetList.Add(entity)
			return true

		return false

	def GetMembers():
		return array(IEntity, 0)


class ProcessVariableDeclarations(Steps.ProcessMethodBodiesWithDuckTyping):

	_namespace as InterpreterNamespace
	_interpreter as InteractiveInterpreter

	def constructor(interpreter):
		_interpreter = interpreter

	override def Initialize(context as CompilerContext):
		super(context)

		_namespace = InterpreterNamespace(
							_interpreter,
							TypeSystemServices,
							NameResolutionService.GlobalNamespace)
		NameResolutionService.GlobalNamespace = _namespace

	override def CheckLValue(node as Node):
		# prevent 'Expression can't be assigned to' error
		return true if InterpreterEntity.IsInterpreterEntity(node)
		return super(node) 

	override def DeclareLocal(name as string, type as IType, privateScope as bool):
		return super(name, type, privateScope) if privateScope
		return _namespace.Declare(name, type)

class ProcessInterpreterReferences(Steps.AbstractTransformerCompilerStep):

	static InteractiveInterpreter_GetValue = typeof(InteractiveInterpreter).GetMethod("GetValue")
	static InteractiveInterpreter_SetValue = typeof(InteractiveInterpreter).GetMethod("SetValue")

	_interpreterField as Field
	_namespace as InterpreterNamespace
	_interpreter as InteractiveInterpreter

	def constructor(interpreter):
		_interpreter = interpreter

	override def Run():
		Visit(CompileUnit) if 0 == len(Errors)

	override def OnModule(node as Module):

		module = cast(ModuleEntity, node.Entity).ModuleClass
		return unless module

		_interpreterField = CodeBuilder.CreateField("ParentInterpreter", TypeSystemServices.Map(InteractiveInterpreter))
		_interpreterField.Modifiers = TypeMemberModifiers.Public | TypeMemberModifiers.Static
		module.Members.Add(_interpreterField)

		super(node)

	override def OnReferenceExpression(node as ReferenceExpression):
		super(node)
		return unless InterpreterEntity.IsInterpreterEntity(node) and not AstUtil.IsLhsOfAssignment(node)

		ReplaceCurrentNode(CreateGetValue(node))

	override def LeaveBinaryExpression(node as BinaryExpression):
		if InterpreterEntity.IsInterpreterEntity(node.Left):
			ReplaceCurrentNode(CreateSetValue(node))

	def CreateGetValue(node as ReferenceExpression):
		return CodeBuilder.CreateCast(
				node.ExpressionType,
				CodeBuilder.CreateMethodInvocation(
					CodeBuilder.CreateReference(_interpreterField),
					TypeSystemServices.Map(InteractiveInterpreter_GetValue),
					CodeBuilder.CreateStringLiteral(node.Name)))

	def CreateSetValue(node as BinaryExpression):
		return CodeBuilder.CreateMethodInvocation(
					CodeBuilder.CreateReference(_interpreterField),
					TypeSystemServices.Map(InteractiveInterpreter_SetValue),
					CodeBuilder.CreateStringLiteral(cast(ReferenceExpression, node.Left).Name),
					node.Right)


class InteractiveInterpreter:

	_compiler = BooCompiler()

	_values = {}

	def constructor():
		pipeline = Pipelines.CompileToMemory()
		index = pipeline.Find(Steps.ProcessMethodBodiesWithDuckTyping)
		pipeline[index] = ProcessVariableDeclarations(self)
		pipeline.InsertBefore(Steps.EmitAssembly, ProcessInterpreterReferences(self))
		//pipeline.Add(Steps.PrintBoo())

		_compiler.Parameters.Pipeline = pipeline

	def Eval(code as string):
		_compiler.Parameters.Input.Clear()
		_compiler.Parameters.Input.Add(StringInput("src", code))

		result = _compiler.Run()
		if 0 == len(result.Errors):
			entry = result.GeneratedAssemblyEntryPoint
			entry.DeclaringType.GetField("ParentInterpreter").SetValue(null, self)
			entry.Invoke(null, (null,))
		return result

	def SetValue(name as string, value):
		_values[name] = value
		return value

	def GetValue(name as string):
		return _values[name]
/*
interpreter = InteractiveInterpreter()
interpreter.SetValue("name", "boo")
interpreter.SetValue("age", 3)

interpreter.Eval("""
print(name)
print(age)
age += 1
""")

assert 4 == interpreter.GetValue("age")

interpreter.Eval("age = 42")
assert 42 == interpreter.GetValue("age")

interpreter.Eval("""
value = 3
print(value*2)
""")

assert 3 == interpreter.GetValue("value")

interpreter.Eval("x2 = { v as int | return v*2 }")

x2 as callable = interpreter.GetValue("x2")
assert 4 == x2(2)

interpreter.Eval("e = i*2 for i in range(value)")
assert array(interpreter.GetValue("e")) == (0, 2, 4)

# let's loop
interpreter.Eval("""
for i in range(3):
	print(i*2)
""")
print("i: ${interpreter.GetValue('i')}")
*/

interpreter = InteractiveInterpreter()
while line=prompt(">>> "):
	try:		
		result = interpreter.Eval(line)
		for error in result.Errors:
			print("ERROR: ${error.Message}")
	except x:
		print(x)
	





