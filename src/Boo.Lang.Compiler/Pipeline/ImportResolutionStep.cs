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
using System.Collections;
using System.Reflection;
using List=Boo.Lang.List;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Bindings;

namespace Boo.Lang.Compiler.Pipeline
{	
	// todo: CompilerParameters.References.Changed += OnChanged
	// recalculate namespaces on reference changes
	// todo: optimize this class so it only reescans
	// the references when they change
	public class ImportResolutionStep : AbstractCompilerStep, INamespace
	{		
		Hashtable _namespaces = new Hashtable();
		
		Hashtable _externalTypes = new Hashtable();		
		
		override public void Run()
		{
			ResolveNamespaces();
			
			CompileUnitBinding binding = new CompileUnitBinding(this);			
			BindingManager.Bind(CompileUnit, binding);
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
				ModuleBinding moduleBinding = new ModuleBinding(BindingManager, module);
				BindingManager.Bind(module, moduleBinding);
				
				NamespaceDeclaration namespaceDeclaration = module.Namespace;
				if (null != namespaceDeclaration)
				{
					module.Imports.Add(new Import(namespaceDeclaration.LexicalInfo, namespaceDeclaration.Name));
				}
				GetNamespaceBinding(moduleBinding.Namespace).AddModule(moduleBinding);
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
					IBinding binding = ResolveQualifiedName(import.Namespace);					
					if (null == binding)
					{
						binding = ErrorBinding.Default;
						Errors.Add(CompilerErrorFactory.InvalidNamespace(import));
					}
					else
					{
						if (null != import.AssemblyReference)
						{	
							NamespaceBinding nsBinding = binding as NamespaceBinding;
							if (null == nsBinding)
							{
								Errors.Add(CompilerErrorFactory.NotImplemented(import, "assembly qualified type references"));
							}
							else
							{								
								binding = new AssemblyQualifiedNamespaceBinding(GetBoundAssembly(import.AssemblyReference), nsBinding);
							}
						}
						if (null != import.Alias)
						{
							binding = new AliasedNamespaceBinding(import.Alias.Name, binding);
							BindingManager.Bind(import.Alias, binding);
						}
					}
					
					_context.TraceInfo("{1}: import reference '{0}' bound to {2}.", import, import.LexicalInfo, binding.Name);
					BindingManager.Bind(import, binding);
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
							CompilerParameters.References.Add(asm);
							BindingManager.Bind(reference, new Bindings.AssemblyBinding(asm));
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
			return ((AssemblyBinding)BindingManager.GetBinding(reference)).Assembly;
		}
		
		public INamespace ParentNamespace
		{
			get
			{
				return null;
			}
		}
		
		public IBinding Resolve(string name)
		{
			IBinding binding = (IBinding)_namespaces[name];
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
		
		IBinding ResolveQualifiedName(string name)
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
			return (IBinding)ns;
		}
		
		void OrganizeExternalNamespaces()
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
					
					GetNamespaceBinding(ns).Add(type);
					
					List typeList = GetList(_externalTypes, type.FullName);
					typeList.Add(type);
				}				
			}
		}
		
		Bindings.NamespaceBinding GetNamespaceBinding(string ns)
		{
			string[] namespaceHierarchy = ns.Split('.');
			string topLevelName = namespaceHierarchy[0];
			Bindings.NamespaceBinding topLevel = GetTopLevelNamespaceBinding(topLevelName);
			Bindings.NamespaceBinding current = topLevel;
			for (int i=1; i<namespaceHierarchy.Length; ++i)
			{
				current = current.GetChildNamespace(namespaceHierarchy[i]);
			}
			return current;
		}
		
		Bindings.NamespaceBinding GetTopLevelNamespaceBinding(string topLevelName)
		{
			Bindings.NamespaceBinding binding = (Bindings.NamespaceBinding)_namespaces[topLevelName];	
			if (null == binding)
			{
				_namespaces[topLevelName] = binding = new Bindings.NamespaceBinding(this, BindingManager, topLevelName);
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
