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
using System.Reflection;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;
using Module = Boo.Lang.Compiler.Ast.Module;

namespace Boo.Lang.Compiler.Steps.MacroProcessing
{
	sealed class ApplyAttributeTask
	{
		CompilerContext _context;

		Ast.Attribute _attribute;

		Type _type;
		
		Node _targetNode;

		public ApplyAttributeTask(CompilerContext context, Ast.Attribute attribute, Type type)
		{
			_context = context;
			_attribute = attribute;
			_type = type;
			_targetNode = TargetNode();
		}
		
		private Node TargetNode()
		{
			return IsAssemblyAttribute() ? _context.CompileUnit : _attribute.ParentNode;
		}

		private bool IsAssemblyAttribute()
		{
			var module = _attribute.ParentNode as Module;
			return module != null && module.AssemblyAttributes.Contains(_attribute);
		}

		public void Execute()
		{
			try
			{
				var attribute = CreateAstAttributeInstance();
				if (attribute != null)
				{
					attribute.Initialize(_context);
					attribute.Apply(_targetNode);
				}
			}
			catch (Exception x)
			{
				_context.TraceError(x);
				_context.Errors.Add(CompilerErrorFactory.AttributeApplicationError(x, _attribute, _type));
			}
		}

		public IAstAttribute CreateAstAttributeInstance()
		{
			var attribute = CreateInstance(ConstructorArguments());
			if (attribute == null) return null;
			return _attribute.NamedArguments.Count > 0 ? InitializeProperties(attribute) : attribute;
		}

		private object[] ConstructorArguments()
		{
			return _attribute.Arguments.Count > 0 ? _attribute.Arguments.ToArray() : new object[0];
		}

		private IAstAttribute CreateInstance(object[] arguments)
		{
			try
			{
				var aa = (IAstAttribute)Activator.CreateInstance(_type, arguments);
				aa.Attribute = _attribute;
				return aa;
			}
			catch (MissingMethodException x)
			{
				_context.Errors.Add(CompilerErrorFactory.MissingConstructor(x, _attribute, _type, arguments));
				return null;
			}
		}

		private IAstAttribute InitializeProperties(IAstAttribute aa)
		{
			var initialized = true;
			foreach (var p in _attribute.NamedArguments)
			{
				var success = SetFieldOrProperty(aa, p);
				initialized = initialized && success;
			}
			return initialized ? aa : null;
		}

		bool SetFieldOrProperty(IAstAttribute aa, ExpressionPair p)
		{
			var name = p.First as ReferenceExpression;
			if (name == null)
			{
				_context.Errors.Add(CompilerErrorFactory.NamedParameterMustBeIdentifier(p));
				return false;
			}

			var members = FindMembers(name);
			if (members.Length <= 0)
			{
				_context.Errors.Add(CompilerErrorFactory.NotAPublicFieldOrProperty(name, name.Name, Type()));
				return false;
			}

			if (members.Length > 1)
			{
				_context.Errors.Add(CompilerErrorFactory.AmbiguousReference(name, members));
				return false;
			}

			var member = members[0];
			var property = member as PropertyInfo;
			if (property != null)
			{
				property.SetValue(aa, p.Second, null);
				return true;
			}

			var field = (FieldInfo)member;
			field.SetValue(aa, p.Second);
			return true;
		}

		private IType Type()
		{
			return My<TypeSystemServices>.Instance.Map(_type);
		}

		private MemberInfo[] FindMembers(ReferenceExpression name)
		{
			return _type.FindMembers(MemberTypes.Property | MemberTypes.Field, BindingFlags.Instance | BindingFlags.Public, System.Type.FilterName, name.Name);
		}
	}

	/// <summary>
	/// Step 2. Processes AST attributes.
	/// </summary>
	public class BindAndApplyAttributes : AbstractNamespaceSensitiveTransformerCompilerStep
	{	
		private readonly List<ApplyAttributeTask> _pendingApplications = new List<ApplyAttributeTask>();

		System.Text.StringBuilder _buffer = new System.Text.StringBuilder();
		
		IType _astAttributeInterface;

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
					break;
				++iteration;
			}
		}

		public bool BindAndApply()
		{
			return BindAndApply(CompileUnit);
		}

		public bool BindAndApply(Node node)
		{
			Visit(node);
			return FlushPendingApplications();
		}

		private bool FlushPendingApplications()
		{
			if (_pendingApplications.Count == 0)
				return false;

			foreach (var application in _pendingApplications)
				application.Execute();

			_pendingApplications.Clear();

			return true;
		}

		override public void OnModule(Module module)
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

		override public void OnAttribute(Ast.Attribute attribute)
		{
			if (null != attribute.Entity)
				return;

			var entity = NameResolutionService.ResolveQualifiedName(BuildAttributeName(attribute.Name, true))
				?? NameResolutionService.ResolveQualifiedName(BuildAttributeName(attribute.Name, false))
				?? NameResolutionService.ResolveQualifiedName(attribute.Name);

			if (entity == null)
			{
				var suggestion = NameResolutionService.GetMostSimilarTypeName(BuildAttributeName(attribute.Name, true))
					?? NameResolutionService.GetMostSimilarTypeName(BuildAttributeName(attribute.Name, false));

				Error(attribute, CompilerErrorFactory.UnknownAttribute(attribute, attribute.Name, suggestion));
				return;
			}
						
			if (entity.IsAmbiguous())
			{
				Error(attribute, CompilerErrorFactory.AmbiguousReference(
				                 	attribute,
				                 	attribute.Name,
				                 	((Ambiguous)entity).Entities));
				return;
			}

			if (EntityType.Type != entity.EntityType)
			{
				Error(attribute, CompilerErrorFactory.NameNotType(attribute, attribute.Name, entity, null));
				return;
			}
			
			IType attributeType = ((ITypedEntity)entity).Type;
			if (IsAstAttribute(attributeType))
			{
				ExternalType externalType = attributeType as ExternalType;
				if (null == externalType)
				{
					Error(attribute, CompilerErrorFactory.AstAttributeMustBeExternal(attribute, attributeType));
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
					Error(attribute, CompilerErrorFactory.TypeNotAttribute(attribute, attributeType));
				}
				else
				{
					// remember the attribute's type
					attribute.Name = attributeType.FullName;
					attribute.Entity = entity;
					CheckAttributeParameters(attribute);
				}
			}

		}
		
		private void CheckAttributeParameters(Ast.Attribute node)
		{
			foreach (var e in node.Arguments)
				if (e.NodeType == NodeType.BinaryExpression && ((BinaryExpression) e).Operator == BinaryOperatorType.Assign)
					Error(node, CompilerErrorFactory.ColonInsteadOfEquals(node));
		}
		
		void Error(Ast.Attribute node, CompilerError error)
		{
			node.Entity = TypeSystemServices.ErrorEntity;
			Errors.Add(error);
		}

		void ScheduleAttributeApplication(Ast.Attribute attribute, Type type)
		{
			_pendingApplications.Add(new ApplyAttributeTask(Context, attribute, type));
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
			return TypeCompatibilityRules.IsAssignableFrom(_astAttributeInterface, type);
		}
	}
}
