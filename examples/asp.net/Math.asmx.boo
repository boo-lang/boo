import System.Web.Services

[WebService]
class Math:
	
	[WebMethod]
	def Add(a as int, b as int):
		return a+b
		
	[WebMethod]
	def Multiply(a as int, b as int):
		return a*b
