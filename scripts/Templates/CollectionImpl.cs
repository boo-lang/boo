${header}
namespace Boo.Lang.Compiler.Ast
{
	using System;
<%

itemType = "Boo.Lang.Compiler.Ast." + model.GetCollectionItemType(node)

%>	
	[Serializable]
	public partial class ${node.Name} : NodeCollection<${itemType}>
	{
		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		public static ${node.Name} FromArray(params ${itemType}[] items)
		{
			var collection = new ${node.Name}();
			collection.AddRange(items);
			return collection;
		}

		[System.CodeDom.Compiler.GeneratedCodeAttribute("astgen.boo", "1")]
		public ${itemType}Collection PopRange(int begin)
		{
			var range = new ${itemType}Collection(ParentNode);
			range.InnerList.AddRange(InternalPopRange(begin));
			return range;
		}
	}
}

