def value(x as long, y as long, z as long):
	return 2**x * 3**y * 5**z
end

def ugly(max as int):
	uglies = []
	counter = 1L
	dict = {1L : (0L, 0L, 0L)}
	
	while len(uglies) < max:
		uglies.Add(counter)
		x, y, z = dict[counter] as (long)
		
		dict[value(x+1, y, z)] = (x+1, y, z)
		dict[value(x, y+1, z)] = (x, y+1, z)
		dict[value(x, y, z+1)] = (x, y, z+1)
		
		dict.Remove(counter)
		
		keys = tuple(long, dict.Count)
		dict.Keys.CopyTo(keys, 0)
		System.Array.Sort(keys)
		
		counter = keys[0]
	end
	
	return uglies[-1]
end
	
iter = 1500
start = date.Now
for i in range(10):
	uvalue = ugly(iter)
end

stop = date.Now
print("${iter} ugly value = ${uvalue} in ${(stop-start).TotalMilliseconds}ms")

