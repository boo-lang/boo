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

namespace Boo.Lang.Compiler
{
	using System;
	using System.IO;
	using System.Collections;
	
	/// <summary>
	/// A ordered set of <see cref="ICompilerStep"/> implementations
	/// that should be executed in sequence.
	/// </summary>
	public class CompilerPipeline : System.MarshalByRefObject
	{	
		ArrayList _items;

		public CompilerPipeline()
		{
			_items = new ArrayList();
		}
		
		public CompilerPipeline Add(ICompilerStep step)
		{
			if (null == step)
			{
				throw new ArgumentNullException("step");
			}
			
			_items.Add(step);
			return this;
		}
		
		public CompilerPipeline Insert(int index, ICompilerStep step)
		{
			if (null == step)
			{
				throw new ArgumentNullException("step");
			}
			
			_items.Insert(index, step);
			return this;
		}

		public int Count
		{
			get
			{
				return _items.Count;
			}
		}

		public ICompilerStep this[int index]
		{
			get
			{
				return (ICompilerStep)_items[index];
			}
		}
		
		virtual public void Clear()
		{
			_items.Clear();
		}

		virtual public void Run(CompilerContext context)
		{
			foreach (ICompilerStep step in _items)
			{				
				context.TraceEnter("Entering {0}...", step);			
				
				step.Initialize(context);
				try
				{
					step.Run();
				}
				catch (Boo.Lang.Compiler.CompilerError error)
				{
					context.Errors.Add(error);
				}
				catch (Exception x)
				{
					context.Errors.Add(CompilerErrorFactory.StepExecutionError(x, step));
				}
				context.TraceLeave("Left {0}.", step);
			}
			
			foreach (ICompilerStep step in _items)
			{
				step.Dispose();
			}
		}
	}
}
