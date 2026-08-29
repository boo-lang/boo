import System
import System.Data
import Microsoft.Data.Sqlite from Microsoft.Data.Sqlite

sql = SqliteCommand() as IDbCommand

try:
	reader = sql.ExecuteReader()
except as InvalidOperationException:
	caught = true
	
assert caught

try:
	print reader[0]
except as NullReferenceException:
	caughtAgain = true

assert caughtAgain