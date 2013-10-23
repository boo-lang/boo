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

using System.Text;
using antlr;

namespace Boo.Lang.Parser
{
	abstract public class ErrorPattern
	{
		public string Message { get; protected set; }
		public string Rule { get; protected set; }

		public ErrorPattern(string message, string rule)
		{
			Message = message;
			Rule = rule;
		}

		abstract public bool Matches(string rule, RecognitionException ex);

		virtual public string ToCodeString()
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("new ")
				   .Append(GetType().FullName)
				   .AppendLine("(");

			builder.Append("    ")
				   .AppendLine("\"" + Message + "\",");
			builder.Append("    ")
				   .AppendLine("\"" + Rule + "\"");

			builder.Append(")");
			return builder.ToString();
		}

	}

	public class RecognitionErrorPattern : ErrorPattern
	{
		public RecognitionErrorPattern(string message, string rule) : base(message, rule)
		{
		}

		override public bool Matches(string rule, RecognitionException ex)
		{
			if (rule != Rule)
				return false;

			return true;
		}

	}

	public class MismatchedErrorPattern : RecognitionErrorPattern
	{
		public int Token { get; set; }

		public MismatchedErrorPattern(string message, string rule) : base(message, rule)
		{
		}

		public MismatchedErrorPattern(string message, string rule, int token) : base(message, rule)
		{
			Token = token;
		}

		override public bool Matches(string rule, RecognitionException ex)
		{
			if (!base.Matches(rule, ex))
				return false;

			MismatchedTokenException mmt = ex as MismatchedTokenException;
			if (mmt == null)
				return false;

			return Token == mmt.expecting;
		}

		override public string ToCodeString()
		{
			return base.ToCodeString() + " { Token = " + Token + " }";
		}		
	}

	public class NoViableAltErrorPattern : RecognitionErrorPattern
	{
		public int Token { get; set; }

		public NoViableAltErrorPattern(string message, string state) : base(message, state)
		{
		}

		public NoViableAltErrorPattern(string message, string state, int token) : this(message, state)
		{
			Token = token;
		}

		override public bool Matches(string rule, RecognitionException ex)
		{
			if (!base.Matches(rule, ex))
				return false;

			NoViableAltException nva = ex as NoViableAltException;
			if (nva == null)
				return false;

			return Token == nva.token.Type;
		}

		override public string ToCodeString()
		{
			return base.ToCodeString() + " { Token = " + Token + " }";
		}
	}

}
