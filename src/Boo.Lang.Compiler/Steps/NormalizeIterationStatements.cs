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
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.TypeSystem;

	/// <summary>
	/// AST semantic evaluation.
	/// </summary>
	public class NormalizeIterationStatements : AbstractTransformerCompilerStep
	{
		IMethod IEnumerable_GetEnumerator;
		IMethod IEnumerator_MoveNext;
		IMethod IEnumerator_get_Current;
		Method _current;
		
		public NormalizeIterationStatements()
		{
		}
		
		override public void Initialize(CompilerContext context)
		{
			base.Initialize(context);
			
			IEnumerable_GetEnumerator = TypeSystemServices.Map(EmitAssembly.IEnumerable_GetEnumerator);
			IEnumerator_MoveNext = TypeSystemServices.Map(EmitAssembly.IEnumerator_MoveNext);
			IEnumerator_get_Current = TypeSystemServices.Map(EmitAssembly.IEnumerator_get_Current);
		}
		
		override public void Run()
		{
			if (0 == Errors.Count)
			{				
				Visit(CompileUnit);
			}
		}
		
		override public void OnMethod(Method node)
		{
			_current = node;
			Visit(node.Body);
		}
		
		override public void LeaveForStatement(ForStatement node)
		{						
			IType enumeratorType = GetExpressionType(node.Iterator);
			IType enumeratorItemType = TypeSystemServices.GetEnumeratorItemType(enumeratorType);
			DeclarationCollection declarations = node.Declarations;
			Block body = new Block(node.LexicalInfo);
			
			InternalLocal iterator = CodeBuilder.DeclareLocal(
										_current,
										"___iterator" + _context.AllocIndex(),
										TypeSystemServices.IEnumeratorType);
			
			// ___iterator = <node.Iterator>.GetEnumerator()
			body.Add(
				CodeBuilder.CreateAssignment(
					CodeBuilder.CreateReference(iterator),
					CodeBuilder.CreateMethodInvocation(
						node.Iterator,
						IEnumerable_GetEnumerator)));
					
			// while __iterator.MoveNext():
			WhileStatement ws = new WhileStatement(node.LexicalInfo);
			ws.Condition = CodeBuilder.CreateMethodInvocation(
							CodeBuilder.CreateReference(iterator),
							IEnumerator_MoveNext);			
			
			Expression current = CodeBuilder.CreateMethodInvocation(
							CodeBuilder.CreateReference(iterator),
							IEnumerator_get_Current);							
			
			if (1 == declarations.Count)
			{
				//	item = __iterator.Current
				ws.Block.Add(
					CodeBuilder.CreateAssignment(
						CodeBuilder.CreateReference((InternalLocal)declarations[0].Entity),
						current));
			}
			else
			{
				ws.Block.Add(
					CodeBuilder.CreateUnpackStatement(
						node.Declarations,
						CodeBuilder.CreateCast(
							enumeratorItemType,
							current)));						
			}
			
			ws.Block.Add(node.Block);
			
			body.Add(ws);
			
			ReplaceCurrentNode(body);
		}
	}
}	

