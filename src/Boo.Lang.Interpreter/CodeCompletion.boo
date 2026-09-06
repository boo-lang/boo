#region license
// Copyright (c) 2026 the Boo contributors
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

namespace Boo.Lang.Interpreter

import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Services

class FindCodeCompleteSuggestion(AbstractVisitorCompilerStep):
"""
Resolves the marker left where the cursor was.

The code handed to the compiler has the name being typed replaced by the
marker, so binding it says what the thing before the dot is, and that is what
the suggestions come from.
"""

	public static final Marker = "__codecomplete__"

	override def Run():
		Visit(CompileUnit)

	override def LeaveMemberReferenceExpression(node as MemberReferenceExpression):
		if Marker == node.Name:
			suggestion as IEntity
			target = node.Target
			if target.ExpressionType is not null:
				suggestion = target.ExpressionType
			else:
				suggestion = TypeSystemServices.GetOptionalEntity(target)
			if suggestion is not null and suggestion.EntityType != EntityType.Error:
				_context["suggestion"] = suggestion
				// TODO: use target to display static members only for type reference expressions
				_context["target"] = target

static class CodeCompletion:
"""What is worth offering for an entity the marker resolved to."""

	def SuggestionsFor(entity as IEntity, namespacesOnly as bool) as (IEntity):
		ns = entity as INamespace
		return array(IEntity, 0) if ns is null
		return ChildNamespaces(ns) if namespacesOnly
		return Members(MemberCollector.CollectAllMembers(ns))

	def Members(members as (IEntity)) as (IEntity):
		return array(
				item
				for item in members
				unless IsSpecial(item) or not IsPublic(item))

	def ChildNamespaces(parent as INamespace) as (IEntity):
		return array(member
					for member in parent.GetMembers()
					if member.EntityType == EntityType.Namespace)

	def IsSpecial(entity as IEntity) as bool:
		for prefix in ".", "___", "add_", "remove_", "raise_", "get_", "set_", "<":
			return true if entity.Name.StartsWith(prefix)
		return false

	def IsPublic(entity as IEntity) as bool:
		member = entity as IMember
		return member is null or member.IsPublic
