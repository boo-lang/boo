using System;
using System.Collections;
using Boo.Ast;
using Boo.Ast.Compilation;
using Boo.Ast.Compilation.Binding;

namespace Boo.Ast.Compilation.Steps
{
	public abstract class AbstractNamespaceSensitiveCompilerStep : AbstractTransformerCompilerStep
	{
		static readonly char[] DotArray = new char[] { '.' };
		
		protected Stack _namespaces = new Stack();
		
		public override bool EnterCompileUnit(CompileUnit cu, ref CompileUnit resultingNode)
		{
			// Global names at the highest level
			PushNamespace(UsingResolutionStep.GetGlobalNamespace(CompilerContext));
			
			// then Boo.Lang
			PushNamespace(UsingResolutionStep.GetBooLangNamespace(CompilerContext));
			                           
			// then builtins resolution			
			PushNamespace(new ExternalTypeBinding(BindingManager, typeof(Boo.Lang.Builtins)));
			return true;
		}
		
		public override void Dispose()
		{
			base.Dispose();
			_namespaces.Clear();
		}
		
		protected IBinding Resolve(Node sourceNode, string name)
		{
			if (null == sourceNode)
			{
				throw new ArgumentNullException("sourceNode");
			}
			IBinding binding = BindingManager.ResolvePrimitive(name);
			if (null == binding)
			{
				foreach (INamespace ns in _namespaces)
				{
					_context.TraceVerbose("Trying to resolve {0} against {1}...", name, ns);
					binding = ns.Resolve(name);
					if (null != binding)
					{
						break;
					}
				}
			}
			_context.TraceVerbose("{0}: {1} bound to {2}.", sourceNode.LexicalInfo, name, binding);
			return binding;
		}
		
		protected IBinding ResolveQualifiedName(Node sourceNode, string name)
		{			
			string[] parts = name.Split(DotArray);
			string topLevel = parts[0];
			IBinding binding = Resolve(sourceNode, topLevel);
			for (int i=1; i<parts.Length; ++i)				
			{				
				INamespace ns = binding as INamespace;
				if (null == ns)
				{
					binding = null;
					break;
				}
				binding = ns.Resolve(parts[i]);
			}
			return binding;
		}
		
		protected void PushNamespace(INamespace ns)
		{
			if (null == ns)
			{
				throw new ArgumentNullException("ns");
			}
			_namespaces.Push(ns);
		}
		
		protected void PopNamespace()
		{
			_namespaces.Pop();
		}
	}
}
