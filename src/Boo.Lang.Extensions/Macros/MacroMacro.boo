#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace Boo.Lang.Extensions

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem


class MacroMacro(LexicalInfoPreservingGeneratorMacro):

	private static final Usage = "Usage: `macro [<parent.>+]<name>[(arg0,...)]`"

	_macro as MacroStatement
	_name as string
	_argumentsPattern as ExpressionCollection


	override protected def ExpandGeneratorImpl(macro as MacroStatement):
		raise Usage if len(macro.Arguments) != 1

		_macro = macro

		arg = macro.Arguments[0]
		mie = arg as MethodInvocationExpression
		if mie and mie.Arguments.Count:
			_argumentsPattern = ArgumentsPatternBuilder(Context, mie.Arguments).Output
			arg = mie.Target

		if arg.NodeType == NodeType.ReferenceExpression:
			_name = cast(ReferenceExpression, arg).Name
			yield CreateMacroType()
		elif arg.NodeType == NodeType.MemberReferenceExpression:
			if macro.GetAncestor[of MacroStatement]() is not null:
				raise "Nested macro extension cannot be itself a nested macro"
			_name = cast(MemberReferenceExpression, arg).Name
			CreateMacroExtensionType(cast(MemberReferenceExpression, arg))
		else:
			raise Usage


	private static def BuildMacroTypeName(name as string) as string:
		return char.ToUpper(name[0]) + name[1:] + "Macro"

	private static def BuildMacroTypeName(name as Expression) as string:
		return BuildMacroTypeName(name, false)

	private static def BuildMacroTypeName(name as Expression, external as bool) as string:
		sb = System.Text.StringBuilder()
		for part in UnpackReferences(name):
			sb.Append(char('.')) if sb.Length and not external
			sb.Append(char('$')) if external
			sb.Append(BuildMacroTypeName(part))
		return sb.ToString()


	private static def UnpackReferences(refs as Expression) as string*:
		if refs.NodeType == NodeType.ReferenceExpression:
			re = cast(ReferenceExpression, refs)
			raise "Extending macro `macro` is not supported" if re.Name == "macro"
			yield re.Name
		elif refs.NodeType == NodeType.MemberReferenceExpression:
			mre = cast(MemberReferenceExpression, refs)
			for name in UnpackReferences(mre.Target):
				yield name
			yield mre.Name
		else:
			raise "Invalid node type: ${refs.NodeType}"


	private static def GetParentMacroNames(macro as MacroStatement) as string*:
		for parent in macro.GetAncestors[of MacroStatement]():
			continue if parent.Name != 'macro'

			arg = parent.Arguments[0]
			if arg.NodeType == NodeType.MethodInvocationExpression:
				arg = cast(MethodInvocationExpression, arg).Target

			if arg.NodeType == NodeType.ReferenceExpression:
				yield cast(ReferenceExpression, arg).Name

			elif arg.NodeType == NodeType.MemberReferenceExpression:
				for name in UnpackReferences(cast(MemberReferenceExpression, arg)):
					yield name


	private def CreateMacroExtensionType(mre as MemberReferenceExpression):
		#resolve the macro type we want to expand
		parentTypeName = BuildMacroTypeName(mre.Target)
		parent = NameResolutionService.ResolveQualifiedName(parentTypeName, EntityType.Type)
		if parent is null: #oh, this is a external extension maybe?
			parentTypeName = BuildMacroTypeName(mre.Target, true)
			parent = NameResolutionService.ResolveQualifiedName(parentTypeName, EntityType.Type)
		if parent is null:
			raise "No macro `${mre.Target.ToString()}` has been found to extend"

		elif parent isa InternalClass:
			#create macro type and add it as usual into its parent macro type
			type = CreateMacroType(UnpackReferences(mre.Target))
			cast(InternalClass, parent).TypeDefinition.Members.Add(type)

		elif parent isa ExternalType:
			#create macro type with parent(s)-based prefix then add an extension method for redirection
			type = CreateMacroType(BuildMacroTypeName(mre, true), UnpackReferences(mre.Target))
			module = _macro.GetAncestor[of Module]()
			module.Members.Add(type)
			method = CreateMacroExtensionProxy(parent, type.Name)
			module.Members.Add(method)

		else:
			raise "Cannot bind a nested macro extension to entity: ${parent}"


	private def CreateMacroExtensionProxy(parent as IType, extension as string):
		return [|
			[Boo.Lang.ExtensionAttribute]
			[System.Runtime.CompilerServices.CompilerGeneratedAttribute]
			static def $(_name)(parent as $(parent.FullName), context as Boo.Lang.Compiler.CompilerContext) as $(extension):
				return $(ReferenceExpression(extension))(context)
		|]


	private def CreateMacroType() as ClassDefinition:
		return CreateMacroType(BuildMacroTypeName(_name), GetParentMacroNames(_macro))

	private def CreateMacroType(parents as string*) as ClassDefinition:
		return CreateMacroType(BuildMacroTypeName(_name), parents)

	private def CreateMacroType(typeName as string, parents as string*) as ClassDefinition:
		if YieldFinder(_macro).Found:
			macroType = CreateGeneratorMacroType(typeName)
		else:
			macroType = CreateOldStyleMacroType(typeName)

		#add parent macro(s) accessor(s)
		for parent in parents:
			if not macroType.Members[parent]:
				for accessor in CreateParentMacroAccessor(parent):
					macroType.Members.Add(accessor)
		return macroType


	#BOO-1077 style
	private def CreateGeneratorMacroType(typeName as string) as ClassDefinition:
		arg = ReferenceExpression(_name)
		return [|
				final class $(typeName) (Boo.Lang.Compiler.LexicalInfoPreservingGeneratorMacro):
					[System.Runtime.CompilerServices.CompilerGeneratedAttribute]
					private __macro as Boo.Lang.Compiler.Ast.MacroStatement
					def constructor():
						super()
					def constructor(context as Boo.Lang.Compiler.CompilerContext):
						raise System.ArgumentNullException("context") if not context
						super(context)
					override protected def ExpandGeneratorImpl($_name as Boo.Lang.Compiler.Ast.MacroStatement) as Boo.Lang.Compiler.Ast.Node*:
						raise System.ArgumentNullException($_name) if not $arg
						self.__macro = $arg
						$(ExpandBody())
					[System.Runtime.CompilerServices.CompilerGeneratedAttribute]
					override protected def ExpandImpl($_name as Boo.Lang.Compiler.Ast.MacroStatement) as Boo.Lang.Compiler.Ast.Statement:
						raise System.NotImplementedException("Boo installed version is older than the new macro syntax '${$(_name)}' uses. Read BOO-1077 for more info.")
			|]

	private def CreateOldStyleMacroType(typeName as string) as ClassDefinition:
		arg = ReferenceExpression(_name)
		return [|
				final class $(typeName) (Boo.Lang.Compiler.LexicalInfoPreservingMacro):
					[System.Runtime.CompilerServices.CompilerGeneratedAttribute]
					private __macro as Boo.Lang.Compiler.Ast.MacroStatement
					def constructor():
						super()
					def constructor(context as Boo.Lang.Compiler.CompilerContext):
						raise System.ArgumentNullException("context") if not context
						super(context)
					override protected def ExpandImpl($_name as Boo.Lang.Compiler.Ast.MacroStatement) as Boo.Lang.Compiler.Ast.Statement:
						raise System.ArgumentNullException($_name) if not $arg
						self.__macro = $arg
						$(ExpandBody())
			|]


	private def ExpandBody():
		body = _macro.Body

		if _argumentsPattern:
			case = CaseStatement()
			case.Pattern = QuasiquoteExpression(pattern = MacroStatement(_name))
			pattern.Arguments = _argumentsPattern
			case.Body = body
			otherwise = OtherwiseStatement() #TODO: macro overload without arguments def
			otherwise.Body = [| raise "`${$(_name)}` macro invocation argument(s) did not match definition: `${$(_macro.Arguments[0].ToString())}`" |].ToBlock()
			body = [|
				$case
				$otherwise
			|]

		if ContainsCase(body):
			return ExpandWithPatternMatching(_name, body)
		return body


	#region PatternMatching
	private class CustomBlockStatement(CustomStatement):
		public Body as Block

	private class CaseStatement(CustomBlockStatement):
		public Pattern as Expression

	private class OtherwiseStatement(CustomBlockStatement):
		pass

	class CaseMacro(LexicalInfoPreservingGeneratorMacro):
		override protected def ExpandGeneratorImpl(case as MacroStatement):
			if len(case.Arguments) != 1:
				raise "Usage: case <pattern>"
			pattern, = case.Arguments
			yield CaseStatement(Pattern: pattern, Body: case.Body)

	class OtherwiseMacro(LexicalInfoPreservingGeneratorMacro):
		override protected def ExpandGeneratorImpl(case as MacroStatement):
			if len(case.Arguments) != 0:
				raise "Usage: otherwise: <block>"
			yield OtherwiseStatement(Body: case.Body)

	private static def ContainsCase(body as Block):
		for stmt in body.Statements:
			return true if stmt isa CaseStatement

	private static def ExpandWithPatternMatching(name as string, body as Block):
		matchBlock = [|
			match $(ReferenceExpression(name)):
				pass
		|]
		statementsEnumerator = body.Statements.GetEnumerator()
		while statementsEnumerator.MoveNext():
			stmt as Statement = statementsEnumerator.Current
			if stmt isa CaseStatement:
				case as CaseStatement = stmt
				caseBlock = [|
					case $(case.Pattern):
						$(case.Body)
				|]
				matchBlock.Body.Add(caseBlock)
			elif stmt isa OtherwiseStatement:
				otherwise as OtherwiseStatement = stmt
				otherwiseBlock = [|
					otherwise:
						$(otherwise.Body)
				|]
				matchBlock.Body.Add(otherwiseBlock)
				
				// otherwise marks the end of the match block
				resultingBlock = Block()
				resultingBlock.Add(matchBlock)
				for remaining as Statement in statementsEnumerator:
					resultingBlock.Add(remaining)
				return resultingBlock
			else:
				// statements after a sequence of 'case <pattern>' macros
				// also mark the end of a match block
				resultingBlock = Block()
				resultingBlock.Add(matchBlock)
				resultingBlock.Add(stmt)
				for remaining as Statement in statementsEnumerator:
					resultingBlock.Add(remaining)
				return resultingBlock
		return matchBlock
	#endregion


	private static def CreateParentMacroAccessor(name as string):
		cacheField = [|
			[System.Runtime.CompilerServices.CompilerGeneratedAttribute]
			private $("$" + name) as Boo.Lang.Compiler.Ast.MacroStatement
		|]
		yield cacheField
		
		cacheFieldRef = ReferenceExpression(cacheField.Name)
		prop = [|
			[System.Runtime.CompilerServices.CompilerGeneratedAttribute]
			private $name:
				get:
					$cacheFieldRef = __macro.GetParentMacroByName($name) unless $cacheFieldRef
					return $cacheFieldRef
		|]
		prop.IsSynthetic = true #avoid BCW0014 if not used
		yield prop


	private final class YieldFinder(DepthFirstVisitor):
		private _found = false

		def constructor(macro as MacroStatement):
			macro.Body.Accept(self)
			
		Found:
			get: return _found
			
		override def OnMethod(node as Method):
			pass
			
		override def OnCustomStatement(node as CustomStatement):
			customBlock = node as CustomBlockStatement
			if customBlock is null:
				super(node)
			else:
				customBlock.Body.Accept(self)

		override def OnYieldStatement(node as YieldStatement):
			_found = true

		override def EnterMacroStatement(node as MacroStatement) as bool:
			return false


	private final class ArgumentsPatternBuilder():
		_input as ExpressionCollection
		_output as ExpressionCollection
		_tss as TypeSystemServices
		_nrs as NameResolutionService

		def constructor(context as CompilerContext, input as ExpressionCollection):
			_tss = context.TypeSystemServices
			_nrs = context.NameResolutionService
			_input = input


		Output:
			get:
				Build() if not _output
				return _output

		TypeSystemServices:
			get: return _tss

		NameResolutionService:
			get: return _nrs


		def Build():
			_output = ExpressionCollection()
			for arg in _input:
				Append(arg) #FIXME: run-time (duck) dispatch here makes mono cry


		private def Append(e as Expression):
			if e.NodeType == NodeType.ReferenceExpression:
				Append(e as ReferenceExpression)
			elif e.NodeType == NodeType.TryCastExpression:
				Append(e as TryCastExpression)
			else:
				raise "Invalid macro argument declaration: `${e}`"


		private def Append(e as ReferenceExpression):
			#TODO: body pattern
			_output.Add(SpliceExpression([| $e = Boo.Lang.Compiler.Ast.Expression() |]))


		private def Append(e as TryCastExpression):
			re = e.Target as ReferenceExpression
			raise "Invalid macro argument name: `${e.Target}`" unless re

			NameResolutionService.ResolveTypeReference(e.Type)
			type = TypeSystemServices.GetType(e.Type)

			#TODO: body pattern
			#TODO: AppendArray() + AppendEnumerable() + >>AppendPrimitive()
			if type == TypeSystemServices.StringType:
				_output.Add(SpliceExpression([| Boo.Lang.Compiler.Ast.StringLiteralExpression(Value: $re) |]))

			elif type == TypeSystemServices.BoolType:
				_output.Add(SpliceExpression([| Boo.Lang.Compiler.Ast.BoolLiteralExpression(Value: $re) |]))

			elif type == TypeSystemServices.LongType or type == TypeSystemServices.ULongType:
				_output.Add(SpliceExpression([| Boo.Lang.Compiler.Ast.IntegerLiteralExpression(Value: $re, IsLong: true) |]))

			elif TypeSystemServices.IsIntegerNumber(type):
				_output.Add(SpliceExpression([| Boo.Lang.Compiler.Ast.IntegerLiteralExpression(Value: $re, IsLong: false) |]))

			elif type == TypeSystemServices.SingleType:
				_output.Add(SpliceExpression([| Boo.Lang.Compiler.Ast.DoubleLiteralExpression(Value: $re, IsSingle: true) |]))

			elif type == TypeSystemServices.DoubleType:
				_output.Add(SpliceExpression([| Boo.Lang.Compiler.Ast.DoubleLiteralExpression(Value: $re, IsSingle: false) |]))

			else:
				Append(re)

