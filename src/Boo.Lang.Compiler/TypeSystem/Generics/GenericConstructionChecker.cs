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


using System;
using System.Collections.Generic;
using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.TypeSystem.Generics
{
	/// <summary>
	/// Checks a generic construction for several kinds of errors.
	/// </summary>
	public class GenericConstructionChecker
	{
		Node _constructionNode;
		IType[] _typeArguments;
		CompilerErrorCollection _errors = new CompilerErrorCollection();
		TypeSystemServices _typeSystemServices = My<TypeSystemServices>.Instance;

		public GenericConstructionChecker(IType[] typeArguments, Node constructionNode)
		{
			_constructionNode = constructionNode;
			_typeArguments = typeArguments;
		}

		public IType[] TypeArguments
		{
			get { return _typeArguments; }
		}

		public Node ConstructionNode
		{
			get { return _constructionNode; }
			set { _constructionNode = value; }
		}

		public CompilerErrorCollection Errors
		{
			get { return _errors; }
		}

		public IEnumerable<Predicate<IEntity>> Checks
		{
			get
			{
				yield return IsGenericDefinition;
				yield return HasCorrectGenerity;
				yield return MaintainsParameterConstraints;
			}
		}

		private bool IsGenericDefinition(IEntity definition)
		{
			if (GenericsServices.IsGenericMethod(definition) || GenericsServices.IsGenericType(definition))
				return true;

			Errors.Add(CompilerErrorFactory.NotAGenericDefinition(ConstructionNode, definition.FullName));
			return false;
		}

		private bool HasCorrectGenerity(IEntity definition)
		{
			IGenericParameter[] typeParameters = GenericsServices.GetGenericParameters(definition);

			if (typeParameters.Length != TypeArguments.Length)
			{
			    var error = CompilerErrorFactory.GenericDefinitionArgumentCount(ConstructionNode, definition, typeParameters.Length);
                var internalType = definition as IInternalEntity;
                if (internalType != null)
                {
                    var node = internalType.Node;
                    var replacementNode = node["TypeRefReplacement"] as GenericReferenceExpression;
                    if (replacementNode != null && replacementNode.GenericArguments.Count == typeParameters.Length)
                    {
                        error.Data["TypeRefReplacement"] = replacementNode.CloneNode();
                        throw error;
                    }
                }
				Errors.Add(error);
				return false;
			}
			return true;
		}

		IType _definition;

		private bool MaintainsParameterConstraints(IEntity definition)
		{
			IGenericParameter[] parameters = GenericsServices.GetGenericParameters(definition);
			_definition = definition as IType;

			bool valid = true;
			for (int i = 0; i < parameters.Length; i++)
				valid &= MaintainsParameterConstraints(parameters[i], TypeArguments[i]);
			return valid;
		}

		private bool MaintainsParameterConstraints(IGenericParameter parameter, IType argument)
		{
			if (argument == null || TypeSystemServices.IsError(argument))
				return true;

			if (argument == parameter)
				return true;

			if (argument == _typeSystemServices.VoidType)
			{
				Errors.Add(CompilerErrorFactory.InvalidGenericParameterType(ConstructionNode, argument));
				return false;
			}

			bool valid = true;

			// Check type semantics constraints
			if (parameter.IsClass && !(argument.IsClass || argument.IsInterface))
			{
				Errors.Add(CompilerErrorFactory.GenericArgumentMustBeReferenceType(ConstructionNode, parameter, argument));
				valid = false;
			}

			if (parameter.IsValueType && !argument.IsValueType)
			{
				Errors.Add(CompilerErrorFactory.GenericArgumentMustBeValueType(ConstructionNode, parameter, argument));
				valid = false;
			}
				// Don't check for default constructor constraint if value type constraint failed
			else if (parameter.MustHaveDefaultConstructor && !HasDefaultConstructor(argument))
			{
				Errors.Add(CompilerErrorFactory.GenericArgumentMustHaveDefaultConstructor(ConstructionNode, parameter, argument));
				valid = false;
			}

			// Check base type constraints
			IType[] baseTypes = parameter.GetTypeConstraints();
			if (baseTypes != null)
			{
				foreach (IType baseType in baseTypes)
				{
					// Foo<T> where T : Foo<T>
					if (null != _definition
					    && TypeCompatibilityRules.IsAssignableFrom(baseType, _definition)
					    && argument == _constructionNode.ParentNode.Entity)
						continue;

					// Don't check for System.ValueType supertype constraint 
					// if parameter also has explicit value type constraint
					if (baseType == _typeSystemServices.ValueTypeType && parameter.IsValueType)
						continue;

					if (!TypeCompatibilityRules.IsAssignableFrom(baseType, argument))
					{
						Errors.Add(CompilerErrorFactory.GenericArgumentMustHaveBaseType(ConstructionNode, parameter, argument, baseType));
						valid = false;
					}
				}
			}

			return valid;
		}

		private bool HasDefaultConstructor(IType argument)
		{
			if (argument.IsValueType)
				return true;
			return argument.GetConstructors().Any(ctor => ctor.GetParameters().Length == 0);
		}

		public void ReportErrors(CompilerErrorCollection targetErrorCollection)
		{
			targetErrorCollection.Extend(Errors);
		}

		public void DiscardErrors()
		{
			Errors.Clear();
		}
	}
}