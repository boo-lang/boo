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

namespace Boo.Lang.Interpreter

import System
import System.Collections
import System.IO
import Boo.Lang
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.IO

class AbstractInterpreter:

	_compiler = BooCompiler()
	
	_parser = BooCompiler()
	
	_imports = ImportCollection()
	
	_referenceProcessor = ProcessInterpreterReferences(self)
	
	[property(RememberLastValue)]
	_rememberLastValue = false
	
	Ducky:
		get:
			return _compiler.Parameters.Ducky
			
		set:
			_compiler.Parameters.Ducky = value
			if _suggestionCompiler is not null:
				_suggestionCompiler.Parameters.Ducky = value
	
	_inputId = 0
	
	_suggestionCompiler as BooCompiler
	
	def constructor([required] parser as ICompilerStep):
		
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
		_compiler.Parameters.Ducky = true		
		_parser.Parameters.Pipeline = CompilerPipeline().Add(parser)
		
	def constructor():
		self(Pipelines.Parse.NewParserStep())
		
	abstract def Declare(name as string, type as System.Type):
		pass

	abstract def Lookup(name as string) as System.Type:
		pass
		
	virtual def SetLastValue(value):
		pass
		
	abstract def SetValue(name as string, value) as object:
		pass

	abstract def GetValue(name as string) as object:
		pass
		
	private def GetSuggestionCompiler():
		if _suggestionCompiler is null:
			pipeline = Pipelines.ResolveExpressions(BreakOnErrors: false)
			pipeline.Insert(1, AddRecordedImports(_imports))
			pipeline.Replace(
				Steps.ProcessMethodBodiesWithDuckTyping,
				ProcessExpressionsWithInterpreterNamespace(self))
			pipeline.Replace(
				Steps.InitializeTypeSystemServices,
				_compiler.Parameters.Pipeline.Get(InitializeTypeSystemServices))
			pipeline.Add(FindCodeCompleteSuggestion())
			_suggestionCompiler = BooCompiler()
			_suggestionCompiler.Parameters.Ducky = self.Ducky
			_suggestionCompiler.Parameters.Pipeline = pipeline	
			// keep the references in sync
			_suggestionCompiler.Parameters.References = self.References
			
		return _suggestionCompiler
		
	Pipeline:
		get:
			return _compiler.Parameters.Pipeline
			
	References:
		get:
			return _compiler.Parameters.References
				
	def SuggestCodeCompletion(code as string) as (IEntity):
	"""
	The code must contain a __codecomplete__ member reference as a placeholder
	to the suggestion.
	
	The return value is a an array of possible members or namespaces to be inserted
	at the __codecomplete__.
	"""
		return FilterSuggestions(code, ResolveEntity(code))
		
	def ResolveEntity(code as string) as IEntity:
		compiler = GetSuggestionCompiler()
		try:
			compiler.Parameters.Input.Add(StringInput("<code>", PreProcessImportLine(code)))
			result = compiler.Run()
			return result["suggestion"]			
		ensure:
			compiler.Parameters.Input.Clear()
			
	def FilterSuggestions(code as string, entity as IEntity):
		ns = entity as INamespace
		return array(IEntity, 0) if ns is null
		return GetChildNamespaces(ns) if code.StartsWith("import ")
		return FilteredMembers(TypeSystemServices.GetAllMembers(ns))
		
	def FilteredMembers(members as (IEntity)):
		return array(
				item
				for item in members
				unless IsSpecial(item) or not IsPublic(item))

	def IsSpecial(entity as IEntity):
		for prefix in ".", "___", "add_", "remove_", "raise_", "get_", "set_":
			return true if entity.Name.StartsWith(prefix)
			
	def IsPublic(entity as IEntity):
		member = entity as IMember
		return member is null or member.IsPublic
		
	private def GetChildNamespaces(parent as INamespace):
		return array(member
					for member in parent.GetMembers()
					if member.EntityType == EntityType.Namespace)
			
	private def PreProcessImportLine(code as string):
		match = @/^\s*import\s+((\w|\.)+)\s*$/.Match(code)
		if match.Success:
			return match.Groups[1].Value
		return code
		
	def Eval(code as string):
		return EvalCompilerInput(StringInput("input${++_inputId}", code))
		
	def EvalCompilerInput(input as ICompilerInput):
		result = Parse(input)
		return result if len(result.Errors)
		return EvalCompileUnit(result.CompileUnit)
		
	def EvalCompileUnit(cu as CompileUnit):
		assert 1 == len(cu.Modules)
		
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
			return CompilerContext(cu)
		
		savedImports = module.Imports.Clone()
		module.Imports.ExtendWithClones(_imports)
		
		if hasStatements:	
			if IsSingleEmptyMacroStatement(module):
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
		
		SetLastValue(null) if _rememberLastValue
		result = _compiler.Run(cu)
		return result if len(result.Errors)
		
		RecordImports(savedImports)
		
		asm = result.GeneratedAssembly
		_compiler.Parameters.References.Add(asm) if hasMembers
		
		InitializeModuleInterpreter(asm, module)
		
		result.GeneratedAssembly.EntryPoint.Invoke(null, (null,)) if hasStatements
		return result
		
	def Parse(input as ICompilerInput):
		_parser.Parameters.Input.Clear()
		_parser.Parameters.Input.Add(input)
		return _parser.Run()
	
	private def InitializeModuleInterpreter(asm as System.Reflection.Assembly,
										module as Module):
		moduleType = cast(AbstractInternalType,
						GetEntity(GetModuleEntity(module).ModuleClass)).GeneratedType
		moduleType.GetField("ParentInterpreter").SetValue(null, self)
		
	private static def GetModuleEntity(module as Module) as ModuleEntity:
		return GetEntity(module)
		
	static def GetEntity(node as Node):
		return TypeSystemServices.GetEntity(node)
		
	static def GetOptionalEntity(node as Node):
		return TypeSystemServices.GetOptionalEntity(node)
		
	private def RecordImports(imports as ImportCollection):
		for imp in imports:
			imp.AssemblyReference = null
			_imports.Add(imp)
			
	private def IsSingleEmptyMacroStatement(m as Module):
		if 1 == len(m.Globals.Statements):
			macro = m.Globals.Statements[0] as MacroStatement
			if macro is not null:
				return 0 == len(macro.Arguments) and 0 == len(macro.Block.Statements)
		
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
				
		static def IsInterpreterEntity(entity as IEntity):
			return entity is not null and TypeSystem.EntityType.Custom == entity.EntityType
	
	class InterpreterNamespace(INamespace):
	
		[getter(ParentNamespace)]
		_parent as INamespace
	
		_tss as TypeSystemServices
	
		_interpreter as AbstractInterpreter
	
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
				
	class ProcessExpressionsWithInterpreterNamespace(Steps.ProcessMethodBodiesWithDuckTyping):
		
		_namespace as InterpreterNamespace
		_interpreter as AbstractInterpreter
		
		def constructor(interpreter):
			_interpreter = interpreter
			
		override def Initialize(context as CompilerContext):
			super(context)	
			_namespace = InterpreterNamespace(
								_interpreter,
								TypeSystemServices,
								NameResolutionService.GlobalNamespace)
			NameResolutionService.GlobalNamespace = _namespace
			
		override def Dispose():
			_namespace = null

	class ProcessVariableDeclarations(ProcessExpressionsWithInterpreterNamespace):
	
		_entryPoint as Method
	
		def constructor(interpreter):
			super(interpreter)
			
		InEntryPoint:
			get:
				return _currentMethod.Method is _entryPoint
	
		override def Initialize(context as CompilerContext):
			super(context)
			_entryPoint = Steps.ContextAnnotations.GetEntryPoint(Context)			
			
		override def LeaveExpressionStatement(node as ExpressionStatement):
			# force standalone method references types to be completely
			# resolved
			GetConcreteExpressionType(node.Expression) if InEntryPoint
			super(node)
			
		override def HasSideEffect(node as Expression):
			return true if _interpreter.RememberLastValue and InEntryPoint
			return super(node)
	
		override def CheckLValue(node as Node, entity as IEntity):
			# prevent 'Expression can't be assigned to' error
			return true if InterpreterEntity.IsInterpreterEntity(entity)
			return super(node, entity) 
	
		override def DeclareLocal(sourceNode as Node, name as string, type as IType, privateScope as bool):			
			return super(sourceNode, name, type, privateScope) if privateScope or not InEntryPoint
			
			external = type as ExternalType
			_interpreter.Declare(name, external.ActualType) if external
			
			return _namespace.Declare(name, type)
	
	class ProcessInterpreterReferences(Steps.AbstractTransformerCompilerStep):
	
		static AbstractInterpreter_GetValue = typeof(AbstractInterpreter).GetMethod("GetValue")
		static AbstractInterpreter_SetValue = typeof(AbstractInterpreter).GetMethod("SetValue")
		static AbstractInterpreter_SetLastValue = typeof(AbstractInterpreter).GetMethod("SetLastValue")
	
		_interpreterField as Field
		_interpreter as AbstractInterpreter
		_isEntryPoint = false
		
		def constructor(interpreter):
			_interpreter = interpreter
	
		override def Run():
			Visit(CompileUnit) if 0 == len(Errors)
	
		override def EnterModule(node as Module):
	
			module = GetModuleEntity(node).ModuleClass
			return false unless module
	
			_interpreterField = CodeBuilder.CreateField("ParentInterpreter", TypeSystemServices.Map(AbstractInterpreter))
			_interpreterField.Modifiers = TypeMemberModifiers.Public | TypeMemberModifiers.Static
			module.Members.Add(_interpreterField)
	
			return true
			
		override def OnConstructor(node as Constructor):
			OnMethod(node)
			
		override def OnMethod(node as Method):
			_isEntryPoint = node is Steps.ContextAnnotations.GetEntryPoint(Context);
			super(node)
	
		override def OnReferenceExpression(node as ReferenceExpression):
			
			if (InterpreterEntity.IsInterpreterEntity(GetOptionalEntity(node)) and
					not AstUtil.IsLhsOfAssignment(node)):	
				ReplaceCurrentNode(CreateGetValue(node))
	
		override def LeaveBinaryExpression(node as BinaryExpression):
			if InterpreterEntity.IsInterpreterEntity(GetOptionalEntity(node.Left)):
				ReplaceCurrentNode(CreateSetValue(node))
				
		override def LeaveExpressionStatement(node as ExpressionStatement):
			
			return unless _interpreter.RememberLastValue and _isEntryPoint
			
			if node.Expression.ExpressionType is not TypeSystemServices.VoidType:
				node.Expression = CreateSetLastValue(	node.Expression)
			else:
				eval = CodeBuilder.CreateEvalInvocation(node.LexicalInfo)
				eval.Arguments.Add(node.Expression)
				eval.Arguments.Add(CreateSetLastValue(CodeBuilder.CreateNullLiteral()))			
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
						
		def CreateSetLastValue(value as Expression):
			return CodeBuilder.CreateMethodInvocation(
							CreateInterpreterReference(),
							TypeSystemServices.Map(AbstractInterpreter_SetLastValue),
							value)
	
		def CreateGetValue(node as ReferenceExpression):
			return CastIfNeeded(
					node,
					CreateInterpreterInvocation(
						AbstractInterpreter_GetValue,
						node.Name))
	
		def CreateSetValue(node as BinaryExpression):
			return CastIfNeeded(
					node,
					CreateInterpreterInvocation(
						AbstractInterpreter_SetValue,
						cast(ReferenceExpression, node.Left).Name,
						node.Right))
						
		def CastIfNeeded(srcNode as Expression, expression as Expression):
			if NodeType.ExpressionStatement == srcNode.ParentNode.NodeType:
				return expression
				
			return CodeBuilder.CreateCast(srcNode.ExpressionType, expression)
			
	class AddRecordedImports(Steps.AbstractCompilerStep):
		
		_imports as ImportCollection
		
		def constructor(imports):
			_imports = imports
			
		override def Run():
			CompileUnit.Modules[0].Imports.ExtendWithClones(_imports)

	class FindCodeCompleteSuggestion(Steps.AbstractVisitorCompilerStep):
		
		override def Run():
			Visit(CompileUnit)
		
		override def LeaveMemberReferenceExpression(node as MemberReferenceExpression):
			if "__codecomplete__" == node.Name:
				suggestion as IEntity
				target = node.Target
				if target.ExpressionType is not null:									
					suggestion = target.ExpressionType
				else:
					suggestion = GetOptionalEntity(target)
				if suggestion is not null and suggestion.EntityType != EntityType.Error:
					_context["suggestion"] = suggestion
					// TODO: use target to display static members only for type reference expressions
					_context["target"] = target
