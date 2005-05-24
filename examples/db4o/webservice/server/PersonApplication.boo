namespace BooWebService.Server

import System.Web
import com.db4o

class PersonApplication(HttpApplication):

	static _server as ObjectServer
	
	static def OpenSession():
		return _server.openClient() 

	def Application_Start():
		# in a real world scenario you would probably want
		# to move person.yap to a less accessible location
		_server = Db4o.openServer("person.yap", 0)
		
	def Application_End():
		_server.close()