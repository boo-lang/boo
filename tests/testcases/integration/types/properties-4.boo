class Buffer:
	
	[property(Data)]
	_data = ""
	
b = Buffer()
b.Data += "it works!"
b.Data += " it really does!"

assert "it works! it really does!" == b.Data
