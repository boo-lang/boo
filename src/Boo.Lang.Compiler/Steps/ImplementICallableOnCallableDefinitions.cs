#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Compiler.Steps
{
	using System;
	using System.Diagnostics;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class ImplementICallableOnCallableDefinitions : AbstractVisitorCompilerStep
	{
		override public void Run()
		{
			Visit(CompileUnit);
		}
		
		override public void OnModule(Module node)
		{
			Visit(node.Members, NodeType.ClassDefinition);
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{
			Visit(node.Members, NodeType.ClassDefinition);
			
			InternalCallableType type = node.Entity as InternalCallableType;
			if (null != type)
			{
				ImplementICallableCall(type, node);
			}
		}
		
		void ImplementICallableCall(InternalCallableType type, ClassDefinition node)
		{
			Method call = (Method)node.Members["Call"];
			Debug.Assert(null != call);
			Debug.Assert(0 == call.Body.Statements.Count);
			
			CallableSignature signature = type.GetSignature();
			MethodInvocationExpression mie = CreateMethodInvocation(CreateSelfReference(type),
													type.GetInvokeMethod());
							
			IParameter[] parameters = signature.Parameters;
			if (parameters.Length > 0)
			{
				ReferenceExpression args = CreateParameterReference(call.Parameters[0]);
				for (int i=0; i<parameters.Length; ++i)
				{
					mie.Arguments.Add(CreateSlicing(args.CloneNode(), i));
				}
			}
			
			if (TypeSystemServices.VoidType == signature.ReturnType)
			{
				call.Body.Add(mie);
			}
			else
			{
				call.Body.Add(new ReturnStatement(mie));
			}
		}
		
		SlicingExpression CreateSlicing(Expression target, int begin)
		{
			SlicingExpression expression = new SlicingExpression(target,
												new IntegerLiteralExpression(begin));
			BindExpressionType(expression, TypeSystemServices.ObjectType);
			BindExpressionType(expression.Begin, TypeSystemServices.IntType);
			return expression;
		}
		
		ReferenceExpression CreateParameterReference(ParameterDeclaration parameter)
		{
			InternalParameter entity = (InternalParameter)GetEntity(parameter);
			ReferenceExpression reference = new ReferenceExpression(parameter.Name);
			Bind(reference, entity);
			BindExpressionType(reference, entity.Type);
			return reference;
		}
	}
}
