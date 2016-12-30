using System;

namespace Boo.Lang
{
	public enum TypeInferenceRules
	{	
		/// <summary>
		/// (typeof(T)) as T
		/// </summary>
		TypeReferencedByFirstArgument,

		/// <summary>
		/// (, typeof(T)) as T
		/// </summary>
		TypeReferencedBySecondArgument,

		/// <summary>
		/// (typeof(T)) as (T)
		/// </summary>
		ArrayOfTypeReferencedByFirstArgument,

		/// <summary>
		/// (T) as T
		/// </summary>
		TypeOfFirstArgument,
	}

	/// <summary>
	/// Adds a special type inference rule to a method.
	/// 
	/// See Boo.Lang.Compiler.TypeSystem.Services.InvocationTypeInferenceRules.BuiltinRules.
	/// </summary>
#if !NO_SERIALIZATION_INFO
	[Serializable]
#endif
	[AttributeUsage(AttributeTargets.Method)]
	public class TypeInferenceRuleAttribute : Attribute
	{
		private readonly string _rule;

		public TypeInferenceRuleAttribute(TypeInferenceRules rule) : this(rule.ToString())
		{	
		}

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