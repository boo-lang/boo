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


using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.Steps
{
	class MethodBodyState
	{
		private int _loopDepth;

		private int _protectedBlockDepth;
		private int _exceptionHandlerDepth;

		private Stack<TryStatement> _tryBlocks = new Stack<TryStatement>();

		private List _labelReferences = new List();
		
		private Hashtable _labels = new Hashtable();
		
		public void Reset()
		{
			_loopDepth = 0;
			_exceptionHandlerDepth = 0;
			_tryBlocks.Clear();
			_labelReferences.Clear();
			_labels.Clear();
		}

		public void AddLabelReference(ReferenceExpression node)
		{
			_labelReferences.Add(node);
		}

		public List LabelReferences
		{
			get { return _labelReferences; }
		}

		public void EnterTryBlock(TryStatement tryBlock)
		{
			_tryBlocks.Push(tryBlock);
		}
		
		public void LeaveTryBlock()
		{
			_tryBlocks.Pop();
		}
		
		public void EnterProtectedBlock()
		{
			++_protectedBlockDepth;
		}
		
		public void LeaveProtectedBlock()
		{
			--_protectedBlockDepth;
		}
		
		public int TryBlockDepth {
			get { return _tryBlocks.Count; }
		}
		
		public IEnumerable<TryStatement> TryBlocks {
			get { return _tryBlocks; }
		}
		
		public int ProtectedBlockDepth {
			get { return _protectedBlockDepth; }
		}

		public void EnterExceptionHandler()
		{
			++_exceptionHandlerDepth;
		}

		public bool InExceptionHandler
		{
			get { return _exceptionHandlerDepth > 0; }
		}

		public void LeaveExceptionHandler()
		{
			--_exceptionHandlerDepth;
		}

		public void EnterLoop()
		{
			++_loopDepth;
		}

		public bool InLoop
		{
			get { return _loopDepth > 0; }
		}

		public void LeaveLoop()
		{
			--_loopDepth;
		}

		public InternalLabel ResolveLabel(string name)
		{
			return (InternalLabel)_labels[name];
		}

		public void AddLabel(InternalLabel label)
		{
			_labels.Add(label.Name, label);
		}
	}
	
	public class BranchChecking : AbstractVisitorCompilerStep
	{
		private InternalMethod _currentMethod;

		private MethodBodyState _state = new MethodBodyState();

		public override void Run()
		{
			Visit(CompileUnit);
		}

		override public void OnTryStatement(TryStatement node)
		{
			_state.EnterTryBlock(node);
			_state.EnterProtectedBlock();
			Visit(node.ProtectedBlock);
			_state.LeaveProtectedBlock();

			Visit(node.ExceptionHandlers);
			CheckExceptionHandlers(node.ExceptionHandlers);

			Visit(node.FailureBlock);

			Visit(node.EnsureBlock);
			_state.LeaveTryBlock();
		}

		override public void OnExceptionHandler(ExceptionHandler node)
		{	
			_state.EnterExceptionHandler();
			Visit(node.Block);
			_state.LeaveExceptionHandler();
		}

		override public void LeaveRaiseStatement(RaiseStatement node)
		{
			if (node.Exception != null) return;
			if (_state.InExceptionHandler) return;
			Error(CompilerErrorFactory.ReRaiseOutsideExceptionHandler(node));
		}

		public override void OnConstructor(Constructor node)
		{
			OnMethod(node);
		}

		public override void OnDestructor(Destructor node)
		{
			OnMethod(node);
		}
		
		override public void OnMethod(Method node)
		{
			_currentMethod = (InternalMethod) node.Entity;
			_state.Reset();
			Visit(node.Body);
			ResolveLabelReferences();
		}

		void ResolveLabelReferences()
		{
			foreach (ReferenceExpression reference in _state.LabelReferences)
			{
				InternalLabel label = _state.ResolveLabel(reference.Name);
				if (null == label)
				{
					Error(reference, CompilerErrorFactory.NoSuchLabel(reference, reference.Name));
				}
				else
				{
					reference.Entity = label;
				}
			}
		}

		override public void OnWhileStatement(WhileStatement node)
		{
			VisitLoop(node.Block);
			Visit(node.OrBlock);
			Visit(node.ThenBlock);
		}

		private void VisitLoop(Block block)
		{
			_state.EnterLoop();
			Visit(block);
			_state.LeaveLoop();
		}

		override public void OnForStatement(ForStatement node)
		{
			VisitLoop(node.Block);
			Visit(node.OrBlock);
			Visit(node.ThenBlock);
		}

		override public void OnLabelStatement(LabelStatement node)
		{
			AstAnnotations.SetTryBlockDepth(node, _state.TryBlockDepth);

			if (null == _state.ResolveLabel(node.Name))
			{
				_state.AddLabel(new InternalLabel(node));
			}
			else
			{
				Error(
					CompilerErrorFactory.LabelAlreadyDefined(node,
											_currentMethod.FullName,
											node.Name));
			}
		}

		override public void OnYieldStatement(YieldStatement node)
		{
			if (_state.TryBlockDepth == _state.ProtectedBlockDepth) {
				// we are currently only in the protected blocks, not in any "except" or "ensure" blocks.
				foreach (TryStatement tryBlock in _state.TryBlocks) {
					// only allow yield in the try part of try-ensure blocks, fail if it is a try-except block
					if (tryBlock.FailureBlock != null || tryBlock.ExceptionHandlers.Count > 0) {
						Error(CompilerErrorFactory.YieldInsideTryExceptOrEnsureBlock(node));
						break;
					}
				}
			} else {
				Error(CompilerErrorFactory.YieldInsideTryExceptOrEnsureBlock(node));
			}
		}

		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			if (BuiltinFunction.Switch != node.Target.Entity) return;

			for (int i = 1; i < node.Arguments.Count; ++i)
			{
				ReferenceExpression label = (ReferenceExpression)node.Arguments[i];
				_state.AddLabelReference(label);
			}
		}

		override public void OnGotoStatement(GotoStatement node)
		{
			AstAnnotations.SetTryBlockDepth(node, _state.TryBlockDepth);

			_state.AddLabelReference(node.Label);
		}

		override public void OnBreakStatement(BreakStatement node)
		{
			CheckInLoop(node);
		}

		override public void OnContinueStatement(ContinueStatement node)
		{
			CheckInLoop(node);
		}

		override public void OnBlockExpression(BlockExpression node)
		{
			// nothing to do
		}

		private void CheckInLoop(Statement node)
		{
			if (_state.InLoop) return;

			Error(CompilerErrorFactory.NoEnclosingLoop(node));
		}

		void CheckExceptionHandlers(ExceptionHandlerCollection handlers)
		{
			for (int i = 1; i < handlers.Count; ++i) {
				ExceptionHandler handler = handlers[i];
				for (int j = i - 1; j >= 0; --j) {
					ExceptionHandler previous = handlers[j];
					IType handlerType = handler.Declaration.Type.Entity as IType;
					IType previousType = previous.Declaration.Type.Entity as IType;

					if (null == handlerType || null == previousType)
						continue;

					if ((handlerType == previousType && null == previous.FilterCondition)
						|| handlerType.IsSubclassOf(previousType)) {
						Error(CompilerErrorFactory.ExceptionAlreadyHandled(
							handler, previous));
						break;
					}
				}
			}
		}

	}
}

