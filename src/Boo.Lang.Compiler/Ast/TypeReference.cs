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

using System;

namespace Boo.Lang.Compiler.Ast
{
	[System.Xml.Serialization.XmlInclude(typeof(SimpleTypeReference))]
	[System.Xml.Serialization.XmlInclude(typeof(ArrayTypeReference))]
	[System.Xml.Serialization.XmlInclude(typeof(GenericTypeReference))]
	[System.Xml.Serialization.XmlInclude(typeof(GenericTypeDefinitionReference))]
	public abstract partial class TypeReference
	{	
		public static TypeReference Lift(System.Type type)
		{
			return new SimpleTypeReference(Boo.Lang.Compiler.Util.TypeUtilities.GetFullName(type));
		}
		
		public static TypeReference Lift(string name)
		{
			return new SimpleTypeReference(name);
		}
		
		public static TypeReference Lift(TypeReference typeRef)
		{
			return typeRef.CloneNode();
		}

		public static TypeReference Lift(TypeDefinition node)
		{
			if (node.HasGenericParameters) return LiftGenericTypeDefinition(node);
			return new SimpleTypeReference(node.FullName);
		}

		private static TypeReference LiftGenericTypeDefinition(TypeDefinition node)
		{
			GenericTypeReference typeRef = new GenericTypeReference(node.LexicalInfo, node.QualifiedName);
			foreach (GenericParameterDeclaration parameter in node.GenericParameters)
			{
				typeRef.GenericArguments.Add(Lift(parameter.Name));
			}
			return typeRef;
		}

		public static TypeReference Lift(Expression e)
		{
			switch (e.NodeType)
			{
				case NodeType.TypeofExpression:
					return Lift((TypeofExpression) e);
				case NodeType.GenericReferenceExpression:
					return Lift((GenericReferenceExpression) e);
				case Ast.NodeType.ReferenceExpression:
					return Lift((ReferenceExpression) e);
				case Ast.NodeType.MemberReferenceExpression:
					return Lift((MemberReferenceExpression) e);
			}
			throw new NotImplementedException(e.ToCodeString());
		}

		public static TypeReference Lift(ReferenceExpression e)
		{
			return new SimpleTypeReference(e.LexicalInfo, e.ToString());
		}

		public static TypeReference Lift(TypeofExpression e)
		{
			return e.Type.CloneNode();
		}

		public static TypeReference Lift(GenericReferenceExpression e)
		{
			GenericTypeReference typeRef = new GenericTypeReference(e.LexicalInfo);
			typeRef.Name = TypeNameFor(e.Target);
			typeRef.GenericArguments.ExtendWithClones(e.GenericArguments);
			return typeRef;
		}

		private static string TypeNameFor(Expression target)
		{
			return ((ReferenceExpression) target).ToString();
		}

		public TypeReference()
		{
 		}
		
		public TypeReference(LexicalInfo lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
	}
}
