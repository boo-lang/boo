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
			_context.TraceInfo("{0}: {1} bound to {2}.", sourceNode.LexicalInfo, name, binding);
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
