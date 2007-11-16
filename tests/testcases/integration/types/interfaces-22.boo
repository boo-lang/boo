import System
import System.Data
import System.Data.SqlClient

sql = SqlCommand() as IDbCommand

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