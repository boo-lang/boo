#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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

//Authored by Cameron Kenneth Knight: http://jira.codehaus.org/browse/BOO-137

using Boo.Lang.Compiler.TypeSystem.Core;

namespace Boo.Lang.Compiler.Steps
{
	using System;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;

	/// <summary>
	/// AST semantic evaluation.
	/// </summary>
	public class OptimizeIterationStatements : AbstractTransformerCompilerStep
	{
		static readonly System.Reflection.MethodInfo Array_get_Length = Types.Array.GetProperty("Length").GetGetMethod();
		static readonly System.Reflection.MethodInfo System_Math_Ceiling = typeof(System.Math).GetMethod("Ceiling", new System.Type[] { typeof(double) });
		static readonly System.Reflection.ConstructorInfo System_ArgumentOutOfRangeException_ctor = typeof(System.ArgumentOutOfRangeException).GetConstructor(new System.Type[] { typeof(string) });
		
		IMethod _range_End;
		IMethod _range_Begin_End;
		IMethod _range_Begin_End_Step;

		Method _currentMethod;

		public OptimizeIterationStatements()
		{
		}
		
		override public void Initialize(CompilerContext context)
		{
			base.Initialize(context);
			
			Type builtins = typeof(Boo.Lang.Builtins);
			_range_End = Map(builtins.GetMethod("range", new Type[] { Types.Int }));
			_range_Begin_End = Map(builtins.GetMethod("range", new Type[] { Types.Int, Types.Int }));
			_range_Begin_End_Step = Map(builtins.GetMethod("range", new Type[] { Types.Int, Types.Int, Types.Int }));
		}
		
		IMethod Map(System.Reflection.MethodInfo method)
		{
			return TypeSystemServices.Map(method);
		}
		
		override public void Run()
		{
			Visit(CompileUnit);
		}
		
		override public void OnMethod(Method node)
		{
			_currentMethod = node;
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
		
		override public void LeaveForStatement(ForStatement node)
		{
			//do not optimize local-reusing loops (BOO-1111)
			//TODO: optimize anyway (modify end value to match generator model vs optimized model)
			if (node.Declarations.Count == 1
				&& null != AstUtil.GetLocalByName(_currentMethod, node.Declarations[0].Name))
				return;

			CheckForItemInRangeLoop(node);
			CheckForItemInArrayLoop(node);
		}
		
		bool IsRangeInvocation(MethodInvocationExpression mi)
		{
			IEntity entity = mi.Target.Entity;
			return entity == _range_End
				|| entity == _range_Begin_End
				|| entity == _range_Begin_End_Step;
		}

		/// <summary>
		/// Optimize the <c>for item in range()</c> construct
		/// </summary>
		/// <param name="node">the for statement to check</param>
		private void CheckForItemInRangeLoop(ForStatement node)
		{
			MethodInvocationExpression mi = node.Iterator as MethodInvocationExpression;
			if (null == mi) return;
			if (!IsRangeInvocation(mi)) return;
			
			DeclarationCollection declarations = node.Declarations;
			if (declarations.Count != 1) return;			
						
			ExpressionCollection args = mi.Arguments;
			Block body = new Block(node.LexicalInfo);

			Expression min;
			Expression max;
			Expression step;
			IntegerLiteralExpression mini;
			IntegerLiteralExpression maxi;
			IntegerLiteralExpression stepi;

			if (args.Count == 1)
			{
				mini = CodeBuilder.CreateIntegerLiteral(0);
				min = mini;
				max = args[0];
				maxi = max as IntegerLiteralExpression;
				stepi = CodeBuilder.CreateIntegerLiteral(1);
				step = stepi;
			}
			else if (args.Count == 2)
			{
				min = args[0];
				mini = min as IntegerLiteralExpression;
				max = args[1];
				maxi = max as IntegerLiteralExpression;
				stepi = CodeBuilder.CreateIntegerLiteral(1);
				step = stepi;
			}
			else
			{
				min = args[0];
				mini = min as IntegerLiteralExpression;
				max = args[1];
				maxi = max as IntegerLiteralExpression;
				step = args[2];
				stepi = step as IntegerLiteralExpression;
			}

			InternalLocal numVar = CodeBuilder.DeclareTempLocal(
										_currentMethod,
										TypeSystemServices.IntType);
			Expression numRef = CodeBuilder.CreateReference(numVar);

			// __num = <min>
			body.Add(
				CodeBuilder.CreateAssignment(
					numRef,
					min));
			
			Expression endRef;

			if (null != maxi)
			{
				endRef = max;
			}
			else
			{
				InternalLocal endVar = CodeBuilder.DeclareTempLocal(
							_currentMethod,
							TypeSystemServices.IntType);
				endRef = CodeBuilder.CreateReference(endVar);

				// __end = <end>
				body.Add(
					CodeBuilder.CreateAssignment(
						endRef,
						max));
			}

			if (args.Count == 1)
			{
				if (null != maxi)
				{
					if (maxi.Value < 0)
					{
						// raise ArgumentOutOfRangeException("max") (if <max> < 0)
						Statement statement = CodeBuilder.RaiseException(
							body.LexicalInfo,
							TypeSystemServices.Map(System_ArgumentOutOfRangeException_ctor),
							CodeBuilder.CreateStringLiteral("max"));
						body.Add(statement);
					}
				}
				else
				{
					IfStatement ifStatement = new IfStatement(body.LexicalInfo);
					ifStatement.TrueBlock = new Block();
					
					// raise ArgumentOutOfRangeException("max") if __end < 0
					Statement statement = CodeBuilder.RaiseException(
							body.LexicalInfo,
							TypeSystemServices.Map(System_ArgumentOutOfRangeException_ctor),
							CodeBuilder.CreateStringLiteral("max"));

					ifStatement.Condition = CodeBuilder.CreateBoundBinaryExpression(
							TypeSystemServices.BoolType,
							BinaryOperatorType.LessThan,
							endRef,
							CodeBuilder.CreateIntegerLiteral(0));

					ifStatement.TrueBlock.Add(statement);

					body.Add(ifStatement);
				}
			}

			Expression stepRef;

			switch (args.Count)
			{
				case 1:
					stepRef = CodeBuilder.CreateIntegerLiteral(1);
					break;
				case 2:
					if (null != mini && null != maxi)
					{
						if (maxi.Value < mini.Value)
							// __step = -1
							stepRef = CodeBuilder.CreateIntegerLiteral(-1);
						else
							// __step = 1
							stepRef = CodeBuilder.CreateIntegerLiteral(1);
					}
					else
					{
						InternalLocal stepVar = CodeBuilder.DeclareTempLocal(
								_currentMethod,
								TypeSystemServices.IntType);
						stepRef = CodeBuilder.CreateReference(stepVar);

						// __step = 1
						body.Add(
							CodeBuilder.CreateAssignment(
								stepRef,
								CodeBuilder.CreateIntegerLiteral(1)));

						// __step = -1 if __end < __num
						IfStatement ifStatement = new IfStatement(node.LexicalInfo);

						ifStatement.Condition = CodeBuilder.CreateBoundBinaryExpression(
							TypeSystemServices.BoolType,
							BinaryOperatorType.LessThan,
							endRef,
							numRef);

						ifStatement.TrueBlock = new Block();

						ifStatement.TrueBlock.Add(
							CodeBuilder.CreateAssignment(
								stepRef,
								CodeBuilder.CreateIntegerLiteral(-1)));

						body.Add(ifStatement);
					}
					break;
				default:
					if (null != stepi)
					{
						stepRef = step;
					}
					else
					{
						InternalLocal stepVar = CodeBuilder.DeclareTempLocal(
									_currentMethod,
									TypeSystemServices.IntType);
						stepRef = CodeBuilder.CreateReference(stepVar);

						// __step = <step>
						body.Add(
							CodeBuilder.CreateAssignment(
								stepRef,
								step));
					}
					break;
			}


			if (args.Count == 3)
			{
				Expression condition = null;
				bool run = false;

				if (null != stepi)
				{
					if (stepi.Value < 0)
					{
						if (null != maxi && null != mini)
						{
							run = maxi.Value > mini.Value;
						}
						else
						{
							condition = CodeBuilder.CreateBoundBinaryExpression(
								TypeSystemServices.BoolType,
								BinaryOperatorType.GreaterThan,
								endRef,
								numRef);
						}
					}
					else
					{
						if (null != maxi && null != mini)
						{
							run = maxi.Value < mini.Value;
						}
						else
						{
							condition = CodeBuilder.CreateBoundBinaryExpression(
								TypeSystemServices.BoolType,
								BinaryOperatorType.LessThan,
								endRef,
								numRef);
						}
					}
				}
				else
				{
					if (null != maxi && null != mini)
					{
						if (maxi.Value < mini.Value)
						{
							condition = CodeBuilder.CreateBoundBinaryExpression(
								TypeSystemServices.BoolType,
								BinaryOperatorType.GreaterThan,
								stepRef,
								CodeBuilder.CreateIntegerLiteral(0));
						}
						else
						{
							condition = CodeBuilder.CreateBoundBinaryExpression(
								TypeSystemServices.BoolType,
								BinaryOperatorType.LessThan,
								stepRef,
								CodeBuilder.CreateIntegerLiteral(0));
						}
					}
					else
					{
						condition = CodeBuilder.CreateBoundBinaryExpression(
							TypeSystemServices.BoolType,
							BinaryOperatorType.Or,
							CodeBuilder.CreateBoundBinaryExpression(
								TypeSystemServices.BoolType,
								BinaryOperatorType.And,
								CodeBuilder.CreateBoundBinaryExpression(
									TypeSystemServices.BoolType,
									BinaryOperatorType.LessThan,
									stepRef,
									CodeBuilder.CreateIntegerLiteral(0)),
								CodeBuilder.CreateBoundBinaryExpression(
									TypeSystemServices.BoolType,
									BinaryOperatorType.GreaterThan,
									endRef,
									numRef)),
							CodeBuilder.CreateBoundBinaryExpression(
								TypeSystemServices.BoolType,
								BinaryOperatorType.And,
								CodeBuilder.CreateBoundBinaryExpression(
									TypeSystemServices.BoolType,
									BinaryOperatorType.GreaterThan,
									stepRef,
									CodeBuilder.CreateIntegerLiteral(0)),
								CodeBuilder.CreateBoundBinaryExpression(
									TypeSystemServices.BoolType,
									BinaryOperatorType.LessThan,
									endRef,
									numRef)));
					}
				}

				// raise ArgumentOutOfRangeException("step") if (__step < 0 and __end > __begin) or (__step > 0 and __end < __begin)
				Statement statement = CodeBuilder.RaiseException(
							body.LexicalInfo,
							TypeSystemServices.Map(System_ArgumentOutOfRangeException_ctor),
							CodeBuilder.CreateStringLiteral("step"));

				if (condition != null)
				{
					IfStatement ifStatement = new IfStatement(body.LexicalInfo);
					ifStatement.TrueBlock = new Block();

					ifStatement.Condition = condition;

					ifStatement.TrueBlock.Add(statement);

					body.Add(ifStatement);
				}
				else if (run)
				{
					body.Add(statement);
				}

				// __end = __num + __step * cast(int, Math.Ceiling((__end - __num)/cast(double, __step)))
				if (null != stepi && null != maxi && null != mini)
				{
					int stepVal = (int) stepi.Value;
					int maxVal = (int) maxi.Value;
					int minVal = (int) mini.Value;
					endRef = CodeBuilder.CreateIntegerLiteral(
						minVal + stepVal * (int)System.Math.Ceiling((maxVal - minVal) / ((double)stepVal)));
				}
				else
				{
					Expression endBak = endRef;
					if (null != maxi)
					{
						InternalLocal endVar = CodeBuilder.DeclareTempLocal(
									_currentMethod,
									TypeSystemServices.IntType);
						endRef = CodeBuilder.CreateReference(endVar);
					}

					body.Add(
						CodeBuilder.CreateAssignment(
							endRef,
							CodeBuilder.CreateBoundBinaryExpression(
								TypeSystemServices.IntType,
								BinaryOperatorType.Addition,
								numRef,
								CodeBuilder.CreateBoundBinaryExpression(
									TypeSystemServices.IntType,
									BinaryOperatorType.Multiply,
									stepRef,
									CodeBuilder.CreateCast(
										TypeSystemServices.IntType,
										CodeBuilder.CreateMethodInvocation(
											TypeSystemServices.Map(System_Math_Ceiling),
											CodeBuilder.CreateBoundBinaryExpression(
												TypeSystemServices.DoubleType,
												BinaryOperatorType.Division,
												CodeBuilder.CreateBoundBinaryExpression(
													TypeSystemServices.IntType,
													BinaryOperatorType.Subtraction,
													endBak,
													numRef),
												CodeBuilder.CreateCast(
													TypeSystemServices.DoubleType,
													stepRef))))))));
				}
			}

			// while __num != __end:
			WhileStatement ws = new WhileStatement(node.LexicalInfo);

			BinaryOperatorType op = BinaryOperatorType.Inequality;

			if (stepRef.NodeType == NodeType.IntegerLiteralExpression)
			{
				if (((IntegerLiteralExpression)stepRef).Value > 0)
				{
					op = BinaryOperatorType.LessThan;
				}
				else
				{
					op = BinaryOperatorType.GreaterThan;
				}
			}

			ws.Condition = CodeBuilder.CreateBoundBinaryExpression(
			    TypeSystemServices.BoolType,
				op,
				numRef,
				endRef);
			ws.Condition.LexicalInfo = node.LexicalInfo;

			//	item = __num
			ws.Block.Add(
				CodeBuilder.CreateAssignment(
					CodeBuilder.CreateReference((InternalLocal)declarations[0].Entity),
					numRef));

			Block rawBlock = new Block();
			rawBlock["checked"] = false;
			
			//  __num += __step
			rawBlock.Add(
				CodeBuilder.CreateAssignment(
					numRef,
					CodeBuilder.CreateBoundBinaryExpression(
						TypeSystemServices.IntType,
						BinaryOperatorType.Addition,
						numRef,
						stepRef)));

			ws.Block.Add(rawBlock as Statement);

			//	<block>
			ws.Block.Add(node.Block);
			
			ws.OrBlock = node.OrBlock;
			ws.ThenBlock = node.ThenBlock;

			body.Add(ws);

			ReplaceCurrentNode(body);
		}
		
		private sealed class EntityPredicate
		{
			private IEntity _entity;

			public EntityPredicate(IEntity entity)
			{
				_entity = entity;
			}
			
			public bool Matches(Node node)
			{
				return _entity == TypeSystemServices.GetOptionalEntity(node);
			}
		}

		/// <summary>
		/// Optimize the <c>for item in array</c> construct
		/// </summary>
		/// <param name="node">the for statement to check</param>
		private void CheckForItemInArrayLoop(ForStatement node)
		{	
			ArrayType enumeratorType = GetExpressionType(node.Iterator) as ArrayType;
			if (enumeratorType == null || enumeratorType.GetArrayRank() > 1) return;
			IType elementType = enumeratorType.GetElementType();
			if (elementType is InternalCallableType) return;

			Block body = new Block(node.LexicalInfo);

			InternalLocal indexVariable = DeclareTempLocal(TypeSystemServices.IntType);
			Expression indexReference = CodeBuilder.CreateReference(indexVariable);

			// __num = 0
			body.Add(
				CodeBuilder.CreateAssignment(
					indexReference,
					CodeBuilder.CreateIntegerLiteral(0)));
			
			
			InternalLocal arrayVar = DeclareTempLocal(node.Iterator.ExpressionType);
			ReferenceExpression arrayRef = CodeBuilder.CreateReference(arrayVar);

			// __arr = <arr>
			body.Add(
				CodeBuilder.CreateAssignment(
					arrayRef,
					node.Iterator));

			InternalLocal endVar = CodeBuilder.DeclareTempLocal(
							_currentMethod,
							TypeSystemServices.IntType);
			ReferenceExpression endRef = CodeBuilder.CreateReference(endVar);

			// __end = __arr.Length
			body.Add(
				CodeBuilder.CreateAssignment(
					node.Iterator.LexicalInfo,
					endRef,
					CodeBuilder.CreateMethodInvocation(
							arrayRef,
							Array_get_Length)));

			// while __num < __end:
			WhileStatement ws = new WhileStatement(node.LexicalInfo);
			
			ws.Condition = CodeBuilder.CreateBoundBinaryExpression(
					TypeSystemServices.BoolType,
					BinaryOperatorType.LessThan,
					indexReference,
					endRef);

			if (1 == node.Declarations.Count)
			{
				IEntity loopVariable = node.Declarations[0].Entity;
				node.Block.ReplaceNodes(
					new NodePredicate(new EntityPredicate(loopVariable).Matches),
					CreateRawArraySlicing(arrayRef, indexReference, elementType));
			}
			else
			{
				//  alpha, bravo, charlie = arr[__num]
				UnpackExpression(
					ws.Block,
					CreateRawArraySlicing(arrayRef, indexReference, elementType),
					node.Declarations);
			}

			//	<block>
			ws.Block.Add(node.Block);

			FixContinueStatements(node, ws);

			//  __num += 1
			BinaryExpression assignment = CodeBuilder.CreateAssignment(
				indexReference,
				CodeBuilder.CreateBoundBinaryExpression(
					TypeSystemServices.IntType,
					BinaryOperatorType.Addition,
					indexReference,
					CodeBuilder.CreateIntegerLiteral(1)));
			AstAnnotations.MarkUnchecked(assignment);
			
			ws.Block.Add(assignment);
			ws.OrBlock = node.OrBlock;
			ws.ThenBlock = node.ThenBlock;
			body.Add(ws);
			ReplaceCurrentNode(body);
		}

		private void FixContinueStatements(ForStatement node, WhileStatement ws)
		{
			// :update
			LabelStatement label = CreateUpdateLabel(node);
			GotoOnTopLevelContinue continueFixup = new GotoOnTopLevelContinue(label);
			node.Block.Accept(continueFixup);
			if (continueFixup.UsageCount > 0) ws.Block.Add(label);
		}

		private LabelStatement CreateUpdateLabel(ForStatement node)
		{
			return new LabelStatement(LexicalInfo.Empty, Context.GetUniqueName("label"));
		}

		private static SlicingExpression CreateRawArraySlicing(ReferenceExpression arrayRef, Expression numRef, IType elementType)
		{
			SlicingExpression expression = new SlicingExpression(arrayRef.CloneNode(), numRef.CloneNode());
			expression.ExpressionType = elementType;
			AstAnnotations.MarkRawArrayIndexing(expression);
			return expression;
		}

		private InternalLocal DeclareTempLocal(IType type)
		{
			return CodeBuilder.DeclareTempLocal(
				_currentMethod,
				type);
		}

		/// <summary>
		/// Unpacks an expression onto a list of declarations.
		/// </summary>
		/// <param name="block">Block this takes place in</param>
		/// <param name="expression">expression to explode</param>
		/// <param name="declarations">list of declarations to set</param>
		void UnpackExpression(Block block, Expression expression, DeclarationCollection declarations)
		{
			NormalizeIterationStatements.UnpackExpression(CodeBuilder, _currentMethod, block, expression, declarations);
		}
	}
}
