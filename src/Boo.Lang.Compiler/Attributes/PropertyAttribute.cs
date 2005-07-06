#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace Boo.Lang
{
	/// <summary>
	/// Creates a property over a field.
	/// </summary>
	public class PropertyAttribute : Boo.Lang.Compiler.AbstractAstAttribute
	{
		protected ReferenceExpression _propertyName;
		
		protected Expression _setPreCondition;
		
		protected BoolLiteralExpression _observable;
		
		protected ListLiteralExpression _attributes;

		public PropertyAttribute(ReferenceExpression propertyName) : this(propertyName, null)
		{
		}
		
		public PropertyAttribute(ReferenceExpression propertyName, Expression setPreCondition)
		{
			if (null == propertyName)
			{
				throw new ArgumentNullException("propertyName");
			}
			_propertyName = propertyName;
			_setPreCondition = setPreCondition;
		}
		
		public BoolLiteralExpression Observable
		{
			get
			{
				return _observable;
			}
			
			set
			{
				_observable = value;
			}
		}
		
		protected bool IsObservable
		{
			get
			{
				if (null == _observable)
				{
					return false;
				}
				return _observable.Value;
			}
		}
		
		protected string ChangedEventName
		{
			get
			{
				return _propertyName.Name + "Changed";
			}
		}
		
		public ListLiteralExpression Attributes
		{
			get
			{
				return _attributes;
			}
			
			set
			{
				_attributes = value;
			}
		}
		
		override public void Apply(Node node)
		{
			Field f = node as Field;
			if (null == f)
			{
				InvalidNodeForAttribute("Field");
				return;
			}			
			
			Property p = new Property();
			if (f.IsStatic)
			{
				p.Modifiers |= TypeMemberModifiers.Static;
			}
			p.Name = _propertyName.Name;
			p.Type = f.Type;
			p.Getter = CreateGetter(f);
			p.Setter = CreateSetter(f);
			p.LexicalInfo = LexicalInfo;
			
			if (Attributes != null && Attributes.Items.Count > 0)
			{
				foreach (Expression item in Attributes.Items)
				{
					p.Attributes.Add(ConvertExpressionToAttribute(item));
				}
			}
			
			f.DeclaringType.Members.Add(p);
			
			if (IsObservable)
			{
				f.DeclaringType.Members.Add(CreateChangedEvent(f));
			}
		}
		
		static public Boo.Lang.Compiler.Ast.Attribute ConvertExpressionToAttribute(
								Expression item)
		{
			Boo.Lang.Compiler.Ast.Attribute att =
				new Boo.Lang.Compiler.Ast.Attribute(item.LexicalInfo);
			if (item is MethodInvocationExpression)
			{
				MethodInvocationExpression m = (MethodInvocationExpression)item;
				att.Name = m.Target.ToString();
				att.Arguments = m.Arguments;
				att.NamedArguments = m.NamedArguments;
			}
			else
			{
				att.Name = item.ToString();
			}
			return att;
		}
		
		virtual protected Method CreateGetter(Field f)
		{
			// get:
			//		return <f.Name>
			Method getter = new Method();
			getter.Name = "get";
			getter.Body.Statements.Add(
				new ReturnStatement(
					base.LexicalInfo,
					new ReferenceExpression(f.Name),
					null
					)
				);
			return getter;
		}
		
		virtual protected Method CreateSetter(Field f)
		{
			Method setter = new Method();
			setter.Name = "set";
			
			if (null != _setPreCondition)
			{
				setter.Body.Add(
					new RaiseStatement(
						_setPreCondition.LexicalInfo,
						AstUtil.CreateMethodInvocationExpression(
							AstUtil.CreateReferenceExpression("System.ArgumentException"),
							new StringLiteralExpression("precondition '" +
														_setPreCondition.ToString() +
														"' failed:")),
						new StatementModifier(
							StatementModifierType.Unless,
							_setPreCondition)));						
			}
			setter.Body.Add(
				new BinaryExpression(
					base.LexicalInfo,
					BinaryOperatorType.Assign,
					new ReferenceExpression(f.Name),
					new ReferenceExpression("value")
					)
				);
			
			if (IsObservable)
			{
				MethodInvocationExpression mie = new MethodInvocationExpression(
														new ReferenceExpression(ChangedEventName));
				mie.Arguments.Add(new SelfLiteralExpression());
				mie.Arguments.Add(
					new MemberReferenceExpression(
						new MemberReferenceExpression(
							new ReferenceExpression("System"),
							"EventArgs"),
					"Empty"));
				setter.Body.Add(mie);
			}
			return setter;
		}
		
		protected Event CreateChangedEvent(Field f)
		{
			Event e = new Event(_observable.LexicalInfo);
			e.Name = ChangedEventName;
			e.Type = CodeBuilder.CreateTypeReference(typeof(System.EventHandler));
			return e;
		}
	}
}
