import Boo.Lang.Compiler

start = date.Now
for i in range(1000):
	pipeline = CompilerPipeline()
	pipeline.Load("booc")
end = date.Now
print("booc pipeline loaded in ${end-start}.")
