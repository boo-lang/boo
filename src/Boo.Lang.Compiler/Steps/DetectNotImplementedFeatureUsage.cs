#region license
// Copyright (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)
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

using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	public class DetectNotImplementedFeatureUsage : AbstractFastVisitorCompilerStep
	{
		private IType _currentType;
		private IMethod _currentMethod;

		public override void Run()
		{
			if (Errors.Count > 0)
				return;
			base.Run();
		}

		public override void OnClassDefinition(ClassDefinition node)
		{
			OnTypeDefinition(node);
		}

		public override void OnInterfaceDefinition(InterfaceDefinition node)
		{
			OnTypeDefinition(node);
		}

		public override void OnStructDefinition(StructDefinition node)
		{
			OnTypeDefinition(node);
		}

		private void OnTypeDefinition(TypeDefinition node)
		{
			var old = _currentType;
			_currentType = (IType)node.Entity;
			Visit(node.Attributes);
			Visit(node.BaseTypes);
			Visit(node.Members);
			Visit(node.GenericParameters);
			_currentType = old;
		}

		public override void OnMethod(Method node)
		{
			var old = _currentMethod;
			_currentMethod = (IMethod) node.Entity;
			base.OnMethod(node);
			_currentMethod = old;
		}

		public override void OnReferenceExpression(ReferenceExpression node)
		{
			CheckForInvalidGenericParameterReferenceUsage(node);
		}

		public override void OnSimpleTypeReference(SimpleTypeReference node)
		{
			CheckForInvalidGenericParameterReferenceUsage(node);
		}

		private void CheckForInvalidGenericParameterReferenceUsage(Node node)
		{
			var genericParameterRef = node.Entity as IGenericParameter;
			if (genericParameterRef == null)
				return;

			var declaringEntity = genericParameterRef.DeclaringEntity;
			if (declaringEntity == _currentMethod || declaringEntity == _currentType)
				return;

            var currentTypeNode = ((IInternalEntity)_currentType).Node as TypeDefinition;
            var currentTypeParameter = currentTypeNode.GenericParameters.FirstOrDefault(p => p.Name.Equals(genericParameterRef.Name));
            if (currentTypeParameter != null && currentTypeParameter.Entity is IGenericParameter)
            {
                node.Entity = currentTypeParameter.Entity;
                return;
            }

			Errors.Add(CompilerErrorFactory.NotImplemented(node, "referencing generic parameter of outer type"));
		}
	}
}
