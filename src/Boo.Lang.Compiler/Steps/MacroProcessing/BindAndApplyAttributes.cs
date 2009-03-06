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
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.Steps.MacroProcessing
{
	sealed class ApplyAttributeTask : ITask
	{
		CompilerContext _context;

		Boo.Lang.Compiler.Ast.Attribute _attribute;

		Type _type;
		
		Node _targetNode;

		public ApplyAttributeTask(CompilerContext context, Boo.Lang.Compiler.Ast.Attribute attribute, Type type)
		{
			_context = context;
			_attribute = attribute;
			_type = type;
			_targetNode = GetTargetNode();
		}
		
		private Node GetTargetNode()
		{
			Module module = _attribute.ParentNode as Module;
			if (module != null && module.AssemblyAttributes.Contains(_attribute))
			{
				return module.ParentNode;
			}
			return _attribute.ParentNode;
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
						aa.Apply(_targetNode);
					}
				}
			}
			catch (Exception x)
			{
				_context.TraceError(x);
				_context.Errors.Add(CompilerErrorFactory.AttributeApplicationError(x, _attribute, _type));
				System.Console.WriteLine(x.StackTrace);
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
				System.Reflection.MemberInfo[] members = _type.FindMembers(
					System.Reflection.MemberTypes.Property | System.Reflection.MemberTypes.Field,
					System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public,
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
						System.Reflection.MemberInfo m = members[0];
						System.Reflection.PropertyInfo property = m as System.Reflection.PropertyInfo;
						if (null != property)
						{
							property.SetValue(aa, p.Second, null);
						}
						else
						{
							System.Reflection.FieldInfo field = m as System.Reflection.FieldInfo;
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

		readonly List<IEntity> _elements = new List<IEntity>();

		public BindAndApplyAttributes()
		{
			_tasks = new TaskList();
		}

		public override void Initialize(CompilerContext context)
		{
			base.Initialize(context);
			_astAttributeInterface = TypeSystemServices.Map(typeof(IAstAttribute));
		}

		override public void Run()
		{	
			int iteration = 0;
			while (iteration < Parameters.MaxExpansionIterations)
			{
				if (!BindAndApply())
				{
					break;
				}
				++iteration;
			}
		}

		public bool BindAndApply()
		{
			Visit(CompileUnit);
			if (_tasks.Count == 0)
			{
				return false;
			}
			_tasks.Flush();
			return true;
		}

		override public void OnModule(Boo.Lang.Compiler.Ast.Module module)
		{
			EnterNamespace(InternalModule.ScopeFor(module));
			try
			{
				Visit(module.Members);
				Visit(module.Globals);
				Visit(module.Attributes);
				Visit(module.AssemblyAttributes);
			}
			finally
			{
				LeaveNamespace();
			}
		}
		
		void VisitTypeDefinition(TypeDefinition node)
		{
			Visit(node.Members);
			Visit(node.Attributes);
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{
			VisitTypeDefinition(node);
		}
		
		override public void OnInterfaceDefinition(InterfaceDefinition node)
		{
			VisitTypeDefinition(node);
		}
		
		override public void OnStructDefinition(StructDefinition node)
		{
			VisitTypeDefinition(node);
		}
		
		override public void OnEnumDefinition(EnumDefinition node)
		{
			VisitTypeDefinition(node);
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
			
			if (!NameResolutionService.ResolveQualifiedName(_elements, BuildAttributeName(attribute.Name, true)))
			{
				if (!NameResolutionService.ResolveQualifiedName(_elements, BuildAttributeName(attribute.Name, false)))
				{
					NameResolutionService.ResolveQualifiedName(_elements, attribute.Name);
				}
			}

			if (_elements.Count == 0)
			{
				string suggestion = NameResolutionService.GetMostSimilarTypeName(BuildAttributeName(attribute.Name, true));
				if (null == suggestion)
					suggestion = NameResolutionService.GetMostSimilarTypeName(BuildAttributeName(attribute.Name, false));

				Error(attribute, CompilerErrorFactory.UnknownAttribute(attribute, attribute.Name, suggestion));
				return;
			}
						
			if (_elements.Count > 1)
			{
				Error(attribute, CompilerErrorFactory.AmbiguousReference(
				                 	attribute,
				                 	attribute.Name,
				                 	_elements));
				return;
			}

			// if _elements.Count == 1
			IEntity tag = (IEntity)_elements[0];
			if (EntityType.Type != tag.EntityType)
			{
				Error(attribute, CompilerErrorFactory.NameNotType(attribute, attribute.Name, tag.ToString(), null));
				return;
			}
			
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
		
		void Error(Boo.Lang.Compiler.Ast.Attribute node, CompilerError error)
		{
			node.Entity = TypeSystemServices.ErrorEntity;
			Errors.Add(error);
		}

		void ScheduleAttributeApplication(Boo.Lang.Compiler.Ast.Attribute attribute, Type type)
		{
			_tasks.Add(new ApplyAttributeTask(_context, attribute, type));
		}

		string BuildAttributeName(string name, bool forcePascalNaming)
		{
			_buffer.Length = 0;
			if (forcePascalNaming && !Char.IsUpper(name[0]))
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
			return TypeSystemServices.IsAttribute(type);
		}

		bool IsAstAttribute(IType type)
		{
			return _astAttributeInterface.IsAssignableFrom(type);
		}
	}
}
