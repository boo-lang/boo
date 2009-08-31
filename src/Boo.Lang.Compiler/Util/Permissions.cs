#region license
// Copyright (c) 2009, Rodrigo B. de Oliveira (rbo@acm.org)
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

using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace Boo.Lang.Compiler.Util
{
	internal static class Permissions
	{
		public static bool HasAppDomainPermission
		{
			get
			{
				if (null == hasAppDomainPermission)
					hasAppDomainPermission = CheckPermission(new SecurityPermission(SecurityPermissionFlag.ControlAppDomain));
				return (bool) hasAppDomainPermission;
			}
		}
		static bool? hasAppDomainPermission = null;

		public static bool HasEnvironmentPermission
		{
			get
			{
				if (null == hasEnvironmentPermission)
					hasEnvironmentPermission = CheckPermission(new EnvironmentPermission(PermissionState.Unrestricted));
				return (bool) hasEnvironmentPermission;
			}
		}
		static bool? hasEnvironmentPermission = null;

		public static bool HasDiscoveryPermission
		{
			get
			{
				if (null == hasDiscoveryPermission)
					hasDiscoveryPermission = CheckPermission(new FileIOPermission(PermissionState.Unrestricted));
				return (bool) hasDiscoveryPermission;
			}
		}
		static bool? hasDiscoveryPermission = null;

		static bool CheckPermission(IPermission permission)
		{
			bool hasPermission = false;
			try
			{
				permission.Demand();
				hasPermission = true;
			}
			catch (SecurityException)
			{
			}
			return hasPermission;
		}
	}
}

