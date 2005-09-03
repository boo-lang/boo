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
		
		void ImplementICallableCall(InternalCallableType type, ClassDefinition node)
		{
			Method call = (Method)node.Members["Call"];
			Debug.Assert(null != call);
			Debug.Assert(0 == call.Body.Statements.Count);
						
			CallableSignature signature = type.GetSignature();
			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
								CodeBuilder.CreateSelfReference(type),
								type.GetInvokeMethod());
			
			int byrefcount = 0;
			ILocalEntity invokeresults = null;
			ILocalEntity[] tempvals;
			IParameter[] parameters = signature.Parameters;
			ReferenceExpression args = null;
			
			foreach(IParameter param in parameters)
			{
				if (param.IsByRef)
				{
					++byrefcount;
				}
			}
			
			tempvals = new InternalLocal[byrefcount];
			
			if (parameters.Length > 0)
			{
				args = CodeBuilder.CreateReference(call.Parameters[0]);
			}
			
			int byrefindex = 0;
			for (int i=0; i<parameters.Length; ++i)
			{
				SlicingExpression slice = CodeBuilder.CreateSlicing(args.CloneNode(), i);
				
				if (parameters[i].IsByRef)
				{
						tempvals[byrefindex] = CodeBuilder.DeclareLocal(call,
									"__temp_"+parameters[i].Name,
									parameters[i].Type);
									
						call.Body.Add(
							CodeBuilder.CreateAssignment(
							CodeBuilder.CreateReference(tempvals[byrefindex]),
								CodeBuilder.CreateCast(
									parameters[i].Type,
									slice)));
							
						mie.Arguments.Add(CodeBuilder.CreateReference(tempvals[byrefindex]));
						
						++byrefindex;
				}
				else
				{
					mie.Arguments.Add(slice);
				}
			}
			
			if (TypeSystemServices.VoidType == signature.ReturnType)
			{
				call.Body.Add(mie);
			}
			else if (byrefcount > 0)
			{
				invokeresults = CodeBuilder.DeclareLocal(call,
							"__result", signature.ReturnType);
				call.Body.Add(
					CodeBuilder.CreateAssignment(
						CodeBuilder.CreateReference(invokeresults),
						mie));
			}
			
			byrefindex = 0;
			for (int i=0; i<parameters.Length; ++i)
			{
				if (parameters[i].IsByRef)
				{
						SlicingExpression slice = CodeBuilder.CreateSlicing(args.CloneNode(), i);
						
						call.Body.Add(
							CodeBuilder.CreateAssignment(
							slice,
							CodeBuilder.CreateReference(tempvals[byrefindex])));
						
						++byrefindex;
				}
			}
			
			if (TypeSystemServices.VoidType != signature.ReturnType)
			{
				if (byrefcount > 0)
				{
					call.Body.Add(new ReturnStatement(
						CodeBuilder.CreateReference(invokeresults)));
				}
				else
				{
					call.Body.Add(new ReturnStatement(mie));
				}
			}
		}
	}
}
