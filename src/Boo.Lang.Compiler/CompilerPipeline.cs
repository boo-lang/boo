#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
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
		public static CompilerPipeline GetPipeline(string name)
		{
			switch (name)
			{
				case "parse": return new Pipelines.Parse();
				case "compile": return new Pipelines.Compile();
				case "run": return new Pipelines.Run();
				case "default": return new Pipelines.CompileToFile();
				case "roundtrip": return new Pipelines.ParseAndPrint();
				case "boo": return new Pipelines.CompileToBoo();
				case "xml": return new Pipelines.ParseAndPrintXml();
				case "dumpreferences":
				{
					CompilerPipeline pipeline = new Pipelines.CompileToBoo();
					pipeline.Add(new Boo.Lang.Compiler.Steps.DumpReferences());
					return pipeline;
				}
				case "duck":
				{
					CompilerPipeline pipeline = new Pipelines.CompileToBoo();
					Pipelines.Quack.MakeItQuack(pipeline);
					return pipeline;
				}
				case "quack": return new Pipelines.Quack();
			}
			return (CompilerPipeline)Activator.CreateInstance(Type.GetType(name, true));
		}
		
		protected Boo.Lang.List _items;

		public CompilerPipeline()
		{
			_items = new Boo.Lang.List();
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
		
		public int Find(Type stepExactType)
		{
			if (null == stepExactType)
			{
				throw new ArgumentNullException("stepExactType");
			}
			for (int i=0; i<_items.Count; ++i)
			{
				if (_items[i].GetType() == stepExactType)
				{
					return i;
				}
			}
			return -1;
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
			
			set
			{
				if (null == value)
				{
					throw new ArgumentNullException("value");
				}
				_items[index] = value;
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
