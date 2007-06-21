"""
Statement.Select(variable, variables)
"""
class Database:
	def Select(variable as string, *variables as (string)):
		return Statement.Select(self, variable, *variables)

class Statement:
	private def constructor(database as Database):
		_database = database
		
	static def Select(database as Database, variable as string, *variables as (string)) as Statement:
		return Statement(database).Select(variable, *variables)
		
	def Select(variable as string, *variables as (string)) as Statement:
		print "Statement.Select(variable, variables)"
		return self

	_database as Database

Database().Select("variable", "1", "2", "3")
