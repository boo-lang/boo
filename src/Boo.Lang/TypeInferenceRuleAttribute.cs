using System;

namespace Boo.Lang
{
	/// <summary>
	/// Adds a special type inference rule to a method.
	/// 
	/// See Boo.Lang.Compiler.TypeSystem.Services.InvocationTypeInferenceRules.BuiltinRules.
	/// </summary>
	[Serializable]
	[AttributeUsage(AttributeTargets.Method)]
	public class TypeInferenceRuleAttribute : Attribute
	{
		private readonly string _rule;

		public TypeInferenceRuleAttribute(string rule)
		{
			_rule = rule;
		}

		public override string ToString()
		{
			return _rule;
		}
	}
}