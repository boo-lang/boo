using System;
using System.Collections;
using System.Reflection;
using List=Boo.Lang.List;
using Boo.Ast;
using Boo.Ast.Compilation;
using Boo.Ast.Compilation.Binding;
using AssemblyInfo=Boo.Ast.Compilation.Binding.NamespaceBinding.AssemblyInfo;

namespace Boo.Ast.Compilation.Steps
{
	public class UsingResolutionStep : AbstractCompilerStep
	{
		Hashtable _externalNamespaces = new Hashtable();
		
		Hashtable _externalTypes = new Hashtable();
		
		public override void Run()
		{
			ResolveUsingAssemblyReferences();
			OrganizeNamespaces();
			foreach (Module module in CompileUnit.Modules)
			{
				foreach (Using using_ in module.Using)
				{
					IBinding binding = ErrorBinding.Default;
					
					List assemblies = (List)_externalNamespaces[using_.Namespace];
					if (null == assemblies)
					{
						List types = (List)_externalTypes[using_.Namespace];
						if (null != types)
						{
							if (types.Count > 1)
							{
								Errors.AmbiguousTypeReference(using_, types);								
							}
							else
							{
								binding = BindingManager.ToTypeBinding((Type)types[0]);
							}
						}
						else
						{
							Errors.InvalidNamespace(using_);
						}
					}
					else
					{
						if (null != using_.AssemblyReference)
						{	
							// todo:
						}
						binding = new Binding.NamespaceBinding(BindingManager, using_, (AssemblyInfo[])assemblies.ToArray(typeof(AssemblyInfo)));
					}
					
					BindingManager.Bind(using_, binding);
				}
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
					List assemblies = GetList(_externalNamespaces, ns);
					assemblies.AddUnique(new AssemblyInfo(asm, types));
					
					List typeList = GetList(_externalTypes, type.FullName);
					typeList.Add(type);
				}				
			}
		}
		
		List GetList(Hashtable hash, string key)
		{
			List list = (List)hash[key];
			if (null == list)
			{
				list = new List();
				hash[key] = list;
			}
			return list;
		}
	}
}
