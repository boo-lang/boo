#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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


using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using System;

namespace Boo.Lang.Compiler.Steps.MacroProcessing
{
	sealed class NodeGeneratorExpander
	{
		private readonly MacroStatement _node;
		private readonly bool _addTypeMembersToEnclosingTypeDefinition;
		private readonly StatementTypeMember _typeMemberPrototype;
		private TypeDefinition _enclosingTypeDefCache;

		public NodeGeneratorExpander(MacroStatement node, bool addTypeMembersToEnclosingTypeDefinition)
		{
			_node = node;
			_addTypeMembersToEnclosingTypeDefinition = addTypeMembersToEnclosingTypeDefinition;
			_typeMemberPrototype = node.ParentNode as StatementTypeMember;
		}

		public Statement Expand(IEnumerable<Node> generator)
		{
			Block resultingBlock = new Block();
			foreach (Node node in generator)
			{
				//'yield' (ie. implicit 'yield null') means 'yield `macro`.Body'
				Node generatedNode = node ?? _node.Body;
				if (null == generatedNode)
					continue;

				TypeMember member = generatedNode as TypeMember;
				if (null != member)
				{
					ExpandTypeMember(member, resultingBlock);
					continue;
				}

				Block block = generatedNode as Block;
				if (null != block)
				{
					resultingBlock.Add(block);
					continue;
				}

				Statement statement = generatedNode as Statement;
				if (null != statement)
				{
					resultingBlock.Add(statement);
					continue;
				}

				Expression expression = generatedNode as Expression;
				if (null != expression)
				{
					resultingBlock.Add(expression);
					continue;
				}

				Import import = generatedNode as Import;
				if (null != import)
				{
					ExpandImport(import);
					continue;
				}

				throw new CompilerError(_node, "Unsupported expansion: " + generatedNode.ToCodeString());
			}

			return resultingBlock.IsEmpty
					? null
					: resultingBlock.Simplify();
		}

		private void ExpandTypeMember(TypeMember member, Block resultingBlock)
		{
			ApplyPrototypeModifiersAndAttributesTo(member);
			if (_addTypeMembersToEnclosingTypeDefinition)
				AddMemberToEnclosingTypeDef(member);
			else
				resultingBlock.Add(new TypeMemberStatement(member));
		}

		private void AddMemberToEnclosingTypeDef(TypeMember member)
		{
			if (null == _typeMemberPrototype)
				EnclosingTypeDef().Members.Add(member);
			else
				EnclosingTypeDef().Members.Insert(TypeMemberInsertionPoint(), member);
		}

		private int TypeMemberInsertionPoint()
		{
			TypeMemberCollection members = EnclosingTypeDef().Members;
			return members.IndexOf(_typeMemberPrototype);
		}

		private TypeDefinition EnclosingTypeDef()
		{
			if (null == _enclosingTypeDefCache)
				_enclosingTypeDefCache = _node.GetAncestor<TypeDefinition>();
			return _enclosingTypeDefCache;
		}

		private void ApplyPrototypeModifiersAndAttributesTo(TypeMember member)
		{
			if (null == _typeMemberPrototype) return;

			member.Attributes.ExtendWithClones(_typeMemberPrototype.Attributes);
			member.Modifiers |= _typeMemberPrototype.Modifiers;
		}

		private void ExpandImport(Import import)
		{
			ImportCollection imports = _node.GetAncestor<Module>().Imports;
			if (imports.Contains(import.Matches))
				return;

			imports.Add(import);
			BindImport(import);
		}

		private void BindImport(Import import)
		{
			CompilerContext context = CompilerContext.Current;
			INamespace previous = context.NameResolutionService.CurrentNamespace;
			try
			{
				context.NameResolutionService.Reset();

				BindNamespaces namespaceBinder = new BindNamespaces();
				namespaceBinder.Initialize(context);
				import.Accept(namespaceBinder);
			}
			catch (Exception x)
			{
				throw new CompilerError(_node, "Error expanding " + import.ToCodeString(), x);
			}
			finally
			{
				context.NameResolutionService.Restore(previous);
			}
		}
	}
}
