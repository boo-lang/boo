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
