using System;

namespace Boo.Lang.Compiler
{
	public class LanguageAmbiance
	{
		public virtual string SelfKeyword
		{
			get { return "self"; }
		}

		public virtual string IsaKeyword
		{
			get { return "isa"; }
		}

		public virtual string IsKeyword
		{
			get { return "is"; }
		}

		public virtual string TryKeyword
		{
			get { return "try"; }
		}

		public virtual string ExceptKeyword
		{
			get { return "except"; }
		}

		public virtual string FailureKeyword
		{
			get { return "failure"; }
		}

		public virtual string EnsureKeyword
		{
			get { return "ensure"; }
		}

		public virtual string RaiseKeyword
		{
			get { return "raise"; }
		}

		public virtual string DefaultGeneratorTypeFor(string typeName)
		{
			return typeName + "*";
		}
	}
}
