using System;
using Boo.Ast;
using Boo.Ast.Compilation;
using Boo.Ast.Compilation.Binding;
using Boo.Ast.Compilation.Util;
using Boo.Lang;
using Reflection = System.Reflection;

namespace Boo.Ast.Compilation.Steps
{
	class ApplyAttributeTask : ITask
	{
		CompilerContext _context;

		Attribute _attribute;

		Type _type;

		public ApplyAttributeTask(CompilerContext context, Attribute attribute, Type type)
		{
			_context = context;
			_attribute = attribute;
			_type = type;
		}

		public void Execute()
		{
			RemoveAttributeFromNode();

			try
			{
				AstAttribute aa = CreateAstAttributeInstance();
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

		public AstAttribute CreateAstAttributeInstance()
		{
			object[] parameters = _attribute.Arguments.Count > 0 ? _attribute.Arguments.ToArray() : new object[0];

			AstAttribute aa = null;
			try
			{
				aa = (AstAttribute)Activator.CreateInstance(_type, parameters);
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

		void RemoveAttributeFromNode()
		{
			((INodeWithAttributes)_attribute.ParentNode).Attributes.Remove(_attribute);
		}

		bool SetFieldOrProperty(AstAttribute aa, ExpressionPair p)
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
					_context.Errors.NotAPublicFieldOrProperty(name, _type, name.Name);
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

		public AstAttributesStep()
		{			
			_tasks = new TaskList();
		}

		public override void Run()
		{
			int step = 0;
			while (step < CompilerParameters.MaxAttributeSteps)
			{
				OnCompileUnit(CompileUnit);
				if (0 == _tasks.Count)
				{
					// Colocar informao de tracing aqui...
					break;
				}
				_tasks.Flush();
				++step;
			}
		}		

		public override void OnModule(Module module)
		{			
			PushNamespace(new ModuleNameSpace(BindingManager, module));

			// do mdulo precisamos apenas visitar os membros
			module.Members.Switch(this);
			
			PopNamespace();
		}

		public override void OnBlock(Block node)
		{
			// No precisamos visitar blocos, isso
			// vai deixar o processamento um pouco mais
			// rpido
		}

		public override void OnAttribute(Boo.Ast.Attribute attribute)
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
						Type attributeType = ((ITypedBinding)binding).BoundType.Type;
						if (IsAstAttribute(attributeType))
						{						
							ScheduleAttributeApplication(attribute, attributeType);
						}
					}
				}
			}
		}

		void ScheduleAttributeApplication(Attribute attribute, Type type)
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

		public static bool IsAstAttribute(Type type)
		{
			return typeof(AstAttribute).IsAssignableFrom(type);
		}
	}
}
