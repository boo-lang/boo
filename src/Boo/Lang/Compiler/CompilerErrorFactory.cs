namespace Boo.Lang.Compiler
{
	using System;
	using System.Text;
	using Boo.Lang.Ast;
	
	public class CompilerErrorFactory
	{
		private CompilerErrorFactory()
		{
		}
		
		public static CompilerError ClassAlreadyHasBaseType(Node node, string className, string baseType)
		{
			return new CompilerError("BC0001", node.LexicalInfo, className, baseType);
		}
		
		public static CompilerError NamedParameterMustBeIdentifier(ExpressionPair pair)
		{
			return new CompilerError("BC0002", pair.First.LexicalInfo);
		}
		
		public static CompilerError NamedArgumentsNotAllowed(Node node)
		{
			return new CompilerError("BC0003", node.LexicalInfo);
		}
		
		public static CompilerError AmbiguousReference(ReferenceExpression reference, System.Reflection.MemberInfo[] members)
		{
			return new CompilerError("BC0004",
									  reference.LexicalInfo,
									  reference.Name,
									  ToNameList(members)
									  );
		}
		
		public static CompilerError AmbiguousReference(Node node, string name, System.Collections.IEnumerable names)
		{
			return new CompilerError("BC0004", node.LexicalInfo, name, ToStringList(names));
		}
		
		public static CompilerError UnknownIdentifier(Node node, string name)
		{
			return new CompilerError("BC0005", node.LexicalInfo, name);
		}
		
		public static CompilerError CantCastToValueType(Node node, string typeName)
		{
			return new CompilerError("BC0006", node.LexicalInfo, typeName);
		}

		public static CompilerError NotAPublicFieldOrProperty(Node node, string name, string typeName)
		{
			return new CompilerError("BC0007", node.LexicalInfo, typeName, name);
		}
		
		public static CompilerError MissingConstructor(Exception error, Node node, Type type, object[] parameters)
		{
			return new CompilerError("BC0008", node.LexicalInfo, error, type, GetSignature(parameters));
		}
		
		public static CompilerError AttributeApplicationError(Exception error, Boo.Lang.Ast.Attribute attribute, Type attributeType)
		{
			return new CompilerError("BC0009",
					                  attribute.LexicalInfo,
					                  error,
					                  attributeType,
					                  error.Message);
		}
		
		public static CompilerError AstAttributeMustBeExternal(Node node, string attributeType)
		{
			return new CompilerError("BC0010", node.LexicalInfo, attributeType);
		}		
		
		public static CompilerError StepExecutionError(Exception error, ICompilerStep step)
		{
			return new CompilerError("BC0011", error, step);
		}
		
		public static CompilerError MethodArgumentCount(Node node, string name, int count)
		{
			return new CompilerError("BC0016", node.LexicalInfo, name, count);
		}
		
		public static CompilerError MethodSignature(Node node, string expectedSignature, string actualSignature)
		{
			return new CompilerError("BC0017", node.LexicalInfo, expectedSignature, actualSignature);
		}
		
		public static CompilerError NameNotType(Node node, string name)
		{
			return new CompilerError("BC0018", node.LexicalInfo, name);
		}
		
		public static CompilerError MemberNotFound(MemberReferenceExpression node, string namespace_)
		{
			return new CompilerError("BC0019", node.LexicalInfo, node.Name, namespace_);
		}
		
		public static CompilerError MemberNeedsInstance(Node node, string memberName)
		{
			return new CompilerError("BC0020", node.LexicalInfo, memberName);
		}
		
		public static CompilerError InvalidNamespace(Import import)
		{
			return new CompilerError("BC0021", import.LexicalInfo, import.Namespace);
		}
		
		public static CompilerError IncompatibleExpressionType(Node node, string expectedType, string actualType)
		{
			return new CompilerError("BC0022", node.LexicalInfo, expectedType, actualType);
		}
		
		public static CompilerError NoApropriateOverloadFound(Node node, string signature, string memberName)
		{
			return new CompilerError("BC0023", node.LexicalInfo, signature, memberName);
		}
		
		public static CompilerError NoApropriateConstructorFound(Node node, string typeName, string signature)
		{
			return new CompilerError("BC0024", node.LexicalInfo, typeName, signature);
		}
		
		public static CompilerError InvalidArray(Node node)
		{
			return new CompilerError("BC0025", node.LexicalInfo);
		}
		
		public static CompilerError BoolExpressionRequired(Node node, string typeName)
		{
			return new CompilerError("BC0026", node.LexicalInfo, typeName);
		}
		
		public static CompilerError NoEntryPoint()
		{
			return new CompilerError("BC0028", LexicalInfo.Empty);
		}
		
		public static CompilerError MoreThanOneEntryPoint(Method method)
		{
			return new CompilerError("BC0029", method.LexicalInfo);
		}
		
		public static CompilerError NotImplemented(Node node, string message)
		{
			return new CompilerError("BC0031", node.LexicalInfo, message);
		}
		
		public static CompilerError EventArgumentMustBeAMethod(Node node, string eventName, string eventType)
		{
			return new CompilerError("BC0032", node.LexicalInfo, eventName, eventType);
		}
		
		public static CompilerError TypeNotAttribute(Node node, string attributeType)
		{
			return new CompilerError("BC0033", node.LexicalInfo, attributeType);
		}
		
		public static CompilerError ExpressionStatementMustHaveSideEffect(Node node)
		{
			return new CompilerError("BC0034", node.LexicalInfo);
		}
		
		public static CompilerError InvalidTypeof(Node node)
		{
			return new CompilerError("BC0036", node.LexicalInfo);
		}
		
		public static CompilerError UnknownMacro(Node node, string name)
		{
			return new CompilerError("BC0037", node.LexicalInfo, name);
		}
		
		public static CompilerError InvalidMacro(Node node, string name)
		{
			return new CompilerError("BC0038", node.LexicalInfo, name);
		}
		
		public static CompilerError AstMacroMustBeExternal(Node node, string typeName)
		{
			return new CompilerError("BC0039", node.LexicalInfo, typeName);
		}
		
		public static CompilerError UnableToLoadAssembly(Node node, string name, Exception error)
		{
			return new CompilerError("BC0041", node.LexicalInfo, error, name);
		}
		
		public static CompilerError InputError(string inputName, Exception error)
		{
			return new CompilerError("BC0042", LexicalInfo.Empty, error, inputName, error.Message);
		}
		
		public static CompilerError UnexpectedToken(LexicalInfo lexicalInfo, Exception error, string token)
		{
			return new CompilerError("BC0043", lexicalInfo, error, token);
		}
		
		public static CompilerError GenericParserError(LexicalInfo lexicalInfo, Exception error)
		{
			return new CompilerError("BC0044", lexicalInfo, error, error.Message);
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
