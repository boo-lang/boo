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
	using System.Collections.Generic;

	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;


	/// <summary>
	/// AST semantic evaluation.
	/// </summary>
	public class NormalizeIterationStatements : AbstractTransformerCompilerStep
	{
		static System.Reflection.MethodInfo RuntimeServices_MoveNext = Types.RuntimeServices.GetMethod("MoveNext");
		static System.Reflection.MethodInfo RuntimeServices_GetEnumerable = Types.RuntimeServices.GetMethod("GetEnumerable");
		static System.Reflection.MethodInfo IEnumerable_GetEnumerator = Types.IEnumerable.GetMethod("GetEnumerator");
		static System.Reflection.MethodInfo IDisposable_Dispose = Types.IDisposable.GetMethod("Dispose");

		Method _current;

		override public void Run()
		{
			Visit(CompileUnit);
			CompileUnit.Accept(new OrBlockNormalizer(Context));
		}

		internal class OrBlockNormalizer : DepthFirstTransformer
		{
			private readonly CompilerContext _context;
			private Method _currentMethod;

			public OrBlockNormalizer(CompilerContext context)
			{
				_context = context;
			}

			public override bool EnterMethod(Method node)
			{
				_currentMethod = node;
				return base.EnterMethod(node);
			}

			public override void OnWhileStatement(WhileStatement node)
			{
				if (node.OrBlock == null) return;

				InternalLocal enteredLoop = CodeBuilder().DeclareTempLocal(_currentMethod, BoolType());

				IfStatement orPart = new IfStatement(
					node.OrBlock.LexicalInfo,
					CodeBuilder().CreateNotExpression(CodeBuilder().CreateReference(enteredLoop)),
					node.OrBlock,
					null);

				node.OrBlock = orPart.ToBlock();
				node.Block.Insert(0,
					CodeBuilder().CreateAssignment(
						CreateReference(enteredLoop),
						CreateTrueLiteral()));

			}

			private BoolLiteralExpression CreateTrueLiteral()
			{
				return CodeBuilder().CreateBoolLiteral(true);
			}

			private ReferenceExpression CreateReference(InternalLocal enteredLoop)
			{
				return CodeBuilder().CreateReference(enteredLoop);
			}

			private IType BoolType()
			{
				return _context.TypeSystemServices.BoolType;
			}

			private BooCodeBuilder CodeBuilder()
			{
				return _context.CodeBuilder;
			}
		}
		
		override public void OnMethod(Method node)
		{
			_current = node;
			Visit(node.Body);
		}
		
		override public void OnConstructor(Constructor node)
		{
			OnMethod(node);
		}

		override public void OnDestructor(Destructor node)
		{
			OnMethod(node);
		}

		override public void OnBlockExpression(BlockExpression node)
		{
			// ignore closure's body since it will be visited
			// through the closure's newly created method
		}
		
		override public void LeaveUnpackStatement(UnpackStatement node)
		{
			Block body = new Block(node.LexicalInfo);
			UnpackExpression(body, node.Expression, node.Declarations);
			ReplaceCurrentNode(body);
		}

		override public void LeaveForStatement(ForStatement node)
		{
			_iteratorNode = node.Iterator;
			CurrentEnumeratorType = GetExpressionType(node.Iterator);

			if (null == CurrentBestEnumeratorType)
				return; //error

			DeclarationCollection declarations = node.Declarations;
			Block body = new Block(node.LexicalInfo);

			InternalLocal iterator = CodeBuilder.DeclareLocal(_current,
				Context.GetUniqueName("iterator"),
				CurrentBestEnumeratorType);

			if (CurrentBestEnumeratorType == CurrentEnumeratorType)
			{
				//$iterator = <node.Iterator>
				body.Add(
					CodeBuilder.CreateAssignment(
						node.LexicalInfo,
						CodeBuilder.CreateReference(iterator),
						node.Iterator));
			}
			else
			{
				//$iterator = <node.Iterator>.GetEnumerator()
				body.Add(
					CodeBuilder.CreateAssignment(
						node.LexicalInfo,
						CodeBuilder.CreateReference(iterator),
						CodeBuilder.CreateMethodInvocation(node.Iterator, CurrentBestGetEnumerator)));
			}

			// while __iterator.MoveNext():
			if (null == CurrentBestMoveNext)
				return; //error
			WhileStatement ws = new WhileStatement(node.LexicalInfo);
			ws.Condition = CodeBuilder.CreateMethodInvocation(
				CodeBuilder.CreateReference(iterator),
				CurrentBestMoveNext);

			if (null == CurrentBestGetCurrent)
				return; //error
			Expression current = CodeBuilder.CreateMethodInvocation(
				CodeBuilder.CreateReference(iterator),
				CurrentBestGetCurrent);

			if (1 == declarations.Count)
			{
				//	item = __iterator.Current
				ws.Block.Add(
					CodeBuilder.CreateAssignment(
						node.LexicalInfo,
						CodeBuilder.CreateReference((InternalLocal)declarations[0].Entity),
						current));
			}
			else
			{
				UnpackExpression(ws.Block,
				                 CodeBuilder.CreateCast(
				                 	CurrentEnumeratorItemType,
				                 	current),
				                 node.Declarations);
			}
			
			ws.Block.Add(node.Block);
			ws.OrBlock = node.OrBlock;
			ws.ThenBlock = node.ThenBlock;
			
			// try:
			//   while...
			// ensure:
			//   d = iterator as IDisposable
			//   d.Dispose() unless d is null
			if (TypeSystemServices.IDisposableType.IsAssignableFrom(CurrentBestEnumeratorType))
			{
				TryStatement tryStatement = new TryStatement();
				tryStatement.ProtectedBlock.Add(ws);
				tryStatement.EnsureBlock = new Block();
			
				CastExpression castExpression = new CastExpression();
				castExpression.Type = CodeBuilder.CreateTypeReference(TypeSystemServices.IDisposableType);
				castExpression.Target = CodeBuilder.CreateReference(iterator);
				castExpression.ExpressionType = TypeSystemServices.IDisposableType;
				tryStatement.EnsureBlock.Add(
					CodeBuilder.CreateMethodInvocation(castExpression, IDisposable_Dispose));

				body.Add(tryStatement);
			}
			else
			{
				body.Add(ws);
			}

			ReplaceCurrentNode(body);
		}
		
		void UnpackExpression(Block block, Expression expression, DeclarationCollection declarations)
		{
			UnpackExpression(CodeBuilder, _current, block, expression, declarations);
		}
		
		public static void UnpackExpression(BooCodeBuilder codeBuilder, Method method, Block block, Expression expression, DeclarationCollection declarations)
		{
			if (expression.ExpressionType.IsArray)
			{
				UnpackArray(codeBuilder, method, block, expression, declarations);
			}
			else
			{
				UnpackEnumerable(codeBuilder, method, block, expression, declarations);
			}
		}
		
		public static void UnpackEnumerable(BooCodeBuilder codeBuilder, Method method, Block block, Expression expression, DeclarationCollection declarations)
		{
			TypeSystemServices tss = codeBuilder.TypeSystemServices;
			
			InternalLocal local = codeBuilder.DeclareTempLocal(method,
			                                                   tss.IEnumeratorType);
			
			IType expressionType = expression.ExpressionType;
			
			if (expressionType.IsSubclassOf(codeBuilder.TypeSystemServices.IEnumeratorType))
			{
				block.Add(
					codeBuilder.CreateAssignment(
						codeBuilder.CreateReference(local),
						expression));
			}
			else
			{
				if (!expressionType.IsSubclassOf(codeBuilder.TypeSystemServices.IEnumerableType))
				{
					expression = codeBuilder.CreateMethodInvocation(
						RuntimeServices_GetEnumerable, expression);
				}
				
				block.Add(
					codeBuilder.CreateAssignment(
						block.LexicalInfo,
						codeBuilder.CreateReference(local),
						codeBuilder.CreateMethodInvocation(
							expression, IEnumerable_GetEnumerator)));
			}
			
			for (int i=0; i<declarations.Count; ++i)
			{
				Declaration declaration = declarations[i];
				
				block.Add(
					codeBuilder.CreateAssignment(
						codeBuilder.CreateReference(declaration.Entity),
						codeBuilder.CreateMethodInvocation(RuntimeServices_MoveNext,
						                                   codeBuilder.CreateReference(local))));
			}
		}
		
		public static void UnpackArray(BooCodeBuilder codeBuilder, Method method, Block block, Expression expression, DeclarationCollection declarations)
		{
			ILocalEntity local = expression.Entity as ILocalEntity;
			if (null == local)
			{
				local = codeBuilder.DeclareTempLocal(method,
				                                     expression.ExpressionType);
				block.Add(
					codeBuilder.CreateAssignment(
						codeBuilder.CreateReference(local),
						expression));
			}
			for (int i=0; i<declarations.Count; ++i)
			{
				Declaration declaration = declarations[i];
				block.Add(
					codeBuilder.CreateAssignment(
						codeBuilder.CreateReference(
							declaration.Entity),
						codeBuilder.CreateSlicing(
							codeBuilder.CreateReference(local),
							i)));
			}
		}


		Node _iteratorNode;
		IType _enumeratorType;
		IType _enumeratorItemType;
		IType _bestEnumeratorType;
		IMethod _bestGetEnumerator;
		IMethod _bestMoveNext;
		IMethod _bestGetCurrent;

		IType CurrentEnumeratorType
		{
			get { return _enumeratorType; }
			set
			{
				if (_enumeratorType != value) //if same type reuse cache
				{
					_enumeratorItemType = null;
					_bestEnumeratorType = null;
					_bestGetEnumerator = null;
					_bestMoveNext = null;
					_bestGetCurrent = null;

					_enumeratorType = value;
				}
			}
		}

		IType CurrentEnumeratorItemType
		{
			get
			{
				if (null == _enumeratorItemType)
					_enumeratorItemType = TypeSystemServices.GetEnumeratorItemType(CurrentEnumeratorType);
				return _enumeratorItemType;
			}
		}

		IType CurrentBestEnumeratorType
		{
			get
			{
				if (null == _bestEnumeratorType)
					_bestEnumeratorType = FindBestEnumeratorType();
				return _bestEnumeratorType;
			}
		}

		IMethod CurrentBestGetEnumerator
		{
			get {
				if (null == _bestGetEnumerator)
					_bestGetEnumerator = FindBestEnumeratorMethod("GetEnumerator");
				return _bestGetEnumerator;
			}
		}

		IMethod CurrentBestMoveNext
		{
			get {
				if (null == _bestMoveNext)
					_bestMoveNext = FindBestEnumeratorMethod("MoveNext");
				return _bestMoveNext;
			}
		}

		IMethod CurrentBestGetCurrent
		{
			get {
				if (null == _bestGetCurrent)
					_bestGetCurrent = FindBestEnumeratorMethod("Current");
				return _bestGetCurrent;
			}
		}

		List<IEntity> _candidates = new List<IEntity>();

		IType FindBestEnumeratorType()
		{
			//type is already an IEnumerator, use it
			if (TypeSystemServices.IEnumeratorType.IsAssignableFrom(CurrentEnumeratorType))
				return CurrentEnumeratorType;

			IType bestEnumeratorType = null;
			_candidates.Clear();

			//resolution order:
			//1) type contains an applicable GetEnumerator() [whether or not type implements IEnumerator (as C# does)]
			CurrentEnumeratorType.Resolve(_candidates, "GetEnumerator", EntityType.Method);
			foreach (IEntity candidate in _candidates)
			{
				IMethod m = (IMethod) candidate;
				if (null != m.GenericInfo || 0 != m.GetParameters().Length || !m.IsPublic)
					continue; //only check public non-generic GetEnumerator with no argument

				if (!TypeSystemServices.IEnumeratorGenericType.IsAssignableFrom(m.ReturnType)
				    && !TypeSystemServices.IEnumeratorType.IsAssignableFrom(m.ReturnType))
					continue; //GetEnumerator does not return an IEnumerator or IEnumerator[of T]

				bestEnumeratorType = m.ReturnType;
				_bestGetEnumerator = m;
				break;
			}

			//2) type explicitly implements IEnumerable[of T]
			if (null == bestEnumeratorType)
			{
				if (TypeSystemServices.IEnumerableGenericType.IsAssignableFrom(CurrentEnumeratorType))
				{
					bestEnumeratorType = TypeSystemServices.IEnumeratorGenericType;
					_bestGetEnumerator = TypeSystemServices.Map(Types.IEnumerableGeneric.GetMethod("GetEnumerator"));
				}
			}

			//3) type explicitly implements IEnumerable
			if (null == bestEnumeratorType)
			{
				if (TypeSystemServices.IEnumerableType.IsAssignableFrom(CurrentEnumeratorType))
				{
					bestEnumeratorType = TypeSystemServices.IEnumeratorType;
					_bestGetEnumerator = TypeSystemServices.Map(Types.IEnumerable.GetMethod("GetEnumerator"));
				}
			}

			//4) error
			if (null == bestEnumeratorType)
				Errors.Add(CompilerErrorFactory.InvalidIteratorType(_iteratorNode, CurrentEnumeratorType));

			return bestEnumeratorType;
		}

		IMethod FindBestEnumeratorMethod(string name)
		{
			_candidates.Clear();
			CurrentBestEnumeratorType.Resolve(_candidates, name, EntityType.Method | EntityType.Property);

			foreach (IEntity candidate in _candidates)
			{
				if (candidate is IMethod)
				{
					IMethod m = (IMethod) candidate;
					if (null != m.GenericInfo || 0 != m.GetParameters().Length)
						continue; //only check non-generic void methods with no argument

					return m;
				}
				else
				{
					IProperty p = (IProperty) candidate;
					return p.GetGetMethod();
				}
			}

			Errors.Add(CompilerErrorFactory.InvalidIteratorType(_iteratorNode, CurrentEnumeratorType));
			return null;
		}
	}
}

