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

namespace Boo.Web

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

class ViewStateAttribute(AbstractAstAttribute):

	[property(Default)]
	_default as Expression
			
	override def Apply(node as Node):
		
		assert node isa Field			
		
		f as Field = node
		
		p = Property(Name: f.Name, Type: f.Type)
		p.Getter = CreateGetter(f)
		p.Setter = CreateSetter(f)

		f.ParentNode.Replace(f, p)

	protected def CreateGetter(f as Field):

		getter = Method()
		getter.ReturnType = f.Type
		
		if _default:
			
			// value = ViewState[<FieldName>]
			getter.Body.Add(BinaryExpression(
							BinaryOperatorType.Assign,
							ReferenceExpression("value"),
							CreateViewStateSlice(f)))							
			// return value if value 
			getter.Body.Add(
					ReturnStatement(							
							ReferenceExpression("value"),
								StatementModifier(
									StatementModifierType.If,
									ReferenceExpression("value"))))
			// return <default>
			getter.Body.Add(ReturnStatement(_default))	
		else:			
			// return ViewState[<FieldName>]
			getter.Body.Add(ReturnStatement(CreateViewStateSlice(f)))
		
		return getter
		
	protected def CreateSetter(f as Field):
		
		setter = Method()
		
		// ViewState[<FieldName>] = value
		setter.Body.Add(
			BinaryExpression(
						BinaryOperatorType.Assign,
						CreateViewStateSlice(f),
						ReferenceExpression("value")))
		
		return setter
		
	protected def CreateViewStateSlice(f as Field):
		// ViewState["<f.Name>"]
		slice = SlicingExpression()
		slice.Target = ReferenceExpression("ViewState")
		slice.Indices.Add(
			Slice(StringLiteralExpression(f.Name)))
		return slice						
