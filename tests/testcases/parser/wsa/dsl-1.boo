"""
def foo():
	return bar({ return 42 })

def baz():
	a = bar({ return 42 })

doc = XmlBuilder()
doc.html({ doc.body({ doc.text('Hello, world!') }) })
"""
def foo():
	return bar: // dsl friendly invocation syntax
		return 42
	end
end
		
def baz():
	a = bar:
		return 42
	end
end
		
doc = XmlBuilder()
doc.html:
	doc.body:
		doc.text("Hello, world!")
	end
end
