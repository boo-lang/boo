#region license
// Copyright (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace Boo.Lang.Runtime.DynamicDispatching
{
	public class NumericPromotions
	{ 
		public static object FromSByteToByte(object value, object[] args)
		{
			return (Byte) ((SByte)value);
		}
 
		public static object FromSByteToInt16(object value, object[] args)
		{
			return (Int16) ((SByte)value);
		}
 
		public static object FromSByteToUInt16(object value, object[] args)
		{
			return (UInt16) ((SByte)value);
		}
 
		public static object FromSByteToInt32(object value, object[] args)
		{
			return (Int32) ((SByte)value);
		}
 
		public static object FromSByteToUInt32(object value, object[] args)
		{
			return (UInt32) ((SByte)value);
		}
 
		public static object FromSByteToInt64(object value, object[] args)
		{
			return (Int64) ((SByte)value);
		}
 
		public static object FromSByteToUInt64(object value, object[] args)
		{
			return (UInt64) ((SByte)value);
		}
 
		public static object FromSByteToSingle(object value, object[] args)
		{
			return (Single) ((SByte)value);
		}
 
		public static object FromSByteToDouble(object value, object[] args)
		{
			return (Double) ((SByte)value);
		}
 
		public static object FromSByteToChar(object value, object[] args)
		{
			return (Char) ((SByte)value);
		}
 
		public static object FromByteToSByte(object value, object[] args)
		{
			return (SByte) ((Byte)value);
		}
 
		public static object FromByteToInt16(object value, object[] args)
		{
			return (Int16) ((Byte)value);
		}
 
		public static object FromByteToUInt16(object value, object[] args)
		{
			return (UInt16) ((Byte)value);
		}
 
		public static object FromByteToInt32(object value, object[] args)
		{
			return (Int32) ((Byte)value);
		}
 
		public static object FromByteToUInt32(object value, object[] args)
		{
			return (UInt32) ((Byte)value);
		}
 
		public static object FromByteToInt64(object value, object[] args)
		{
			return (Int64) ((Byte)value);
		}
 
		public static object FromByteToUInt64(object value, object[] args)
		{
			return (UInt64) ((Byte)value);
		}
 
		public static object FromByteToSingle(object value, object[] args)
		{
			return (Single) ((Byte)value);
		}
 
		public static object FromByteToDouble(object value, object[] args)
		{
			return (Double) ((Byte)value);
		}
 
		public static object FromByteToChar(object value, object[] args)
		{
			return (Char) ((Byte)value);
		}
 
		public static object FromInt16ToSByte(object value, object[] args)
		{
			return (SByte) ((Int16)value);
		}
 
		public static object FromInt16ToByte(object value, object[] args)
		{
			return (Byte) ((Int16)value);
		}
 
		public static object FromInt16ToUInt16(object value, object[] args)
		{
			return (UInt16) ((Int16)value);
		}
 
		public static object FromInt16ToInt32(object value, object[] args)
		{
			return (Int32) ((Int16)value);
		}
 
		public static object FromInt16ToUInt32(object value, object[] args)
		{
			return (UInt32) ((Int16)value);
		}
 
		public static object FromInt16ToInt64(object value, object[] args)
		{
			return (Int64) ((Int16)value);
		}
 
		public static object FromInt16ToUInt64(object value, object[] args)
		{
			return (UInt64) ((Int16)value);
		}
 
		public static object FromInt16ToSingle(object value, object[] args)
		{
			return (Single) ((Int16)value);
		}
 
		public static object FromInt16ToDouble(object value, object[] args)
		{
			return (Double) ((Int16)value);
		}
 
		public static object FromInt16ToChar(object value, object[] args)
		{
			return (Char) ((Int16)value);
		}
 
		public static object FromUInt16ToSByte(object value, object[] args)
		{
			return (SByte) ((UInt16)value);
		}
 
		public static object FromUInt16ToByte(object value, object[] args)
		{
			return (Byte) ((UInt16)value);
		}
 
		public static object FromUInt16ToInt16(object value, object[] args)
		{
			return (Int16) ((UInt16)value);
		}
 
		public static object FromUInt16ToInt32(object value, object[] args)
		{
			return (Int32) ((UInt16)value);
		}
 
		public static object FromUInt16ToUInt32(object value, object[] args)
		{
			return (UInt32) ((UInt16)value);
		}
 
		public static object FromUInt16ToInt64(object value, object[] args)
		{
			return (Int64) ((UInt16)value);
		}
 
		public static object FromUInt16ToUInt64(object value, object[] args)
		{
			return (UInt64) ((UInt16)value);
		}
 
		public static object FromUInt16ToSingle(object value, object[] args)
		{
			return (Single) ((UInt16)value);
		}
 
		public static object FromUInt16ToDouble(object value, object[] args)
		{
			return (Double) ((UInt16)value);
		}
 
		public static object FromUInt16ToChar(object value, object[] args)
		{
			return (Char) ((UInt16)value);
		}
 
		public static object FromInt32ToSByte(object value, object[] args)
		{
			return (SByte) ((Int32)value);
		}
 
		public static object FromInt32ToByte(object value, object[] args)
		{
			return (Byte) ((Int32)value);
		}
 
		public static object FromInt32ToInt16(object value, object[] args)
		{
			return (Int16) ((Int32)value);
		}
 
		public static object FromInt32ToUInt16(object value, object[] args)
		{
			return (UInt16) ((Int32)value);
		}
 
		public static object FromInt32ToUInt32(object value, object[] args)
		{
			return (UInt32) ((Int32)value);
		}
 
		public static object FromInt32ToInt64(object value, object[] args)
		{
			return (Int64) ((Int32)value);
		}
 
		public static object FromInt32ToUInt64(object value, object[] args)
		{
			return (UInt64) ((Int32)value);
		}
 
		public static object FromInt32ToSingle(object value, object[] args)
		{
			return (Single) ((Int32)value);
		}
 
		public static object FromInt32ToDouble(object value, object[] args)
		{
			return (Double) ((Int32)value);
		}
 
		public static object FromInt32ToChar(object value, object[] args)
		{
			return (Char) ((Int32)value);
		}
 
		public static object FromUInt32ToSByte(object value, object[] args)
		{
			return (SByte) ((UInt32)value);
		}
 
		public static object FromUInt32ToByte(object value, object[] args)
		{
			return (Byte) ((UInt32)value);
		}
 
		public static object FromUInt32ToInt16(object value, object[] args)
		{
			return (Int16) ((UInt32)value);
		}
 
		public static object FromUInt32ToUInt16(object value, object[] args)
		{
			return (UInt16) ((UInt32)value);
		}
 
		public static object FromUInt32ToInt32(object value, object[] args)
		{
			return (Int32) ((UInt32)value);
		}
 
		public static object FromUInt32ToInt64(object value, object[] args)
		{
			return (Int64) ((UInt32)value);
		}
 
		public static object FromUInt32ToUInt64(object value, object[] args)
		{
			return (UInt64) ((UInt32)value);
		}
 
		public static object FromUInt32ToSingle(object value, object[] args)
		{
			return (Single) ((UInt32)value);
		}
 
		public static object FromUInt32ToDouble(object value, object[] args)
		{
			return (Double) ((UInt32)value);
		}
 
		public static object FromUInt32ToChar(object value, object[] args)
		{
			return (Char) ((UInt32)value);
		}
 
		public static object FromInt64ToSByte(object value, object[] args)
		{
			return (SByte) ((Int64)value);
		}
 
		public static object FromInt64ToByte(object value, object[] args)
		{
			return (Byte) ((Int64)value);
		}
 
		public static object FromInt64ToInt16(object value, object[] args)
		{
			return (Int16) ((Int64)value);
		}
 
		public static object FromInt64ToUInt16(object value, object[] args)
		{
			return (UInt16) ((Int64)value);
		}
 
		public static object FromInt64ToInt32(object value, object[] args)
		{
			return (Int32) ((Int64)value);
		}
 
		public static object FromInt64ToUInt32(object value, object[] args)
		{
			return (UInt32) ((Int64)value);
		}
 
		public static object FromInt64ToUInt64(object value, object[] args)
		{
			return (UInt64) ((Int64)value);
		}
 
		public static object FromInt64ToSingle(object value, object[] args)
		{
			return (Single) ((Int64)value);
		}
 
		public static object FromInt64ToDouble(object value, object[] args)
		{
			return (Double) ((Int64)value);
		}
 
		public static object FromInt64ToChar(object value, object[] args)
		{
			return (Char) ((Int64)value);
		}
 
		public static object FromUInt64ToSByte(object value, object[] args)
		{
			return (SByte) ((UInt64)value);
		}
 
		public static object FromUInt64ToByte(object value, object[] args)
		{
			return (Byte) ((UInt64)value);
		}
 
		public static object FromUInt64ToInt16(object value, object[] args)
		{
			return (Int16) ((UInt64)value);
		}
 
		public static object FromUInt64ToUInt16(object value, object[] args)
		{
			return (UInt16) ((UInt64)value);
		}
 
		public static object FromUInt64ToInt32(object value, object[] args)
		{
			return (Int32) ((UInt64)value);
		}
 
		public static object FromUInt64ToUInt32(object value, object[] args)
		{
			return (UInt32) ((UInt64)value);
		}
 
		public static object FromUInt64ToInt64(object value, object[] args)
		{
			return (Int64) ((UInt64)value);
		}
 
		public static object FromUInt64ToSingle(object value, object[] args)
		{
			return (Single) ((UInt64)value);
		}
 
		public static object FromUInt64ToDouble(object value, object[] args)
		{
			return (Double) ((UInt64)value);
		}
 
		public static object FromUInt64ToChar(object value, object[] args)
		{
			return (Char) ((UInt64)value);
		}
 
		public static object FromSingleToSByte(object value, object[] args)
		{
			return (SByte) ((Single)value);
		}
 
		public static object FromSingleToByte(object value, object[] args)
		{
			return (Byte) ((Single)value);
		}
 
		public static object FromSingleToInt16(object value, object[] args)
		{
			return (Int16) ((Single)value);
		}
 
		public static object FromSingleToUInt16(object value, object[] args)
		{
			return (UInt16) ((Single)value);
		}
 
		public static object FromSingleToInt32(object value, object[] args)
		{
			return (Int32) ((Single)value);
		}
 
		public static object FromSingleToUInt32(object value, object[] args)
		{
			return (UInt32) ((Single)value);
		}
 
		public static object FromSingleToInt64(object value, object[] args)
		{
			return (Int64) ((Single)value);
		}
 
		public static object FromSingleToUInt64(object value, object[] args)
		{
			return (UInt64) ((Single)value);
		}
 
		public static object FromSingleToDouble(object value, object[] args)
		{
			return (Double) ((Single)value);
		}
 
		public static object FromSingleToChar(object value, object[] args)
		{
			return (Char) ((Single)value);
		}
 
		public static object FromDoubleToSByte(object value, object[] args)
		{
			return (SByte) ((Double)value);
		}
 
		public static object FromDoubleToByte(object value, object[] args)
		{
			return (Byte) ((Double)value);
		}
 
		public static object FromDoubleToInt16(object value, object[] args)
		{
			return (Int16) ((Double)value);
		}
 
		public static object FromDoubleToUInt16(object value, object[] args)
		{
			return (UInt16) ((Double)value);
		}
 
		public static object FromDoubleToInt32(object value, object[] args)
		{
			return (Int32) ((Double)value);
		}
 
		public static object FromDoubleToUInt32(object value, object[] args)
		{
			return (UInt32) ((Double)value);
		}
 
		public static object FromDoubleToInt64(object value, object[] args)
		{
			return (Int64) ((Double)value);
		}
 
		public static object FromDoubleToUInt64(object value, object[] args)
		{
			return (UInt64) ((Double)value);
		}
 
		public static object FromDoubleToSingle(object value, object[] args)
		{
			return (Single) ((Double)value);
		}
 
		public static object FromDoubleToChar(object value, object[] args)
		{
			return (Char) ((Double)value);
		}
 
		public static object FromCharToSByte(object value, object[] args)
		{
			return (SByte) ((Char)value);
		}
 
		public static object FromCharToByte(object value, object[] args)
		{
			return (Byte) ((Char)value);
		}
 
		public static object FromCharToInt16(object value, object[] args)
		{
			return (Int16) ((Char)value);
		}
 
		public static object FromCharToUInt16(object value, object[] args)
		{
			return (UInt16) ((Char)value);
		}
 
		public static object FromCharToInt32(object value, object[] args)
		{
			return (Int32) ((Char)value);
		}
 
		public static object FromCharToUInt32(object value, object[] args)
		{
			return (UInt32) ((Char)value);
		}
 
		public static object FromCharToInt64(object value, object[] args)
		{
			return (Int64) ((Char)value);
		}
 
		public static object FromCharToUInt64(object value, object[] args)
		{
			return (UInt64) ((Char)value);
		}
 
		public static object FromCharToSingle(object value, object[] args)
		{
			return (Single) ((Char)value);
		}
 
		public static object FromCharToDouble(object value, object[] args)
		{
			return (Double) ((Char)value);
		}
	}
}

