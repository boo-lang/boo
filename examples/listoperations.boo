start = date.Now

l = []
for i in range(500000):
	l.Add(i)
	
print("Total time: ${date.Now-start}")
start = date.Now

for i in [100, -100]*1000:
	l.RemoveAt(i)

print("Total time: ${date.Now-start}")
start = date.Now

for i as int in range(len(l)):
	l[i] = l[-i]

print("Total time: ${date.Now-start}")
