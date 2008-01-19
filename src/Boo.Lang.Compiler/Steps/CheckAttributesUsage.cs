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


namespace Boo.Lang.Compiler.Steps
{
	using System;
	using System.Reflection;
	using System.Collections.Generic;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;


	public class CheckAttributesUsage: AbstractVisitorCompilerStep
	{
		override public void Run()
		{
			Visit(CompileUnit);
		}

		override public void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
			OnReferenceExpression(node);
		}

		override public void OnAttribute(Boo.Lang.Compiler.Ast.Attribute node)
		{
			IExternalEntity attr = TypeSystemServices.GetOptionalEntity(node) as IExternalEntity;
			if (attr == null) return;
			Type attrType = attr.MemberInfo.DeclaringType;

			// check allowed attribute usage(s)
			System.Attribute[] usages = System.Attribute.GetCustomAttributes(attrType, typeof(AttributeUsageAttribute));
			if (usages.Length > 0)
			{
				//only one AttributeUsage is allowed anyway
				AttributeUsageAttribute usage = usages[0] as AttributeUsageAttribute;
				
				if (AttributeTargets.All != usage.ValidOn)
				{
					if (null == _nodesUsageTargets) SetupNodesUsageTargetsDictionary();

					Type nodeType = node.ParentNode.GetType();
					if (_nodesUsageTargets.ContainsKey(nodeType))
						if (( nodeType != typeof(Ast.Module) && _nodesUsageTargets[nodeType] != (usage.ValidOn & _nodesUsageTargets[nodeType]) )
							|| (_nodesUsageTargets[nodeType] != (usage.ValidOn & _nodesUsageTargets[nodeType])
								&& _nodesUsageTargets[typeof(Assembly)] != (usage.ValidOn & _nodesUsageTargets[typeof(Assembly)]) ) )
							Errors.Add(
								CompilerErrorFactory.InvalidAttributeTarget(node, attrType, usage.ValidOn));
				}
				if (!usage.AllowMultiple)
				{
					INodeWithAttributes m = node.ParentNode as INodeWithAttributes;
					foreach (Boo.Lang.Compiler.Ast.Attribute mAttr in m.Attributes) {
						if (mAttr == node) continue;
						IExternalEntity mAttrEnt = TypeSystemServices.GetOptionalEntity(mAttr) as IExternalEntity;
						if (null != mAttrEnt && mAttrEnt.MemberInfo.DeclaringType == attrType)
							Errors.Add(
								CompilerErrorFactory.MultipleAttributeUsage(node, attrType));
					}
				}
			}

			// handle special compiler-supported attributes
			//TODO: ObsoleteAttribute
		}

		override public void OnReferenceExpression(ReferenceExpression node)
		{
			IExternalEntity member = TypeSystemServices.GetOptionalEntity(node) as IExternalEntity;
			if (member == null) {//extract to OnInternalReferenceExpression
				OnInternalReferenceExpression(node);
				return;
			}
			
			System.Attribute[] attributes = System.Attribute.GetCustomAttributes(member.MemberInfo, typeof(ObsoleteAttribute));
			foreach (ObsoleteAttribute attr in attributes)
			{
				if (attr.IsError)
					Errors.Add(
						CompilerErrorFactory.Obsolete(node, member.ToString(), attr.Message));
				else
					Warnings.Add(
						CompilerWarningFactory.Obsolete(node, member.ToString(), attr.Message));
			}
		}

		protected void OnInternalReferenceExpression(ReferenceExpression node)
		{
		}

		private static Dictionary<Type, AttributeTargets> _nodesUsageTargets = null;

		private static void SetupNodesUsageTargetsDictionary()
		{
			_nodesUsageTargets = new Dictionary<Type, AttributeTargets>();
			_nodesUsageTargets.Add(typeof(Assembly), AttributeTargets.Assembly);
			_nodesUsageTargets.Add(typeof(Ast.Module), AttributeTargets.Module);
			_nodesUsageTargets.Add(typeof(ClassDefinition), AttributeTargets.Class);
			_nodesUsageTargets.Add(typeof(StructDefinition), AttributeTargets.Struct);
			_nodesUsageTargets.Add(typeof(EnumDefinition), AttributeTargets.Enum);
			_nodesUsageTargets.Add(typeof(Constructor), AttributeTargets.Constructor);
			_nodesUsageTargets.Add(typeof(Method), AttributeTargets.Method);
			_nodesUsageTargets.Add(typeof(Property), AttributeTargets.Property);
			_nodesUsageTargets.Add(typeof(Field), AttributeTargets.Field);
			_nodesUsageTargets.Add(typeof(Ast.Event), AttributeTargets.Event);
			_nodesUsageTargets.Add(typeof(InterfaceDefinition), AttributeTargets.Interface);
			_nodesUsageTargets.Add(typeof(ParameterDeclaration), AttributeTargets.Parameter);
			_nodesUsageTargets.Add(typeof(CallableDefinition), AttributeTargets.Delegate);
			_nodesUsageTargets.Add(typeof(ReturnStatement), AttributeTargets.ReturnValue);
			_nodesUsageTargets.Add(typeof(GenericParameterDeclaration), AttributeTargets.GenericParameter);
		}

	}
}

