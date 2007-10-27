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
		public IEntity ConstructEntity(IEntity definition, Node node, TypeReferenceCollection argumentNodes)
		{
			if (!CheckGenericConstruction(definition, node, argumentNodes))
			{
				return TypeSystemServices.ErrorEntity;
			}

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
		/// Checks whether a given set of arguments can be used to construct a generic type or method from a specified definition.
		/// </summary>
		/// <returns>A list of compiler errors discovered during the check. If the list is empty, the check is successful.</returns>
		public bool CheckGenericConstruction(IEntity definition, Node node, TypeReferenceCollection arguments)
		{
			// Ensure definition is a valid entity
			if (definition == null || TypeSystemServices.IsError(definition))
			{
				return false;
			}
			
			// Ensure definition really is a generic definition
			IGenericParameter[] parameters = GetGenericParameters(definition);
			if (parameters == null)
			{
				Errors.Add(CompilerErrorFactory.NotAGenericDefinition(node, definition.FullName));
				return false;
			}

			// Ensure number of arguments matches number of parameters
			if (parameters.Length != arguments.Count)
			{
				Errors.Add(CompilerErrorFactory.GenericDefinitionArgumentCount(node, definition.FullName, parameters.Length));
				return false;
			}

			// Check each argument against its matching parameter
			bool valid = true;
			for (int i = 0; i < parameters.Length; i++)
			{
				if (!CheckGenericParameter(arguments[i], parameters[i], (IType)arguments[i].Entity))
				{
					valid = false;
				}
			}

			return valid;
		}

		/// <summary>
		/// Checks whether a given type can substitute a given generic parameter.
		/// </summary>
		/// <returns>A list of compiler errors discovered during the check. If the list is empty, the check is successful.</returns>
		public bool CheckGenericParameter(Node node, IGenericParameter parameter, IType argument)
		{
			// Ensure argument is a valid type
			if (TypeSystemServices.IsError(argument))
			{
				return false;
			}

			bool valid = true;

			// Check type semantics constraints
			if (parameter.IsClass && !argument.IsClass)
			{
				Errors.Add(CompilerErrorFactory.GenericArgumentMustBeReferenceType(node, parameter, argument));
				return false;
			}

			if (parameter.IsValueType && !argument.IsValueType)
			{
				Errors.Add(CompilerErrorFactory.GenericArgumentMustBeValueType(node, parameter, argument));
				return false;
			}

			// Check for default constructor
			if (parameter.MustHaveDefaultConstructor && !HasDefaultConstructor(argument))
			{
				Errors.Add(CompilerErrorFactory.GenericArgumentMustHaveDefaultConstructor(node, parameter, argument));
				return false;
			}

			// Check base type constraints
			IType[] baseTypes = parameter.GetBaseTypeConstraints();
			if (baseTypes != null)
			{
				foreach (IType baseType in baseTypes)
				{
					// Don't check for System.ValueType supertype constraint 
					// if parameter also has explicit value type constraint
					if (baseType == TypeSystemServices.ValueTypeType && parameter.IsValueType) continue;

					if (!baseType.IsAssignableFrom(argument))
					{
						Errors.Add(CompilerErrorFactory.GenericArgumentMustHaveBaseType(node, parameter, argument, baseType));
						valid = false;
					}
				}
			}

			return valid;
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
		public static System.Collections.Generic.IEnumerable<IType> FindConstructedTypes(IType type, IType definition)
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
		/// Checks whether a specified type is an open generic type - that is, if it contains generic parameters.
		/// </summary>
		public static bool IsOpenGenericType(IType type)
		{
			// A generic parameter is itself open
			if (type is IGenericParameter)
			{
				return true;
			}

			// A partially constructed type is open
			if (type.ConstructedInfo != null)
			{
				return !type.ConstructedInfo.FullyConstructed;
			}

			// An array of open types, or a reference to an open type, is itself open
			if (type.IsByRef || type.IsArray)
			{
				return IsOpenGenericType(type.GetElementType());
			}

			return false;
		}
	}

}