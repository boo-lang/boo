"""
WARNING: albatross!
"""
enum LogMessageLevel:
	INFO     = 0
	WARNING  = 1
	ERROR    = 2

class FileLogService:
	
	def log(msg as string):
	"""
	Loga mensagem com nivel de verbosidade WARNING
	"""
		log(LogMessageLevel.WARNING, msg)

	def log(logMessageLevel as LogMessageLevel, [required] msg as string):
	"""
	Loga mensagem com o nivel de log especificado.
	"""
		print("${logMessageLevel}: ${msg}")

FileLogService().log("albatross!")
