from codelets.core import *

def isEnum(context):
	return "enum" == context.item.stereotype
	
def isCollection(context):
	return "collection" == context.item.stereotype
	
def isNode(context):
	return "" == context.item.stereotype

project = Project("Boo.Ast")
project.items.extend([
	CheetahCodelet("switcher.tmpl", scope=Scope.Model),
	CheetahCodelet("DepthFirstAdapter.tmpl", scope=Scope.Model),
	CheetahCodelet("transformer.tmpl", scope=Scope.Model),
	CheetahCodelet("DepthFirstTransformer.tmpl", scope=Scope.Model),
	CheetahCodelet("NodeType.tmpl", scope=Scope.Model),	
	CheetahCodelet("enum.tmpl", condition=isEnum),
	CheetahCodelet("collection.tmpl", overwrite=False, condition=isCollection),
	CheetahCodelet("collectionImpl.tmpl", condition=isCollection),
	CheetahCodelet("node.tmpl", overwrite=False, condition=isNode),
	CheetahCodelet("nodeImpl.tmpl", condition=isNode)
	
])

project.main()
