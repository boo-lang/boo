#region license
// Copyright (c) 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace Boo.Lang.Compiler.Steps
{
	using System;
	using System.Collections;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	using Attribute=Boo.Lang.Compiler.Ast.Attribute;

	public class MergePartialClasses : AbstractNamespaceSensitiveTransformerCompilerStep
	{
		Hashtable _partials = new Hashtable();
		ClassDefinition _current = null;
		
		override public void Run()
		{			
			Visit(CompileUnit.Modules);
		}
		
		override public void Dispose()
		{
			base.Dispose();
			_current = null;
			_partials.Clear();
		}
		
		override public bool EnterClassDefinition(ClassDefinition node)
		{
			if (!node.IsPartial)
			{
				return false;
			}

			if (_partials.Contains(node.FullName))
			{
				_current = (ClassDefinition)_partials[node.FullName];
				MergeImports(node, _current);
				RemoveCurrentNode();
				return true;
			}
			else
			{
				_partials[node.FullName] = node;
				return false;
			}
		}
		
		void MergeImports(ClassDefinition from, ClassDefinition to)
		{
			Module fromModule = from.EnclosingModule;
			Module toModule = to.EnclosingModule;
			if (fromModule == toModule) return;
			if (toModule.ContainsAnnotation(fromModule)) return;
			
			toModule.Imports.ExtendWithClones(fromModule.Imports);
			// annotate so we remember not to merge the imports
			// again in the future
			toModule.Annotate(fromModule);

			//annotate so that we know these modules have been merged
			//this is used by checkneverusedmembers step
			if (!fromModule.ContainsAnnotation("merged-module"))
				fromModule.Annotate("merged-module");
			if (!toModule.ContainsAnnotation("merged-module"))
				toModule.Annotate("merged-module");
		}
		
		override public void LeaveClassDefinition(ClassDefinition node)
		{
			_current = null;
		}
		
		override public void OnAttribute(Attribute node)
		{
			if (_current == null) return;
			
			//attributes for the merged class contain any and all attributes declared on each partial definition
			_current.Attributes.Add(node);
		}
		
		private void AddMember(TypeMember member)
		{
			if (_current == null) return;
			
			_current.Members.Add(member);
		}
		
		override public void OnStructDefinition(StructDefinition node) { AddMember(node); }
		override public void OnInterfaceDefinition(InterfaceDefinition node) { AddMember(node); }
		override public void OnEnumDefinition(EnumDefinition node) { AddMember(node); }
		override public void OnField(Field node) { AddMember(node); }
		override public void OnProperty(Property node) { AddMember(node); }
		override public void OnEvent(Event node) { AddMember(node); }
		override public void OnMethod(Method node) { AddMember(node); }
		override public void OnConstructor(Constructor node) { AddMember(node); }
		
		/* the simple type references that happen while visiting a 
		 * class defintion are its base class and implemented interfaces
		 * */
		override public void OnSimpleTypeReference(SimpleTypeReference node) 
		{
			if (_current != null && !_current.BaseTypes.Contains(node.Name))
			{
				_current.BaseTypes.Add(node);
			}
		}
	}
}
