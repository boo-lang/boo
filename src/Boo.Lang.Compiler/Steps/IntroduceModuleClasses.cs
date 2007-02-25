#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class IntroduceModuleClasses : AbstractVisitorCompilerStep
	{
		public const string EntryPointMethodName = "Main";
		
		protected IType _booModuleAttributeType;
		
		protected bool _forceModuleClass = false;
		
		public bool ForceModuleClass
		{
			get
			{
				return _forceModuleClass;
			}
			
			set
			{
				_forceModuleClass = true;
			}
		}
		
		override public void Initialize(CompilerContext context)
		{
			base.Initialize(context);
			_booModuleAttributeType = TypeSystemServices.Map(typeof(System.Runtime.CompilerServices.CompilerGlobalScopeAttribute));
		}
		
		override public void Run()
		{
			Visit(CompileUnit.Modules);
		}
		
		override public void Dispose()
		{
			_booModuleAttributeType = null;
			base.Dispose();
		}
		
		override public void OnModule(Module node)
		{
			bool hasModuleClass = true;
			ClassDefinition moduleClass = FindModuleClass(node);
			if (null == moduleClass)
			{
				moduleClass = new ClassDefinition();
				hasModuleClass = false;
			}
			
			Method entryPoint = moduleClass.Members["Main"] as Method;
			
			int removed = 0;
			TypeMember[] members = node.Members.ToArray();
			for (int i=0; i<members.Length; ++i)
			{
				TypeMember member = members[i];
				if (member is TypeDefinition) continue;
				if (member.NodeType == NodeType.Method)
				{
					if (EntryPointMethodName == member.Name)
					{
						entryPoint = (Method)member;
					}
					member.Modifiers |= TypeMemberModifiers.Static;
				}
				node.Members.RemoveAt(i-removed);
				moduleClass.Members.Add(member);
				++removed;
			}
			
			if (node.Globals.Statements.Count > 0)
			{
				Method method = new Method(node.Globals.LexicalInfo);
				method.Parameters.Add(new ParameterDeclaration("argv", new ArrayTypeReference(new SimpleTypeReference("string"))));
				method.ReturnType = CodeBuilder.CreateTypeReference(TypeSystemServices.VoidType);
				method.Body = node.Globals;
				method.Name = EntryPointMethodName;
				method.Modifiers = TypeMemberModifiers.Static | TypeMemberModifiers.Private;
				moduleClass.Members.Add(method);
				
				node.Globals = null;
				entryPoint = method;
			}
			
			if (null != entryPoint)
			{
				ContextAnnotations.SetEntryPoint(Context, entryPoint);
			}
			
			if (hasModuleClass || _forceModuleClass || (moduleClass.Members.Count > 0))
			{
				if (!hasModuleClass)
				{
					moduleClass.Name = BuildModuleClassName(node);
					moduleClass.Attributes.Add(CreateBooModuleAttribute());
					node.Members.Add(moduleClass);
				}
				
				moduleClass.Members.Add(AstUtil.CreateConstructor(node, TypeMemberModifiers.Private));
				moduleClass.Modifiers = TypeMemberModifiers.Public |
										TypeMemberModifiers.Final |
										TypeMemberModifiers.Transient;
				
				((ModuleEntity)node.Entity).InitializeModuleClass(moduleClass);
			}
		}
		
		ClassDefinition FindModuleClass(Module node)
		{
			ClassDefinition found = null;
			
			foreach (TypeMember member in node.Members)
			{
				if (NodeType.ClassDefinition == member.NodeType &&
					member.Attributes.Contains("System.Runtime.CompilerServices.CompilerGlobalScopeAttribute"))
				{
					if (null == found)
					{
						found = (ClassDefinition)member;
					}
					else
					{
						// ERROR: only a single module class is allowed per module
					}
				}
			}
			return found;
		}
		
		Boo.Lang.Compiler.Ast.Attribute CreateBooModuleAttribute()
		{
			Boo.Lang.Compiler.Ast.Attribute attribute = new Boo.Lang.Compiler.Ast.Attribute("System.Runtime.CompilerServices.CompilerGlobalScopeAttribute");
			attribute.Entity = _booModuleAttributeType;
			return attribute;
		}
		
		string BuildModuleClassName(Module module)
		{
			string name = module.Name;
			if (null != name)
			{
				char c = name[0];
				if (!(char.IsLetter(c) || c=='_'))
				{
					name = "_"+name;
				}
				name = name.Substring(0, 1).ToUpper() + name.Substring(1) + "Module";
			}
			else
			{
				module.Name = name = string.Format("__Module{0}__", _context.AllocIndex());
			}
			return name;
		}
	}
}
