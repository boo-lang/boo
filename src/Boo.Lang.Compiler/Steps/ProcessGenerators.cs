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
		ForeignReferenceCollector _collector = new ForeignReferenceCollector();
		
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
			
			InternalMethod entity = (InternalMethod)method.Entity;
			if (entity.IsGenerator)
			{
				GeneratorMethodProcessor processor = new GeneratorMethodProcessor(_context, entity);
				processor.Run();
			}
		}
		
		override public void LeaveGeneratorExpression(GeneratorExpression node)
		{
			if (!AstUtil.IsListGenerator(node.ParentNode))
			{
				ProcessGenerator(node);							
			}
		}
		
		void ProcessGenerator(GeneratorExpression node)
		{
			using (_collector)
			{
				_collector.ForeignMethod = _current;
				_collector.Initialize(_context);
				_collector.Visit(node);

				GeneratorExpressionProcessor processor = new GeneratorExpressionProcessor(_context, _collector, node);
				processor.Run();
				ReplaceCurrentNode(processor.CreateEnumerableConstructorInvocation());
			}
		}
	}
	
	class GeneratorMethodProcessor : AbstractGeneratorProcessor
	{
		InternalMethod _method;
		
		BooClassBuilder _enumerable;
		
		Field _state;
		
		public GeneratorMethodProcessor(CompilerContext context,
						InternalMethod method) : base(context)
		{
			_method = method;			
		}
		
		public void Run()
		{
			_enumerable = (BooClassBuilder)_method.Method["GeneratorClassBuilder"];
			CreateEnumerableConstructor();
			CreateEnumerator();						
			_method.Method.Body.Statements.Clear();
			_method.Method.Body.Add(
				new ReturnStatement(
					CreateConstructorInvocation(_enumerable.ClassDefinition)));
			CreateGetEnumerator();					
		}
		
		void CreateGetEnumerator()
		{	
			BooMethodBuilder method = (BooMethodBuilder)_method.Method["GetEnumeratorBuilder"];
			method.Body.Add(
				new ReturnStatement(
					CreateConstructorInvocation(_enumerator.ClassDefinition)));
		}
		
		void CreateEnumerableConstructor()
		{
			CreateConstructor(_enumerable);
		}
		
		void CreateEnumeratorConstructor()
		{
			CreateConstructor(_enumerator);
		}
		
		void CreateEnumerator()
		{
			_enumerator = CodeBuilder.CreateClass("Enumerator");
			_enumerator.AddBaseType(TypeSystemServices.ObjectType);
			_enumerator.AddBaseType(TypeSystemServices.IEnumeratorType);
			
			_current = _enumerator.AddField("___current",
									TypeSystemServices.ObjectType);
									
			_state = _enumerator.AddField("___state",
									TypeSystemServices.IntType);
			
			CreateReset();
			CreateCurrent();
			CreateMoveNext();
			CreateEnumeratorConstructor();
			
			_enumerable.ClassDefinition.Members.Add(_enumerator.ClassDefinition);
		}
		
		void CreateReset()
		{
			BooMethodBuilder method = _enumerator.AddVirtualMethod("Reset", TypeSystemServices.VoidType);
		}
		
		void CreateMoveNext()
		{
			BooMethodBuilder method = _enumerator.AddVirtualMethod("MoveNext", TypeSystemServices.BoolType);
		}
		
		void CreateConstructor(BooClassBuilder builder)
		{	
			BooMethodBuilder constructor = builder.AddConstructor();			
			constructor.Body.Add(CodeBuilder.CreateSuperConstructorInvocation(builder.Entity.BaseType));
		}
	}
	
	class GeneratorExpressionProcessor : AbstractGeneratorProcessor
	{		
		GeneratorExpression _generator;
		
		BooClassBuilder _enumerable;
		
		Field _enumeratorField;
		
		ForeignReferenceCollector _collector;
		
		public GeneratorExpressionProcessor(CompilerContext context,
								ForeignReferenceCollector collector,
								GeneratorExpression node) : base(context)
		{			
			_collector = collector;
			_generator = node;
		}
		
		public void Run()
		{				
			RemoveReferencedDeclarations();			
			CreateAnonymousGeneratorType();
		}
		
		void RemoveReferencedDeclarations()
		{
			Hash referencedEntities = _collector.ReferencedEntities;
			foreach (Declaration d in _generator.Declarations)
			{
				referencedEntities.Remove(d.Entity);
			}
		}
		
		void CreateAnonymousGeneratorType()
		{	
			_enumerator = _collector.CreateSkeletonClass("Enumerator");
			_enumerator.AddBaseType(TypeSystemServices.IEnumeratorType);
			_enumerator.AddBaseType(TypeSystemServices.Map(typeof(ICloneable)));
			
			_enumeratorField = _enumerator.AddField("____enumerator",
									TypeSystemServices.IEnumeratorType);
			_current = _enumerator.AddField("____current",
									TypeSystemServices.ObjectType);
			
			CreateReset();
			CreateCurrent();
			CreateMoveNext();
			CreateClone();
			EnumeratorConstructorMustCallReset();
			
			_collector.AdjustReferences();
			
			_enumerable = (BooClassBuilder)_generator["GeneratorClassBuilder"];
			_collector.DeclareFieldsAndConstructor(_enumerable);
			
			CreateGetEnumerator();
			_enumerable.ClassDefinition.Members.Add(_enumerator.ClassDefinition);
		}
		
		public MethodInvocationExpression CreateEnumerableConstructorInvocation()
		{
			return _collector.CreateConstructorInvocationWithReferencedEntities(
							_enumerable.Entity);						
		}		
		
		void EnumeratorConstructorMustCallReset()
		{
			Constructor constructor = _enumerator.ClassDefinition.GetConstructor(0);
			constructor.Body.Add(CreateMethodInvocation(_enumerator.ClassDefinition, "Reset"));			
		}
		
		MethodInvocationExpression CreateMethodInvocation(ClassDefinition cd, string name)
		{
			IMethod method = (IMethod)((Method)cd.Members[name]).Entity;
			return CodeBuilder.CreateMethodInvocation(
						CodeBuilder.CreateSelfReference(method.DeclaringType),
						method);
		}
		
		void CreateGetEnumerator()
		{	
			BooMethodBuilder method = (BooMethodBuilder)_generator["GetEnumeratorBuilder"];
			
			MethodInvocationExpression mie = CreateConstructorInvocation(_enumerator.ClassDefinition);
			foreach (TypeMember member in _enumerable.ClassDefinition.Members)
			{
				if (NodeType.Field == member.NodeType)
				{
					IField field = (IField)member.Entity;
					mie.Arguments.Add(CodeBuilder.CreateMemberReference(field));
				}
			}
			
			method.Body.Add(new ReturnStatement(mie));
		}		
		
		void CreateClone()
		{			
			BooMethodBuilder method = _enumerator.AddVirtualMethod("Clone", TypeSystemServices.ObjectType);
			method.Body.Add(
				new ReturnStatement(
					CodeBuilder.CreateMethodInvocation(
						CodeBuilder.CreateSelfReference((IType)_enumerator.Entity),
						GetMemberwiseCloneMethod())));
		}
		
		void CreateReset()
		{
			BooMethodBuilder method = _enumerator.AddVirtualMethod("Reset", TypeSystemServices.VoidType);
			method.Body.Add(
				CodeBuilder.CreateAssignment(
					CodeBuilder.CreateReference((InternalField)_enumeratorField.Entity),
					CodeBuilder.CreateMethodInvocation(_generator.Iterator,
						TypeSystemServices.Map(Types.IEnumerable.GetMethod("GetEnumerator")))));
		}
		
		void CreateMoveNext()
		{
			BooMethodBuilder method = _enumerator.AddVirtualMethod("MoveNext", TypeSystemServices.BoolType);
			
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
								CodeBuilder.CreateReference(local),
								current));
			}
			
			if (null != filter)
			{
				outerBlock.Add(filter);
			}
			
			innerBlock.Add(CodeBuilder.CreateAssignment(
								CodeBuilder.CreateReference((InternalField)_current.Entity),
								_generator.Expression));
			innerBlock.Add(new ReturnStatement(new BoolLiteralExpression(true)));
			
			method.Body.Add(stmt);
			method.Body.Add(new ReturnStatement(new BoolLiteralExpression(false)));
		}
	}
	
	class AbstractGeneratorProcessor : AbstractCompilerComponent
	{
		protected BooClassBuilder _enumerator;
		
		protected Field _current;
		
		public AbstractGeneratorProcessor(CompilerContext context)
		{
			Initialize(context);
		}
		
		protected void CreateCurrent()
		{
			Property property = _enumerator.AddReadOnlyProperty("Current", TypeSystemServices.ObjectType);
			property.Getter.Modifiers |= TypeMemberModifiers.Virtual;
			property.Getter.Body.Add(
				new ReturnStatement(
					CodeBuilder.CreateReference((InternalField)_current.Entity)));
		}
		
		protected IMethod GetMemberwiseCloneMethod()
		{
			return TypeSystemServices.Map(
						typeof(object).GetMethod("MemberwiseClone",
							System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance));
		}
		
		protected MethodInvocationExpression CreateConstructorInvocation(ClassDefinition cd)
		{
			IConstructor constructor = ((IType)cd.Entity).GetConstructors()[0];
			return CodeBuilder.CreateConstructorInvocation(constructor);
		}
	}
}
