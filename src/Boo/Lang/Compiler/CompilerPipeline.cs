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

using System;
using System.IO;
using System.Collections;
using System.Xml;

namespace Boo.Lang.Compiler
{
	/// <summary>
	/// A group of <see cref="ICompilerComponent"/> implementations
	/// that should be executed in sequence.
	/// </summary>
	public class CompilerPipeline
	{
		ArrayList _steps;

		public CompilerPipeline()
		{
			_steps = new ArrayList();
		}

		public CompilerPipeline Add(ICompilerStep step)
		{
			if (null == step)
			{
				throw new ArgumentNullException("step");
			}
			_steps.Add(step);
			return this;
		}

		public int Count
		{
			get
			{
				return _steps.Count;
			}
		}

		public ICompilerStep this[int index]
		{
			get
			{
				return (ICompilerStep)_steps[index];
			}
		}

		public void Configure(System.Xml.XmlElement configuration)
		{
			if (null == configuration)
			{
				throw new ArgumentNullException("configuration");
			}

			_steps.Clear();
			InnerConfigure(configuration);
		}	
		
		public void Load(string baseDirectory, string name)
		{			
			if (!name.EndsWith(".pipeline"))
			{				
				name += ".pipeline";				
			}
			
			if (!Path.IsPathRooted(name))
			{
				string path = Path.Combine(baseDirectory, name);
				if (!File.Exists(path))
				{
					path = Path.Combine(CodeBase, name);
				}
				name = path;
			}
			
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
			foreach (ICompilerStep step in _steps)
			{
				context.TraceEnter("Entering {0}...", step);			
				
				step.Initialize(context);
				try
				{
					step.Run();
				}
				catch (Boo.Lang.Compiler.Error error)
				{
					context.Errors.Add(error);
				}
				catch (Exception x)
				{
					context.Errors.StepExecution(step, x);
				}
				context.TraceLeave("Left {0}.", step);
			}
			
			foreach (ICompilerComponent step in _steps)
			{
				step.Dispose();
			}
		}
		
		string CodeBase
		{
			get
			{
				return Path.GetDirectoryName(GetType().Assembly.Location);
			}
		}

		string GetRequiredAttribute(XmlElement element, string attributeName)
		{
			XmlAttribute attribute = element.GetAttributeNode(attributeName);
			if (null == attribute || 0 == attribute.Value.Length)
			{
				throw new ArgumentException(ResourceManager.Format("RequiredAttribute", element.Name, attributeName));
			}
			return attribute.Value;
		}

		void InnerConfigure(XmlElement configuration)
		{
			string extends = configuration.GetAttribute("extends");
			if (extends.Length > 0)
			{
				InnerConfigure(LoadBasePipeline(configuration, extends));
			}

			foreach (XmlElement element in configuration.SelectNodes("step"))
			{
				string typeName = GetRequiredAttribute(element, "type");
				Type type = Type.GetType(typeName, true);
				if (!typeof(ICompilerComponent).IsAssignableFrom(type))
				{
					throw new ArgumentException(ResourceManager.Format("ICompilerComponentInterface", type.Name));
				}
				_steps.Add(Activator.CreateInstance(type));
			}
		}

		XmlElement LoadBasePipeline(XmlElement rel, string name)
		{
			if (!name.EndsWith(".pipeline"))
			{
				name += ".pipeline";
			}
			Uri baseUri = new Uri(rel.OwnerDocument.BaseURI);
			return LoadXmlDocument(new Uri(baseUri, name).AbsoluteUri);
		}
		
		XmlElement LoadXmlDocument(string path)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(path);
			return doc.DocumentElement;
		}
	}
}
