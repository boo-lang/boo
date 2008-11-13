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


class MacroMacro(AbstractAstMacro):

	override def Expand(macro as MacroStatement) as Statement:
		if len(macro.Arguments) != 1 or macro.Arguments[0].NodeType != NodeType.ReferenceExpression:
			raise System.ArgumentException("Usage: macro <reference>", "reference")
		klass = CreateMacroType(macro)
		klass.LexicalInfo = macro.LexicalInfo
		#TODO: create macro as a nested class of the current type
		#      if not at module-level ? (=> macro namespaces)
		EnclosingModule(macro).Members.Add(klass)
		return null


	private def PascalCase(name as string) as string:
		return char.ToUpper(name[0]) + name[1:]


	private def CreateMacroType(macro as MacroStatement) as ClassDefinition:
		name = (macro.Arguments[0] as ReferenceExpression).Name
		newStyle = YieldFinder(macro).Found
		return CreateNewStyleMacroType(name, macro) if newStyle
		return CreateOldStyleMacroType(name, macro)


	#BOO-1077 style
	private def CreateNewStyleMacroType(name as string, macro as MacroStatement) as ClassDefinition:
		return [|
				class $(PascalCase(name) + "Macro") (Boo.Lang.Compiler.LexicalInfoPreservingGeneratorMacro):
					def constructor():
						super()
					def constructor(context as Boo.Lang.Compiler.CompilerContext):
						raise System.ArgumentNullException("context") if not context
						super(context)
					override protected def ExpandGeneratorImpl($name as Boo.Lang.Compiler.Ast.MacroStatement) as Boo.Lang.Compiler.Ast.Node*:
						raise System.ArgumentNullException($name) if not $(macro.Arguments[0])
						$(macro.Block)
					[System.Runtime.CompilerServices.CompilerGeneratedAttribute]
					override protected def ExpandImpl($name as Boo.Lang.Compiler.Ast.MacroStatement) as Boo.Lang.Compiler.Ast.Statement:
						raise System.ArgumentNullException($name) if not $(macro.Arguments[0])
						raise System.NotImplementedException("Boo installed version is older than the new macro syntax '${$(name)}' uses. Read BOO-1077 for more info.")
			|]


	private def CreateOldStyleMacroType(name as string, macro as MacroStatement) as ClassDefinition:
		return [|
				class $(PascalCase(name) + "Macro") (Boo.Lang.Compiler.LexicalInfoPreservingMacro):
					def constructor():
						super()
					def constructor(context as Boo.Lang.Compiler.CompilerContext):
						raise System.ArgumentNullException("context") if not context
						super(context)
					override protected def ExpandImpl($name as Boo.Lang.Compiler.Ast.MacroStatement) as Boo.Lang.Compiler.Ast.Statement:
						raise System.ArgumentNullException($name) if not $(macro.Arguments[0])
						$(macro.Block)
			|]


	private def EnclosingModule(macro as Node) as Module:
		return macro.GetAncestor(NodeType.Module)

	private class YieldFinder(DepthFirstVisitor):
		Found as bool:
			get:
				return _found
		_found = false

		def constructor(macro as MacroStatement):
			super.OnMacroStatement(macro)

		override def OnYieldStatement(node as YieldStatement):
			_found = true if _macroDepth == 1

		override def EnterMacroStatement(node as MacroStatement) as bool:
			_macroDepth++
			return true

		override def LeaveMacroStatement(node as MacroStatement):
			_macroDepth--

		_macroDepth = 0

