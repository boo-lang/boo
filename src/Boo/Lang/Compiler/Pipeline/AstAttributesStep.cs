#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using Boo.Lang.Ast;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Bindings;
using Boo.Lang.Compiler.Util;
using Boo.Lang;
using Reflection = System.Reflection;

namespace Boo.Lang.Compiler.Pipeline
{
	class ApplyAttributeTask : ITask
	{
		CompilerContext _context;

		Boo.Lang.Ast.Attribute _attribute;

		Type _type;

		public ApplyAttributeTask(CompilerContext context, Boo.Lang.Ast.Attribute attribute, Type type)
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
					aa.Apply(_attribute.ParentNode);
				}
			}
			catch (Exception x)
			{
				_context.Errors.AttributeResolution(_attribute, _type, x);
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
				_context.Errors.MissingConstructor(_attribute, _type, parameters, x);
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
				_context.Errors.NamedParameterMustBeReference(p);
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
						_context.Errors.AmbiguousName(p, name.Name, members);
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
					_context.Errors.NotAPublicFieldOrProperty(name, _type.FullName, name.Name);
					return false;
				}
			}
			return true;
		}
	}

	/// <summary>
	/// Step 2. Processes AST attributes.
	/// </summary>
	public class AstAttributesStep : AbstractNamespaceSensitiveCompilerStep
	{				
		TaskList _tasks;

		System.Text.StringBuilder _buffer = new System.Text.StringBuilder();
		
		ITypeBinding _astAttributeInterface;
		
		ITypeBinding _systemAttributeBaseClass;

		public AstAttributesStep()
		{			
			_tasks = new TaskList();
		}

		public override void Run()
		{
			_astAttributeInterface = BindingManager.ToTypeBinding(typeof(IAstAttribute));
			_systemAttributeBaseClass = BindingManager.ToTypeBinding(typeof(System.Attribute));
			
			int step = 0;
			while (step < CompilerParameters.MaxAttributeSteps)
			{
				Switch(CompileUnit);
				if (0 == _tasks.Count)
				{					
					break;
				}
				_tasks.Flush();
				++step;
			}
		}		

		public override void OnModule(Module module, ref Module resultingModule)
		{			
			PushNamespace(new ModuleNamespace(BindingManager, module));

			// do mdulo precisamos apenas visitar os membros
			Switch(module.Members);
			
			PopNamespace();
		}

		public override void OnBlock(Block node, ref Block resultingNode)
		{
			// No precisamos visitar blocos, isso
			// vai deixar o processamento um pouco mais
			// rpido
		}

		public override void OnAttribute(Boo.Lang.Ast.Attribute attribute, ref Boo.Lang.Ast.Attribute resultingNode)
		{
			// Neste primeiro passo tentamos apenas
			// resolver ast attributes.
			// Um passo posterior (resoluo de nomes e tipos) ir
			// assegurar que todos os nomes tenham sido resolvidos e colocar
			// mensagens de erro de acordo
			IBinding binding = ResolveQualifiedName(attribute, attribute.Name);
			if (null == binding)
			{
				binding = ResolveQualifiedName(attribute, BuildAttributeName(attribute.Name));
			}

			if (null != binding)
			{
				if (BindingType.Ambiguous == binding.BindingType)
				{
					Errors.AmbiguousName(attribute, attribute.Name, ((AmbiguousBinding)binding).Bindings);
				}
				else
				{
					if (BindingType.TypeReference != binding.BindingType)
					{
						Errors.NameNotType(attribute, attribute.Name);
					}
					else
					{
						ITypeBinding attributeType = ((ITypedBinding)binding).BoundType;
						if (IsAstAttribute(attributeType))
						{
							ExternalTypeBinding externalType = attributeType as ExternalTypeBinding;
							if (null == externalType)
							{
								Errors.AstAttributeMustBeExternal(attribute, attributeType);
							}
							else
							{							
								ScheduleAttributeApplication(attribute, externalType.Type);
								
								// remove it from parent
								resultingNode = null;
							}
						}
						else
						{
							if (!IsSystemAttribute(attributeType))
							{
								Errors.TypeNotAttribute(attribute, attributeType.FullName);
							}
							else
							{
								// remember the attribute's type
								BindingManager.Bind(attribute, attributeType);
							}
						}
					}
				}
			}
		}

		void ScheduleAttributeApplication(Boo.Lang.Ast.Attribute attribute, Type type)
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
		
		bool IsSystemAttribute(ITypeBinding type)
		{
			return type.IsSubclassOf(_systemAttributeBaseClass);
		}

		bool IsAstAttribute(ITypeBinding type)
		{
			return _astAttributeInterface.IsAssignableFrom(type);
		}
	}
}
