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
		/// <param name="node">The node in which construction occurs.</param>
		/// <param name="argumentNodes">The nodes of the arguments supplied for generic construction.</param>
		/// <returns>The constructed entity.</returns>
		public IEntity ConstructEntity(IEntity definition, Node constructionNode, TypeReferenceCollection argumentNodes)
		{
			// Ambiguous generic constructions are handled separately
			if (definition.EntityType == EntityType.Ambiguous)
			{
				return ConstructAmbiguousEntity((Ambiguous)definition, constructionNode, argumentNodes);
			}

			// Check that the construction is valid
			if (!CheckGenericConstruction(definition, constructionNode, argumentNodes, Errors))
			{
				return TypeSystemServices.ErrorEntity;
			}

			// Construct a type or a method according to the definition
			IType[] arguments = Array.ConvertAll<TypeReference, IType>(
				argumentNodes.ToArray(),
				delegate(TypeReference tr) { return (IType)tr.Entity; });

			if (IsGenericType(definition))
			{
				return ((IType)definition).GenericInfo.ConstructType(arguments);
			}
			if (IsGenericMethod(definition))
			{
				return ((IMethod)definition).GenericInfo.ConstructMethod(arguments);
			}

			// Should never be reached - if definition is neither a generic type nor a generic method,
			// CheckGenericConstruction would've indicated this
			return TypeSystemServices.ErrorEntity;
		}

		/// <summary>
		/// Constructs generic entities out of an ambiguous definition.
		/// </summary>
		private IEntity ConstructAmbiguousEntity(Ambiguous ambiguousDefinition, Node constructionNode, TypeReferenceCollection argumentNodes)
		{
			GenericConstructionChecker checker = new GenericConstructionChecker(
				TypeSystemServices, 
				constructionNode, 
				argumentNodes, 
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
					return ConstructEntity(matches[0], constructionNode, argumentNodes);
				}
			}

			// Several matches have passed the filter - 
			// construct all of them and return another Ambiguous entity
			IEntity[] constructed = Array.ConvertAll<IEntity, IEntity>(
				matches.ToArray(),
				delegate(IEntity def) { return ConstructEntity(def, constructionNode, argumentNodes); });

			return new Ambiguous(constructed);
		}

		/// <summary>
		/// Checks whether a given set of arguments can be used to construct a generic type or method from a specified definition.
		/// </summary>
		public bool CheckGenericConstruction(IEntity definition, Node node, TypeReferenceCollection arguments, CompilerErrorCollection errors)
		{
			// Ensure definition is a valid entity
			if (definition == null || TypeSystemServices.IsError(definition))
			{
				return false;
			}

			// Ensure definition really is a generic definition
			GenericConstructionChecker checker = new GenericConstructionChecker(
				TypeSystemServices, node, arguments, Errors);

			return !(
				checker.NotGenericDefinition(definition) ||
				checker.IncorrectGenerity(definition) ||
				checker.ViolatesParameterConstraints(definition));			
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
	}

	/// <summary>
	/// Checks a generic construction for several kinds of errors.
	/// </summary>
	public class GenericConstructionChecker
	{
		Node _constructionNode;
		TypeReferenceCollection _argumentNodes;
		CompilerErrorCollection _errors;
		TypeSystemServices _tss;

		public GenericConstructionChecker(TypeSystemServices tss, Node constructionNode, TypeReferenceCollection argumentNodes, CompilerErrorCollection errorCollection)
		{
			_tss = tss;
			_constructionNode = constructionNode;
			_argumentNodes = argumentNodes;
			_errors = errorCollection;
		}

		public TypeReferenceCollection ArgumentNodes
		{
			get { return _argumentNodes; }
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
			if (parametersCount != ArgumentNodes.Count)
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
				if (ViolatesParameterConstraints(parameters[i], ArgumentNodes[i]))
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
		public bool ViolatesParameterConstraints(IGenericParameter parameter, TypeReference argumentNode)
		{
			IType argument = TypeSystemServices.GetEntity(argumentNode) as IType;
			
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
				Errors.Add(CompilerErrorFactory.GenericArgumentMustBeValueType(argumentNode, parameter, argument));
				valid = false;
			}

			// Check for default constructor
			if (parameter.MustHaveDefaultConstructor && !HasDefaultConstructor(argument))
			{
				Errors.Add(CompilerErrorFactory.GenericArgumentMustHaveDefaultConstructor(argumentNode, parameter, argument));
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
						Errors.Add(CompilerErrorFactory.GenericArgumentMustHaveBaseType(argumentNode, parameter, argument, baseType));
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
			IConstructor[] constructors = argument.GetConstructors();

			if (constructors == null || constructors.Length == 0)
			{
				return true;
			}

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
