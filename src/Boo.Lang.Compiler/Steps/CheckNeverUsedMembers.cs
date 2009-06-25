#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;

	public class CheckNeverUsedMembers : AbstractTransformerCompilerStep
	{

		override public void Run()
		{
			Visit(CompileUnit.Modules);
		}

		public override void LeaveModule(Boo.Lang.Compiler.Ast.Module module)
		{
			if (module.ContainsAnnotation("merged-module"))
				return;

			foreach (Import import in module.Imports)
			{
				//do not be pedantic about System, the corlib is to be ref'ed anyway
				if (ImportAnnotations.IsUsedImport(import) || import.Namespace == "System")
					continue;
				if (null == module.EnclosingNamespace
					|| module.EnclosingNamespace.Name != import.Namespace)
				{
					Warnings.Add(
						CompilerWarningFactory.NamespaceNeverUsed(import) );
				}
			}
		}

		override public bool EnterClassDefinition(ClassDefinition node)
		{
			CheckMembers(node);
			return false;
		}

		override public bool EnterInterfaceDefinition(InterfaceDefinition node)
		{
			return false;
		}

		override public bool EnterStructDefinition(StructDefinition node)
		{
			return false;
		}

		protected void CheckMembers(ClassDefinition node)
		{
			foreach (TypeMember member in node.Members)
			{
				WarnIfPrivateMemberNeverUsed(member);
				WarnIfProtectedMemberInSealedClass(member);
			}
		}

		protected void WarnIfPrivateMemberNeverUsed(TypeMember node)
		{
			if (NodeType.Constructor == node.NodeType && node.IsStatic) return;

			if (!IsVisible(node) && node.ContainsAnnotation("PrivateMemberNeverUsed"))
			{
				Warnings.Add(
					CompilerWarningFactory.PrivateMemberNeverUsed(node) );
			}
		}

		protected void WarnIfProtectedMemberInSealedClass(TypeMember member)
		{
			if (member.IsProtected && !member.IsSynthetic && !member.IsOverride && member.DeclaringType.IsFinal)
			{
				Warnings.Add(CompilerWarningFactory.NewProtectedMemberInSealedType(member));
			}
		}

		private bool IsVisible(TypeMember member)
		{
			return HasInternalsVisibleToAttribute
						? !member.IsPrivate : !(member.IsPrivate || member.IsInternal);
		}

		private bool HasInternalsVisibleToAttribute
		{
			get {
				if (null == hasInternalsVisibleToAttribute)
				{
					//we don't know yet if we have that attribute
					foreach (Module module in CompileUnit.Modules)
					{
						foreach (Attribute attribute in module.AssemblyAttributes)
						{
							IType attrType = ((IMethod) attribute.Entity).DeclaringType;
							if (attrType.FullName == "System.Runtime.CompilerServices.InternalsVisibleToAttribute")
							{
								hasInternalsVisibleToAttribute = true;
								return true;
							}
						}
					}
					hasInternalsVisibleToAttribute = false;
				}
				return hasInternalsVisibleToAttribute.Value;
			}
		}
		bool? hasInternalsVisibleToAttribute;

	}

}
