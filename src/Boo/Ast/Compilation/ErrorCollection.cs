using System;
using System.Text;
using Boo.Lang;

namespace Boo.Ast.Compilation
{
	/// <summary>
	/// Compilation errors.
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
		
		public void NamedParametersNotAllowed(Node node)
		{
			Add(new Error(node, GetString("NamedParametersNotAllowed")));
		}

		public void NamedParameterMustBeReference(ExpressionPair pair)		
		{
			Add(new Error(pair, GetString("NamedParameterMustBeReference")));
		}
		
		public void NameNotType(Node node, string name)
		{
			Add(new Error(node, Format("NameNotType", name)));
		}
		
		public void NoEntryPoint(Module module)
		{
			Add(new Error(module, Format("NoEntryPoint", module.Name)));
		}
		
		public void MemberNeedsInstance(Expression node, string member)
		{
			Add(new Error(node, Format("MemberNeedsInstance", member)));
		}
		
		public void MemberNotFound(MemberReferenceExpression node, string targetBindingName)
		{
			Add(new Error(node, Format("MemberNotFound", node.Name, targetBindingName)));
		}
		
		public void BoolExpressionRequired(Expression node, Type type)
		{
			Add(new Error(node, Format("BoolExpressionRequired", type)));
		}
		
		public void NoApropriateOverloadFound(Node node, string args, string name)
		{
			Add(new Error(node, Format("NoApropriateOverloadFound", args, name)));
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
		
		public void InvalidNamespace(Using node)
		{
			Add(new Error(node, Format("InvalidNamespace", node.Namespace)));
		}

		public void AmbiguousName(Node node, string name, System.Collections.IList resolvedNames)
		{
			string msg = Format("AmbiguousName", name, ToStringList(resolvedNames));
			Add(new Error(node, msg));
		}

		public void AmbiguousName(Node node, string name, System.Reflection.MemberInfo[] resolvedNames)
		{
			string msg = Format("AmbiguousName", name, ToNameList(resolvedNames));
			Add(new Error(node, msg));
		}

		public void NotAPublicFieldOrProperty(Node node, Type type, string name)
		{
			string msg = Format("NotAPublicFieldOrProperty", name, type);
			Add(new Error(node, msg));
		}

		public void MissingConstructor(Node node, Type type, object[] parameters, Exception cause)
		{
			string msg = Format("MissingConstructor", type, GetSignature(parameters));
			Add(new Error(node, msg, cause));
		}
		
		public void MethodArgumentCount(Node sourceNode, Binding.IMethodBinding binding, int actualArgumentCount)
		{
			Add(new Error(sourceNode, Format("MethodArgumentCount", binding, actualArgumentCount)));
		}
		
		public void MethodSignature(Node node, string actualSignature, string expectedSignature)
		{
			Add(new Error(node, Format("MethodSignature", actualSignature, expectedSignature)));
		}

		public void AttributeResolution(Attribute attribute, Type type, Exception cause)
		{
			string msg = Format("AttributeResolution", type, cause.Message);
			Add(new Error(attribute, msg, cause));
		}
		
		public void IncompatibleExpressionType(Node node, Type expected, Type actual)
		{
			Add(new Error(node, Format("IncompatibleExpressionType", expected, actual)));
		}

		public void StepExecution(ICompilerStep step, Exception cause)
		{
			string msg = Format("StepExecution", step.GetType(), cause.Message);
			Add(new Error(LexicalInfo.Empty, msg, cause));
		}

		public void InputError(ICompilerInput input, Exception error)
		{
			Add(new Error(LexicalInfo.Empty, error.Message, error));
		}

		public void ParserError(antlr.RecognitionException error)
		{
			LexicalInfo data = new LexicalInfo(error.getFilename(), error.getLine(), error.getColumn(), error.getColumn());

			antlr.NoViableAltException nvae = error as antlr.NoViableAltException;
			if (null != nvae)
			{
				ParserError(data, nvae);
			}
			else
			{
				Add(new Error(data, error.Message, error));
			}
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

		void ParserError(LexicalInfo data, antlr.NoViableAltException error)
		{
			string msg = Format("NoViableAltException", error.token.getText());
			Add(new Error(data, msg, error));
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
