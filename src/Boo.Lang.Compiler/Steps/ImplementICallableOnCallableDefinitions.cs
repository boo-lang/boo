#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
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
