#region license
// Copyright (c) 2009, Rodrigo B. de Oliveira (rbo@acm.org)
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

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

final class UnsafeMacro (AbstractAstGeneratorMacro):

	private static final Usage = "Usage: `unsafe [<ptrName> as <ptrType> = <data>]+'"

	override public def Expand(macro as MacroStatement) as Statement:
		raise NotSupportedException("boo version is too old to use this macro")

	override public def ExpandGenerator(macro as MacroStatement) as Node*:
		if not Context.Parameters.Unsafe:
			raise "`unsafe' requires unsafe code support to be enabled (e.g: booc -unsafe)"

		nArgs = len(macro.Arguments)
		raise Usage if not nArgs

		for arg in macro.Arguments:
			assert IsPointerInitializer(arg), Usage
			
			# [| $ptrName as $ptrType = $data |]
			declaration as BinaryExpression = arg
			lhs as TryCastExpression = declaration.Left
			ptrName = lhs.Target
			ptrType = lhs.Type
			data = declaration.Right
			
			indirector = Indirector(macro, ptrName)
			if 0 == indirector._count and nArgs == 1:
				#warn if there is only one arg (ignore otherwise since pointer arithmetic is probably used)
				Context.Warnings.Add(
					CompilerWarningFactory.CustomWarning(macro.LexicalInfo,
						"Pointer `${$(ptrName)}' is never dereferenced. This `unsafe' block is probably useless."))

			ptrType.IsPointer = true
			ptrDecl = Declaration(indirector._privateName, ptrType)
			yield DeclarationStatement(ptrDecl, UnaryExpression(data.LexicalInfo, UnaryOperatorType.AddressOf, data))

		if not len(macro.Body.Statements):
			raise "`unsafe` is useless without a body"

		yield
		
	private def IsPointerInitializer(e as Expression):
		assignment = e as BinaryExpression
		if assignment is null:
			return false
		
		if assignment.Operator != BinaryOperatorType.Assign:
			return false
			
		if not assignment.Left isa TryCastExpression:
			return false
			
		return true

	private class Indirector (DepthFirstVisitor):
	"""
	Replaces all occurences of explode operator on the declared pointer variable
	with indirection operators within the unsafe context.
	"""
		public _count as int
		public _name as string
		public _privateName as string

		def constructor(macro as MacroStatement, name as ReferenceExpression):
			_name = name.Name
			_privateName = CompilerContext.Current.GetUniqueName("unsafe", name.Name);
			macro.Body.Accept(self)

		override def OnReferenceExpression(node as ReferenceExpression):
			return if node.Name != _name

			node.Name = _privateName
			ue = node.GetAncestor[of UnaryExpression]()
			if ue and ue.Operator == UnaryOperatorType.Explode:
				ue.Operator = UnaryOperatorType.Indirection
				_count++

