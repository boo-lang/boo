using System;
using System.Collections;
using Boo.Ast;
using Boo.Ast.Compilation;
using Boo.Ast.Compilation.Binding;

namespace Boo.Ast.Compilation.Steps
{
	public abstract class AbstractNamespaceSensitiveCompilerStep : AbstractCompilerStep
	{
		static readonly char[] DotArray = new char[] { '.' };
		
		protected Stack _namespaces = new Stack();
		
		public override bool EnterCompileUnit(CompileUnit cu)
		{
			// Global names at the highest level
			PushNamespace(UsingResolutionStep.GetGlobalNamespace(CompilerContext));
			
			// then Boo.Lang
			PushNamespace(UsingResolutionStep.GetBooLangNamespace(CompilerContext));
			                           
			// then builtins resolution			
			PushNamespace(new ExternalTypeBinding(BindingManager, typeof(Boo.Lang.Builtins)));
			return true;
		}
		
		public override void LeaveCompileUnit(CompileUnit cu)
		{
			PopNamespace();
			PopNamespace();
			PopNamespace();
		}
		
		protected IBinding Resolve(string name)
		{
			IBinding binding = BindingManager.ResolvePrimitive(name);
			if (null == binding)
			{
				foreach (INameSpace ns in _namespaces)
				{
					binding = ns.Resolve(name);
					if (null != binding)
					{
						break;
					}
				}
			}
			return binding;
		}
		
		protected IBinding ResolveQualifiedName(string name)
		{			
			string[] parts = name.Split(DotArray);
			string topLevel = parts[0];
			IBinding binding = Resolve(name);
			for (int i=1; i<parts.Length; ++i)				
			{				
				INameSpace ns = binding as INameSpace;
				if (null == ns)
				{
					binding = null;
					break;
				}
				binding = ns.Resolve(name);
			}
			return binding;
		}
		
		protected void PushNamespace(INameSpace ns)
		{
			_namespaces.Push(ns);
		}
		
		protected void PopNamespace()
		{
			_namespaces.Pop();
		}
	}
}
