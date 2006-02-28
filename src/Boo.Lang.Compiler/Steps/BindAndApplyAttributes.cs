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

namespace Boo.Lang.Compiler.Steps
{
	using System;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.TypeSystem;
	using Boo.Lang.Compiler.Util;
	using Reflection = System.Reflection;

	class ApplyAttributeTask : ITask
	{
		CompilerContext _context;

		Boo.Lang.Compiler.Ast.Attribute _attribute;

		Type _type;

		public ApplyAttributeTask(CompilerContext context, Boo.Lang.Compiler.Ast.Attribute attribute, Type type)
		{
			_context = context;
			_attribute = attribute;
			_type = type;
		}

		public void Execute()
		{
			try
			{
				IAstAttribute aa = CreateAstAttributeInstance();
				if (null != aa)
				{
					aa.Initialize(_context);
					using (aa)
					{
						aa.Apply(_attribute.ParentNode);
					}
				}
			}
			catch (Exception x)
			{
				_context.Errors.Add(CompilerErrorFactory.AttributeApplicationError(x, _attribute, _type));
			}
		}

		public IAstAttribute CreateAstAttributeInstance()
		{
			object[] parameters = _attribute.Arguments.Count > 0 ? _attribute.Arguments.ToArray() : new object[0];

			IAstAttribute aa = null;
			try
			{
				aa = (IAstAttribute)Activator.CreateInstance(_type, parameters);
			}
			catch (MissingMethodException x)
			{
				_context.Errors.Add(CompilerErrorFactory.MissingConstructor(x, _attribute, _type, parameters));
				return null;
			}

			aa.Attribute = _attribute;

			if (_attribute.NamedArguments.Count > 0)
			{
				bool initialized = true;

				foreach (ExpressionPair p in _attribute.NamedArguments)
				{
					bool success = SetFieldOrProperty(aa, p);
					initialized = initialized && success;
				}

				if (!initialized)
				{
					return null;
				}
			}

			return aa;
		}

		bool SetFieldOrProperty(IAstAttribute aa, ExpressionPair p)
		{
			ReferenceExpression name = p.First as ReferenceExpression;
			if (null == name)
			{
				_context.Errors.Add(CompilerErrorFactory.NamedParameterMustBeIdentifier(p));
				return false;
			}
			else
			{
				Reflection.MemberInfo[] members = _type.FindMembers(
					Reflection.MemberTypes.Property | Reflection.MemberTypes.Field,
					Reflection.BindingFlags.Instance | Reflection.BindingFlags.Public,
					Type.FilterName, name.Name);
				if (members.Length > 0)
				{
					if (members.Length > 1)
					{
						_context.Errors.Add(CompilerErrorFactory.AmbiguousReference(name, members));
						return false;
					}
					else
					{
						Reflection.MemberInfo m = members[0];
						Reflection.PropertyInfo property = m as Reflection.PropertyInfo;
						if (null != property)
						{
							property.SetValue(aa, p.Second, null);
						}
						else
						{
							Reflection.FieldInfo field = m as Reflection.FieldInfo;
							if (null != field)
							{
								field.SetValue(aa, p.Second);
							}
							else
							{
								throw new InvalidOperationException();
							}
						}
					}
				}
				else
				{
					_context.Errors.Add(CompilerErrorFactory.NotAPublicFieldOrProperty(name, name.Name, _type.FullName));
					return false;
				}
			}
			return true;
		}
	}

	/// <summary>
	/// Step 2. Processes AST attributes.
	/// </summary>
	public class BindAndApplyAttributes : AbstractNamespaceSensitiveTransformerCompilerStep
	{
		
		TaskList _tasks;

		System.Text.StringBuilder _buffer = new System.Text.StringBuilder();
		
		IType _astAttributeInterface;
		
		IType _systemAttributeBaseClass;
		
		Boo.Lang.List _elements = new Boo.Lang.List();

		public BindAndApplyAttributes()
		{
			_tasks = new TaskList();
		}

		override public void Run()
		{
			_astAttributeInterface = TypeSystemServices.Map(typeof(IAstAttribute));
			_systemAttributeBaseClass = TypeSystemServices.Map(typeof(System.Attribute));
			
			int step = 0;
			while (step < Parameters.MaxAttributeSteps)
			{
				Visit(CompileUnit);
				if (0 == _tasks.Count)
				{
					break;
				}
				_tasks.Flush();
				++step;
			}
		}

		override public bool EnterModule(Boo.Lang.Compiler.Ast.Module module)
		{
			EnterNamespace((INamespace)TypeSystemServices.GetEntity(module));
			return true;
		}
		
		override public void LeaveModule(Boo.Lang.Compiler.Ast.Module module)
		{
			LeaveNamespace();
		}

		override public void OnBlock(Block node)
		{
			// No need to visit blocks
		}

		override public void OnAttribute(Boo.Lang.Compiler.Ast.Attribute attribute)
		{
			if (null != attribute.Entity)
			{
				return;
			}
			
			_elements.Clear();
			
			if (!NameResolutionService.ResolveQualifiedName(_elements, BuildAttributeName(attribute.Name)))
			{
				NameResolutionService.ResolveQualifiedName(_elements, attribute.Name);
			}

			if (_elements.Count > 0)
			{
				if (_elements.Count > 1)
				{
					Error(attribute, CompilerErrorFactory.AmbiguousReference(
									attribute,
									attribute.Name,
									_elements));
				}
				else
				{
					IEntity tag = (IEntity)_elements[0];
					if (EntityType.Type != tag.EntityType)
					{
						Error(attribute, CompilerErrorFactory.NameNotType(attribute, attribute.Name));
					}
					else
					{
						IType attributeType = ((ITypedEntity)tag).Type;
						if (IsAstAttribute(attributeType))
						{
							ExternalType externalType = attributeType as ExternalType;
							if (null == externalType)
							{
								Error(attribute, CompilerErrorFactory.AstAttributeMustBeExternal(attribute, attributeType.FullName));
							}
							else
							{
								ScheduleAttributeApplication(attribute, externalType.ActualType);
								
								RemoveCurrentNode();
							}
						}
						else
						{
							if (!IsSystemAttribute(attributeType))
							{
								Error(attribute, CompilerErrorFactory.TypeNotAttribute(attribute, attributeType.FullName));
							}
							else
							{
								// remember the attribute's type
								attribute.Name = attributeType.FullName;
								attribute.Entity = attributeType;
								CheckAttributeParameters(attribute);
							}
						}
					}
				}
			}
			else
			{
				Error(attribute, CompilerErrorFactory.UnknownAttribute(attribute, attribute.Name));
			}
		}
		
		private void CheckAttributeParameters(Boo.Lang.Compiler.Ast.Attribute node)
		{
			foreach(Expression e in node.Arguments)
			{
				if (e.NodeType == NodeType.BinaryExpression
					&& ((BinaryExpression)e).Operator == BinaryOperatorType.Assign)
				{
					Error(node, CompilerErrorFactory.ColonInsteadOfEquals(node));
				}
			}
		}
		
		override public void LeaveProperty(Property node)
		{
			if (node.Name == "self")
			{
				node.Name = "Item";
			}
			if (node.Name == "Item" && node.Parameters.Count > 0 && !node.IsStatic)
			{
				TypeDefinition t = node.ParentNode as TypeDefinition;
				if (t != null)
				{
					bool already_has_attribute = false;
					foreach(Boo.Lang.Compiler.Ast.Attribute a in t.Attributes)
					{
						if (a.Name.IndexOf("DefaultMember") >= 0)
						{
							already_has_attribute = true;
							break;
						}
					}
					if (!already_has_attribute)
					{
						Boo.Lang.Compiler.Ast.Attribute att = new Boo.Lang.Compiler.Ast.Attribute(t.LexicalInfo);
						att.Name = Types.DefaultMemberAttribute.FullName;
						att.Arguments.Add(new StringLiteralExpression(node.Name));
						t.Attributes.Add(att);
						Visit(att);
					}
				}
			}
		}
		
		void Error(Boo.Lang.Compiler.Ast.Attribute node, CompilerError error)
		{
			node.Entity = TypeSystemServices.ErrorEntity;
			Errors.Add(error);
		}

		void ScheduleAttributeApplication(Boo.Lang.Compiler.Ast.Attribute attribute, Type type)
		{
			_tasks.Add(new ApplyAttributeTask(_context, attribute, type));
		}

		string BuildAttributeName(string name)
		{
			_buffer.Length = 0;
			if (!Char.IsUpper(name[0]))
			{
				_buffer.Append(Char.ToUpper(name[0]));
				_buffer.Append(name.Substring(1));
				_buffer.Append("Attribute");
			}
			else
			{
				_buffer.Append(name);
				_buffer.Append("Attribute");
			}
			return _buffer.ToString();
		}
		
		bool IsSystemAttribute(IType type)
		{
			return type.IsSubclassOf(_systemAttributeBaseClass);
		}

		bool IsAstAttribute(IType type)
		{
			return _astAttributeInterface.IsAssignableFrom(type);
		}
	}
}
