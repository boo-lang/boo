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

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem.Builders;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Steps
{

	public class TransformCallableDefinitions : AbstractTransformerCompilerStep, ITypeMemberReifier
	{	
		override public void OnMethod(Method node)
		{
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{
			Visit(node.Members);
		}
		
		override public void OnCallableDefinition(CallableDefinition node)
		{
			CompleteOmittedReturnType(node);
			CompleteOmittedParameterTypes(node);

			ClassDefinition cd = My<CallableTypeBuilder>.Instance.ForCallableDefinition(node);
			ReplaceCurrentNode(cd);
		}

		private void CompleteOmittedReturnType(CallableDefinition node)
		{
			if (node.ReturnType == null)
				node.ReturnType = CodeBuilder.CreateTypeReference(TypeSystemServices.VoidType);
		}

		private void CompleteOmittedParameterTypes(CallableDefinition node)
		{
			ParameterDeclarationCollection parameters = node.Parameters;
			if (parameters.Count == 0)
				return;

			foreach (ParameterDeclaration parameter in parameters)
			{
				if (parameter.Type != null)
					continue;

				parameter.Type = CodeBuilder.CreateTypeReference(
					parameter.IsParamArray
						? TypeSystemServices.ObjectArrayType
						: TypeSystemServices.ObjectType);
			}
		}

		public TypeMember Reify(TypeMember node)
		{
			return Visit(node);
		}
	}
}
