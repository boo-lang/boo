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

using System;
using System.IO;
using System.Collections;
using System.Xml;

namespace Boo.Lang.Compiler
{
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
	/// A group of <see cref="ICompilerComponent"/> implementations
	/// that should be executed in sequence.
	/// </summary>
	public class CompilerPipeline : System.MarshalByRefObject
	{	
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
		
		public void Configure(System.Xml.XmlElement configuration)
		{
			if (null == configuration)
			{
				throw new ArgumentNullException("configuration");
			}
			
			_items.Clear();
			InnerConfigure(configuration);
		}
		
		public void Load(string name)
		{	
			try
			{
				Configure(LoadXmlDocument(name));
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
		
		string GetRequiredAttribute(XmlElement element, string attributeName)
		{
			XmlAttribute attribute = element.GetAttributeNode(attributeName);
			if (null == attribute || 0 == attribute.Value.Length)
			{
				throw CompilerErrorFactory.AttributeNotFound(element.Name, attributeName);
				
			}
			return attribute.Value;
		}
		
		string GetOptionalAttribute(XmlElement element, string attributeName)
		{
			XmlAttribute attribute = element.GetAttributeNode(attributeName);
			if (null != attribute)
			{
				return attribute.Value;
			}
			return null;
		}

		void InnerConfigure(XmlElement configuration)
		{
			string extends = configuration.GetAttribute("extends");
			if (extends.Length > 0)
			{
				InnerConfigure(LoadXmlDocument(extends));
			}

			foreach (XmlElement element in configuration.SelectNodes("step"))
			{
				string typeName = GetRequiredAttribute(element, "type");
				Type type = Type.GetType(typeName, true);
				if (!typeof(ICompilerStep).IsAssignableFrom(type))
				{
					throw CompilerErrorFactory.TypeMustImplementICompilerStep(typeName);
				}
				
				ICompilerStep step = (ICompilerStep)Activator.CreateInstance(type);
				
				string id = element.GetAttribute("id");
				
				CompilerPipelineItem item = null;
				if (id.Length > 0)
				{
					item = new CompilerPipelineItem(id, step);
				}
				else
				{
					item = new CompilerPipelineItem(step);					
				}
				
				string insertBeforeId = GetOptionalAttribute(element, "insertBefore");
				if (null != insertBeforeId)
				{
					InsertBefore(insertBeforeId, item);
				}
				else
				{
					string insertAfterId = GetOptionalAttribute(element, "insertAfter");
					if (null != insertAfterId)
					{
						InsertAfter(insertAfterId, item);
					}
					else
					{
						_items.Add(item);
					}
				}
			}
		}
		
		XmlElement LoadXmlFromResource(string name)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(GetType().Assembly.GetManifestResourceStream(name));
			return doc.DocumentElement;
		}
		
		XmlElement LoadXmlDocument(string name)
		{
			if (!name.EndsWith(".pipeline"))
			{				
				name += ".pipeline";				
			}
			
			if (!Path.IsPathRooted(name))
			{
				string path = Path.Combine(_baseDirectory, name);
				if (!File.Exists(path))
				{
					return LoadXmlFromResource(name);
				}
				name = path;
			}
			
			XmlDocument doc = new XmlDocument();
			doc.Load(name);
			return doc.DocumentElement;
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
