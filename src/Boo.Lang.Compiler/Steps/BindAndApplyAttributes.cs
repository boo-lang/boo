#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Bindings;
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
	public class BindAndApplyAttributes : AbstractNamespaceSensitiveCompilerStep
	{				
		TaskList _tasks;

		System.Text.StringBuilder _buffer = new System.Text.StringBuilder();
		
		ITypeBinding _astAttributeInterface;
		
		ITypeBinding _systemAttributeBaseClass;

		public BindAndApplyAttributes()
		{			
			_tasks = new TaskList();
		}

		override public void Run()
		{
			using (BindBaseTypes binder = new BindBaseTypes())
			{
				binder.Initialize(_context);
				binder.Run();
			}
			
			_astAttributeInterface = BindingManager.AsTypeBinding(typeof(IAstAttribute));
			_systemAttributeBaseClass = BindingManager.AsTypeBinding(typeof(System.Attribute));
			
			int step = 0;
			while (step < CompilerParameters.MaxAttributeSteps)
			{
				Accept(CompileUnit);
				if (0 == _tasks.Count)
				{					
					break;
				}
				_tasks.Flush();
				++step;
			}
		}		

		override public void OnModule(Module module)
		{			
			PushNamespace((INamespace)BindingManager.GetBinding(module));

			// do mdulo precisamos apenas visitar os membros
			Accept(module.Members);
			
			PopNamespace();
		}

		override public void OnBlock(Block node)
		{
			// No precisamos visitar blocos, isso
			// vai deixar o processamento um pouco mais
			// rpido
		}

		override public void OnAttribute(Boo.Lang.Compiler.Ast.Attribute attribute)
		{			
			if (BindingManager.IsBound(attribute))
			{
				return;
			}
			
			IBinding binding = ResolveQualifiedName(attribute, attribute.Name);
			if (null == binding)
			{
				binding = ResolveQualifiedName(attribute, BuildAttributeName(attribute.Name));
			}

			if (null != binding)
			{
				if (BindingType.Ambiguous == binding.BindingType)
				{
					Error(attribute, CompilerErrorFactory.AmbiguousReference(
									attribute,
									attribute.Name,
									((AmbiguousBinding)binding).Bindings));
				}
				else
				{
					if (BindingType.TypeReference != binding.BindingType)
					{
						Error(attribute, CompilerErrorFactory.NameNotType(attribute, attribute.Name));
					}
					else
					{
						ITypeBinding attributeType = ((ITypedBinding)binding).BoundType;
						if (IsAstAttribute(attributeType))
						{
							ExternalTypeBinding externalType = attributeType as ExternalTypeBinding;
							if (null == externalType)
							{
								Error(attribute, CompilerErrorFactory.AstAttributeMustBeExternal(attribute, attributeType.FullName));
							}
							else
							{							
								ScheduleAttributeApplication(attribute, externalType.Type);
								
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
								BindingManager.Bind(attribute, attributeType);
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
		
		void Error(Boo.Lang.Compiler.Ast.Attribute attribute, CompilerError error)
		{
			BindingManager.Error(attribute);
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
