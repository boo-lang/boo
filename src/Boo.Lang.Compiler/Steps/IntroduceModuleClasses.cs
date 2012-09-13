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

using System.Linq;
using System.Text;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.Steps
{
	public class IntroduceModuleClasses : AbstractFastVisitorCompilerStep
	{
		public const string ModuleAttributeName = "System.Runtime.CompilerServices.CompilerGlobalScopeAttribute";
		
		public const string EntryPointMethodName = "Main";
		
		public static bool IsModuleClass(TypeMember member)
		{
			return NodeType.ClassDefinition == member.NodeType && member.Attributes.Contains(ModuleAttributeName);
		}

		private IType _booModuleAttributeType;

		public bool ForceModuleClass { get; set; }

		override public void Initialize(CompilerContext context)
		{
			base.Initialize(context);
			_booModuleAttributeType = TypeSystemServices.Map(typeof(System.Runtime.CompilerServices.CompilerGlobalScopeAttribute));
		}
		
		override public void Run()
		{
			Visit(CompileUnit.Modules);

			DetectOutputType();
		}

		private void DetectOutputType()
		{
			if (Parameters.OutputType != CompilerOutputType.Auto)
				return;

			Parameters.OutputType = HasEntryPoint()
				? CompilerOutputType.ConsoleApplication
				: CompilerOutputType.Library;
		}

		private bool HasEntryPoint()
		{
			return ContextAnnotations.GetEntryPoint(Context) != null;
		}

		override public void Dispose()
		{
			_booModuleAttributeType = null;
			base.Dispose();
		}
		
		override public void OnModule(Module module)
		{	
			var existingModuleClass = ExistingModuleClassFor(module);
			var moduleClass = existingModuleClass ?? NewModuleClassFor(module);

			MoveModuleMembersToModuleClass(module, moduleClass);
			MoveModuleAttributesToModuleClass(module, moduleClass);

			DetectEntryPoint(module, moduleClass);

			if (existingModuleClass != null || ForceModuleClass || (moduleClass.Members.Count > 0))
			{
				if (moduleClass != existingModuleClass)
				{
					moduleClass.Members.Add(AstUtil.CreateConstructor(module, TypeMemberModifiers.Private));
					module.Members.Add(moduleClass);
				}
				InitializeModuleClassEntity(module, moduleClass);
			}
		}

		private static void MoveModuleAttributesToModuleClass(Module module, ClassDefinition moduleClass)
		{
			moduleClass.Attributes.AddRange(module.Attributes);
			module.Attributes.Clear();
		}

		private void DetectEntryPoint(Module module, ClassDefinition moduleClass)
		{
			var entryPoint = module.Globals.IsEmpty
             	? moduleClass.Members[EntryPointMethodName] as Method
             	: TransformModuleGlobalsIntoEntryPoint(module, moduleClass);

			if (entryPoint == null)
				return;

			if (Parameters.OutputType == CompilerOutputType.Library)
				return;

			ContextAnnotations.SetEntryPoint(Context, entryPoint);
		}

		private Method TransformModuleGlobalsIntoEntryPoint(Module node, ClassDefinition moduleClass)
		{
			var method = new Method
			{
				Name = EntryPointMethodName,
				IsSynthetic = true,
				Body = node.Globals,
				ReturnType = CodeBuilder.CreateTypeReference(TypeSystemServices.VoidType),
				Modifiers = TypeMemberModifiers.Static | TypeMemberModifiers.Private,
				LexicalInfo = node.Globals.Statements[0].LexicalInfo,
				EndSourceLocation = node.EndSourceLocation,
				Parameters = { new ParameterDeclaration("argv", new ArrayTypeReference(new SimpleTypeReference("string"))) }
			};
			moduleClass.Members.Add(method);
			node.Globals = null;
			return method;
		}

		private static void MoveModuleMembersToModuleClass(Module node, ClassDefinition moduleClass)
		{
			var removed = 0;
			var members = node.Members.ToArray();
			for (int i=0; i<members.Length; ++i)
			{
				var member = members[i];
				if (member is TypeDefinition) continue;
				node.Members.RemoveAt(i-removed);
				member.Modifiers |= TypeMemberModifiers.Static;
				moduleClass.Members.Add(member);
				++removed;
			}
		}

		private ClassDefinition NewModuleClassFor(Module node)
		{
			var moduleClass = new ClassDefinition(node.LexicalInfo)
          	{
          		IsSynthetic = true,
          		Modifiers = TypeMemberModifiers.Public | TypeMemberModifiers.Final | TypeMemberModifiers.Transient,
          		EndSourceLocation = node.EndSourceLocation,
          		Name = BuildModuleClassName(node)
          	};
			moduleClass.Attributes.Add(CreateBooModuleAttribute());
			return moduleClass;
		}

		private static void InitializeModuleClassEntity(Module node, ClassDefinition moduleClass)
		{
			InternalModule entity = ((InternalModule)node.Entity);
			if (null != entity) entity.InitializeModuleClass(moduleClass);
		}

		static ClassDefinition ExistingModuleClassFor(Module node)
		{
			return node.Members.OfType<ClassDefinition>().Where(IsModuleClass).SingleOrDefault();
		}
		
		Attribute CreateBooModuleAttribute()
		{
			return new Attribute(ModuleAttributeName) { Entity = _booModuleAttributeType };
		}
		
		string BuildModuleClassName(Module module)
		{
			var moduleName = module.Name;
			if (string.IsNullOrEmpty(moduleName))
			{
				module.Name = Context.GetUniqueName("Module");
				return module.Name;
			}

			var className = new StringBuilder();
			if (!(char.IsLetter(moduleName[0]) || moduleName[0]=='_'))
				className.Append('_');

			className.Append(char.ToUpper(moduleName[0]));
			for (int i = 1; i < moduleName.Length; ++i)
			{
				var c = moduleName[i];
				className.Append(char.IsLetterOrDigit(c) ? c : '_');
			}
			return className.Append("Module").ToString();
		}
	}
}

