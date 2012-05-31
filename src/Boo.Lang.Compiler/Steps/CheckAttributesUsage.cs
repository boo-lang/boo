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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Attribute = Boo.Lang.Compiler.Ast.Attribute;

namespace Boo.Lang.Compiler.Steps
{
	public class CheckAttributesUsage: AbstractFastVisitorCompilerStep
	{
		override public void OnMemberReferenceExpression(MemberReferenceExpression node)
		{
			base.OnMemberReferenceExpression(node);
			OnReferenceExpression(node);
		}

		override public void OnAttribute(Attribute node)
		{
			var attrType = AttributeType(node);
			if (attrType == null)
				return;

			var usage = AttributeUsageFor(attrType);
			if (usage == null)
				return;

			CheckAttributeUsage(node, attrType, usage);
		}

		private void CheckAttributeUsage(Attribute node, Type attrType, AttributeUsageAttribute usage)
		{
			CheckAttributeUsageTarget(node, attrType, usage);
			CheckAttributeUsageCardinality(node, attrType, usage);
		}

		private void CheckAttributeUsageTarget(Attribute node, Type attrType, AttributeUsageAttribute usage)
		{
			var validAttributeTargets = ValidAttributeTargetsFor(attrType, usage);
			if (validAttributeTargets == AttributeTargets.All)
				return;

			var target = AttributeTargetsFor(node);
			if (target.HasValue && !IsValid(target.Value, validAttributeTargets))
				Errors.Add(CompilerErrorFactory.InvalidAttributeTarget(node, attrType, validAttributeTargets));
		}

		private static AttributeTargets ValidAttributeTargetsFor(Type attrType, AttributeUsageAttribute usage)
		{
			return IsExtensionAttribute(attrType)
				? usage.ValidOn | AttributeTargets.Property
				: usage.ValidOn;
		}

		private void CheckAttributeUsageCardinality(Attribute node, Type attrType, AttributeUsageAttribute usage)
		{
			if (usage.AllowMultiple)
				return;

			if (HasSiblingAttributesOfSameType(node, attrType))
				MultipleAttributeUsageError(node, attrType);
		}

		private static bool HasSiblingAttributesOfSameType(Attribute node, Type attrType)
		{
			return SiblingAttributesOfSameType(node, attrType).Any();
		}

		private static IEnumerable<Attribute> SiblingAttributesOfSameType(Attribute node, Type attrType)
		{
			var attributeContainer = ((INodeWithAttributes) node.ParentNode);
			return attributeContainer.Attributes.Where(_ => _ != node && IsAttributeOfType(attrType, _));
		}

		private void MultipleAttributeUsageError(Attribute attribute, Type attrType)
		{
			Errors.Add(CompilerErrorFactory.MultipleAttributeUsage(attribute, attrType));
		}

		private static bool IsAttributeOfType(Type attrType, Attribute attribute)
		{
			var entity = attribute.Entity as IExternalEntity;
			return entity != null && entity.MemberInfo.DeclaringType == attrType;
		}

		private static bool IsExtensionAttribute(Type attrType)
		{
			return attrType == Types.ClrExtensionAttribute;
		}

		private static AttributeUsageAttribute AttributeUsageFor(Type attrType)
		{
			return (AttributeUsageAttribute) System.Attribute.GetCustomAttributes(attrType, typeof(AttributeUsageAttribute)).FirstOrDefault();
		}

		private static AttributeTargets? AttributeTargetsFor(Attribute node)
		{
			var parentNode = node.ParentNode as Method;
			if (parentNode != null)
			{
				var returnTypeAttributes = (parentNode).ReturnTypeAttributes;
				if (returnTypeAttributes.Contains(node))
					return AttributeTargets.ReturnValue;
			}

			AttributeTargets target;
			if (NodeUsageTargets().TryGetValue(node.ParentNode.GetType(), out target))
				return target;

			return null;
		}

		private static bool IsValid(AttributeTargets target, AttributeTargets validAttributeTargets)
		{
			return target == (validAttributeTargets & target);
		}

		private static Type AttributeType(Attribute node)
		{
			var attr = node.Entity as IExternalEntity;
			return attr == null ? null : attr.MemberInfo.DeclaringType;
		}

		override public void OnReferenceExpression(ReferenceExpression node)
		{
			var member = node.Entity as IMember;
			if (member == null) return;

			foreach (var attr in ObsoleteAttributesIn(member))
				if (attr.IsError)
					Errors.Add(CompilerErrorFactory.Obsolete(node, member, attr.Message));
				else
					Warnings.Add(CompilerWarningFactory.Obsolete(node, member, attr.Message));
		}

		private static IEnumerable<ObsoleteAttribute> ObsoleteAttributesIn(IMember member)
		{
			var external = member as IExternalEntity;
			if (external == null)
				return new ObsoleteAttribute[0];
			return System.Attribute.GetCustomAttributes(external.MemberInfo, typeof(ObsoleteAttribute)).Cast<ObsoleteAttribute>();
		}

		protected void OnInternalReferenceExpression(ReferenceExpression node)
		{
			// TODO:
		}

		private static Dictionary<Type, AttributeTargets> NodeUsageTargets()
		{
			return _nodesUsageTargets ?? (_nodesUsageTargets = NewUsageTargetsDictionary());
		}

		private static Dictionary<Type, AttributeTargets> NewUsageTargetsDictionary()
		{
			return new Dictionary<Type, AttributeTargets>
			{
				{typeof(Assembly), AttributeTargets.Assembly},
				{typeof(Ast.Module), AttributeTargets.Assembly},
				{typeof(ClassDefinition), AttributeTargets.Class},
				{typeof(StructDefinition), AttributeTargets.Struct},
				{typeof(EnumDefinition), AttributeTargets.Enum},
				{typeof(Constructor), AttributeTargets.Constructor},
				{typeof(Method), AttributeTargets.Method},
				{typeof(Property), AttributeTargets.Property},
				{typeof(Field), AttributeTargets.Field},
				{typeof(Event), AttributeTargets.Event},
				{typeof(InterfaceDefinition), AttributeTargets.Interface},
				{typeof(ParameterDeclaration), AttributeTargets.Parameter},
				{typeof(CallableDefinition), AttributeTargets.Delegate},
				{typeof(GenericParameterDeclaration), AttributeTargets.GenericParameter}
			};
		}

		private static Dictionary<Type, AttributeTargets> _nodesUsageTargets;
	}
}

