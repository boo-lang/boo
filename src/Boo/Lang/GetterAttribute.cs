using System;
using Boo.Ast;

namespace Boo.Lang
{
	/// <summary>
	/// Cria um acessador para um campo.
	/// </summary>
	/// <example>
	/// <pre>
	/// class Customer:
	///		[getter(FirstName)] _fname as string
	///		[getter(LastName)] _lname as string
	/// </pre>
	/// </example>
	public class GetterAttribute : AstAttribute
	{
		ReferenceExpression _propertyName;

		public GetterAttribute(ReferenceExpression propertyName)
		{
			if (null == propertyName)
			{
				throw new ArgumentNullException("propertyName");
			}
			_propertyName = propertyName;
		}

		public override void Apply(Node node)
		{
			Field f = node as Field;
			if (null == f)
			{
				throw new ApplicationException(ResourceManager.Format("InvalidNodeForAttribute", "Field"));
			}

			Property p = new Property();
			p.Name = _propertyName.Name;
			p.Type = f.Type;

			// get:
			//		return <f.Name>
			Method getter = new Method();
			getter.Name = "get";
			getter.Body.Statements.Add(
				new ReturnStatement(
					new ReferenceExpression(f.Name)
					)
				);

			p.Getter = getter;
			p.LexicalInfo = LexicalInfo;
			((TypeDefinition)f.ParentNode).Members.Add(p);
		}
	}
}
