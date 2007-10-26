#region license
// Copyright (c) 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace Boo.Lang.Useful.Tests.Attributes

import NUnit.Framework
import System.Runtime.Serialization
import System.Runtime.CompilerServices
import Boo.Lang.Useful.Attributes.SingletonAttribute as CUT

[TestFixture]
class SingletonAttributeTestFixture(AbstractAttributeTestFixture):
"""
@author Marcus Griep (neoeinstein+boo@gmail.com)
@author Sorin Ionescu (sorin.ionescu@gmail.com)
@author Rodrigo B. de Oliveira
"""	
	final static SINGLETON_CLASS_NAME = "SingletonObject"

	[Test]
	def DefaultSingleton():
		code = """
import Useful.Attributes

[Singleton]
transient class ${SINGLETON_CLASS_NAME}:
	pass
"""
		expected = """
import Useful.Attributes

public final transient class ${SINGLETON_CLASS_NAME}(object):

	[${CUT.COMPILERGENERATEDATTRIBUTE_FULL_NAME}]
	private def constructor():
		super()

	[${CUT.COMPILERGENERATEDATTRIBUTE_FULL_NAME}]
	public static ${CUT.DEFAULT_INSTANCE_PROPERTY_NAME} as ${SINGLETON_CLASS_NAME}:
		public static get:
			return self.${CUT.NESTED_CLASS_NAME}.${CUT.INSTANCE_FIELD_NAME}

	[${CUT.COMPILERGENERATEDATTRIBUTE_FULL_NAME}]
	private final transient class ${CUT.NESTED_CLASS_NAME}(object):

		public static def constructor():
			self.${CUT.INSTANCE_FIELD_NAME} = ${SINGLETON_CLASS_NAME}()

		private def constructor():
			super()

		public static final ${CUT.INSTANCE_FIELD_NAME} as ${SINGLETON_CLASS_NAME}
"""
		RunTestCase(expected, code)

	[Test]
	def Serializable():
		code = """
import Useful.Attributes

[Singleton]
class ${SINGLETON_CLASS_NAME}:
	pass
"""
		expected = """
import Useful.Attributes

public final class ${SINGLETON_CLASS_NAME}(object, ${CUT.ISERIALIZABLE_FULL_NAME}):

	[${CUT.COMPILERGENERATEDATTRIBUTE_FULL_NAME}]
	private def constructor():
		super()

	[${CUT.COMPILERGENERATEDATTRIBUTE_FULL_NAME}]
	public static ${CUT.DEFAULT_INSTANCE_PROPERTY_NAME} as ${SINGLETON_CLASS_NAME}:
		public static get:
			return self.${CUT.NESTED_CLASS_NAME}.${CUT.INSTANCE_FIELD_NAME}

	[${CUT.COMPILERGENERATEDATTRIBUTE_FULL_NAME}]
	private final transient class ${CUT.NESTED_CLASS_NAME}(object):

		public static def constructor():
			self.${CUT.INSTANCE_FIELD_NAME} = ${SINGLETON_CLASS_NAME}()

		private def constructor():
			super()

		public static final ${CUT.INSTANCE_FIELD_NAME} as ${SINGLETON_CLASS_NAME}

	[${CUT.COMPILERGENERATEDATTRIBUTE_FULL_NAME}]
	private final class ${CUT.SERIALIZATION_OBJECT_CLASS_NAME}(object, ${CUT.IOBJECTREFERENCE_FULL_NAME}):

		public virtual def GetRealObject(context as ${CUT.STREAMINGCONTEXT_FULL_NAME}) as object:
			return ${SINGLETON_CLASS_NAME}.${CUT.DEFAULT_INSTANCE_PROPERTY_NAME}

		public def constructor():
			super()

	[${CUT.COMPILERGENERATEDATTRIBUTE_FULL_NAME}]
	private virtual def ${CUT.ISERIALIZABLE_FULL_NAME}.${CUT.GETOBJECTDATA_METHOD_NAME}(info as ${CUT.SERIALIZATIONINFO_FULL_NAME}, context as ${CUT.STREAMINGCONTEXT_FULL_NAME}) as void:
		info.SetType(${CUT.SERIALIZATION_OBJECT_CLASS_NAME})
"""
		RunTestCase(expected, code)

	[Test]
	def GivenAccessorName():
		accessorPropertyName = "Factory"
		
		code = """
import Useful.Attributes

[Singleton(AccessorName: ${accessorPropertyName})]
transient class ${SINGLETON_CLASS_NAME}:
	pass
"""
		expected = """
import Useful.Attributes

public final transient class ${SINGLETON_CLASS_NAME}(object):

	[${CUT.COMPILERGENERATEDATTRIBUTE_FULL_NAME}]
	private def constructor():
		super()

	[${CUT.COMPILERGENERATEDATTRIBUTE_FULL_NAME}]
	public static ${accessorPropertyName} as ${SINGLETON_CLASS_NAME}:
		public static get:
			return self.${CUT.NESTED_CLASS_NAME}.${CUT.INSTANCE_FIELD_NAME}

	[${CUT.COMPILERGENERATEDATTRIBUTE_FULL_NAME}]
	private final transient class ${CUT.NESTED_CLASS_NAME}(object):

		public static def constructor():
			self.${CUT.INSTANCE_FIELD_NAME} = ${SINGLETON_CLASS_NAME}()

		private def constructor():
			super()

		public static final ${CUT.INSTANCE_FIELD_NAME} as ${SINGLETON_CLASS_NAME}
"""
		RunTestCase(expected, code)

	[Test]
	def ParameterizedConstructor():
		code = """
import Useful.Attributes

[Singleton]
transient class ${SINGLETON_CLASS_NAME}:
	public def constructor(o as object):
		pass
"""
		expected = """
import Useful.Attributes

public final transient class ${SINGLETON_CLASS_NAME}(object):

	[${CUT.COMPILERGENERATEDATTRIBUTE_FULL_NAME}]
	private def constructor():
		super()

	[${CUT.COMPILERGENERATEDATTRIBUTE_FULL_NAME}]
	public static ${CUT.DEFAULT_INSTANCE_PROPERTY_NAME} as ${SINGLETON_CLASS_NAME}:
		public static get:
			return self.${CUT.NESTED_CLASS_NAME}.${CUT.INSTANCE_FIELD_NAME}

	[${CUT.COMPILERGENERATEDATTRIBUTE_FULL_NAME}]
	private final transient class ${CUT.NESTED_CLASS_NAME}(object):

		public static def constructor():
			self.${CUT.INSTANCE_FIELD_NAME} = ${SINGLETON_CLASS_NAME}()

		private def constructor():
			super()

		public static final ${CUT.INSTANCE_FIELD_NAME} as ${SINGLETON_CLASS_NAME}
"""
		RunTestCase(expected, code)

	[Test]
	def VerifyConstants():
		Assert.AreEqual(typeof(ISerializable).FullName, CUT.ISERIALIZABLE_FULL_NAME)
		Assert.AreEqual(typeof(ISerializable).GetMethods()[0].Name, CUT.GETOBJECTDATA_METHOD_NAME)
		Assert.AreEqual(typeof(IObjectReference).FullName, CUT.IOBJECTREFERENCE_FULL_NAME)
		Assert.AreEqual(typeof(SerializationInfo).FullName, CUT.SERIALIZATIONINFO_FULL_NAME)
		Assert.AreEqual(typeof(StreamingContext).FullName, CUT.STREAMINGCONTEXT_FULL_NAME)
		Assert.AreEqual(typeof(CompilerGeneratedAttribute).FullName, CUT.COMPILERGENERATEDATTRIBUTE_FULL_NAME)