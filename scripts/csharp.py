def toPrivateName(name):
	return "_" + name[0].lower() + name[1:]
	
def toLocalName(name):
	return checkReservedWord(name[0].lower() + name[1:])
	
def checkReservedWord(word):
	if word in _reserved or word in _primitives:
		return word + "_"
	return word
	
def isCollection(node):
	if node:
		if hasattr(node, "stereotype"):
			if "collection" == node.stereotype:
				return True
	return False
	
def isCollectionField(field):
	if field:
		return isCollection(field.model.resolve(field.type))
	return False
	
def isNotCollectionField(field):
	return not isCollectionField(field)
	
def getNonAutoFields(fields):
	for field in fields:
		if isNotCollectionField(field) and not "auto" in field.attributes:
			yield field
			
def resolveBaseTypes(item):
	for baseTypeName in item.baseTypes:
		baseType = item.model.resolve(baseTypeName)
		if baseType:
			for baseBaseTypes in resolveBaseTypes(baseType):
				yield baseBaseTypes
			yield baseType
			
def getAllFields(item):
	"""yields all fields including inherited fields"""	
	for field in item.fields:
		yield field
	for baseType in resolveBaseTypes(item):
		for field in baseType.fields:
			yield field
			
def getSwitchableFields(fields):
	for field in fields:
		fieldType = field.model.resolve(field.type)
		if fieldType:
			if "collection" == fieldType.stereotype or "" == fieldType.stereotype:
				yield field
	
def getDerivedTypes(item):	
	for c in item.model:
		if item.name in c.baseTypes:
			yield c.name

def isPrimitive(typeName):
	return typeName in _primitives
	
_primitives = { "int" : 1, "bool" : 1, "string" : 1, "float" : 1, "double" : 1 }
_reserved = { "operator" : 1, "namespace" : 1 }
