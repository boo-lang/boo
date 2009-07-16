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
	using System.Diagnostics;
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
		
		int GetByRefParamCount(CallableSignature signature)
		{
			int count = 0;
			foreach (IParameter param in signature.Parameters)
			{
				if (param.IsByRef) ++count;
			}
			return count;
		}
		
		void ImplementICallableCall(InternalCallableType type, ClassDefinition node)
		{
			Method call = (Method)node.Members["Call"];
			Debug.Assert(null != call);
			Debug.Assert(call.Body.IsEmpty);
						
			CallableSignature signature = type.GetSignature();
			int byRefCount = GetByRefParamCount(signature);
			if (byRefCount > 0)
			{
				ImplementByRefICallableCall(call, type, node, signature, byRefCount);
			}
			else
			{
				ImplementRegularICallableCall(call, type, node, signature);
			}
		}
		
		void ImplementByRefICallableCall(
									Method call,
									InternalCallableType type,
									ClassDefinition node,
									CallableSignature signature,
									int byRefCount)
		{			
			MethodInvocationExpression mie = CreateInvokeInvocation(type);
			IParameter[] parameters = signature.Parameters;			
			ReferenceExpression args = CodeBuilder.CreateReference(call.Parameters[0]);
			InternalLocal[] temporaries = new InternalLocal[byRefCount];
			
			int byRefIndex = 0;
			for (int i=0; i<parameters.Length; ++i)
			{				
				SlicingExpression slice = CodeBuilder.CreateSlicing(args.CloneNode(), i);

				IParameter parameter = parameters[i];				
				if (parameter.IsByRef)
				{
					IType tempType = parameter.Type;
					if (tempType.IsByRef)
					{
						tempType = tempType.GetElementType();
					}
					temporaries[byRefIndex] = CodeBuilder.DeclareLocal(call,
								"__temp_" + parameter.Name,
								tempType);
								
					call.Body.Add(
						CodeBuilder.CreateAssignment(
						CodeBuilder.CreateReference(temporaries[byRefIndex]),
							CodeBuilder.CreateCast(
								tempType,
								slice)));
						
					mie.Arguments.Add(
						CodeBuilder.CreateReference(
							temporaries[byRefIndex]));
					
					++byRefIndex;
				}
				else
				{
					mie.Arguments.Add(slice);
				}
			}
			
			if (TypeSystemServices.VoidType == signature.ReturnType)
			{
				call.Body.Add(mie);
				PropagateByRefParameterChanges(call, parameters, temporaries);
			}
			else
			{
				InternalLocal invokeReturnValue = CodeBuilder.DeclareLocal(call,
							"__returnValue", signature.ReturnType);
				call.Body.Add(
					CodeBuilder.CreateAssignment(
						CodeBuilder.CreateReference(invokeReturnValue),
						mie));
				PropagateByRefParameterChanges(call, parameters, temporaries);
				call.Body.Add(
					new ReturnStatement(
						CodeBuilder.CreateReference(invokeReturnValue)));
			}
		}
		
		void PropagateByRefParameterChanges(Method call, IParameter[] parameters, InternalLocal[] temporaries)
		{
			int byRefIndex = 0;
			for (int i=0; i<parameters.Length; ++i)
			{
				if (!parameters[i].IsByRef) continue;
				
				SlicingExpression slice = CodeBuilder.CreateSlicing(
											CodeBuilder.CreateReference(call.Parameters[0]),
											i);						
				call.Body.Add(
					CodeBuilder.CreateAssignment(
						slice,
						CodeBuilder.CreateReference(temporaries[byRefIndex])));
				++byRefIndex;
			}
		}
		
		void ImplementRegularICallableCall(
									Method call,
									InternalCallableType type,
									ClassDefinition node,
									CallableSignature signature)
		{	
			MethodInvocationExpression mie = CreateInvokeInvocation(type);
			IParameter[] parameters = signature.Parameters;
			int fixedParametersLength = signature.AcceptVarArgs ? parameters.Length - 1 : parameters.Length;
			for (int i=0; i<fixedParametersLength; ++i)
			{
				SlicingExpression slice = CodeBuilder.CreateSlicing(
							CodeBuilder.CreateReference(call.Parameters[0]),
							i);
				mie.Arguments.Add(slice);
			}

			if (signature.AcceptVarArgs)
			{
				if (parameters.Length == 1)
				{
					mie.Arguments.Add(CodeBuilder.CreateReference(call.Parameters[0]));
				}
				else
				{
					mie.Arguments.Add(
						CodeBuilder.CreateMethodInvocation(
							RuntimeServices_GetRange1,
							CodeBuilder.CreateReference(call.Parameters[0]),
							CodeBuilder.CreateIntegerLiteral(fixedParametersLength)));
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
		
		MethodInvocationExpression CreateInvokeInvocation(InternalCallableType type)
		{
			return CodeBuilder.CreateMethodInvocation(
								CodeBuilder.CreateSelfReference(type),
								type.GetInvokeMethod());
		}

		IMethod _RuntimeServices_GetRange1;

		IMethod RuntimeServices_GetRange1
		{
			get
			{
				if (null == _RuntimeServices_GetRange1)
				{
					_RuntimeServices_GetRange1 = NameResolutionService.ResolveMethod(TypeSystemServices.RuntimeServicesType, "GetRange1");
				}
				return _RuntimeServices_GetRange1;
			}
		}
	}
}
