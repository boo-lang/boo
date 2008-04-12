"""
<html>
<body>
Hello, world!
</body>
</html>
"""

callable Block()

def blockTag(tagName as string, block as Block):
	print "<${tagName}>"
	block()
	print "</${tagName}>"

def html(block as Block):
	blockTag "html", block

def body(block as Block):
	blockTag "body", block

def text(s as string):
	print s

html:
	body:
		text "Hello, world!"

