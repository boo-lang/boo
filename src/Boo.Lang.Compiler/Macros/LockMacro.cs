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

namespace Boo.Lang
{
	using System;
	using Boo.Lang.Compiler.Ast;
	
	public class LockAttribute : Boo.Lang.Compiler.AbstractAstAttribute
	{
		Expression _monitor;
		
		public LockAttribute(Expression monitor)
		{
			if (null == monitor)
			{
				throw new ArgumentNullException("monitor");
			}
			_monitor = monitor;
		}
		
		public LockAttribute()
		{
		}
		
		override public void Apply(Node node)
		{
			if (NodeType.Method != node.NodeType)
			{
				InvalidNodeForAttribute("Method");
				return;
			}
			
			if (null == _monitor)
			{
				_monitor = new SelfLiteralExpression(LexicalInfo);
			}
			
			Method method = (Method)node;
			method.Body = CreateLockedBlock(method.Body);
		}
		
		Block CreateLockedBlock(Block body)
		{
			using (LockMacro macro = new LockMacro(_context))
			{
				return macro.CreateLockedBlock(_monitor, body);
			}
		}
	}
	
	/// <summary>
	/// lock obj1, obj2:
	///		obj1.Foo(obj2)
	/// </summary>
	public class LockMacro : Boo.Lang.Compiler.IAstMacro
	{
		public const string MonitorLocalName = "__monitor{0}__";
		
		static Expression Monitor_Enter = AstUtil.CreateReferenceExpression("System.Threading.Monitor.Enter");
		
		static Expression Monitor_Exit = AstUtil.CreateReferenceExpression("System.Threading.Monitor.Exit");
		
		Boo.Lang.Compiler.CompilerContext _context;
		
		public LockMacro()
		{
		}
		
		public LockMacro(Boo.Lang.Compiler.CompilerContext context)
		{
			_context = context;
		}
		
		public void Initialize(Boo.Lang.Compiler.CompilerContext context)
		{			
			_context = context;
		}
		
		public void Dispose()
		{			
			_context = null;
		}
		
		public Statement Expand(MacroStatement macro)
		{				
			if (null == _context)
			{
				throw new InvalidOperationException("macro was not property initialized!");
			}
			
			if (0 == macro.Arguments.Count)
			{
				throw Boo.Lang.Compiler.CompilerErrorFactory.InvalidLockMacroArguments(macro);
			}
			
			Block resulting = macro.Block;
			ExpressionCollection args = macro.Arguments;
			for (int i=args.Count; i > 0; --i)
			{
				Expression arg = args[i-1];
				
				resulting = CreateLockedBlock(arg, resulting);
			}
			
			return resulting;
		}
		
		ReferenceExpression CreateMonitorReference(LexicalInfo lexicalInfo)
		{
			int localIndex = _context.AllocIndex();
			return new ReferenceExpression(lexicalInfo,
							string.Format(MonitorLocalName, localIndex));
		}
		
		internal Block CreateLockedBlock(Expression monitor, Block body)
		{	
			ReferenceExpression monitorReference = CreateMonitorReference(monitor.LexicalInfo);
			
			Block block = new Block(body.LexicalInfo);
			
			// __monitorN__ = <expression>
			block.Add(new BinaryExpression(BinaryOperatorType.Assign,
										monitorReference,
										monitor));

			// System.Threading.Monitor.Enter(__monitorN__)			
			block.Add(AstUtil.CreateMethodInvocationExpression(Monitor_Enter, monitorReference));	
			
			// try:			
			// 		<the rest>
			// ensure:
			//		Monitor.Leave			
			TryStatement stmt = new TryStatement();			
			stmt.ProtectedBlock = body;
			stmt.EnsureBlock = new Block();
			stmt.EnsureBlock.Add(
				AstUtil.CreateMethodInvocationExpression(Monitor_Exit, monitorReference));
				
			block.Add(stmt);
			
			return block;
		}
	}
}
