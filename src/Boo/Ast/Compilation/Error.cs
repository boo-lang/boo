using System;
using System.Text;
using Boo.Ast;

namespace Boo.Ast.Compilation
{
	/// <summary>
	/// Representa um erro de compilao.
	/// </summary>
	[Serializable]
	public class Error : ApplicationException
	{
		LexicalInfo _ldata;

		public Error(LexicalInfo data, string message, Exception cause) : base(message, cause)
		{
			if (null == data)
			{
				throw new ArgumentNullException("data");
			}

			if (null == message)
			{
				throw new ArgumentNullException("message");
			}

			_ldata = data;
		}

		public Error(Node node, string message, Exception cause) : this(node.LexicalInfo, message, cause)
		{
		}

		public Error(Node node, string message) : this(node, message, null)
		{
		}

		public Error(LexicalInfo data, string message) : this(data, message, null)
		{
		}

		public Error(LexicalInfo data, Exception cause) : this(data, cause.Message, cause)
		{
		}

		public LexicalInfo LexicalInfo
		{
			get
			{
				return _ldata;
			}
		}

		public override string ToString()
		{
			return ToString(false);
		}

		public string ToString(bool verbose)
		{
			StringBuilder sb = new StringBuilder();
			if (_ldata.Line > 0)
			{
				sb.Append(_ldata);
				sb.Append(": ");
			}
			if (verbose)
			{
				sb.Append(base.ToString());
			}
			else
			{
				sb.Append(Message);			
			}
			return sb.ToString();
		}
	}
}
