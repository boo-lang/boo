//
// Gendarme.Rules.Abstract.DependenciesMacro DSL for DependencyCheckingRule
//
// Authors:
//	Cedric Vivier  <cedricv@neonux.com>
//
// Copyright (C) 2009 Cedric Vivier
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace Gendarme.Rules.Abstract

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Services
import Boo.Lang.PatternMatching


macro dependencies(name as ReferenceExpression):
"""
This macro/DSL generates a Gendarme rule that checks if there is disallowed
internal dependencies in a project.

Example below denies using any BCL type from within type/namespace Foo:
<code>
dependencies NameOfTheDependencySet:
	within Foo:
		deny System
</code>

Example below denies using classes from namespace Bar within namespace Foo
(and its inner namespaces), this means usage of Bar interfaces is still allowed:
<code>
dependencies NameOfTheDependencySet:
	within Foo:
		deny Bar, Class
</code>

Example below denies using interfaces of Bar namespace within visible members
of type/namespace Foo:
<code>
dependencies NameOfTheDependencySet:
	within Foo:
		deny Bar, Interface|Visible
</code>
"""
	raise dependencies.Documentation if not len(dependencies.Body.Statements)


	macro within(reference):
	"""
	Macro `within' must contain at least one `deny' or `allow' macro.
	"""
		raise within.Documentation if not len(within.Body.Statements)

		macro deny(reference, options as ReferenceExpression*):
			yield [| yield $(BuildDependencyPermission(NameResolutionService, reference, true, options)) |]

		macro allow(reference, options as ReferenceExpression*):
			yield [| yield $(BuildDependencyPermission(NameResolutionService, reference, false, options)) |]


		nref = GetNormalizedReference(reference)
		entity = NameResolutionService.ResolveQualifiedName(nref, EntityType.Namespace|EntityType.Type)
		if not entity or entity.EntityType != EntityType.Namespace:
			matcher = [| DoesTargetMatchNamespace(CurrentType, typeof($nref).FullName) |] #TODO: MatchType
		else: #namespace
			matcher = [| DoesTargetMatchNamespace(CurrentType.FullName, $(nref)) |]
		yield [|
			if $matcher:
				$(within.Body)
		|]


	yield [|
		public class $(name + "DependencyCheckingRule") (DependencyCheckingRule):
			protected Permissions as DependencyPermission*:
				override get:
					$(dependencies.Body)
	|]


internal def GetNormalizedReference(reference as Expression):
	nref = reference.ToCodeString()
	//nref = nref.Substring(0, len(nref)-2) if nref.EndsWith('_')
	return nref


internal def BuildDependencyPermission(nre as NameResolutionService, reference as ReferenceExpression, deny as bool, options as ReferenceExpression*):
	nref = GetNormalizedReference(reference)
	entity = nre.ResolveQualifiedName(nref, EntityType.Namespace|EntityType.Type)
	perm = [| DependencyPermission(self) |]

	if not entity or entity.EntityType != EntityType.Namespace:
		perm.NamedArguments.Add([| Namespace: typeof($nref).FullName |]) #TODO: Type:
	else: #namespace
		perm.NamedArguments.Add([| Namespace: $nref |])

	if not deny:
		perm.NamedArguments.Add([| Deny: false |])

	for option in options:
		if option.Name == "Class": perm.NamedArguments.Add([| Relation: DependencyRelation.Static |])
		elif option.Name == "Interface": perm.NamedArguments.Add([| Relation: DependencyRelation.Dynamic |])
		elif option.Name == "Visible": perm.NamedArguments.Add([| Visibility: DependencyVisibility.Visible |])
		elif option.Name == "NonVisible": perm.NamedArguments.Add([| Visibility: DependencyVisibility.NonVisible |])
		else: raise "Invalid option: `${option.Name}'"

	return perm

