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
using System.Text;
using Boo.Lang;
using Boo.Lang.Ast;

namespace Boo.Lang.Compiler
{
	/// <summary>
	/// Compiler errors.
	/// </summary>
	public class ErrorCollection : System.Collections.CollectionBase
	{
		public ErrorCollection()
		{
		}

		public Error this[int index]
		{
			get
			{
				return (Error)InnerList[index];
			}
		}

		public void Add(Error error)
		{
			if (null == error)
			{
				throw new ArgumentNullException("error");
			}
			InnerList.Add(error);
		}		
		
		public override string ToString()
		{
			return ToString(false);
		}
		
		public string ToString(bool verbose)
		{
			if (Count > 0)
			{
				System.IO.StringWriter writer = new System.IO.StringWriter();
				foreach (Error error in InnerList)
				{
					writer.WriteLine(error.ToString(verbose));
				}
				return writer.ToString();
			}
			return string.Empty;
		}
		
		public void NotImplemented(Node node, string message)
		{
			throw new Error(node, Format("NotImplemented", message));
		}
		
		public void InvalidSuper(Node node)
		{
			Add(new Error(node, GetString("InvalidSuper")));
		}
		
		public void InvalidTypeof(Node node)
		{
			Add(new Error(node, GetString("InvalidTypeof")));
		}
		
		public void MultipleClassInheritance(Node node, string className, string baseClass)
		{
			Add(new Error(node,
						Format("MultipleClassInheritance",
								className,
								baseClass
								)
							)
				);
		}
		
		public void CantCastToValueType(Node node, string typeName)
		{
			Add(new Error(node, Format("CantCastToValueType", typeName)));
		}
		
		public void NamedParametersNotAllowed(Node node)
		{
			Add(new Error(node, GetString("NamedParametersNotAllowed")));
		}

		public void NamedParameterMustBeReference(ExpressionPair pair)		
		{
			Add(new Error(pair, GetString("NamedParameterMustBeReference")));
		}
		
		public void ExpressionStatementMustHaveSideEffect(ExpressionStatement node)
		{
			Add(new Error(node, GetString("ExpressionStatementMustHaveSideEffect")));
		}
		
		public void EventArgumentMustBeAMethod(Node sourceNode, string name, string signature)
		{
			Add(new Error(sourceNode, Format("EventArgumentMustBeAMethod", name, signature)));
		}
		
		public void TypeNotAttribute(Node node, string name)
		{
			Add(new Error(node, Format("TypeNotAttribute", name)));
		}
		
		public void NameNotType(Node node, string name)
		{
			Add(new Error(node, Format("NameNotType", name)));
		}
		
		public void NoEntryPoint()
		{
			Add(new Error(LexicalInfo.Empty, GetString("NoEntryPoint")));
		}
		
		public void MoreThanOneEntryPoint(Method method)
		{
			Add(new Error(method, Format("MoreThanOneEntryPoint")));
		}
		
		public void MemberNeedsInstance(Expression node, string member)
		{
			Add(new Error(node, Format("MemberNeedsInstance", member)));
		}
		
		public void MemberNotFound(MemberReferenceExpression node, string targetBindingName)
		{
			Add(new Error(node, Format("MemberNotFound", node.Name, targetBindingName)));
		}
		
		public void BoolExpressionRequired(Expression node, string actualType)
		{
			Add(new Error(node, Format("BoolExpressionRequired", actualType)));
		}
		
		public void NoApropriateOverloadFound(Node node, string args, string name)
		{
			Add(new Error(node, Format("NoApropriateOverloadFound", args, name)));
		}
		
		public void NoApropriateConstructorFound(Node node, string typeName, string args)
		{
			Add(new Error(node, Format("NoApropriateConstructorFound", typeName, args)));
		}
		
		public void InternalMacro(Node node, string name)
		{
			Add(new Error(node, Format("InternalMacro", name)));
		}
		
		public void UnknownMacro(Node node, string name)
		{
			Add(new Error(node, Format("UnknownMacro", name)));
		}
		
		public void InvalidMacro(Node node, string name)
		{
			Add(new Error(node, Format("InvalidMacro", name)));
		}
		
		public void UnknownName(Node node, string name)
		{
			Error error = new Error(node, Format("UnknownName", name));			
			Add(error);
		}
		
		public void AmbiguousTypeReference(Node node, List types)
		{
			Add(new Error(node, Format("AmbiguousTypeReference", ToAssemblyQualifiedNameList(types))));
		}
		
		public void UnableToLoadAssembly(Node node, string assemblyName, Exception cause)
		{
			Add(new Error(node, Boo.ResourceManager.Format("BooC.UnableToLoadAssembly", assemblyName), cause));
		}
		
		public void InvalidArray(Node node)
		{
			Add(new Error(node, GetString("InvalidArray")));
		}
		
		public void InvalidNamespace(Import node)
		{
			Add(new Error(node, Format("InvalidNamespace", node.Namespace)));
		}

		public void AmbiguousName(Node node, string name, System.Collections.IEnumerable resolvedNames)
		{
			string msg = Format("AmbiguousName", name, ToStringList(resolvedNames));
			Add(new Error(node, msg));
		}

		public void AmbiguousName(Node node, string name, System.Reflection.MemberInfo[] resolvedNames)
		{
			string msg = Format("AmbiguousName", name, ToNameList(resolvedNames));
			Add(new Error(node, msg));
		}

		public void NotAPublicFieldOrProperty(Node node, string typeName, string name)
		{
			string msg = Format("NotAPublicFieldOrProperty", name, typeName);
			Add(new Error(node, msg));
		}

		public void MissingConstructor(Node node, Type type, object[] parameters, Exception cause)
		{
			string msg = Format("MissingConstructor", type, GetSignature(parameters));
			Add(new Error(node, msg, cause));
		}
		
		public void MethodArgumentCount(Node sourceNode, Bindings.IMethodBinding binding, int actualArgumentCount)
		{
			Add(new Error(sourceNode, Format("MethodArgumentCount", binding, actualArgumentCount)));
		}
		
		public void MethodSignature(Node node, string actualSignature, string expectedSignature)
		{
			Add(new Error(node, Format("MethodSignature", actualSignature, expectedSignature)));
		}

		public void AttributeResolution(Boo.Lang.Ast.Attribute attribute, Type type, Exception cause)
		{
			string msg = Format("AttributeResolution", type, cause.Message);
			Add(new Error(attribute, msg, cause));
		}
		
		public void AstAttributeMustBeExternal(Boo.Lang.Ast.Attribute attribute, Bindings.ITypeBinding resolvedType)
		{
			Add(new Error(attribute, Format("AstAttributeMustBeExternal", resolvedType.FullName)));
		}
		
		public void IncompatibleExpressionType(Node node, string expected, string actual)
		{
			Add(new Error(node, Format("IncompatibleExpressionType", expected, actual)));
		}

		public void StepExecution(ICompilerComponent step, Exception cause)
		{
			string msg = Format("StepExecution", step.GetType(), cause.Message);
			Add(new Error(LexicalInfo.Empty, msg, cause));
		}

		public void InputError(ICompilerInput input, Exception error)
		{
			Add(new Error(LexicalInfo.Empty, error.Message, error));
		}
		
		string ToStringList(System.Collections.IEnumerable names)
		{
			StringBuilder builder = new StringBuilder();
			foreach (object name in names)
			{
				if (builder.Length > 0)
				{
					builder.Append(", ");
				}
				builder.Append(name.ToString());
			}
			return builder.ToString();
		}
		
		string ToAssemblyQualifiedNameList(List types)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(((Type)types[0]).AssemblyQualifiedName);			
			for (int i=1; i<types.Count; ++i)
			{
				builder.Append(", ");
				builder.Append(((Type)types[i]).AssemblyQualifiedName);
			}
			return builder.ToString();
		}
		
		static string GetSignature(object[] parameters)
		{
			StringBuilder sb = new StringBuilder("(");
			for (int i=0; i<parameters.Length; ++i)
			{
				if (i>0) { sb.Append(", "); }
				if (null != parameters)
				{
					sb.Append(parameters[i].GetType());
				}
			}
			sb.Append(")");
			return sb.ToString();
		}		

		static string ToNameList(System.Reflection.MemberInfo[] members)
		{
			StringBuilder sb = new StringBuilder();
			for (int i=0; i<members.Length; ++i)
			{
				if (i>0) { sb.Append(", "); }
				sb.Append(members[i].MemberType.ToString());
				sb.Append(" ");
				sb.Append(members[i].Name);
			}
			return sb.ToString();
		}
		
		static string Format(string name, params object[] args)
		{
			return Boo.ResourceManager.Format(name, args);
		}

		static string GetString(string name)
		{
			return Boo.ResourceManager.GetString(name);
		}
	}
}
