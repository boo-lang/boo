#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
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
				ReplaceCurrentNode(processor.CreateConstructorInvocation());			
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
		
		ClassDefinition _generatorType;
		
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
			AdjustReferences();
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
			TypeDefinition parent = _method.DeclaringType;
			
			_generatorType = new ClassDefinition(_generator.LexicalInfo);			
			_generatorType.Entity = new InternalType(TypeSystemServices, _generatorType);
			_generatorType.BaseTypes.Add(CreateTypeReference(TypeSystemServices.ObjectType));
			_generatorType.BaseTypes.Add(CreateTypeReference(TypeSystemServices.IEnumerableType));
			_generatorType.BaseTypes.Add(CreateTypeReference(TypeSystemServices.IEnumeratorType));
			_generatorType.Name = string.Format("__generator{0}__", parent.Members.Count);
			_generatorType.Modifiers = TypeMemberModifiers.Private|TypeMemberModifiers.Final;
			
			DeclareFields();
			
			_generatorType.Members.Add(CreateGetEnumerator());
			_generatorType.Members.Add(CreateReset());
			_generatorType.Members.Add(CreateCurrent());
			_generatorType.Members.Add(CreateMoveNext());
			_generatorType.Members.Add(CreateConstructor());
			
			parent.Members.Add(_generatorType);
		}
		
		void DeclareFields()
		{
			_generatorType.Members.Add(_enumeratorField = CreateField("__enumerator__", TypeSystemServices.IEnumeratorType));
			_generatorType.Members.Add(_currentField = CreateField("__current__", TypeSystemServices.ObjectType));
			foreach (ITypedEntity entity in Builtins.array(_uniqueReferences.Keys))
			{
				Field field = CreateField("__" + entity.Name, entity.Type);
				_generatorType.Members.Add(field);
				_uniqueReferences[entity] = field.Entity;
			}
		}
		
		Field CreateField(string name, IType type)
		{
			Field field = new Field();
			field.Name = name;
			field.Type = CreateTypeReference(type);
			field.Entity = new InternalField(TypeSystemServices, field);
			return field;
		}
		
		public MethodInvocationExpression CreateConstructorInvocation()
		{
			IConstructor constructor = ((IType)_generatorType.Entity).GetConstructors()[0];

			MethodInvocationExpression mie = TypeSystemServices.CreateConstructorInvocation(constructor);
			foreach (ITypedEntity entity in _uniqueReferences.Keys)
			{
				mie.Arguments.Add(CreateReference(entity));
			}
			return mie;			
		}
		
		Expression CreateEntityReference(IEntity entity)
		{
			switch (entity.EntityType)
			{
				case EntityType.Field:
				{
					return CreateMemberReference((InternalField)entity);
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
		
		Constructor CreateConstructor()
		{
			Constructor constructor = new Constructor();
			constructor.Modifiers = TypeMemberModifiers.Public;
			constructor.Entity = new InternalConstructor(TypeSystemServices, constructor);
			constructor.Body.Add(
				TypeSystemServices.CreateSuperConstructorInvocation(TypeSystemServices.ObjectType));
				
			int paramIndex=0;
			foreach (ITypedEntity entity in _uniqueReferences.Keys)
			{
				ParameterDeclaration parameter = TypeSystemServices.CreateParameterDeclaration(++paramIndex,
										entity.Name,
										entity.Type);
				constructor.Parameters.Add(parameter);
										
				InternalField field = (InternalField)_uniqueReferences[entity];
				constructor.Body.Add(
					CreateAssignment(CreateFieldReference(field),
									CreateReference((InternalParameter)parameter.Entity)));
			}
			constructor.Body.Add(CreateMethodInvocation("Reset"));
			return constructor;
		}
		
		MethodInvocationExpression CreateMethodInvocation(string name)
		{
			IMethod method = (IMethod)((Method)_generatorType.Members[name]).Entity;
			MethodInvocationExpression mie = new MethodInvocationExpression();
			mie.Target = CreateMemberReference(method);
			mie.ExpressionType = method.ReturnType;
			return mie;
		}
		
		Method CreateGetEnumerator()
		{	
			IType type = (IType)_generatorType.Entity;
			
			SelfLiteralExpression self = new SelfLiteralExpression();
			self.ExpressionType = type;
			
			Method method = CreateMethod("GetEnumerator", TypeSystemServices.IEnumeratorType);
			method.Body.Add(new ReturnStatement(self));
			
			return method;
		}
		
		Method CreateReset()
		{
			Method method = CreateMethod("Reset", TypeSystemServices.VoidType);
			method.Body.Add(
				CreateAssignment(
					CreateFieldReference((InternalField)_enumeratorField.Entity),
					CreateMethodInvocation(_generator.Iterator,
						TypeSystemServices.Map(Types.IEnumerable.GetMethod("GetEnumerator")))));
			return method;
		}
		
		Property CreateCurrent()
		{
			Property property = new Property("Current");
			property.Modifiers = TypeMemberModifiers.Public;
			property.Type = CreateTypeReference(TypeSystemServices.ObjectType);
			property.Entity = new InternalProperty(TypeSystemServices, property);
			property.Getter = CreateMethod("get_Current", TypeSystemServices.ObjectType);
			property.Getter.Body.Add(
				new ReturnStatement(CreateFieldReference((InternalField)_currentField.Entity)));
			return property;
		}
		
		Expression CreateAssignment(Expression lhs, Expression rhs)
		{
			BinaryExpression expression = new BinaryExpression(BinaryOperatorType.Assign,
												lhs,
												rhs);
												
			//expression.ExpressionType
			return expression;
		}
		
		Expression CreateFieldReference(InternalField field)
		{		
			return CreateMemberReference(field);
		}
		
		Expression CreateMemberReference(ITypedEntity entity)
		{
			MemberReferenceExpression reference = new MemberReferenceExpression();
			reference.Target = new SelfLiteralExpression();
			reference.Name = entity.Name;
			
			reference.Target.ExpressionType = (IType)_generatorType.Entity;
			reference.Entity = entity;
			reference.ExpressionType = entity.Type;
			return reference;
		}
		
		Method CreateMoveNext()
		{
			Method method = CreateMethod("MoveNext", TypeSystemServices.BoolType);
			
			Expression moveNext = CreateMethodInvocation(
												CreateFieldReference((InternalField)_enumeratorField.Entity),
												TypeSystemServices.Map(Types.IEnumerator.GetMethod("MoveNext")));
												
			Expression current = CreateMethodInvocation(
									CreateFieldReference((InternalField)_enumeratorField.Entity),
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
				
				outerBlock.Add(CreateAssignment(
								CreateReference(local),
								current));
			}
			
			if (null != filter)
			{
				outerBlock.Add(filter);
			}
			
			innerBlock.Add(CreateAssignment(
								CreateFieldReference((InternalField)_currentField.Entity),
								_generator.Expression));
			innerBlock.Add(new ReturnStatement(new BoolLiteralExpression(true)));
			
			method.Body.Add(stmt);
			method.Body.Add(new ReturnStatement(new BoolLiteralExpression(false)));
			
			return method;
		}
		
		Method CreateMethod(string name, IType returnType)
		{
			return TypeSystemServices.CreateVirtualMethod(name, returnType);
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
