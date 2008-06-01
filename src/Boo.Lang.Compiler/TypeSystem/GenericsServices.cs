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

namespace Boo.Lang.Compiler.TypeSystem
{
	public class GenericsServices : AbstractCompilerComponent
	{
		public GenericsServices(CompilerContext context)
		{
			Initialize(context);
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
			// Ambiguous generic constructions are handled separately
			if (definition.EntityType == EntityType.Ambiguous)
			{
				return ConstructAmbiguousEntity(constructionNode, (Ambiguous)definition, typeArguments);
			}

			// Check that the construction is valid
			if (!CheckGenericConstruction(constructionNode, definition, typeArguments, Errors))
			{
				return TypeSystemServices.ErrorEntity;
			}

			// Construct a type or a method according to the definition
			if (IsGenericType(definition))
			{
				return ((IType)definition).GenericInfo.ConstructType(typeArguments);
			}
			if (IsGenericMethod(definition))
			{
				return ((IMethod)definition).GenericInfo.ConstructMethod(typeArguments);
			}

			// Should never be reached - if definition is neither a generic type nor a generic method,
			// CheckGenericConstruction would've indicated this
			return TypeSystemServices.ErrorEntity;
		}

		/// <summary>
		/// Constructs generic entities out of an ambiguous definition.
		/// </summary>
		private IEntity ConstructAmbiguousEntity(Node constructionNode, Ambiguous ambiguousDefinition, IType[] typeArguments)
		{
			GenericConstructionChecker checker = new GenericConstructionChecker(
				TypeSystemServices, 
				constructionNode,
				typeArguments, 
				new CompilerErrorCollection()); 

			List<IEntity> matches = new List<IEntity>(ambiguousDefinition.Entities);

			// Filter matches by genericness, generity and constraints
			Predicate<IEntity>[] filters = new Predicate<IEntity>[] { 
				checker.NotGenericDefinition,
				checker.IncorrectGenerity,
				checker.ViolatesParameterConstraints };

			foreach (Predicate<IEntity> filter in filters)
			{
				checker.Errors.Clear();
				matches.RemoveAll(filter);

				// If no matches pass the filter, record the first error only
				// (providing all the distinct errors that occured would be superfluous)
				if (matches.Count == 0)
				{
					Errors.Add(checker.Errors[0]);
					return TypeSystemServices.ErrorEntity;
				}

				// If only one match passes the filter, continue construction normally
				if (matches.Count == 1)
				{
					return ConstructEntity(constructionNode, matches[0], typeArguments);
				}
			}

			// Several matches have passed the filter - 
			// construct all of them and return another Ambiguous entity
			IEntity[] constructed = Array.ConvertAll<IEntity, IEntity>(
				matches.ToArray(),
				delegate(IEntity def) { return ConstructEntity(constructionNode, def, typeArguments); });

			return new Ambiguous(constructed);
		}

		/// <summary>
		/// Checks whether a given set of arguments can be used to construct a generic type or method from a specified definition.
		/// </summary>
		public bool CheckGenericConstruction(Node node, IEntity definition, IType[] argumentTypes, CompilerErrorCollection errors)
		{
			// Ensure definition is a valid entity
			if (definition == null || TypeSystemServices.IsError(definition))
			{
				return false;
			}

			// Ensure definition really is a generic definition
			GenericConstructionChecker checker = new GenericConstructionChecker(
				TypeSystemServices, node, argumentTypes, Errors);

			return !(
				checker.NotGenericDefinition(definition) ||
				checker.IncorrectGenerity(definition) ||
				checker.ViolatesParameterConstraints(definition));			
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
		/// Finds types constructed from the specified definition in the specified type's interfaces and base types.
		/// </summary>
		/// <param name="type">The type in whose hierarchy to search for constructed types.</param>
		/// <param name="definition">The generic type definition whose constructed versions to search for.</param>
		/// <returns>Yields the matching types.</returns>
		public static IEnumerable<IType> FindConstructedTypes(IType type, IType definition)
		{
			while (type != null)
			{
				// Check if type itself is constructed from the definition
				if (type.ConstructedInfo != null &&
					type.ConstructedInfo.GenericDefinition == definition)
				{
					yield return type;
				}

				// Look in type's immediate interfaces
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

				// Move on to the type's base type
				type = type.BaseType;
			}
		}


		/// <summary>
		/// Finds a single constructed occurance of a specified generic definition
		/// in the specified type's inheritence hierarchy.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="definition"></param>
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
			// Dive into arrays and refs
			if (type.IsByRef || type.IsArray)
			{
				return GetTypeGenerity(type.GetElementType());
			}

			// A generic parameter has a generity of one
			if (type is IGenericParameter)
			{
				return 1;
			}

			// Generic parameters and generic arguments both contribute 
			// to a types genrity
			int generity = 0;

			// A generic type gets a generity equals to the number its type parameters
			if (type.GenericInfo != null)
			{
				generity += type.GenericInfo.GenericParameters.Length;
			}

			// A constructed type gets the accumulated generity of its type arguments
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
		CompilerErrorCollection _errors;
		TypeSystemServices _tss;

		public GenericConstructionChecker(TypeSystemServices tss, Node constructionNode, IType[] typeArguments, CompilerErrorCollection errorCollection)
		{
			_tss = tss;
			_constructionNode = constructionNode;
			_typeArguments = typeArguments;
			_errors = errorCollection;
		}

		public IType[] TypeArguments
		{
			get { return _typeArguments; }
		}

		public Node ConstructionNode
		{
			get { return _constructionNode; }
		}

		public CompilerErrorCollection Errors
		{
			get { return _errors; }
		}

		/// <summary>
		/// Checks if a specified entity is not a generic definition.
		/// </summary>
		public bool NotGenericDefinition(IEntity entity)
		{
			if (!(GenericsServices.IsGenericType(entity) || GenericsServices.IsGenericMethod(entity)))
			{
				Errors.Add(CompilerErrorFactory.NotAGenericDefinition(ConstructionNode, entity.FullName));
				return true;
			}
			return false;
		}

		/// <summary>
		/// Checks if the number of generic parameters on a specified definition 
		/// does not match the number of supplied type arguments.
		/// </summary>
		public bool IncorrectGenerity(IEntity definition)
		{
			int parametersCount = GenericsServices.GetGenericParameters(definition).Length;
			if (parametersCount != TypeArguments.Length)
			{
				Errors.Add(CompilerErrorFactory.GenericDefinitionArgumentCount(ConstructionNode, definition.FullName, parametersCount));
				return true;
			}
			return false;
		}

		/// <summary>
		/// Checks if the given type arguments violate any constraints 
		/// declared on the type parameters of a specified generic definition.
		/// </summary>
		public bool ViolatesParameterConstraints(IEntity definition)
		{
			IGenericParameter[] parameters = GenericsServices.GetGenericParameters(definition);

			bool valid = true;
			for (int i = 0; i < parameters.Length; i++)
			{
				if (ViolatesParameterConstraints(parameters[i], TypeArguments[i]))
				{
					valid = false;
				}
			}

			return !valid;
		}

		/// <summary>
		/// Checks if a specified type argument violates the constraints 
		/// declared on a specified type paramter.
		/// </summary>
		public bool ViolatesParameterConstraints(IGenericParameter parameter, IType argument)
		{
			// Ensure argument is a valid type
			if (argument == null || TypeSystemServices.IsError(argument))
			{
				return false;
			}

			bool valid = true;

			// Check type semantics constraints
			if (parameter.IsClass && !argument.IsClass)
			{
				Errors.Add(CompilerErrorFactory.GenericArgumentMustBeReferenceType(ConstructionNode, parameter, argument));
				valid = false;
			}

			if (parameter.IsValueType && !argument.IsValueType)
			{
				Errors.Add(CompilerErrorFactory.GenericArgumentMustBeValueType(ConstructionNode, parameter, argument));
				valid = false;
			}

			if (parameter.MustHaveDefaultConstructor && !HasDefaultConstructor(argument))
			{
				Errors.Add(CompilerErrorFactory.GenericArgumentMustHaveDefaultConstructor(ConstructionNode, parameter, argument));
				valid = false;
			}

			// Check base type constraints
			IType[] baseTypes = parameter.GetBaseTypeConstraints();
			if (baseTypes != null)
			{
				foreach (IType baseType in baseTypes)
				{
					// Don't check for System.ValueType supertype constraint 
					// if parameter also has explicit value type constraint
					if (baseType == _tss.ValueTypeType && parameter.IsValueType)
						continue;

					if (!baseType.IsAssignableFrom(argument))
					{
						Errors.Add(CompilerErrorFactory.GenericArgumentMustHaveBaseType(ConstructionNode, parameter, argument, baseType));
						valid = false;
					}
				}
			}

			return !valid;
		}

		/// <summary>
		/// Checks whether a given type has a default (parameterless) consructor.
		/// </summary>
		private static bool HasDefaultConstructor(IType argument)
		{
			if (argument.IsValueType)
			{
				return true;
			}

			IConstructor[] constructors = argument.GetConstructors();
			foreach (IConstructor ctor in constructors)
			{
				if (ctor.GetParameters().Length == 0)
				{
					return true;
				}
			}

			return false;
		}

	}
}
