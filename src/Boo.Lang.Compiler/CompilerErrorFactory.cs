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

namespace Boo.Lang.Compiler
{
	using System;
	using System.Text;
	using Boo.Lang.Compiler.Ast;
	
	public class CompilerErrorFactory
	{
		private CompilerErrorFactory()
		{
		}
		
		public static CompilerError ClassAlreadyHasBaseType(Node node, string className, string baseType)
		{
			return new CompilerError("BCE0001", node.LexicalInfo, className, baseType);
		}
		
		public static CompilerError NamedParameterMustBeIdentifier(ExpressionPair pair)
		{
			return new CompilerError("BCE0002", pair.First.LexicalInfo);
		}
		
		public static CompilerError NamedArgumentsNotAllowed(Node node)
		{
			return new CompilerError("BCE0003", node.LexicalInfo);
		}
		
		public static CompilerError AmbiguousReference(ReferenceExpression reference, System.Reflection.MemberInfo[] members)
		{
			return new CompilerError("BCE0004",
									  reference.LexicalInfo,
									  reference.Name,
									  ToNameList(members)
									  );
		}
		
		public static CompilerError AmbiguousReference(Node node, string name, System.Collections.IEnumerable names)
		{
			return new CompilerError("BCE0004", node.LexicalInfo, name, ToStringList(names));
		}
		
		public static CompilerError UnknownIdentifier(Node node, string name)
		{
			return new CompilerError("BCE0005", node.LexicalInfo, name);
		}
		
		public static CompilerError CantCastToValueType(Node node, string typeName)
		{
			return new CompilerError("BCE0006", node.LexicalInfo, typeName);
		}

		public static CompilerError NotAPublicFieldOrProperty(Node node, string name, string typeName)
		{
			return new CompilerError("BCE0007", node.LexicalInfo, typeName, name);
		}
		
		public static CompilerError MissingConstructor(Exception error, Node node, Type type, object[] parameters)
		{
			return new CompilerError("BCE0008", node.LexicalInfo, error, type, GetSignature(parameters));
		}
		
		public static CompilerError AttributeApplicationError(Exception error, Boo.Lang.Compiler.Ast.Attribute attribute, Type attributeType)
		{
			return new CompilerError("BCE0009",
					                  attribute.LexicalInfo,
					                  error,
					                  attributeType,
					                  error.Message);
		}
		
		public static CompilerError AstAttributeMustBeExternal(Node node, string attributeType)
		{
			return new CompilerError("BCE0010", node.LexicalInfo, attributeType);
		}		
		
		public static CompilerError StepExecutionError(Exception error, ICompilerStep step)
		{
			return new CompilerError("BCE0011", error, step, error.Message);
		}
		
		public static CompilerError TypeMustImplementICompilerStep(string typeName)
		{
			return new CompilerError("BCE0012", LexicalInfo.Empty, typeName);
		}
		
		public static CompilerError AttributeNotFound(string elementName, string attributeName)
		{
			return new CompilerError("BCE0013", LexicalInfo.Empty, elementName, attributeName);
		}
		
		public static CompilerError InvalidAssemblySetUp(Node node)
		{
			return new CompilerError("BCE0014", node.LexicalInfo);
		}
		
		public static CompilerError NodeNotBound(Node node)
		{
			return new CompilerError("BCE0015", node.LexicalInfo, node);
		}
		
		public static CompilerError MethodArgumentCount(Node node, string name, int count)
		{
			return new CompilerError("BCE0016", node.LexicalInfo, name, count);
		}
		
		public static CompilerError MethodSignature(Node node, string expectedSignature, string actualSignature)
		{
			return new CompilerError("BCE0017", node.LexicalInfo, expectedSignature, actualSignature);
		}
		
		public static CompilerError NameNotType(Node node, string name)
		{
			return new CompilerError("BCE0018", node.LexicalInfo, name);
		}
		
		public static CompilerError MemberNotFound(MemberReferenceExpression node, string namespace_)
		{
			return new CompilerError("BCE0019", node.LexicalInfo, node.Name, namespace_);
		}
		
		public static CompilerError MemberNeedsInstance(Node node, string memberName)
		{
			return new CompilerError("BCE0020", node.LexicalInfo, memberName);
		}
		
		public static CompilerError InvalidNamespace(Import import)
		{
			return new CompilerError("BCE0021", import.LexicalInfo, import.Namespace);
		}
		
		public static CompilerError IncompatibleExpressionType(Node node, string expectedType, string actualType)
		{
			return new CompilerError("BCE0022", node.LexicalInfo, expectedType, actualType);
		}
		
		public static CompilerError NoApropriateOverloadFound(Node node, string signature, string memberName)
		{
			return new CompilerError("BCE0023", node.LexicalInfo, signature, memberName);
		}
		
		public static CompilerError NoApropriateConstructorFound(Node node, string typeName, string signature)
		{
			return new CompilerError("BCE0024", node.LexicalInfo, typeName, signature);
		}
		
		public static CompilerError InvalidArray(Node node)
		{
			return new CompilerError("BCE0025", node.LexicalInfo);
		}
		
		public static CompilerError BoolExpressionRequired(Node node, string typeName)
		{
			return new CompilerError("BCE0026", node.LexicalInfo, typeName);
		}
		
		public static CompilerError NoEntryPoint()
		{
			return new CompilerError("BCE0028", LexicalInfo.Empty);
		}
		
		public static CompilerError MoreThanOneEntryPoint(Method method)
		{
			return new CompilerError("BCE0029", method.LexicalInfo);
		}
		
		public static CompilerError NotImplemented(Node node, string message)
		{
			return new CompilerError("BCE0031", node.LexicalInfo, message);
		}
		
		public static CompilerError EventArgumentMustBeAMethod(Node node, string eventName, string eventType)
		{
			return new CompilerError("BCE0032", node.LexicalInfo, eventName, eventType);
		}
		
		public static CompilerError TypeNotAttribute(Node node, string attributeType)
		{
			return new CompilerError("BCE0033", node.LexicalInfo, attributeType);
		}
		
		public static CompilerError ExpressionStatementMustHaveSideEffect(Node node)
		{
			return new CompilerError("BCE0034", node.LexicalInfo);
		}
		
		public static CompilerError InvalidTypeof(Node node)
		{
			return new CompilerError("BCE0036", node.LexicalInfo);
		}
		
		public static CompilerError UnknownMacro(Node node, string name)
		{
			return new CompilerError("BCE0037", node.LexicalInfo, name);
		}
		
		public static CompilerError InvalidMacro(Node node, string name)
		{
			return new CompilerError("BCE0038", node.LexicalInfo, name);
		}
		
		public static CompilerError AstMacroMustBeExternal(Node node, string typeName)
		{
			return new CompilerError("BCE0039", node.LexicalInfo, typeName);
		}
		
		public static CompilerError UnableToLoadAssembly(Node node, string name, Exception error)
		{
			return new CompilerError("BCE0041", node.LexicalInfo, error, name);
		}
		
		public static CompilerError InputError(string inputName, Exception error)
		{
			return new CompilerError("BCE0042", new LexicalInfo(inputName), error, inputName, error.Message);
		}
		
		public static CompilerError UnexpectedToken(LexicalInfo lexicalInfo, Exception error, string token)
		{
			return new CompilerError("BCE0043", lexicalInfo, error, token);
		}
		
		public static CompilerError GenericParserError(LexicalInfo lexicalInfo, Exception error)
		{
			return new CompilerError("BCE0044", lexicalInfo, error, error.Message);
		}
		
		public static CompilerError MacroExpansionError(Node node, Exception error)
		{
			return new CompilerError("BCE0045", node.LexicalInfo, error, error.Message);
		}
		
		public static CompilerError OperatorCantBeUsedWithValueType(Node node, string operatorName, string typeName)
		{
			return new CompilerError("BCE0046", node.LexicalInfo, operatorName, typeName);
		}
		
		public static CompilerError CantOverrideNonVirtual(Node node, string fullName)
		{
			return new CompilerError("BCE0047", node.LexicalInfo, fullName);
		}
		
		public static CompilerError TypeDoesNotSupportSlicing(Node node, string fullName)
		{
			return new CompilerError("BCE0048", node.LexicalInfo, fullName);
		}
		
		public static CompilerError LValueExpected(Node node)
		{
			return new CompilerError("BCE0049", node.LexicalInfo);
		}
		
		public static CompilerError InvalidOperatorForType(Node node, string operatorName, string typeName)
		{
			return new CompilerError("BCE0050", node.LexicalInfo, operatorName, typeName);
		}
		
		public static CompilerError InvalidOperatorForTypes(Node node, string operatorName, string lhs, string rhs)
		{
			return new CompilerError("BCE0051", node.LexicalInfo, operatorName, lhs, rhs);
		}
		
		public static CompilerError InvalidLen(Node node, string typeName)
		{
			return new CompilerError("BCE0052", node.LexicalInfo, typeName);
		}
		
		public static CompilerError PropertyIsReadOnly(Node node, string propertyName)
		{
			return new CompilerError("BCE0053", node.LexicalInfo, propertyName);
		}
		
		public static CompilerError IsaArgument(Node node)
		{
			return new CompilerError("BCE0054", node.LexicalInfo);
		}
		
		public static CompilerError InternalError(Node node, Exception error)
		{
			return new CompilerError("BCE0055", node.LexicalInfo, error, error.Message);
		}
		
		public static CompilerError FileNotFound(string fname)
		{
			return new CompilerError("BCE0056", new LexicalInfo(fname), fname);
		}
		
		public static CompilerError CantRedefinePrimitive(Node node, string name)
		{
			return new CompilerError("BCE0057", node.LexicalInfo, name);
		}
		
		public static CompilerError ObjectRequired(Node node)
		{
			return new CompilerError("BCE0058", node.LexicalInfo);
		}
		
		public static CompilerError InvalidLockMacroArguments(Node node)
		{
			return new CompilerError("BCE0059", node.LexicalInfo);
		}
		
		public static CompilerError NoMethodToOverride(Node node, string signature)
		{
			return new CompilerError("BCE0060", node.LexicalInfo, signature);
		}
		
		public static CompilerError MethodIsNotOverride(Node node, string signature)
		{
			return new CompilerError("BCE0061", node.LexicalInfo, signature);
		}
		
		public static CompilerError CouldNotInferReturnType(Node node, string signature)
		{
			return new CompilerError("BCE0062", node.LexicalInfo, signature);
		}
		
		public static CompilerError NoEnclosingLoop(Node node)
		{
			return new CompilerError("BCE0063", node.LexicalInfo);
		}
		
		public static CompilerError UnknownAttribute(Node node, string attributeName)
		{
			return new CompilerError("BCE0064", node.LexicalInfo, attributeName);
		}
		
		public static CompilerError InvalidIteratorType(Node node, string typeName)
		{
			return new CompilerError("BCE0065", node.LexicalInfo, typeName);
		}
		
		public static CompilerError InvalidNodeForAttribute(LexicalInfo info, string attributeName, string expectedNodeTypes)
		{
			return new CompilerError("BCE0066", info, attributeName, expectedNodeTypes);
		}
		
		public static CompilerError LocalAlreadyExists(Node node, string name)
		{
			return new CompilerError("BCE0067", node.LexicalInfo, name);
		}
		
		public static CompilerError PropertyRequiresParameters(Node node, string name)
		{
			return new CompilerError("BCE0068", node.LexicalInfo, name);
		}
		
		public static CompilerError InterfaceCanOnlyInheritFromInterface(Node node, string interfaceName, string baseType)
		{
			return new CompilerError("BCE0069", node.LexicalInfo, interfaceName, baseType);
		}
		
		public static CompilerError RecursiveMethodWithoutReturnType(Node node)
		{
			return new CompilerError("BCE0070", node.LexicalInfo);
		}
		
		public static CompilerError InheritanceCycle(Node node, string typeName)
		{
			return new CompilerError("BCE0071", node.LexicalInfo, typeName);
		}
		
		public static CompilerError InvalidOverrideReturnType(Node node, string methodName, string expectedReturnType, string actualReturnType)
		{
			return new CompilerError("BCE0072", node.LexicalInfo, methodName, expectedReturnType, actualReturnType);
		}
		
		public static CompilerError AbstractMethodCantHaveBody(Node node, string methodName)
		{
			return new CompilerError("BCE0073", node.LexicalInfo, methodName);
		}
		
		public static CompilerError SelfOutsideMethod(Node node)
		{
			return new CompilerError("BCE0074", node.LexicalInfo);
		}
		
		public static string ToStringList(System.Collections.IEnumerable names)
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
		
		public static string ToAssemblyQualifiedNameList(List types)
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
		
		public static string GetSignature(object[] parameters)
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

		public static string ToNameList(System.Reflection.MemberInfo[] members)
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
	}
}
