#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
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
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	
	public class TransformCallableDefinitions : AbstractTransformerCompilerStep
	{
		override public void Run()
		{
			Visit(CompileUnit);
		}
		
		override public void OnMethod(Method node)
		{
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{
			Visit(node.Members);
		}
		
		override public void OnCallableDefinition(CallableDefinition node)
		{
			if (null == node.ReturnType)
			{
				node.ReturnType = CreateTypeReference(TypeSystemServices.VoidType);
			}
			
			foreach (ParameterDeclaration parameter in node.Parameters)
			{
				if (null == parameter.Type)
				{
					parameter.Type = CreateTypeReference(TypeSystemServices.ObjectType);
				}
			}
			
			ClassDefinition cd = new ClassDefinition(node.LexicalInfo);
			cd.BaseTypes.Add(CreateTypeReference(TypeSystemServices.MulticastDelegateType));
			cd.Name = node.Name;
			cd.Modifiers = TypeMemberModifiers.Final;
			cd.Members.Add(CreateCallableConstructor());
			cd.Members.Add(CreateInvokeMethod(node));		
			
			ReplaceCurrentNode(cd);
			
			cd.Entity = new TypeSystem.InternalCallableType(TypeSystemServices, cd);
		}
		
		Constructor CreateCallableConstructor()
		{
			Constructor constructor = new Constructor();
			constructor.Modifiers = TypeMemberModifiers.Public;
			constructor.ImplementationFlags = MethodImplementationFlags.Runtime;
			constructor.Parameters.Add(
						new ParameterDeclaration("instance", CreateTypeReference(TypeSystemServices.ObjectType)));
			constructor.Parameters.Add(
						new ParameterDeclaration("method", CreateTypeReference(TypeSystemServices.IntPtrType)));						
			return constructor;
		}
		
		Method CreateInvokeMethod(CallableDefinition node)
		{
			Method method = new Method("Invoke");
			method.Modifiers = TypeMemberModifiers.Public|TypeMemberModifiers.Virtual;
			method.ImplementationFlags = MethodImplementationFlags.Runtime;
			method.Parameters = node.Parameters;
			method.ReturnType = node.ReturnType;
			return method;
		}
	}
}
