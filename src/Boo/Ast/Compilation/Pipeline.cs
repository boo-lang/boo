using System;
using System.Collections;
using System.Xml;

namespace Boo.Ast.Compilation
{
	/// <summary>
	/// A group of <see cref="ICompilerStep"/> implementations
	/// that should be executed in sequence.
	/// </summary>
	public class Pipeline
	{
		ArrayList _steps;

		public Pipeline()
		{
			_steps = new ArrayList();
		}

		public void Add(ICompilerStep step)
		{
			if (null == step)
			{
				throw new ArgumentNullException("step");
			}
			_steps.Add(step);
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

		public void Run(CompilerContext context)
		{
			foreach (ICompilerStep step in _steps)
			{
				step.Initialize(context);
				try
				{
					step.Run();
				}
				catch (Boo.Ast.Compilation.Error error)
				{
					context.Errors.Add(error);
				}
				catch (Exception x)
				{
					context.Errors.StepExecution(step, x);
				}
				finally
				{
					step.Dispose();
				}
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
				if (!typeof(ICompilerStep).IsAssignableFrom(type))
				{
					throw new ArgumentException(ResourceManager.Format("ICompilerStepInterface", type.Name));
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
			XmlDocument doc = new XmlDocument();
			doc.Load(new Uri(baseUri, name).AbsoluteUri);
			return doc.DocumentElement;
		}
	}
}
