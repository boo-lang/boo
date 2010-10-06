#region license
// Copyright (c) 2010 Rodrigo B. de Oliveira (rbo@acm.org)
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
import Boo.Lang.Compiler.Ast

macro property:

	raise "property <name> [as type] [= initialValue]" unless PropertyMacroParser.IsValidProperty(property)
	
	argument = property.Arguments[0]
	initializationForm = argument as BinaryExpression
	if initializationForm is not null:
		declaration = initializationForm.Left
		initializer = initializationForm.Right
	else:
		declaration = argument
		initializer = null
		
	name = PropertyMacroParser.PropertyNameFrom(declaration)
	type = PropertyMacroParser.PropertyTypeFrom(declaration)
	backingField = ReferenceExpression(Name: Context.GetUniqueName(name.ToString()))
		
	prototype = [|
	
		class _:
		
			private $backingField as $type = $initializer
			
			$name:
				get: return $backingField
				set: $backingField = value
	|]
	
	yieldAll prototype.Members
	
internal static class PropertyMacroParser:
	
	def PropertyNameFrom(e as Expression) as ReferenceExpression:
		declaration = e as TryCastExpression
		if declaration is not null:
			return declaration.Target
		return e
		
	def PropertyTypeFrom(e as Expression):
		declaration = e as TryCastExpression
		if declaration is not null:
			return declaration.Type
		return null
		
	def IsValidProperty(property as MacroStatement):
		if len(property.Arguments) != 1:
			return false
		if not property.Body.IsEmpty:
			return false
		argument = property.Arguments[0]
		initializationForm = argument as BinaryExpression
		if initializationForm is not null:
			if initializationForm.Operator != BinaryOperatorType.Assign:
				return false
			return IsValidPropertyDeclaration(initializationForm.Left)
		return IsValidPropertyDeclaration(argument)
		
	def IsValidPropertyDeclaration(e as Expression):
		if e.NodeType == NodeType.ReferenceExpression:
			return true
		declaration = e as TryCastExpression
		if declaration is null: return false
		return declaration.Target.NodeType == NodeType.ReferenceExpression
