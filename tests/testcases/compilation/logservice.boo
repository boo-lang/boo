"""
WARNING: albatross!
"""
enum LogMessageLevel:
	INFO     = 0
	WARNING  = 1
	ERROR    = 2

class FileLogService:
	
	def log(msg as string):
		log(LogMessageLevel.WARNING, msg)

	def log(logMessageLevel as LogMessageLevel, [required] msg as string):
		print("${logMessageLevel}: ${msg}")

FileLogService().log("albatross!")
