#ignore Non-IEnumerable definitions of GetEnumerator() not yet supported
"""
Try:It
You Know:You Want To
"""
import System.Collections

def Test(item as IDictionary)
	for item in hash:
		print join(':', item.Key, item.Value)

Test({'Try':'It', 'You Know':'You Want To'})