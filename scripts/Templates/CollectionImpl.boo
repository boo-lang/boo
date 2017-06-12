${header}
namespace Boo.Lang.Compiler.Ast

import System
<%

itemType = "Boo.Lang.Compiler.Ast." + model.GetCollectionItemType(node)

%>	
[Serializable]
public partial class ${node.Name} (NodeCollection[of ${itemType}]):

	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public static def FromArray(*items as (${itemType})) as ${node.Name}:
		collection = ${node.Name}()
		collection.AddRange(items)
		return collection

	[System.CodeDom.Compiler.GeneratedCodeAttribute("Boo astgen.boo", "1")]
	public def PopRange(begin as int) as ${itemType}Collection:
		aRange = ${itemType}Collection(ParentNode)
		aRange.InnerList.AddRange(InternalPopRange(begin))
		return aRange
