#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace booish

import System
import System.Collections
import System.Diagnostics
import System.IO
import Boo.Lang
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.IO

class InteractiveInterpreter:

	_compiler = BooCompiler()
	
	_parser = BooCompiler()

	_values = {}
	
	_declarations = {}
	
	_imports = ImportCollection()
	
	_referenceProcessor = ProcessInterpreterReferences(self)
	
	_representers = []
	
	[property(Print, value is not null)]
	_print as callable(object) = print
	
	[property(RememberLastValue)]
	_rememberLastValue = false
	
	_inputId = 0
	
	def constructor():
		
		pipeline = Pipelines.CompileToMemory()
		pipeline.RemoveAt(0)
				
		pipeline.Replace(Steps.ProcessMethodBodiesWithDuckTyping,
						ProcessVariableDeclarations(self))
		pipeline.InsertBefore(Steps.EmitAssembly, _referenceProcessor)
		
		index = pipeline.Find(Steps.IntroduceModuleClasses)
		cast(Steps.IntroduceModuleClasses, pipeline[index]).ForceModuleClass = true
		
		// avoid InvalidCastExceptions by always
		// defining callable types only once per run
		pipeline.Replace(Steps.InitializeTypeSystemServices, InitializeTypeSystemServices())
		pipeline.Add(CacheCallableTypes())
		
		_compiler.Parameters.Pipeline = pipeline		
		_parser.Parameters.Pipeline = Pipelines.Parse()
		
		InitializeStandardReferences()
		
	LastValue:
		get:
			return GetValue("@value")
			
	Pipeline:
		get:
			return _compiler.Parameters.Pipeline
			
	References:
		get:
			return _compiler.Parameters.References

	def ConsoleLoopEval():			
		while line=prompt(">>> "):
			try:		
				line = ReadBlock(line) if line[-1:] in ":", "\\"
				LoopEval(line)
			except x as System.Reflection.TargetInvocationException:
				print(x.InnerException)
			except x:
				print(x)
			
	def LoopEval(code as string):
		result = self.Eval(code)
		if len(result.Errors):
			self.DisplayErrors(result.Errors)
		else:
			_ = self.LastValue
			if _ is not null:
				_print(repr(_))
				SetValue("_", _)
		
	def Eval(code as string):
		
		result = Parse(code)
		return result if len(result.Errors)
		
		cu = result.CompileUnit
		module = cu.Modules[0]
		
		# remember the state of the module as of after parsing
		hasStatements = module.Globals.Statements.Count > 0
		hasMembers = module.Members.Count > 0
		
		module.Imports.Reject() do (item as Import):
			for existing in _imports:
				return true if existing.Namespace == item.Namespace
			
		if ((not hasStatements) and
			(not hasMembers) and
			0 == len(module.Imports)):
			return result
		
		savedImports = module.Imports.Clone()
		module.Imports.ExtendWithClones(_imports)
		
		if hasStatements:	
			if IsSimpleReference(code):
				# simple references will be parsed as macros
				# but we want them to be evaluated
				# as references...
				ms = module.Globals.Statements[0] as MacroStatement
				if ms is not null:
					module.Globals.Statements.ReplaceAt(0,
						ExpressionStatement(						
							ReferenceExpression(ms.LexicalInfo, ms.Name)))
			_compiler.Parameters.OutputType = CompilerOutputType.ConsoleApplication
		else:
			_compiler.Parameters.OutputType = CompilerOutputType.Library
		
		SetValue("@value", null)
		result = _compiler.Run(cu)
		return result if len(result.Errors)
		
		RecordImports(savedImports)
		
		asm = result.GeneratedAssembly
		_compiler.Parameters.References.Add(asm) if hasMembers
		
		InitializeModuleInterpreter(asm, module)
		
		result.GeneratedAssemblyEntryPoint.Invoke(null, (null,)) if hasStatements
		return result
		
	def Parse(code as string):
		_parser.Parameters.Input.Clear()
		_parser.Parameters.Input.Add(StringInput("input${++_inputId}", code))
		return _parser.Run()
		
	def Declare([required] name as string,
				[required] type as System.Type):
		_declarations.Add(name, type)
		
	def Lookup([required] name as string):
		type as System.Type = _declarations[name]
		return type if type is not null
		
		value = GetValue(name)
		return value.GetType() if value is not null
		
	def Reset():
		_declarations.Clear()
		_values.Clear()
		InitializeStandardReferences()

	def SetValue(name as string, value):
		_values[name] = value
		return value

	def GetValue(name as string):
		return _values[name]
		
	def DisplayErrors(errors as CompilerErrorCollection):
		for error in errors:
			pos = error.LexicalInfo.StartColumn
			_print("---" + "-" * pos + "^") if pos > 0
			_print("ERROR: ${error.Message}")
			
	def globals():
		return array(
					key for key as string in _values.Keys
					unless key.StartsWith("@"))
			
	def dir([required] obj):
		type = (obj as Type) or obj.GetType()
		return array(
				member for member in type.GetMembers()
				unless (method=(member as System.Reflection.MethodInfo))
				and method.IsSpecialName)
				
	def load([required] path as string):
		References.Add(System.Reflection.Assembly.LoadFrom(path))
		
	def help(obj):		
		type = (obj as Type) or obj.GetType()
		describeType("    ", type)
		
	def describeType(indent as string, type as Type):
		
		baseTypes = (getBooTypeName(type.BaseType),) + array(getBooTypeName(t) for t in type.GetInterfaces())
		_print("class ${type.Name}(${join(baseTypes, ', ')}):")
		_print("")
		
		for ctor in type.GetConstructors():
			_print("${indent}def constructor(${describeParameters(ctor.GetParameters())})")
			_print("")
			
		sortByName = def (lhs as Reflection.MemberInfo, rhs as Reflection.MemberInfo):
			return lhs.Name.CompareTo(rhs.Name)
			
		for f as Reflection.FieldInfo in List(type.GetFields()).Sort(sortByName):
			_print("${indent}public ${describeModifiers(f)}${f.Name} as ${getBooTypeName(f.FieldType)}")
			_print("")
			
		for p as Reflection.PropertyInfo in List(type.GetProperties()).Sort(sortByName):
			modifiers = describeModifiers(p)
			params = describePropertyParameters(p.GetIndexParameters())
			_print("${indent}${modifiers}${p.Name}${params} as ${getBooTypeName(p.PropertyType)}:")
			_print("${indent}${indent}get") if p.GetGetMethod() is not null
			_print("${indent}${indent}set") if p.GetSetMethod() is not null
			_print("")		
		
		for m as Reflection.MethodInfo in List(type.GetMethods()).Sort(sortByName):
			continue if m.IsSpecialName
			modifiers = describeModifiers(m)
			returnType = getBooTypeName(m.ReturnType)
			_print("${indent}${modifiers}def ${m.Name}(${describeParameters(m.GetParameters())}) as ${returnType}")
			_print("")
			
		for e as Reflection.EventInfo in List(type.GetEvents()).Sort(sortByName):
			_print("${indent}${describeModifiers(e)}event ${e.Name} as ${e.EventHandlerType}")
			_print("")
			
	def describeModifiers(f as Reflection.FieldInfo):
		return "static " if f.IsStatic
		return ""
			
	def describeModifiers(m as Reflection.MethodBase):
		return "static " if m.IsStatic
		return ""
		
	def describeModifiers(e as Reflection.EventInfo):
		return describeModifiers(e.GetAddMethod() or e.GetRemoveMethod())
		
	def describeModifiers(p as Reflection.PropertyInfo):
		accessor = p.GetGetMethod() or p.GetSetMethod()
		return describeModifiers(accessor)
			
	def describePropertyParameters(parameters as (Reflection.ParameterInfo)):
		return "" if 0 == len(parameters)
		return "(${describeParameters(parameters)})"
			
	def describeParameters(parameters as (Reflection.ParameterInfo)):
		return join(describeParameter(p) for p in parameters, ", ")
		
	def describeParameter(p as Reflection.ParameterInfo):
		return "${p.Name} as ${getBooTypeName(p.ParameterType)}"
		
	def getBooTypeName(type as System.Type):
		return "(${getBooTypeName(type.GetElementType())})" if type.IsArray
		return "object" if object is type
		return "string" if string is type
		return "void" if void is type
		return "bool" if bool is type		
		return "byte" if byte is type
		return "short" if short is type
		return "int" if int is type
		return "long" if long is type
		return "single" if single is type
		return "double" if double is type
		return "date" if date is type
		return type.FullName
			
	private def InitializeStandardReferences():
		SetValue("interpreter", self)
		SetValue("dir", dir)
		SetValue("help", help)
		SetValue("print", { value | _print(value) })
		SetValue("load", load)
		SetValue("globals", globals)

	private def InitializeModuleInterpreter(asm as System.Reflection.Assembly,
										module as Module):
		moduleType = cast(AbstractInternalType,
						cast(ModuleEntity, module.Entity).ModuleClass.Entity).GeneratedType
		moduleType.GetField("ParentInterpreter").SetValue(null, self)
		
	private def RecordImports(imports as ImportCollection):
		for imp in imports:
			imp.AssemblyReference = null
			_imports.Add(imp)
			
	private def IsSimpleReference(s as string):
		return /^\s*[_a-zA-Z][_a-zA-Z\d]*\s*$/.IsMatch(s)
		
	def repr(value):
		writer = System.IO.StringWriter()
		repr(value, writer)
		return writer.ToString()
	
	def repr(value, writer as System.IO.TextWriter):
		return unless value is not null
		InitializeRepresenters() if 0 == len(_representers)
		GetBestRepresenter(value.GetType())(value, writer)

	private def InitializeRepresenters():
		AddRepresenter(string) do (value as string, writer as TextWriter):
			Visitors.BooPrinterVisitor.WriteStringLiteral(value, writer)
			
		AddRepresenter(bool) do (value as bool, writer as TextWriter):
			writer.Write(("false", "true")[value])
			
		AddRepresenter(Array) do (a as Array, writer as TextWriter):
			writer.Write("(")
			RepresentItems(a, writer)
			writer.Write(")")
				
		AddRepresenter(Delegate) do (d as Delegate, writer as TextWriter):
			method = d.Method
			writer.Write(method.DeclaringType.FullName)
			writer.Write(".")
			writer.Write(method.Name)
		
		AddRepresenter(IDictionary) do (value as IDictionary, writer as TextWriter):
			writer.Write("{")
			i = 0
			for item as DictionaryEntry in value:
				writer.Write(", ") if i
				repr(item.Key, writer)
				writer.Write(": ")
				repr(item.Value, writer)
				++i
			writer.Write("}")
			
		AddRepresenter(IList) do (value as IList, writer as TextWriter):
			writer.Write("[")
			RepresentItems(value, writer)
			writer.Write("]")
				
		AddRepresenter(object) do (value, writer as TextWriter):
			writer.Write(value)
			
	private def RepresentItems(items, writer as TextWriter):
		i = 0
		for item in items:
			writer.Write(", ") if i > 0				
			repr(item, writer)
			++i
			
	callable Representer(value, writer as TextWriter)
	
	private def AddRepresenter(type as Type, value as Representer):
		_representers.Add((type, value))
		
	def GetBestRepresenter(type as Type) as Representer:
		for key as Type, value in _representers:
			return value if key.IsAssignableFrom(type)
		assert false, "An appropriate representer could not be found!"
		
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
			entity = InterpreterEntity(name, type)
			_declarations.Add(name, entity)
			return entity
	
		def Resolve(targetList as List, name as string, flags as EntityType) as bool:
			return false unless flags == EntityType.Any
	
			entity as IEntity = _declarations[name]
			if entity is null:
				type = _interpreter.Lookup(name)
				if type is not null:
					if object is type:
						entity = Declare(name, _tss.DuckType)
					else:
						entity = Declare(name, _tss.Map(type))
	
			if entity is not null:
				targetList.Add(entity)
				return true
	
			return false
	
		def GetMembers():
			return array(IEntity, 0)
			
	class InterpreterTypeSystemServices(TypeSystemServices):
	
		[getter(CachedCallableTypes)]
		_cachedCallableTypes as List
		
		[getter(GeneratedCallableTypes)]
		_generatedCallableTypes = []
		
		def constructor(context, cache):
			super(context)
			_cachedCallableTypes = cache			
		
		override def CreateConcreteCallableType(sourceNode as Node, 
											anonymousType as AnonymousCallableType):
												
			for type as System.Type in _cachedCallableTypes:
				cached = Map(type) as ExternalCallableType
				if anonymousType.GetSignature() == cached.GetSignature():
					return cached
					
			new = super(sourceNode, anonymousType)
			_generatedCallableTypes.Add(new)
			return new
			
	class InitializeTypeSystemServices(Steps.AbstractCompilerStep):
		
		_cachedCallableTypes = []
		
		override def Run():			
			Context.TypeSystemServices = InterpreterTypeSystemServices(Context, _cachedCallableTypes)
			
	class CacheCallableTypes(Steps.AbstractCompilerStep):
		
		override def Run():
			return if len(Errors)			
			
			services as InterpreterTypeSystemServices = self.TypeSystemServices
			types = services.GeneratedCallableTypes
			return unless len(types)
			
			debug "caching", len(types), "callable types"
			for type as InternalCallableType in types:
				debug type
				services.CachedCallableTypes.Add(type.GeneratedType)
	
	class ProcessVariableDeclarations(Steps.ProcessMethodBodiesWithDuckTyping):
	
		_namespace as InterpreterNamespace
		_interpreter as InteractiveInterpreter
		_entryPoint as Method
	
		def constructor(interpreter):
			_interpreter = interpreter
			
		InEntryPoint:
			get:
				return _currentMethod.Method is _entryPoint
	
		override def Initialize(context as CompilerContext):
			super(context)
	
			_namespace = InterpreterNamespace(
								_interpreter,
								TypeSystemServices,
								NameResolutionService.GlobalNamespace)
			NameResolutionService.GlobalNamespace = _namespace
			_entryPoint = Steps.ContextAnnotations.GetEntryPoint(Context)
			
		override def Dispose():
			_namespace = null
			
		override def LeaveExpressionStatement(node as ExpressionStatement):
			# force standalone method references types to be completely
			# resolved
			GetConcreteExpressionType(node.Expression) if InEntryPoint
			super(node)
			
		override def HasSideEffect(node as Expression):
			return true if _interpreter.RememberLastValue and InEntryPoint
			return super(node)
	
		override def CheckLValue(node as Node):
			# prevent 'Expression can't be assigned to' error
			return true if InterpreterEntity.IsInterpreterEntity(node)
			return super(node) 
	
		override def DeclareLocal(sourceNode as Node, name as string, type as IType, privateScope as bool):			
			return super(sourceNode, name, type, privateScope) if privateScope or not InEntryPoint
			
			external = type as ExternalType
			_interpreter.Declare(name, external.ActualType) if external
			
			return _namespace.Declare(name, type)
	
	class ProcessInterpreterReferences(Steps.AbstractTransformerCompilerStep):
	
		static InteractiveInterpreter_GetValue = typeof(InteractiveInterpreter).GetMethod("GetValue")
		static InteractiveInterpreter_SetValue = typeof(InteractiveInterpreter).GetMethod("SetValue")
	
		_interpreterField as Field
		_interpreter as InteractiveInterpreter
		_isEntryPoint = false
		
		def constructor(interpreter):
			_interpreter = interpreter
	
		override def Run():
			Visit(CompileUnit) if 0 == len(Errors)
	
		override def EnterModule(node as Module):
	
			module = cast(ModuleEntity, node.Entity).ModuleClass
			return false unless module
	
			_interpreterField = CodeBuilder.CreateField("ParentInterpreter", TypeSystemServices.Map(InteractiveInterpreter))
			_interpreterField.Modifiers = TypeMemberModifiers.Public | TypeMemberModifiers.Static
			module.Members.Add(_interpreterField)
	
			return true
			
		override def OnConstructor(node as Constructor):
			OnMethod(node)
			
		override def OnMethod(node as Method):
			_isEntryPoint = node is Steps.ContextAnnotations.GetEntryPoint(Context);
			super(node)
	
		override def OnReferenceExpression(node as ReferenceExpression):
			
			if (InterpreterEntity.IsInterpreterEntity(node) and
					not AstUtil.IsLhsOfAssignment(node)):	
				ReplaceCurrentNode(CreateGetValue(node))
	
		override def LeaveBinaryExpression(node as BinaryExpression):
			if InterpreterEntity.IsInterpreterEntity(node.Left):
				ReplaceCurrentNode(CreateSetValue(node))
				
		override def LeaveExpressionStatement(node as ExpressionStatement):
			
			return unless _interpreter.RememberLastValue and _isEntryPoint
			
			if node.Expression.ExpressionType is not TypeSystemServices.VoidType:
				node.Expression = CreateInterpreterInvocation(
								InteractiveInterpreter_SetValue,
								"@value",
								node.Expression)
			else:
				eval = CodeBuilder.CreateEvalInvocation(node.LexicalInfo)
				eval.Arguments.Add(node.Expression)
				eval.Arguments.Add(
						CreateInterpreterInvocation(
							InteractiveInterpreter_SetValue,
							"@value",
							CodeBuilder.CreateNullLiteral()))			
				node.Expression = eval
							
		def CreateInterpreterInvocation(method as System.Reflection.MethodInfo,
										name as string,
										value as Expression):
			mie = CreateInterpreterInvocation(method, name)
			mie.Arguments.Add(value)
			return mie
			
		def CreateInterpreterReference():
			return CodeBuilder.CreateReference(_interpreterField)
			
		def CreateInterpreterInvocation(method as System.Reflection.MethodInfo,
										name as string):
			return CodeBuilder.CreateMethodInvocation(
						CreateInterpreterReference(),
						TypeSystemServices.Map(method),
						CodeBuilder.CreateStringLiteral(name))
	
		def CreateGetValue(node as ReferenceExpression):
			return CastIfNeeded(
					node,
					CreateInterpreterInvocation(
						InteractiveInterpreter_GetValue,
						node.Name))
	
		def CreateSetValue(node as BinaryExpression):
			return CastIfNeeded(
					node,
					CreateInterpreterInvocation(
						InteractiveInterpreter_SetValue,
						cast(ReferenceExpression, node.Left).Name,
						node.Right))
						
		def CastIfNeeded(srcNode as Expression, expression as Expression):
			if NodeType.ExpressionStatement == srcNode.ParentNode.NodeType:
				return expression
				
			return CodeBuilder.CreateCast(srcNode.ExpressionType, expression)			
			
def ReadBlock(line as string):
	newLine = System.Environment.NewLine
	buffer = System.Text.StringBuilder()
	buffer.Append(line)
	buffer.Append(newLine)
	while line=prompt("... "):
		break if 0 == len(line)
		buffer.Append(line)
		buffer.Append(newLine)
	return buffer.ToString()

interpreter = InteractiveInterpreter(RememberLastValue: true)

if "--print-modules" in argv:
	interpreter.Pipeline.Add(Steps.PrintBoo())
	
if "--debug" in argv:
	Debug.Listeners.Add(TextWriterTraceListener(Console.Out))

interpreter.ConsoleLoopEval()
	





