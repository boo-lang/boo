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
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;

	public class NormalizeStatementModifiers : AbstractTransformerCompilerStep
	{
		override public void Run()
		{
			Visit(CompileUnit.Modules);
		}
		
		override public void LeaveUnpackStatement(UnpackStatement node)
		{
			LeaveStatement(node);
		}
		
		override public void LeaveExpressionStatement(ExpressionStatement node)
		{
			LeaveStatement(node);
		}
		
		override public void LeaveRaiseStatement(RaiseStatement node)
		{
			LeaveStatement(node);
		}
		
		override public void LeaveReturnStatement(ReturnStatement node)
		{
			LeaveStatement(node);
		}
		
		override public void LeaveBreakStatement(BreakStatement node)
		{
			LeaveStatement(node);
		}
		
		override public void LeaveContinueStatement(ContinueStatement node)
		{
			LeaveStatement(node);
		}
		
		override public void LeaveGotoStatement(GotoStatement node)
		{
			LeaveStatement(node);
		}
		
		override public void LeaveYieldStatement(YieldStatement node)
		{
			LeaveStatement(node);
		}
		
		override public void LeaveLabelStatement(LabelStatement node)
		{
			if (null != node.Modifier)
			{
				Warnings.Add(
					CompilerWarningFactory.ModifiersInLabelsHaveNoEffect(node.Modifier));
			}
		}
		
		override public void LeaveMacroStatement(MacroStatement node)
		{
			LeaveStatement(node);
		}
		
		public static Statement CreateModifiedStatement(StatementModifier modifier, Statement node)
		{
			Block block;
			Statement stmt = MapStatementModifier(modifier, out block);
			block.Add(node);
			return stmt;
		}
		
		public static Statement MapStatementModifier(StatementModifier modifier, out Block block)
		{
			switch (modifier.Type)
			{
				case StatementModifierType.If:
				{
					IfStatement stmt = new IfStatement(modifier.LexicalInfo);
					stmt.Condition = modifier.Condition;
					stmt.TrueBlock = new Block();
					block = stmt.TrueBlock;
					return stmt;
				}
				
				case StatementModifierType.Unless:
				{
					UnlessStatement stmt = new UnlessStatement(modifier.LexicalInfo);
					stmt.Condition = modifier.Condition;
					block = stmt.Block;
					return stmt;
				}
				
				case StatementModifierType.While:
				{
					WhileStatement stmt = new WhileStatement(modifier.LexicalInfo);
					stmt.Condition = modifier.Condition;
					block = stmt.Block;
					return stmt;
				}
			}
			throw CompilerErrorFactory.NotImplemented(modifier, string.Format("modifier {0} supported", modifier.Type));
		}
		
		public void LeaveStatement(Statement node)
		{
			StatementModifier modifier = node.Modifier;
			if (null != modifier)
			{
				node.Modifier = null;
				ReplaceCurrentNode(CreateModifiedStatement(modifier, node));
			}
		}
	}
}
