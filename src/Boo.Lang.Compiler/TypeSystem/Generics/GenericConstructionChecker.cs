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