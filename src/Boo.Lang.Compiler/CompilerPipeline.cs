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
	using Boo.Lang.Compiler.Pipeline.Definitions;

	/// <summary>
	/// An item in the compilation pipeline. Associates
	/// an ID to an ICompilerStep implementation.
	/// </summary>
	public class CompilerPipelineItem
	{
		string _id;
		ICompilerStep _step;
		
		public CompilerPipelineItem(ICompilerStep step)
		{
			if (null == step)
			{
				throw new ArgumentNullException("step");
			}
			
			_id = Guid.NewGuid().ToString();
			_step = step;
		}
		
		public CompilerPipelineItem(string id, ICompilerStep step)
		{
			if (null == id)
			{
				throw new ArgumentNullException("id");
			}
			if (null == step)
			{
				throw new ArgumentNullException("step");
			}
			if (0 == id.Length)
			{
				throw new ArgumentException("id");
			}
			
			_id = id;
			_step = step;
		}
		
		public string ID
		{
			get
			{
				return _id;
			}
		}
		
		public ICompilerStep CompilerStep
		{
			get
			{
				return _step;
			}
		}
	}
	
	/// <summary>
	/// A ordered set of <see cref="ICompilerComponent"/> implementations
	/// that should be executed in sequence.
	/// </summary>
	public class CompilerPipeline : System.MarshalByRefObject
	{	
		static Hash _definitions = new Hash(true);
		
		public void AddDefinition(string name, ICompilerPipelineDefinition definition)
		{
			if (null == name)
			{
				throw new ArgumentNullException("name");
			}
			
			if (null == definition)
			{
				throw new ArgumentNullException("definition");
			}
			
			lock (_definitions)
			{
				_definitions.Add(name, definition);
			}
		}
		
		public ICompilerPipelineDefinition GetDefinition(string name)
		{
			lock (_definitions)
			{
				return _definitions[name] as ICompilerPipelineDefinition;
			}
		}
		
		static CompilerPipeline()
		{
			_definitions.Add("parse", new ParsePipelineDefinition());
			_definitions.Add("core", new CorePipelineDefinition());
			_definitions.Add("boom", new BoomPipelineDefinition());
			_definitions.Add("booi", new BooiPipelineDefinition());
			_definitions.Add("booc", new BoocPipelineDefinition());
			_definitions.Add("rountrip", new RoundtripPipelineDefinition());
			_definitions.Add("boo", new BooPipelineDefinition());
			_definitions.Add("xml", new XmlPipelineDefinition());
		}
		
		ArrayList _items;
		
		string _baseDirectory = ".";

		public CompilerPipeline()
		{
			_items = new ArrayList();
		}
		
		public CompilerPipeline Add(CompilerPipelineItem item)
		{
			if (null == item)
			{
				throw new ArgumentNullException("item");
			}
			
			_items.Add(Validate(item));
			return this;
		}

		public CompilerPipeline Add(ICompilerStep step)
		{
			return Add(new CompilerPipelineItem(step));
		}
		
		public CompilerPipeline Insert(int index, ICompilerStep step)
		{
			return Insert(index, new CompilerPipelineItem(step));
		}
		
		public CompilerPipeline Insert(int index, CompilerPipelineItem item)
		{
			if (null == item)
			{
				throw new ArgumentNullException("item");
			}
			_items.Insert(index, Validate(item));
			return this;
		}
		
		public CompilerPipeline InsertBefore(string id, ICompilerStep step)
		{			
			return InsertBefore(id, new CompilerPipelineItem(step));
		}
		
		public CompilerPipeline InsertBefore(string id, CompilerPipelineItem item)
		{		
			if (null == id)
			{
				throw new ArgumentNullException("id");
			}
			if (null == item)
			{
				throw new ArgumentNullException("item");
			}
			_items.Insert(FindIndex(id), Validate(item));
			return this;
		}
		
		public CompilerPipeline InsertAfter(string id, ICompilerStep step)
		{
			return InsertAfter(id, new CompilerPipelineItem(step));
		}
		
		public CompilerPipeline InsertAfter(string id, CompilerPipelineItem item)
		{
			if (null == id)
			{
				throw new ArgumentNullException("id");
			}
			if (null == item)
			{
				throw new ArgumentNullException("item");
			}
			_items.Insert(FindIndex(id)+1, Validate(item));
			return this;
		}
		
		public CompilerPipeline Remove(string id)
		{
			if (null == id)
			{
				throw new ArgumentNullException("id");
			}
			_items.RemoveAt(FindIndex(id));
			return this;
		}
		
		public CompilerPipeline Replace(string id, CompilerPipelineItem item)
		{
			if (null == id)
			{
				throw new ArgumentNullException("id");
			}
			if (null == item)
			{
				throw new ArgumentNullException("item");
			}
			_items[FindIndex(id)] = Validate(item);
			return this;
		}
		
		public string BaseDirectory
		{
			get
			{
				return _baseDirectory;
			}
			
			set
			{
				if (null == value)
				{
					throw new ArgumentNullException("value");
				}
				_baseDirectory = value;
			}
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
				return ((CompilerPipelineItem)_items[index]).CompilerStep;
			}
		}
		
		public void Clear()
		{
			_items.Clear();
		}
		
		public void Load(string name)
		{	
			if (null == name)
			{
				throw new ArgumentNullException("name");
			}
			
			try
			{
				ICompilerPipelineDefinition definition = GetDefinition(name);
				if (null == definition)
				{					
					definition = (ICompilerPipelineDefinition)Activator.CreateInstance(Type.GetType(name, true));
				}
				definition.Define(this);
			}
			catch (Exception x)
			{
				throw new ApplicationException(Boo.ResourceManager.Format("BooC.UnableToLoadPipeline", name, x.Message), x);
			}
		}

		public void Run(CompilerContext context)
		{
			foreach (CompilerPipelineItem item in _items)
			{
				ICompilerStep step = item.CompilerStep;
				
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
			
			foreach (CompilerPipelineItem item in _items)
			{
				item.CompilerStep.Dispose();
			}
		}
		
		int FindIndex(string id)
		{
			int index = FindIndexNoException(id);
			if (-1 == index)
			{
				throw new ArgumentException("id");
			}
			return index;
		}
		
		int FindIndexNoException(string id)
		{
			for (int i=0; i<_items.Count; ++i)
			{
				if (id == ((CompilerPipelineItem)_items[i]).ID)
				{
					return i;
				}
			}
			return -1;
		}
		
		CompilerPipelineItem Validate(CompilerPipelineItem item)
		{
			if (-1 != FindIndexNoException(item.ID))
			{
				throw new ArgumentException("item");
			}
			return item;
		}
	}
}
