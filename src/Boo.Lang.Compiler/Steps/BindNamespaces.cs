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
using System.Collections;
using System.Reflection;
using List=Boo.Lang.List;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Taxonomy;

namespace Boo.Lang.Compiler.Steps
{	
	// todo: CompilerParameters.References.Changed += OnChanged
	// recalculate namespaces on reference changes
	// todo: optimize this class so it only reescans
	// the references when they change
	public class BindNamespaces : AbstractCompilerStep, INamespace
	{		
		Hashtable _namespaces = new Hashtable();
		
		Hashtable _externalTypes = new Hashtable();		
		
		override public void Run()
		{
			ResolveNamespaces();
		}
		
		override public void Dispose()
		{
			base.Dispose();
			_namespaces.Clear();
			_externalTypes.Clear();
		}
		
		void ResolveInternalModules()
		{
			foreach (Boo.Lang.Compiler.Ast.Module module in CompileUnit.Modules)
			{
				ModuleInfo moduleInfo = new ModuleInfo(TaxonomyHelper, module);
				TaxonomyHelper.Bind(module, moduleInfo);
				
				NamespaceDeclaration namespaceDeclaration = module.Namespace;
				if (null != namespaceDeclaration)
				{
					module.Imports.Add(new Import(namespaceDeclaration.LexicalInfo, namespaceDeclaration.Name));
				}
				GetNamespaceInfo(moduleInfo.Namespace).AddModule(moduleInfo);
			}
		}
		
		void ResolveNamespaces()
		{				
			ResolveImportAssemblyReferences();
			ResolveInternalModules();
			OrganizeExternalNamespaces();
			
			foreach (Boo.Lang.Compiler.Ast.Module module in CompileUnit.Modules)
			{
				foreach (Import import in module.Imports)
				{
					IInfo binding = ResolveQualifiedName(import.Namespace);					
					if (null == binding)
					{
						binding = ErrorInfo.Default;
						Errors.Add(CompilerErrorFactory.InvalidNamespace(import));
					}
					else
					{
						if (null != import.AssemblyReference)
						{	
							NamespaceInfo nsInfo = binding as NamespaceInfo;
							if (null == nsInfo)
							{
								Errors.Add(CompilerErrorFactory.NotImplemented(import, "assembly qualified type references"));
							}
							else
							{								
								binding = new AssemblyQualifiedNamespaceInfo(GetBoundAssembly(import.AssemblyReference), nsInfo);
							}
						}
						if (null != import.Alias)
						{
							binding = new AliasedNamespaceInfo(import.Alias.Name, binding);
							TaxonomyHelper.Bind(import.Alias, binding);
						}
					}
					
					_context.TraceInfo("{1}: import reference '{0}' bound to {2}.", import, import.LexicalInfo, binding.Name);
					TaxonomyHelper.Bind(import, binding);
				}
			}			
		}
		
		void ResolveImportAssemblyReferences()
		{
			foreach (Boo.Lang.Compiler.Ast.Module module in CompileUnit.Modules)
			{
				ImportCollection imports = module.Imports;
				Import[] importArray = imports.ToArray();
				for (int i=0; i<importArray.Length; ++i)
				{
					Import u = importArray[i];
					ReferenceExpression reference = u.AssemblyReference;
					if (null != reference)
					{
						try
						{
							Assembly asm = Assembly.LoadWithPartialName(reference.Name);
							Parameters.References.Add(asm);
							TaxonomyHelper.Bind(reference, new Taxonomy.AssemblyInfo(asm));
						}
						catch (Exception x)
						{
							Errors.Add(CompilerErrorFactory.UnableToLoadAssembly(reference, reference.Name, x));
							imports.RemoveAt(i);							
						}
					}
				}
			}
		}
		
		Assembly GetBoundAssembly(ReferenceExpression reference)
		{
			return ((AssemblyInfo)TaxonomyHelper.GetInfo(reference)).Assembly;
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return null;
			}
		}
		
		public IInfo Resolve(string name)
		{
			IInfo binding = (IInfo)_namespaces[name];
			if (null == binding)
			{
				INamespace globalns = (INamespace)_namespaces[""];
				if (null != globalns)
				{
					binding = globalns.Resolve(name);
				}
			}			
			return binding;
		}
		
		IInfo ResolveQualifiedName(string name)
		{
			string[] parts = name.Split('.');
			string topLevel = parts[0];
			
			INamespace ns = (INamespace)_namespaces[topLevel];
			if (null != ns)
			{
				for (int i=1; i<parts.Length; ++i)
				{
					ns = (INamespace)ns.Resolve(parts[i]);
					if (null == ns)
					{
						break;
					}
				}
			}
			return (IInfo)ns;
		}
		
		void OrganizeExternalNamespaces()
		{
			foreach (Assembly asm in Parameters.References)
			{
				Type[] types = asm.GetTypes();
				foreach (Type type in types)
				{
					string ns = type.Namespace;
					if (null == ns)
					{
						ns = string.Empty;
					}					
					
					GetNamespaceInfo(ns).Add(type);
					
					List typeList = GetList(_externalTypes, type.FullName);
					typeList.Add(type);
				}				
			}
		}
		
		Taxonomy.NamespaceInfo GetNamespaceInfo(string ns)
		{
			string[] namespaceHierarchy = ns.Split('.');
			string topLevelName = namespaceHierarchy[0];
			Taxonomy.NamespaceInfo topLevel = GetTopLevelNamespaceInfo(topLevelName);
			Taxonomy.NamespaceInfo current = topLevel;
			for (int i=1; i<namespaceHierarchy.Length; ++i)
			{
				current = current.GetChildNamespace(namespaceHierarchy[i]);
			}
			return current;
		}
		
		Taxonomy.NamespaceInfo GetTopLevelNamespaceInfo(string topLevelName)
		{
			Taxonomy.NamespaceInfo binding = (Taxonomy.NamespaceInfo)_namespaces[topLevelName];	
			if (null == binding)
			{
				_namespaces[topLevelName] = binding = new Taxonomy.NamespaceInfo(this, TaxonomyHelper, topLevelName);
			}
			return binding;
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
