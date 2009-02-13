//
// Gendarme.Rules.Abstract.DependencyCheckingRule
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

import System

import Mono.Cecil
import Mono.Cecil.Cil

import Gendarme.Framework
import Gendarme.Framework.Rocks
import Gendarme.Framework.Engines
import Gendarme.Framework.Helpers


[Problem("This type member depends from a type that it is not allowed to depend from.")]
[Solution("Refactor method or type so that this dependency is removed.")]
[EngineDependency(typeof(OpCodeEngine))]
abstract public class DependencyCheckingRule (Rule, ITypeRule):
"""
<summary>
This rule checks if a type contains members that are depending on types they
are, by design, not allowed to depend from.
This rules is abstract and is intended to be inherited by a project-specific rule.
</summary>
<example>
Bad example (with static/class dependency to namespace Foo not allowed):
<code>
public class Doer {
public void Do (Foo.Class klass)
{
}
}
</code>
</example>
<example>
Good example (with static/class dependency to namespace Foo not allowed):
<code>
public class Doer {
public void Do (Foo.IClass klass) //use interface IClass instead of Class
{
}
}
</code>
</example>
"""

	protected Permissions as DependencyPermission*:
	"""
	Permissions get method is the *only* method inheritors are required to implement.

	Implementors should use CurrentType or CurrentMember property to return
	context-dependent permissions.
	"""
		abstract get:
			pass


	protected HasPermissions as bool:
		virtual get:
			return false if not Permissions
			return Permissions.GetEnumerator().MoveNext()


	protected virtual def ReportDefect(type as TypeReference, culprit as object) as void:
		if CurrentMember isa IMethodSignature:
			msg = "Method signature `${CurrentMember}' is not allowed to reference type `${type}'."
		elif CurrentMember isa FieldDefinition:
			msg = "Field `${CurrentMember}` of type ${CurrentMethod}' is not allowed to reference type `${type}'."
		elif culprit and culprit isa VariableDefinition:
			msg = "Variable `${culprit}' of method `${CurrentMember}' is not allowed to reference type `${type}'."
		elif culprit and culprit isa Instruction:
			msg = "Method `${CurrentMember}' is not allowed to call type `${type}'."
		else:
			msg = "Type `${CurrentMember}' is not allowed to reference type `${type}'."
		Runner.Report(Defect(self, CurrentType, culprit or CurrentMember, Severity.High, Confidence.Total, msg));


	protected virtual def NeedsChecking(type as TypeDefinition) as bool:
		return false if type.IsEnum or TypeRocks.IsGeneratedCode(type) #FIXME:resolution bug!?
		return HasPermissions


	protected enum DependencyRelation:
		Any
		Static		//static dependency  (class/struct)
		Dynamic		//dynamic dependency (interface)


	protected enum DependencyVisibility:
		Any
		Visible
		NonVisible


	protected class DependencyPermission:
	"""
	You can inherit this class if you need different or additional behavior.
	"""

		[property(Namespace)] #namespace in a general sense (a type is also a namespace)
		_namespace as string

		Allow:
			get: return not _deny
			set: _deny = false

		[property(Deny)] #default permission is Deny
		_deny as bool = true

		[property(Relation)]
		_rel as DependencyRelation

		[property(Visibility)]
		_vis as DependencyVisibility

		_rule as DependencyCheckingRule

		public def constructor(rule as DependencyCheckingRule):
			_rule = rule


		public virtual def Match(target as TypeReference, member as IMemberReference) as bool:
			return false if not DoesTargetMatchNamespace(target.FullName, _namespace)

			t = target.Resolve()
			return false if not t //resolution failed

			return false if _rel == DependencyRelation.Dynamic and not IsDynamic(t)
			return false if _rel == DependencyRelation.Static and IsDynamic(t)

			return false if _vis == DependencyVisibility.Visible and not IsVisible(member)
			return false if _vis == DependencyVisibility.NonVisible and IsVisible(member)

			return true


		protected virtual def IsDynamic(type as TypeDefinition) as bool:
			return type.IsInterface


		protected virtual def IsVisible(member as IMemberReference) as bool:
			type = member as TypeReference
			if type:
				return TypeRocks.IsVisible(type) #FIXME: NRE FAIL!?
			elif (method = member as MethodReference):
				return MethodRocks.IsVisible(method)
			elif (field = member as FieldReference):
				return FieldRocks.IsVisible(field)

			return false #variable or call


		protected virtual def DoesTargetMatchNamespace(target as string, ns as string) as bool:
			return _rule.DoesTargetMatchNamespace(target, ns)


	protected virtual def DoesTargetMatchNamespace(target as string, ns as string) as bool:
		return target.StartsWith(ns)




	[property(CurrentMember, Protected: true)]
	_member as IMemberReference

	_type as TypeDefinition
	protected CurrentType as TypeDefinition:
		get: return _type
		private set:
			_member = value
			_type = value

	_method as MethodDefinition
	protected CurrentMethod as MethodDefinition:
		get: return _method
		private set:
			_member = value
			_method = value


	public def CheckType(type as TypeDefinition) as RuleResult:
		CurrentType = type
		if not NeedsChecking(type):
			return RuleResult.DoesNotApply

		CheckBaseTypes()
		CheckFields() if type.HasFields
		CheckMethods() if type.HasMethods

		return Runner.CurrentRuleResult


	def CheckPermissionsChain(type as TypeReference) as bool:
		return CheckPermissionsChain[of IMetadataTokenProvider](type, null)

	def CheckPermissionsChain[of T(class)](type as TypeReference, culprit as T) as bool:
		return if not type
		for perm in Permissions:
			if perm.Match(type, CurrentMember):
				if perm.Deny:
					ReportDefect(type, culprit)
					return false
				break
		return true


	def CheckBaseTypes():
		for iface as TypeReference in _type.Interfaces:
			CheckPermissionsChain(iface)

		CheckPermissionsChain(_type.BaseType)


	def CheckFields():
		for field as FieldDefinition in _type.Fields:
			CurrentMember = field
			CheckPermissionsChain(field.FieldType)


	def CheckMethods():
		for method as MethodDefinition in _type.Methods:
			CheckMethod(method)


	def CheckMethod(method as MethodDefinition):
		CurrentMethod = method
		if CheckMethodSignature() and method.HasBody:
			#only run Variables checks if method not FAIL already
			if CheckMethodVariables(method.Body):
				#only run IL-level checks if method not FAIL already
				CheckMethodBody(method.Body)


	def CheckMethodSignature() as bool:
		valid = true
		valid &= CheckPermissionsChain(_method.ReturnType.ReturnType, _method.ReturnType)

		for p as ParameterDefinition in _method.Parameters:
			valid &= CheckPermissionsChain(p.ParameterType, p)

		return valid


	def CheckMethodVariables(body as MethodBody) as bool:
		return if not body.HasVariables

		valid = true
		for variable as VariableReference in body.Variables:
			valid &= CheckPermissionsChain(variable.VariableType, variable)

		return valid


	def CheckMethodBody(body as MethodBody):
		#TODO: FIXME: option to ignore body?
		return if not callsAndNewobjOpCodeBitmask.Intersect(OpCodeEngine.GetBitmask(CurrentMethod))

		valid = true
		for ins as Instruction in body.Instructions:
			continue if not callsAndNewobjOpCodeBitmask.Get(ins.OpCode.Code)
			valid &= CheckPermissionsChain(cast(MethodReference, ins.Operand).DeclaringType, ins)
		return valid


	static final callsAndNewobjOpCodeBitmask = OpCodeBitmask(0x8000000000, 0x4400000000000, 0x0, 0x0)
	"""
		OpCodeBitmask mask = new OpCodeBitmask ();
		mask.UnionWith (OpCodeBitmask.Calls);
		mask.Set (Code.Newobj);
		return mask;
	"""

