#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
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
		
		MethodInvocationExpression CreateMethodInvocation(Expression target, Expression arg)
		{
			MethodInvocationExpression mie = new MethodInvocationExpression(arg.LexicalInfo);
			mie.Target = (Expression)target.Clone();			
			mie.Arguments.Add((Expression)arg.Clone());
			return mie;
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
			block.Add(CreateMethodInvocation(Monitor_Enter, monitorReference));	
			
			// try:			
			// 		<the rest>
			// ensure:
			//		Monitor.Leave			
			TryStatement stmt = new TryStatement();			
			stmt.ProtectedBlock = body;
			stmt.EnsureBlock = new Block();
			stmt.EnsureBlock.Add(
				CreateMethodInvocation(Monitor_Exit, monitorReference));
				
			block.Add(stmt);
			
			return block;
		}
	}
}
