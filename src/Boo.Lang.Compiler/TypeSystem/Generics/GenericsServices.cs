#region license
// Copyright (c) 2004, 2005, 2006, 2007 Rodrigo B. de Oliveira (rbo@acm.org)
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
using System.Collections.Generic;
using System;

namespace Boo.Lang.Compiler.TypeSystem.Generics
{
	public class GenericsServices : AbstractCompilerComponent
	{
		public GenericsServices()
		{
			Initialize(CompilerContext.Current);
		}

		/// <summary>
		/// Constructs an entity from a generic definition and arguments, after ensuring the construction is valid.
		/// </summary>
		/// <param name="definition">The generic definition entity.</param>
		/// <param name="constructionNode">The node in which construction occurs.</param>
		/// <param name="typeArguments">The generic type arguments to substitute for generic parameters.</param>
		/// <returns>The constructed entity.</returns>
		public IEntity ConstructEntity(Node constructionNode, IEntity definition, IType[] typeArguments)
		{
			// Ensure definition is a valid entity
			if (definition == null || TypeSystemServices.IsError(definition))
			{
				return TypeSystemServices.ErrorEntity;
			}

			// Ambiguous generic constructions are handled separately
			if (definition.EntityType == EntityType.Ambiguous)
			{
				return ConstructAmbiguousEntity(constructionNode, (Ambiguous)definition, typeArguments);
			}

			// Check that the construction is valid
			if (!CheckGenericConstruction(constructionNode, definition, typeArguments, true))
			{
				return TypeSystemServices.ErrorEntity;
			}

			return MakeGenericEntity(definition, typeArguments);
		}

		/// <summary>
		/// Validates and constructs generic entities out of an ambiguous generic definition entity.
		/// </summary>
		private IEntity ConstructAmbiguousEntity(Node constructionNode, Ambiguous ambiguousDefinition, IType[] typeArguments)
		{
			GenericConstructionChecker checker = new GenericConstructionChecker(typeArguments, constructionNode); 

			List<IEntity> matches = new List<IEntity>(ambiguousDefinition.Entities);
			bool reportErrors = false;

			foreach (Predicate<IEntity> check in checker.Checks)
			{
				matches = matches.Collect(check);

				if (matches.Count == 0)
				{
					Errors.Add(checker.Errors[0]); // only report first error, assuming the rest are superfluous
					return TypeSystemServices.ErrorEntity;
				}

				if (reportErrors)
				{
					checker.ReportErrors(Errors);
				}
				checker.DiscardErrors();

				// We only want full error reporting once we get down to a single candidate
				if (matches.Count == 1)
				{
					reportErrors = true;
				}			
			}

			IEntity[] constructedMatches = Array.ConvertAll<IEntity, IEntity>(
				matches.ToArray(),
				delegate(IEntity def) { return MakeGenericEntity(def, typeArguments); });

			return constructedMatches.Length == 1 ? 
			                                      	constructedMatches[0] : 
			                                      	                      	new Ambiguous(constructedMatches);
		}

		private IEntity MakeGenericEntity(IEntity definition, IType[] typeArguments)
		{
			if (IsGenericType(definition))
			{
				return ((IType)definition).GenericInfo.ConstructType(typeArguments);
			}

			if (IsGenericMethod(definition))
			{
				return ((IMethod)definition).GenericInfo.ConstructMethod(typeArguments);
			}

			// Should never be reached
			return TypeSystemServices.ErrorEntity;
		}

		/// <summary>
		/// Checks whether a given set of arguments can be used to construct a generic type or method from a specified definition.
		/// </summary>
		public bool CheckGenericConstruction(IEntity definition, IType[] typeArguments)
		{
			return CheckGenericConstruction(null, definition, typeArguments, false);
		}

		/// <summary>
		/// Checks whether a given set of arguments can be used to construct a generic type or method from a specified definition.
		/// </summary>
		public bool CheckGenericConstruction(Node node, IEntity definition, IType[] typeArguments, bool reportErrors)
		{
			GenericConstructionChecker checker = new GenericConstructionChecker(typeArguments, node);

			foreach (Predicate<IEntity> check in checker.Checks)
			{
				if (check(definition)) continue;
				
				if (reportErrors) checker.ReportErrors(Errors);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Attempts to infer the generic parameters of a method from a set of arguments.
		/// </summary>
		/// <returns>
		/// An array consisting of inferred types for the method's generic arguments,
		/// or null if type inference failed.
		/// </returns>
		public IType[] InferMethodGenericArguments(IMethod method, ExpressionCollection arguments)
		{
			if (method.GenericInfo == null) return null;

			GenericParameterInferrer inferrerr = new GenericParameterInferrer(Context, method, arguments);
			if (inferrerr.Run())
			{
				return inferrerr.GetInferredTypes();
			}
			return null;
		}

		public static bool IsGenericMethod(IEntity entity)
		{
			IMethod method = entity as IMethod;
			return (method != null && method.GenericInfo != null);
		}

		public static bool IsGenericType(IEntity entity)
		{
			IType type = entity as IType;
			return (type != null && type.GenericInfo != null);
		}

		public static bool IsGenericParameter(IEntity entity)
		{
			return (entity is IGenericParameter);
		}

		public static bool AreOfSameGenerity(IMethod lhs, IMethod rhs)
		{
			return (GetMethodGenerity(lhs) == GetMethodGenerity(rhs));
		}

		/// <summary>
		/// Yields the generic parameters used in a (bound) type.
		/// </summary>
		public static IEnumerable<IGenericParameter> FindGenericParameters(IType type)
		{
			IGenericParameter genericParameter = type as IGenericParameter;
			if (genericParameter != null)
			{
				yield return genericParameter;
				yield break;
			}

			if (type is IArrayType)
			{
				foreach (IGenericParameter gp in FindGenericParameters(type.GetElementType())) yield return gp;
				yield break;
			}

			if (type.ConstructedInfo != null)
			{
				foreach (IType typeArgument in type.ConstructedInfo.GenericArguments)
				{
					foreach (IGenericParameter gp in FindGenericParameters(typeArgument)) yield return gp;
				}
				yield break;
			}

			ICallableType callableType = type as ICallableType;
			if (callableType != null)
			{
				CallableSignature signature = callableType.GetSignature();
				foreach (IGenericParameter gp in FindGenericParameters(signature.ReturnType)) yield return gp;
				foreach (IParameter parameter in signature.Parameters)
				{
					foreach (IGenericParameter gp in FindGenericParameters(parameter.Type)) yield return gp;
				}
				yield break;
			}
		}

		/// <summary>
		/// Finds types constructed from the specified definition in the specified type's interfaces and base types.
		/// </summary>
		/// <param name="type">The type in whose hierarchy to search for constructed types.</param>
		/// <param name="definition">The generic type definition whose constructed versions to search for.</param>
		/// <returns>Yields the matching types.</returns>
		public static IEnumerable<IType> FindConstructedTypes(IType type, IType definition)
		{
			while (type != null)
			{
				if (type.ConstructedInfo != null &&
				    type.ConstructedInfo.GenericDefinition == definition)
				{
					yield return type;
				}

				IType[] interfaces = type.GetInterfaces();
				if (interfaces != null)
				{
					foreach (IType interfaceType in interfaces)
					{
						foreach (IType match in FindConstructedTypes(interfaceType, definition))
						{
							yield return match;
						}
					}
				}

				type = type.BaseType;
			}
		}


		/// <summary>
		/// Finds a single constructed occurance of a specified generic definition
		/// in the specified type's inheritence hierarchy.
		/// </summary>
		/// <param name="type">The type in whose hierarchy to search for constructed types.</param>
		/// <param name="definition">The generic type definition whose constructed versions to search for.</param>
		/// <returns>
		/// The single constructed occurance of the generic definition in the
		/// specified type, or null if there are none or more than one.
		/// </returns>
		public static IType FindConstructedType(IType type, IType definition)
		{
			IType result = null;
			foreach (IType candidate in FindConstructedTypes(type, definition))
			{
				if (result == null) result = candidate;
				else if (result != candidate) return null;
			}
			return result;
		}


		/// <summary>
		/// Checks that at least one constructed occurence of a specified generic
		/// definition is present in the specified type's inheritance hierarchy.
		/// </summary>
		/// <param name="type">The type in whose hierarchy to search for constructed type.</param>
		/// <param name="definition">The generic type definition whose constructed versions to search for.</param>
		/// <returns>
		/// True if a occurence has been found, False otherwise.
		/// </returns>
		public static bool HasConstructedType(IType type, IType definition)
		{
			return FindConstructedTypes(type, definition).GetEnumerator().MoveNext();
		}


		/// <summary>
		/// Determines whether a specified type is an open generic type - 
		/// that is, if it contains generic parameters.
		/// </summary>
		public static bool IsOpenGenericType(IType type)
		{
			return (GetTypeGenerity(type) != 0);
		}

		/// <summary>
		/// Gets the generic parameters associated with a generic type or generic method definition.
		/// </summary>
		/// <returns>An array of IGenericParameter objects, or null if the specified entity isn't a generic definition.</returns>
		public static IGenericParameter[] GetGenericParameters(IEntity definition)
		{
			if (IsGenericType(definition))
			{
				return ((IType)definition).GenericInfo.GenericParameters;
			}
			if (IsGenericMethod(definition))
			{
				return ((IMethod)definition).GenericInfo.GenericParameters;
			}
			return null;
		}

		/// <summary>
		/// Determines the number of open generic parameters in the specified type.
		/// </summary>
		public static int GetTypeGenerity(IType type)
		{
			if (type.IsByRef || type.IsArray)
			{
				return GetTypeGenerity(type.GetElementType());
			}

			if (type is IGenericParameter)
			{
				return 1;
			}

			// Generic parameters and generic arguments both contribute 
			// to a types genrity. Note that a nested type can be both a generic definition 
			// and a constructed type: Outer[of int].Inner[of T]
			int generity = 0;

			if (type.GenericInfo != null)
			{
				generity += type.GenericInfo.GenericParameters.Length;
			}

			if (type.ConstructedInfo != null)
			{
				foreach (IType typeArg in type.ConstructedInfo.GenericArguments)
				{
					generity += GetTypeGenerity(typeArg);
				}
			}

			return generity;
		}

		public static int GetMethodGenerity(IMethod method)
		{
			IConstructedMethodInfo constructedInfo = method.ConstructedInfo;
			if (constructedInfo != null)
				return constructedInfo.GenericArguments.Length;

			IGenericMethodInfo genericInfo = method.GenericInfo;
			if (genericInfo != null)
				return genericInfo.GenericParameters.Length;

			return 0;
		}
	}

	/// <summary>
	/// Checks a generic construction for several kinds of errors.
	/// </summary>
	public class GenericConstructionChecker
	{
		Node _constructionNode;
		IType[] _typeArguments;
		CompilerErrorCollection _errors = new CompilerErrorCollection();

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

		protected CompilerContext Context
		{
			get { return CompilerContext.Current; }
		}

		private bool IsGenericDefinition(IEntity definition)
		{
			if (GenericsServices.IsGenericMethod(definition) || GenericsServices.IsGenericType(definition))
			{
				return true;
			}

			Errors.Add(CompilerErrorFactory.NotAGenericDefinition(ConstructionNode, definition.FullName));
			return false;
		}

		private bool HasCorrectGenerity(IEntity definition)
		{
			IGenericParameter[] typeParameters = GenericsServices.GetGenericParameters(definition);

			if (typeParameters.Length != TypeArguments.Length)
			{
				Errors.Add(CompilerErrorFactory.GenericDefinitionArgumentCount(ConstructionNode, definition.FullName, typeParameters.Length));
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
			{
				valid &= MaintainsParameterConstraints(parameters[i], TypeArguments[i]);
			}

			return valid;
		}

		private bool MaintainsParameterConstraints(IGenericParameter parameter, IType argument)
		{
			if (argument == null || TypeSystemServices.IsError(argument))
			{
				return true;
			}

			if (argument == My<TypeSystemServices>.Instance.VoidType)
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
					    && baseType.IsAssignableFrom(_definition)
					    && argument == _constructionNode.ParentNode.Entity)
						continue;

					// Don't check for System.ValueType supertype constraint 
					// if parameter also has explicit value type constraint
					if (baseType == Context.TypeSystemServices.ValueTypeType && parameter.IsValueType)
						continue;

					if (!baseType.IsAssignableFrom(argument))
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
			{
				return true;
			}

			return Array.Exists(
				argument.GetConstructors(), 
				delegate(IConstructor ctor) { return ctor.GetParameters().Length == 0; });
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
