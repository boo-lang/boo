using System.IO

fname as string
try:
	fname = prompt("nome de um arquivo: ")
	raise "Você deve selecionar um arquivo!" unless fname	
catch ApplicationException x:
	print(x.Message)
	retry
success:
	f as TextReader
	try:
		f = File.OpenText(fname)
		print(f.ReadLine())
	catch IOException x:
		print("Não foi possível abrir o arquivo ${fname}: ${x.Message}!")
	ensure:
		f.Close() if f
