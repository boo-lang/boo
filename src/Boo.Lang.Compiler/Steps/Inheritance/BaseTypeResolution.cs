using System;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.Steps.Inheritance
{
	class BaseTypeResolution : AbstractCompilerComponent
	{
		private readonly TypeDefinition _typeDefinition;
		private readonly List _visited;
		private int _removed;
		private int _index;
		private Set<TypeDefinition> _ancestors;

		public BaseTypeResolution(CompilerContext context, TypeDefinition typeDefinition, List visited) : base(context)
		{
			_typeDefinition = typeDefinition;
			_visited = visited;
			_visited.Add(_typeDefinition);

			_removed = 0;
			_index = -1;

			NameResolutionService nameResolution = NameResolutionService;
			INamespace previous = nameResolution.CurrentNamespace;
			nameResolution.EnterNamespace(ParentNamespaceOf(_typeDefinition));
			try
			{
				Run();
			}
			finally
			{
				nameResolution.Restore(previous);
			}
		}

		private INamespace ParentNamespaceOf(TypeDefinition typeDefinition)
		{
			return (INamespace) GetEntity(typeDefinition.ParentNode);
		}

		private void Run()
		{
			IType type = (IType)TypeSystemServices.GetEntity(_typeDefinition);

			EnterGenericParametersNamespace(type);

            Boo.Lang.List visitedNonInterfaces = null;
            Boo.Lang.List visitedInterfaces = null;

			if (_typeDefinition is InterfaceDefinition)
            {
                visitedInterfaces = _visited;
                // interfaces won't have noninterface base types so visitedNonInterfaces not necessary here
            }
            else
            {
                visitedNonInterfaces = _visited;
                visitedInterfaces = new Boo.Lang.List();
            }
            
			foreach (SimpleTypeReference baseTypeRef in _typeDefinition.BaseTypes.ToArray())
			{
				NameResolutionService.ResolveSimpleTypeReference(baseTypeRef);

				++_index;

				AbstractInternalType baseType = baseTypeRef.Entity as AbstractInternalType;
				if (null == baseType)
					continue;

				if (IsEnclosingType(baseType.TypeDefinition))
				{
					BaseTypeError(CompilerErrorFactory.NestedTypeCannotExtendEnclosingType(baseTypeRef, _typeDefinition.FullName, baseType.FullName));
					continue;
				}

				if (baseType is InternalInterface)
                    CheckForCycles(baseTypeRef, baseType, visitedInterfaces);
                else
					CheckForCycles(baseTypeRef, baseType, visitedNonInterfaces);
				
			}

			LeaveGenericParametersNamespace(type);
		}

		private bool IsEnclosingType(TypeDefinition node)
		{
			return GetAncestors().Contains(node);
		}

		private Set<TypeDefinition> GetAncestors()
		{
			if (null == _ancestors)
				_ancestors = new Set<TypeDefinition>(_typeDefinition.GetAncestors<TypeDefinition>());
			return _ancestors;
		}

		private void LeaveGenericParametersNamespace(IType type)
		{
			if (type.GenericInfo != null)
				NameResolutionService.LeaveNamespace();
		}

		private void EnterGenericParametersNamespace(IType type)
		{
			if (type.GenericInfo != null)
				NameResolutionService.EnterNamespace(new GenericParametersNamespaceExtender(
				                                     	type, NameResolutionService.CurrentNamespace));
		}

		private void CheckForCycles(SimpleTypeReference baseTypeRef, AbstractInternalType baseType, List visited)
		{
			if (visited.Contains(baseType.TypeDefinition))
			{
				BaseTypeError(CompilerErrorFactory.InheritanceCycle(baseTypeRef, baseType.FullName));
				return;
			}
			
			new BaseTypeResolution(Context, baseType.TypeDefinition, visited);
		}

		private void BaseTypeError(CompilerError error)
		{
			Errors.Add(error);
			_typeDefinition.BaseTypes.RemoveAt(_index - _removed);
			++_removed;
		}
	}
}
