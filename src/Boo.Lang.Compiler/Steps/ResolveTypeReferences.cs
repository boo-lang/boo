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

namespace Boo.Lang.Compiler.Steps
{
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class ResolveTypeReferences : AbstractNamespaceSensitiveVisitorCompilerStep
	{
		public ResolveTypeReferences()
		{
		}

		override public void Run()
		{
			Visit(CompileUnit.Modules);
		}

		override public void OnArrayTypeReference(ArrayTypeReference node)
		{
			NameResolutionService.ResolveArrayTypeReference(node);
		}
		
		override public void OnSimpleTypeReference(SimpleTypeReference node)
		{
			NameResolutionService.ResolveSimpleTypeReference(node);
			if (node.Entity is InternalCallableType)
			{
				//EnsureRelatedNodeWasVisited(node.Entity);
			}
		}
		
		override public void LeaveCallableTypeReference(CallableTypeReference node)
		{
			IParameter[] parameters = new IParameter[node.Parameters.Count];
			for (int i=0; i<parameters.Length; ++i)
			{
				parameters[i] = new SimpleParameter("arg" + i, GetType(node.Parameters[i]));
			}
			
			IType returnType = null;
			if (null != node.ReturnType)
			{
				returnType = GetType(node.ReturnType);
			}
			else
			{
				returnType = TypeSystemServices.VoidType;
			}
			
			node.Entity = TypeSystemServices.GetConcreteCallableType(node, new CallableSignature(parameters, returnType));
		}

	}
}
