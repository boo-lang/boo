class User:
	[getter(Login)]
	_login as string
	
	def constructor(login):
		_login = login
		
l = [User("eric"), User("john"), User("guido")]
assert l[-1] is l.Find({ user as User | return "guido" == user.Login })
assert l[0] is l.Find({ user as User | return "eric" == user.Login })
assert l[-2] is l.Find({ user as User | return "john" == user.Login })
assert l.Find({ return false }) is null
assert l[0] is l.Find({ return true })
