using System;
using System.Collections;
using System.Reflection;
using List=Boo.Lang.List;
using Boo.Ast;
using Boo.Ast.Compilation;
using AssemblyInfo=Boo.Ast.Compilation.Binding.NamespaceBinding.AssemblyInfo;

namespace Boo.Ast.Compilation.Steps
{
	public class UsingResolutionStep : AbstractCompilerStep
	{
		Hashtable _externalNamespaces = new Hashtable();
		
		public override void Run()
		{
			ResolveUsingAssemblyReferences();
			OrganizeNamespaces();
			CompileUnit.Modules.Switch(this);
		}
		
		public override void OnModule(Boo.Ast.Module node)
		{
			node.Using.Switch(this);
		}
		
		public override void OnUsing(Boo.Ast.Using node)
		{
			List assemblies = (List)_externalNamespaces[node.Namespace];
			if (null == assemblies)
			{
				Errors.InvalidNamespace(node);
				BindingManager.Error(node);
			}
			else
			{
				if (null != node.AssemblyReference)
				{						
				}
				BindingManager.Bind(node, new Binding.NamespaceBinding(BindingManager, node, (AssemblyInfo[])assemblies.ToArray(typeof(AssemblyInfo))));
			}
		}
		
		void ResolveUsingAssemblyReferences()
		{
			foreach (Boo.Ast.Module module in CompileUnit.Modules)
			{
				UsingCollection usingCollection = module.Using;
				Using[] usingArray = usingCollection.ToArray();
				for (int i=0; i<usingArray.Length; ++i)
				{
					Using u = usingArray[i];
					ReferenceExpression reference = u.AssemblyReference;
					if (null != reference)
					{
						try
						{
							Assembly asm = Assembly.LoadWithPartialName(reference.Name);
							CompilerParameters.References.Add(asm);
							BindingManager.Bind(reference, new Binding.AssemblyBinding(asm));
						}
						catch (Exception x)
						{
							Errors.UnableToLoadAssembly(reference, reference.Name, x);
							usingCollection.RemoveAt(i);							
						}
					}
				}
			}
		}
		
		void OrganizeNamespaces()
		{
			foreach (Assembly asm in CompilerParameters.References)
			{
				Type[] types = asm.GetTypes();
				foreach (Type type in types)
				{
					string ns = type.Namespace;
					if (null == ns)
					{
						ns = string.Empty;
					}
					List assemblies = (List)_externalNamespaces[ns];
					if (null == assemblies)
					{
						assemblies = new List();
						_externalNamespaces[ns] = assemblies;
					}
					assemblies.AddUnique(new AssemblyInfo(asm, types));
				}
			}
		}
	}
}
