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
	using Boo.Lang;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class ProcessGenerators : AbstractTransformerCompilerStep
	{
		Method _current;
		
		override public void Run()
		{
			Visit(CompileUnit.Modules);
		}
		
		override public void OnInterfaceDefinition(InterfaceDefinition node)
		{
			// ignore
		}
		
		override public void OnEnumDefinition(EnumDefinition node)
		{
			// ignore
		}
		
		override public void OnField(Field node)
		{
			// ignore
		}
		
		override public void OnMethod(Method method)
		{
			_current = method;
			Visit(method.Body);
		}
		
		override public void LeaveGeneratorExpression(GeneratorExpression node)
		{
			if (!AstUtil.IsListGenerator(node.ParentNode))
			{
				GeneratorProcessor processor = new GeneratorProcessor(_context, _current, node);
				processor.Run();
				ReplaceCurrentNode(processor.CreateEnumerableConstructorInvocation());			
			}
		}
	}
	
	class SelfEntity : ITypedEntity
	{
		string _name;
		IType _type;
		
		public SelfEntity(string name, IType type)
		{
			_name = name;
			_type = type;
		}
		
		public string Name
		{
			get
			{
				return _name;
			}
		}
		
		public string FullName
		{
			get
			{
				return _name;
			}
		}
		
		public EntityType EntityType
		{
			get
			{
				return EntityType.Unknown;
			}
		}
		
		public IType Type
		{
			get
			{
				return _type;
			}
		}
	}
	
	class GeneratorProcessor : AbstractVisitorCompilerStep
	{		
		Hash _uniqueReferences = new Hash();
		
		List _references = new List();
		
		GeneratorExpression _generator;
		
		ClassDefinition _enumerableType;
		
		ClassDefinition _enumeratorType;
		
		Field _currentField;
		
		Field _enumeratorField;
		
		Method _method;
		
		SelfEntity _selfEntity;
		
		public GeneratorProcessor(CompilerContext context, Method method, GeneratorExpression node)
		{
			_method = method;
			_generator = node;
			
			if (!_method.IsStatic)
			{
				_selfEntity = new SelfEntity("this", (IType)_method.DeclaringType.Entity);
			}
			Initialize(context);
		}
		
		override public void Run()
		{			
			Visit(_generator);			
			CreateAnonymousGeneratorType();
		}
		
		override public void OnReferenceExpression(ReferenceExpression node)
		{
			if (IsLocalReference(node))
			{
				_references.Add(node);
				if (IsNotDeclarationReference(node.Entity))
				{
					_uniqueReferences.Add(node.Entity, null);
				}
			}
		}
		
		override public void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			_references.Add(node);
			node.Entity = _selfEntity;
			_uniqueReferences.Add(_selfEntity, null);
		}
		
		bool IsLocalReference(ReferenceExpression node)
		{
			IEntity entity = node.Entity;
			if (null != entity)
			{
				EntityType type = entity.EntityType;
				return type == EntityType.Local || type == EntityType.Parameter;
			}
			return false;
		}
		
		bool IsNotDeclarationReference(IEntity entity)
		{
			foreach (Declaration d in _generator.Declarations)
			{
				if (entity == d.Entity)
				{
					return false;
				}
			}
			return true;
		}
		
		void CreateAnonymousGeneratorType()
		{	
			_enumeratorType = CreateClassDefinition("Enumerator");
			_enumeratorType.BaseTypes.Add(CodeBuilder.CreateTypeReference(TypeSystemServices.IEnumeratorType));
			_enumeratorType.BaseTypes.Add(CodeBuilder.CreateTypeReference(TypeSystemServices.Map(typeof(ICloneable))));			
			
			DeclareEnumeratorFields();
			
			_enumeratorType.Members.Add(CreateReset());
			_enumeratorType.Members.Add(CreateCurrent());
			_enumeratorType.Members.Add(CreateMoveNext());
			_enumeratorType.Members.Add(CreateClone());
			_enumeratorType.Members.Add(CreateEnumeratorConstructor());
			
			AdjustReferences();
			
			TypeDefinition parent = _method.DeclaringType;
			string name = string.Format("__generator{0}__", parent.Members.Count);
			
			_enumerableType = CreateClassDefinition(name);
			_enumerableType.BaseTypes.Add(CodeBuilder.CreateTypeReference(TypeSystemServices.IEnumerableType));
			_enumerableType.Modifiers = TypeMemberModifiers.Private|TypeMemberModifiers.Final;
			_enumerableType.LexicalInfo = _generator.LexicalInfo;
			
			DeclareEnumerableFields();
			
			_enumerableType.Members.Add(CreateGetEnumerator());
			_enumerableType.Members.Add(CreateRegularConstructor(_enumerableType));
			_enumerableType.Members.Add(_enumeratorType);
			
			parent.Members.Add(_enumerableType);
		}
		
		ClassDefinition CreateClassDefinition(string name)
		{
			ClassDefinition cd = new ClassDefinition();
			cd.Name = name;
			cd.Entity = new InternalType(TypeSystemServices, cd);
			cd.BaseTypes.Add(CodeBuilder.CreateTypeReference(TypeSystemServices.ObjectType));
			return cd;
		}
		
		void DeclareEnumeratorFields()
		{
			_enumeratorType.Members.Add(
				_enumeratorField = CodeBuilder.CreateField("__enumerator__", TypeSystemServices.IEnumeratorType));
			_enumeratorType.Members.Add(
				_currentField = CodeBuilder.CreateField("__current__", TypeSystemServices.ObjectType));
			DeclareRegularFields(_enumeratorType);
		}
		
		void DeclareEnumerableFields()
		{
			DeclareRegularFields(_enumerableType);
		}
		
		void DeclareRegularFields(ClassDefinition cd)
		{
			foreach (ITypedEntity entity in Builtins.array(_uniqueReferences.Keys))
			{
				Field field = CodeBuilder.CreateField("__" + entity.Name, entity.Type);
				cd.Members.Add(field);
				_uniqueReferences[entity] = field.Entity;
			}
		}
		
		public MethodInvocationExpression CreateEnumerableConstructorInvocation()
		{
			MethodInvocationExpression mie = CreateConstructorInvocation(_enumerableType);
			foreach (ITypedEntity entity in _uniqueReferences.Keys)
			{
				mie.Arguments.Add(CreateReference(entity));
			}
			return mie;			
		}
		
		MethodInvocationExpression CreateConstructorInvocation(ClassDefinition cd)
		{
			IConstructor constructor = ((IType)cd.Entity).GetConstructors()[0];
			return CodeBuilder.CreateConstructorInvocation(constructor);
		}
		
		Expression CreateEntityReference(IEntity entity)
		{
			switch (entity.EntityType)
			{
				case EntityType.Field:
				{
					return CodeBuilder.CreateReference((InternalField)entity);
				}
				
				default:
				{
					throw new ArgumentException(entity.ToString());
				}
			}
		}
		
		Expression CreateReference(ITypedEntity entity)
		{
			if (_selfEntity == entity)
			{
				SelfLiteralExpression self = new SelfLiteralExpression();
				self.ExpressionType = _selfEntity.Type;
				return self;
			}
			ReferenceExpression expression = new ReferenceExpression(entity.Name);
			expression.Entity = entity;
			expression.ExpressionType = entity.Type;
			return expression;
		}
		
		Constructor CreateEnumeratorConstructor()
		{
			Constructor constructor = CreateRegularConstructor(_enumeratorType);
			constructor.Body.Add(CreateMethodInvocation(_enumeratorType, "Reset"));
			return constructor;
		}
		
		Constructor CreateRegularConstructor(ClassDefinition cd)
		{			
			IType type = (IType)cd.Entity;
			
			Constructor constructor = new Constructor();
			constructor.Modifiers = TypeMemberModifiers.Public;
			constructor.Entity = new InternalConstructor(TypeSystemServices, constructor);
			constructor.Body.Add(
				CodeBuilder.CreateSuperConstructorInvocation(TypeSystemServices.ObjectType));
				
			int paramIndex=0;
			foreach (ITypedEntity entity in _uniqueReferences.Keys)
			{
				ParameterDeclaration parameter = CodeBuilder.CreateParameterDeclaration(++paramIndex,
										entity.Name,
										entity.Type);
				constructor.Parameters.Add(parameter);
										
				InternalField field = (InternalField)_uniqueReferences[entity];
				constructor.Body.Add(
					CodeBuilder.CreateAssignment(CodeBuilder.CreateReference(field),
									CodeBuilder.CreateReference(parameter)));
			}			
			return constructor;
		}
		
		MethodInvocationExpression CreateMethodInvocation(ClassDefinition cd, string name)
		{
			IMethod method = (IMethod)((Method)cd.Members[name]).Entity;
			MethodInvocationExpression mie = new MethodInvocationExpression();
			mie.Target = CodeBuilder.CreateMemberReference(method);
			mie.ExpressionType = method.ReturnType;
			return mie;
		}
		
		Method CreateGetEnumerator()
		{	
			Method method = CodeBuilder.CreateVirtualMethod("GetEnumerator", TypeSystemServices.IEnumeratorType);
			
			MethodInvocationExpression mie = CreateConstructorInvocation(_enumeratorType);
			foreach (TypeMember member in _enumerableType.Members)
			{
				if (NodeType.Field == member.NodeType)
				{
					IField field = (IField)member.Entity;
					mie.Arguments.Add(CodeBuilder.CreateMemberReference(field));
				}
			}			
			
			method.Body.Add(new ReturnStatement(mie));			
			return method;
		}
		
		IMethod GetMemberwiseCloneMethod()
		{
			return TypeSystemServices.Map(
						typeof(object).GetMethod("MemberwiseClone",
							System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance));
		}
		
		Method CreateClone()
		{			
			Method method = CodeBuilder.CreateVirtualMethod("Clone", TypeSystemServices.ObjectType);
			method.Body.Add(
				new ReturnStatement(
					CodeBuilder.CreateMethodInvocation(
						CodeBuilder.CreateSelfReference((IType)_enumeratorType.Entity),
						GetMemberwiseCloneMethod())));					
									
			return method;
		}
		
		Method CreateReset()
		{
			Method method = CodeBuilder.CreateVirtualMethod("Reset", TypeSystemServices.VoidType);
			method.Body.Add(
				CodeBuilder.CreateAssignment(
					CodeBuilder.CreateReference((InternalField)_enumeratorField.Entity),
					CodeBuilder.CreateMethodInvocation(_generator.Iterator,
						TypeSystemServices.Map(Types.IEnumerable.GetMethod("GetEnumerator")))));
			return method;
		}
		
		Property CreateCurrent()
		{
			Property property = new Property("Current");
			property.Modifiers = TypeMemberModifiers.Public;
			property.Type = CodeBuilder.CreateTypeReference(TypeSystemServices.ObjectType);
			property.Entity = new InternalProperty(TypeSystemServices, property);
			property.Getter = CodeBuilder.CreateVirtualMethod("get_Current", TypeSystemServices.ObjectType);
			property.Getter.Body.Add(
				new ReturnStatement(
					CodeBuilder.CreateReference((InternalField)_currentField.Entity)));
			return property;
		}
		
		Method CreateMoveNext()
		{
			Method method = CodeBuilder.CreateVirtualMethod("MoveNext", TypeSystemServices.BoolType);
			
			Expression moveNext = CodeBuilder.CreateMethodInvocation(
												CodeBuilder.CreateReference((InternalField)_enumeratorField.Entity),
												TypeSystemServices.Map(Types.IEnumerator.GetMethod("MoveNext")));
												
			Expression current = CodeBuilder.CreateMethodInvocation(
									CodeBuilder.CreateReference((InternalField)_enumeratorField.Entity),
									TypeSystemServices.Map(Types.IEnumerator.GetProperty("Current").GetGetMethod()));
			
			Statement filter = null;
			Statement stmt = null;
			Block outerBlock = null;
			Block innerBlock = null;
			
			if (null == _generator.Filter)
			{								
				IfStatement istmt = new IfStatement(moveNext, new Block(), null);
				outerBlock = innerBlock = istmt.TrueBlock;
				
				stmt = istmt;
			}
			else
			{				 
				WhileStatement wstmt = new WhileStatement(moveNext);
				outerBlock = wstmt.Block;
				
				if (StatementModifierType.If == _generator.Filter.Type)
				{
					IfStatement ifstmt = new IfStatement(_generator.Filter.Condition, new Block(), null);
					innerBlock = ifstmt.TrueBlock;
					filter = ifstmt;
				}
				else
				{
					UnlessStatement ustmt = new UnlessStatement(_generator.Filter.Condition);
					innerBlock = ustmt.Block;					
					filter = ustmt;
				}
				
				stmt = wstmt;
			}
												
			DeclarationCollection declarations = _generator.Declarations;
			if (declarations.Count > 1)
			{
				UnpackStatement unpack = new UnpackStatement();
				
				foreach (Declaration declaration in declarations)
				{
					LocalVariable local = (LocalVariable)declaration.Entity;
					method.Locals.Add(local.Local);
					unpack.Declarations.Add(declaration);
				}
				
				unpack.Expression = current;
				outerBlock.Add(unpack);
			}
			else
			{
				LocalVariable local = (LocalVariable)declarations[0].Entity;
				method.Locals.Add(local.Local);
				
				outerBlock.Add(CodeBuilder.CreateAssignment(
								CreateReference(local),
								current));
			}
			
			if (null != filter)
			{
				outerBlock.Add(filter);
			}
			
			innerBlock.Add(CodeBuilder.CreateAssignment(
								CodeBuilder.CreateReference((InternalField)_currentField.Entity),
								_generator.Expression));
			innerBlock.Add(new ReturnStatement(new BoolLiteralExpression(true)));
			
			method.Body.Add(stmt);
			method.Body.Add(new ReturnStatement(new BoolLiteralExpression(false)));
			
			return method;
		}
		
		void AdjustReferences()
		{
			foreach (Expression reference in _references)
			{
				IEntity entity = (IEntity)_uniqueReferences[reference.Entity];
				if (null != entity)
				{
					reference.ParentNode.Replace(reference,
												CreateEntityReference(entity));
				}
			}
		}
	}
}
