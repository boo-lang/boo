#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.Util;
using Boo.Lang;
using Reflection = System.Reflection;

namespace Boo.Lang.Compiler.Steps
{
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

				// Tenta inicializar todas as propriedades
				// e campos (para obter o maior nmero de erros 
				// de uma nica vez)
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
						// Essa preocupao parece meio idiota, mas
						// como ainda no tenho certeza de que o modelo 
						// IL no permita dois membros diferentes com mesmo
						// nome vou deixar aqui
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
								// No poderia chegar aqui jamais!!!
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

		override public void OnModule(Boo.Lang.Compiler.Ast.Module module)
		{			
			EnterNamespace((INamespace)TypeSystemServices.GetEntity(module));
			Visit(module.Members);
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
			
			if (!NameResolutionService.ResolveQualifiedName(_elements, attribute.Name))
			{
				NameResolutionService.ResolveQualifiedName(_elements, BuildAttributeName(attribute.Name));
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
