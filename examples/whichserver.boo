import System.Net

using response=WebRequest.Create(argv[0]).GetResponse():
	print(response.Headers["Server"])
