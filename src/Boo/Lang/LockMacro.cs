#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang
{
	using System;
	using Boo.Lang.Ast;
	
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
			int localIndex = _context.AllocLocalIndex();
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
