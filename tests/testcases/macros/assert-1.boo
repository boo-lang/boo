"""
assert passed
assert failed: x and z
assert passed
assert failed: msg 1
"""
import BooCompiler.Tests from BooCompiler.Tests
import Boo.Lang.Runtime


x = true
y = true
z = false

try:
	assert x or z
	print("assert passed")
except e as AssertionFailedException:
	print("assert failed: ${e.Message}")

try:
	assert x and z
	print("assert passed")
except e as AssertionFailedException:
	print("assert failed: ${e.Message}")

try:
	assert z or (x and y)
	print("assert passed")
except e as AssertionFailedException:
	print("assert failed: ${e.Message}")

try:
	assert y and (x and z), "msg 1"
	print("assert passed")
except e as AssertionFailedException:
	print("assert failed: ${e.Message}")

