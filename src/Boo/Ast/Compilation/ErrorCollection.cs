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

		public void NamedParameterMustBeReference(ExpressionPair pair)		
		{
			Add(new Error(pair, GetString("NamedParameterMustBeReference")));
		}
		
		public void NameNotType(Node node, string name)
		{
			Add(new Error(node, Format("NameNotType", name)));
		}
		
		public void UnknownName(Node node, string name)
		{
			Error error = new Error(node, Format("UnknownName", name));			
			Add(error);
		}

		public void AmbiguousName(Node node, string name, List resolvedNames)
		{
			string msg = Format("AmbiguousName", name, resolvedNames);
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
		
		public void MethodArgumentCount(MethodInvocationExpression mie, NameBinding.IMethodBinding method)
		{
			Add(new Error(mie, Format("MethodArgumentCount", method, mie.Arguments.Count)));
		}

		public void AttributeResolution(Attribute attribute, Type type, Exception cause)
		{
			string msg = Format("AttributeResolution", type, cause.Message);
			Add(new Error(attribute, msg, cause));
		}

		public void StepExecution(ICompilerStep step, Exception cause)
		{
			string msg = Format("StepExecution", step.GetType(), cause.Message);
			Add(new Error(LexicalInfo.Empty, msg, cause));
		}

		public void InputError(ICompilerInput input, Exception error)
		{
			Add(new Error(new LexicalInfo(input.Name, -1, -1), error.Message, error));
		}

		public void ParserError(antlr.RecognitionException error)
		{
			LexicalInfo data = new LexicalInfo(error.getFilename(), error.getLine(), error.getColumn());

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
