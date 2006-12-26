#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace Boo.Lang.Useful.Resources

import System
import System.Reflection
import System.Globalization

class ResourceFu(IQuackFu):
"""
This class allows dynamic access to resources specified by its constructor.
You are only allowed to get resources; setting or invocation is an immediate
smackdown.

Example:
	resources = Useful.Resources.ResourceFu('MyResources')
	text = resources.DialogPromptResource as string
	print text 

@author Arron Washington (l33ts0n@gmail.com)
"""
	[property(Resource)]
	_rm as System.Resources.ResourceManager
	
	[property(Culture)]
	_culture as CultureInfo
	
	def constructor(resource as string):		
		self(resource, Assembly.GetExecutingAssembly(), CultureInfo.CurrentUICulture)
		
	def constructor(resource as string, info as CultureInfo):
		self(resource, Assembly.GetExecutingAssembly(), info)
		
	def constructor(resource as string, assembly as Assembly):
		self(resource, assembly, CultureInfo.CurrentUICulture)
		
	def constructor([required] resource as string, [required] assembly as Assembly, [required] info as CultureInfo):
		#Careful, Boo has its own "ResourceManager"
		_rm = System.Resources.ResourceManager(resource, assembly)		
		_culture = info
		
	def destructor():
		_rm.ReleaseAllResources()

	def QuackGet([required]name as string, parameters as (object)):
		assert parameters is null
		return _rm.GetObject(name, _culture)

	def QuackSet(name as string, parameters as (object), value):
		raise ArgumentException("You are not allowed to alter embedded resources!")
		
	def QuackInvoke(name as string, args as (object)):
		raise ArgumentException("You cannot invoke code from embedded resources!")
