"""
using System.IO

fname as string
try:
	fname = prompt('select a file: ')
	raise 'you must select a file!' unless fname
except x as ApplicationException:
	print(x.Message)
	retry
success:
	f as TextReader
	try:
		f = File.OpenText(fname)
		print(f.ReadLine())
	except x as IOException:
		print(string.Format('couldn't open the file {0}: {1}!', (fname, x.Message)))
	ensure:
		f.Close() if f

"""

using System.IO

fname as string
try:
	fname = prompt("select a file: ")
	raise "you must select a file!" unless fname	
except x as ApplicationException:
	print(x.Message)
	retry
success:
	f as TextReader
	try:
		f = File.OpenText(fname)
		print(f.ReadLine())
	except x as IOException:
		print("couldn't open the file ${fname}: ${x.Message}!")
	ensure:
		f.Close() if f
