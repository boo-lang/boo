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

namespace Boo.Lang.Compiler.Steps
{
	using System;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
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
				node.ReturnType = CodeBuilder.CreateTypeReference(TypeSystemServices.VoidType);
			}
			
			foreach (ParameterDeclaration parameter in node.Parameters)
			{
				if (null == parameter.Type)
				{
					parameter.Type = CodeBuilder.CreateTypeReference(TypeSystemServices.ObjectType);
				}
			}
			
			ClassDefinition cd = TypeSystemServices.CreateCallableDefinition(node.Name);
			cd.LexicalInfo = node.LexicalInfo;
			cd.Members.Add(CreateInvokeMethod(node));
			cd.Members.Add(CreateBeginInvokeMethod(node));
			cd.Members.Add(CreateEndInvokeMethod(node));
			ReplaceCurrentNode(cd);
		}
		
		Method CreateInvokeMethod(CallableDefinition node)
		{
			Method method = CreateRuntimeMethod("Invoke", node.ReturnType);
			method.Parameters = node.Parameters;
			return method;
		}
		
		Method CreateBeginInvokeMethod(CallableDefinition node)
		{
			Method method = CreateRuntimeMethod("BeginInvoke",
						CodeBuilder.CreateTypeReference(typeof(IAsyncResult)));
			method.Parameters.ExtendWithClones(node.Parameters);
			method.Parameters.Add(
				new ParameterDeclaration("callback",
					CodeBuilder.CreateTypeReference(typeof(AsyncCallback))));
			method.Parameters.Add(
				new ParameterDeclaration("asyncState",
					CodeBuilder.CreateTypeReference(TypeSystemServices.ObjectType)));
			return method;
		}
		
		Method CreateEndInvokeMethod(CallableDefinition node)
		{
			Method method = CreateRuntimeMethod("EndInvoke", node.ReturnType);
			method.Parameters.Add(
				new ParameterDeclaration("asyncResult",
					CodeBuilder.CreateTypeReference(typeof(IAsyncResult))));
			return method;
		}
		
		Method CreateRuntimeMethod(string name, TypeReference returnType)
		{
			Method method = new Method();
			method.Name = name;
			method.ReturnType = returnType;
			method.Modifiers = TypeMemberModifiers.Public|TypeMemberModifiers.Virtual;
			method.ImplementationFlags = MethodImplementationFlags.Runtime;
			return method;
		}
	}
}
