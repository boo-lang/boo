#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace Boo.Microsoft.Build.Tasks

import Microsoft.Build.Framework
import Microsoft.Build.Tasks
import Microsoft.Build.Utilities

class CreateBooManifestResourceName(CreateCSharpManifestResourceName):
"""
Creates the manifest resource name.

Authors:
	Sorin Ionescu (sorin.ionescu@gmail.com)
"""
	
	[property(ResourceFilesWithManifestResourceNames, Attributes: [OutputAttribute])]
	private resourceFilesWithManifestResourceNames as (ITaskItem);
	
	def Execute() as bool:
		if super():
			// Provide ResourceFilesWithManifestResourceNames to make the task
			// compatible with both MSBuild 2.0 and MSBuild 3.5
			resourceFilesWithManifestResourceNames = array(ITaskItem, self.ResourceFiles.Length)
			for i in range(0, self.ResourceFiles.Length):
				resourceName = self.ManifestResourceNames[i].ItemSpec
				newItem = TaskItem(self.ResourceFiles[i])
				newItem.SetMetadata("ManifestResourceName", resourceName)
				if string.IsNullOrEmpty(newItem.GetMetadata("LogicalName")):
					newItem.SetMetadata("ManifestResourceName", resourceName)
				resourceFilesWithManifestResourceNames[i] = newItem
			return true
		else:
			return false

