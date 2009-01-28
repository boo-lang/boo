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

	private static final Usage = "Usage: `macro [<parent>+.]<name>`"

	override protected def ExpandGeneratorImpl(macro as MacroStatement):
		raise Usage if len(macro.Arguments) != 1

		arg = macro.Arguments[0]
		if arg.NodeType == NodeType.ReferenceExpression:
			yield CreateMacroType(macro)
		elif arg.NodeType == NodeType.MemberReferenceExpression:
			if macro.GetAncestor[of MacroStatement]() is not null:
				raise "Nested macro extension cannot be itself a nested macro"
			CreateMacroExtensionType(macro)
		else:
			raise Usage


	private static def BuildMacroTypeName(name as string) as string:
		return char.ToUpper(name[0]) + name[1:] + "Macro"

	private static def BuildMacroTypeName(name as Expression) as string:
		return BuildMacroTypeName(name, false)

	private static def BuildMacroTypeName(name as Expression, external as bool) as string:
		sb = System.Text.StringBuilder()
		for part in UnpackReferences(name):
			if sb.Length:
				sb.Append(char('.'))
				sb.Append(char('$')) if external
			sb.Append(BuildMacroTypeName(part))
		return sb.ToString()


	private static def UnpackReferences(refs as Expression) as string*:
		if refs.NodeType == NodeType.ReferenceExpression:
			re = cast(ReferenceExpression, refs)
			yield re.Name
		elif refs.NodeType == NodeType.MemberReferenceExpression:
			mre = cast(MemberReferenceExpression, refs)
			for name in UnpackReferences(mre.Target):
				yield name
			yield mre.Name
		else:
			raise "Invalid node type: ${refs.NodeType}"


	private static def GetParentMacroNames(macro as MacroStatement) as string*:
		parent = macro.GetAncestor[of MacroStatement]()
		while parent is not null:
			continue if parent.Name != 'macro'

			arg = parent.Arguments[0]
			if arg.NodeType == NodeType.ReferenceExpression:
				yield cast(ReferenceExpression, arg).Name
			elif arg.NodeType == NodeType.MemberReferenceExpression:
				for name in UnpackReferences(cast(MemberReferenceExpression, arg)):
					yield name

			parent = parent.GetAncestor[of MacroStatement]()


	private def CreateMacroExtensionType(macro as MacroStatement):
		mre = cast(MemberReferenceExpression, macro.Arguments[0])

		#resolve the macro type we want to expand
		parentTypeName = BuildMacroTypeName(mre.Target)
		entity = NameResolutionService.ResolveQualifiedName(parentTypeName)

		if entity is null:
			raise "No macro `${mre.Target.ToCodeString()}` has been found to extend"
		elif entity isa InternalClass:
			#add macro as usual to parent macro type
			type = CreateMacroType(macro, mre.Name, UnpackReferences(mre.Target))
			cast(InternalClass, entity).TypeDefinition.Members.Add(type)
		else:
			#TODO:
			raise "External macro extension is not implemented yet"


	private static def CreateMacroType(macro as MacroStatement) as ClassDefinition:
		name = (macro.Arguments[0] as ReferenceExpression).Name
		return CreateMacroType(macro, name, GetParentMacroNames(macro))


	private static def CreateMacroType(macro as MacroStatement, name as string, parents as string*) as ClassDefinition:
		if YieldFinder(macro).Found:
			macroType = CreateNewStyleMacroType(name, macro)
		else:
			macroType = CreateOldStyleMacroType(name, macro)

		#add parent macro(s) accessor(s)
		for parent in parents:
			if not macroType.Members[parent]:
				for accessor in CreateParentMacroAccessor(parent):
					macroType.Members.Add(accessor)

		return macroType


	#BOO-1077 style
	private static def CreateNewStyleMacroType(name as string, macro as MacroStatement) as ClassDefinition:
		arg = ReferenceExpression(name)
		return [|
				final class $(BuildMacroTypeName(name)) (Boo.Lang.Compiler.LexicalInfoPreservingGeneratorMacro):
					private __macro as Boo.Lang.Compiler.Ast.MacroStatement
					def constructor():
						super()
					def constructor(context as Boo.Lang.Compiler.CompilerContext):
						raise System.ArgumentNullException("context") if not context
						super(context)
					override protected def ExpandGeneratorImpl($name as Boo.Lang.Compiler.Ast.MacroStatement) as Boo.Lang.Compiler.Ast.Node*:
						raise System.ArgumentNullException($name) if not $name
						self.__macro = $arg
						$(ExpandBody(name, macro.Body))
					[System.Runtime.CompilerServices.CompilerGeneratedAttribute]
					override protected def ExpandImpl($name as Boo.Lang.Compiler.Ast.MacroStatement) as Boo.Lang.Compiler.Ast.Statement:
						raise System.NotImplementedException("Boo installed version is older than the new macro syntax '${$(name)}' uses. Read BOO-1077 for more info.")
			|]

	private static def CreateOldStyleMacroType(name as string, macro as MacroStatement) as ClassDefinition:
		arg = ReferenceExpression(name)
		return [|
				final class $(BuildMacroTypeName(name)) (Boo.Lang.Compiler.LexicalInfoPreservingMacro):
					private __macro as Boo.Lang.Compiler.Ast.MacroStatement
					def constructor():
						super()
					def constructor(context as Boo.Lang.Compiler.CompilerContext):
						raise System.ArgumentNullException("context") if not context
						super(context)
					override protected def ExpandImpl($name as Boo.Lang.Compiler.Ast.MacroStatement) as Boo.Lang.Compiler.Ast.Statement:
						raise System.ArgumentNullException($name) if not $name
						self.__macro = $arg
						$(ExpandBody(name, macro.Body))
			|]

	private static def ExpandBody(name as string, body as Block):
		if ContainsCase(body):
			return ExpandWithPatternMatching(name, body)
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
			private $("__" + name) as Boo.Lang.Compiler.Ast.MacroStatement
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

