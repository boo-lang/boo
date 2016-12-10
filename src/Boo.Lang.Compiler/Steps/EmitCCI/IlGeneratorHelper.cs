using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Cci;

namespace Boo.Lang.Compiler.Steps.EmitCCI
{
    // helper for calculating MaxStackSize from an IL stream.
    // Algorithm reverse-engineered from CoreCLR Reflection.Emit code found in OpCodes.cs, Opcode.cs and ILGenerator.cs
    // https://github.com/dotnet/coreclr/tree/master/src/mscorlib/src/System/Reflection/Emit
    public static class IlGeneratorHelper
    {
        private static readonly OperationCode[] ZERO =
        {
            OperationCode.Nop, OperationCode.Break, OperationCode.Jmp, OperationCode.Call, OperationCode.Calli,
            OperationCode.Ret, OperationCode.Br_S, OperationCode.Br, OperationCode.Ldind_I1, OperationCode.Ldind_U1,
            OperationCode.Ldind_I2, OperationCode.Ldind_U2, OperationCode.Ldind_I4, OperationCode.Ldind_U4,
            OperationCode.Ldind_I8, OperationCode.Ldind_I, OperationCode.Ldind_R4, OperationCode.Ldind_R8,
            OperationCode.Ldind_Ref, OperationCode.Neg, OperationCode.Not, OperationCode.Conv_I1, OperationCode.Conv_I2,
            OperationCode.Conv_I4, OperationCode.Conv_I8, OperationCode.Conv_R4, OperationCode.Conv_R8,
            OperationCode.Conv_U4, OperationCode.Conv_U8, OperationCode.Callvirt, OperationCode.Ldobj,
            OperationCode.Castclass, OperationCode.Isinst, OperationCode.Conv_R_Un, OperationCode.Unbox,
            OperationCode.Ldfld, OperationCode.Ldflda, OperationCode.Conv_Ovf_I1_Un, OperationCode.Conv_Ovf_I2_Un,
            OperationCode.Conv_Ovf_I4_Un, OperationCode.Conv_Ovf_I8_Un, OperationCode.Conv_Ovf_U1_Un,
            OperationCode.Conv_Ovf_U2_Un, OperationCode.Conv_Ovf_U4_Un, OperationCode.Conv_Ovf_U8_Un,
            OperationCode.Conv_Ovf_I_Un, OperationCode.Conv_Ovf_U_Un, OperationCode.Box,
            OperationCode.Ldlen, OperationCode.Stelem, OperationCode.Unbox_Any, OperationCode.Conv_Ovf_I1,
            OperationCode.Conv_Ovf_U1, OperationCode.Conv_Ovf_I2, OperationCode.Conv_Ovf_U2, OperationCode.Conv_Ovf_I4,
            OperationCode.Conv_Ovf_U4, OperationCode.Conv_Ovf_I8, OperationCode.Conv_Ovf_U8, OperationCode.Refanyval,
            OperationCode.Ckfinite, OperationCode.Mkrefany, OperationCode.Conv_U2, OperationCode.Conv_U1,
            OperationCode.Conv_I, OperationCode.Conv_Ovf_I, OperationCode.Conv_Ovf_U, OperationCode.Endfinally,
            OperationCode.Leave, OperationCode.Leave_S, OperationCode.Conv_U, OperationCode.Ldvirtftn,
            OperationCode.Localloc, OperationCode.Unaligned_, OperationCode.Volatile_, OperationCode.Tail_,
            OperationCode.Constrained_, OperationCode.Rethrow, OperationCode.Refanytype, OperationCode.Readonly_,
            OperationCode.Invalid, OperationCode.No_, 
            OperationCode.Array_Get, OperationCode.Array_Set, OperationCode.Array_Addr
        };

        private static readonly OperationCode[] PLUS_ONE =
        {
            OperationCode.Ldarg_0, OperationCode.Ldarg_1,
            OperationCode.Ldarg_2, OperationCode.Ldarg_3, OperationCode.Ldloc_0, OperationCode.Ldloc_1,
            OperationCode.Ldloc_2, OperationCode.Ldloc_3, OperationCode.Ldarg_S, OperationCode.Ldarga_S,
            OperationCode.Ldloc_S, OperationCode.Ldloca_S, OperationCode.Ldnull, OperationCode.Ldc_I4_M1,
            OperationCode.Ldc_I4_0, OperationCode.Ldc_I4_1, OperationCode.Ldc_I4_2, OperationCode.Ldc_I4_3,
            OperationCode.Ldc_I4_4, OperationCode.Ldc_I4_5, OperationCode.Ldc_I4_6, OperationCode.Ldc_I4_7,
            OperationCode.Ldc_I4_8, OperationCode.Ldc_I4_S, OperationCode.Ldc_I4, OperationCode.Ldc_I8,
            OperationCode.Ldc_R4, OperationCode.Ldc_R8, OperationCode.Dup, OperationCode.Ldstr, OperationCode.Newobj,
            OperationCode.Ldsfld, OperationCode.Ldsflda, OperationCode.Ldtoken, OperationCode.Arglist,
            OperationCode.Ldftn, OperationCode.Ldarg, OperationCode.Ldarga, OperationCode.Ldloc, OperationCode.Ldloca,
            OperationCode.Sizeof, OperationCode.Array_Create, OperationCode.Array_Create_WithLowerBound,
            OperationCode.Newarr
        };

        private static readonly OperationCode[] MINUS_ONE =
        {
            OperationCode.Stloc_0, OperationCode.Stloc_1, OperationCode.Stloc_2, OperationCode.Stloc_3,
            OperationCode.Starg_S, OperationCode.Stloc_S, OperationCode.Pop, OperationCode.Brfalse_S,
            OperationCode.Brtrue_S, OperationCode.Brfalse, OperationCode.Brtrue, OperationCode.Add, OperationCode.Sub,
            OperationCode.Mul, OperationCode.Div, OperationCode.Div_Un, OperationCode.Rem, OperationCode.Rem_Un,
            OperationCode.And, OperationCode.Or, OperationCode.Xor, OperationCode.Shl, OperationCode.Shr,
            OperationCode.Shr_Un, OperationCode.Throw, OperationCode.Stsfld, OperationCode.Ldelema,
            OperationCode.Ldelem_I1, OperationCode.Ldelem_U1, OperationCode.Ldelem_I2, OperationCode.Ldelem_U2,
            OperationCode.Ldelem_I4, OperationCode.Ldelem_U4, OperationCode.Ldelem_I8, OperationCode.Ldelem_I,
            OperationCode.Ldelem_R4, OperationCode.Ldelem_R8, OperationCode.Ldelem_Ref, OperationCode.Ldelem,
            OperationCode.Add_Ovf, OperationCode.Add_Ovf_Un, OperationCode.Mul_Ovf, OperationCode.Mul_Ovf_Un,
            OperationCode.Sub_Ovf, OperationCode.Sub_Ovf_Un, OperationCode.Ceq, OperationCode.Cgt, OperationCode.Cgt_Un,
            OperationCode.Clt, OperationCode.Clt_Un, OperationCode.Starg, OperationCode.Stloc, OperationCode.Endfilter,
            OperationCode.Initobj
        };

        private static readonly OperationCode[] MINUS_TWO =
        {
            OperationCode.Beq_S, OperationCode.Bge_S, OperationCode.Bgt_S, OperationCode.Ble_S, OperationCode.Blt_S,
            OperationCode.Bne_Un_S, OperationCode.Bge_Un_S, OperationCode.Bgt_Un_S, OperationCode.Ble_Un_S,
            OperationCode.Blt_Un_S, OperationCode.Beq, OperationCode.Bge, OperationCode.Bgt, OperationCode.Ble,
            OperationCode.Blt, OperationCode.Bne_Un, OperationCode.Bge_Un, OperationCode.Bgt_Un, OperationCode.Ble_Un,
            OperationCode.Blt_Un, OperationCode.Switch, OperationCode.Stind_Ref, OperationCode.Stind_I1,
            OperationCode.Stind_I2, OperationCode.Stind_I4, OperationCode.Stind_I8, OperationCode.Stind_R4,
            OperationCode.Stind_R8, OperationCode.Cpobj, OperationCode.Stfld, OperationCode.Stobj, OperationCode.Stind_I
        };

        private static readonly OperationCode[] MINUS_THREE =
        {
            OperationCode.Stelem_I, OperationCode.Stelem_I1, OperationCode.Stelem_I2, OperationCode.Stelem_I4,
            OperationCode.Stelem_I8, OperationCode.Stelem_R4, OperationCode.Stelem_R8, OperationCode.Stelem_Ref,
            OperationCode.Cpblk, OperationCode.Initblk
        };

        private static readonly Dictionary<OperationCode, int> _opcodeDict;

        private static readonly ISet<OperationCode> _endsUncondJmpBlk = new HashSet<OperationCode>
        {
            OperationCode.Jmp, OperationCode.Ret, OperationCode.Br_S, OperationCode.Br, OperationCode.Throw, 
            OperationCode.Endfinally, OperationCode.Leave, OperationCode.Leave_S, OperationCode.Endfilter, 
            OperationCode.Rethrow
        };

        private static readonly ISet<OperationCode> _calls = new HashSet<OperationCode>
        {
            OperationCode.Calli, OperationCode.Call, OperationCode.Callvirt, OperationCode.Newobj,
            OperationCode.Array_Addr, OperationCode.Array_Get, OperationCode.Array_Set
        };

        static IlGeneratorHelper()
        {
            var dict = new Dictionary<OperationCode, int>();
            foreach (var op in ZERO)
                dict.Add(op, 0);
            foreach (var op in PLUS_ONE)
                dict.Add(op, 1);
            foreach (var op in MINUS_ONE)
                dict.Add(op, -1);
            foreach (var op in MINUS_TWO)
                dict.Add(op, -2);
            foreach (var op in MINUS_THREE)
                dict.Add(op, -3);
            foreach (OperationCode op in Enum.GetValues(typeof(OperationCode)))
                if (!dict.ContainsKey(op))
                    throw new Exception(string.Format("Opcode {0} not found in list", op));
            _opcodeDict = dict;
        }

        private static int CallStackChange(IOperation call)
        {
            var result = 0;
            var method = call.Value as IMethodReference;
            if (method != null)
            {
                if (method.Type.TypeCode != PrimitiveTypeCode.Void || call.OperationCode == OperationCode.Newobj)
                    ++result;
                result -= method.ParameterCount;
                if (method.ExtraParameters != null)
                    result -= method.ExtraParameters.Count();
                if (call.OperationCode != OperationCode.Newobj && (method.CallingConvention & CallingConvention.HasThis) == CallingConvention.HasThis)
                    --result;
                if (call.OperationCode == OperationCode.Calli)
                    --result; //1 for the function pointer
            }
            else
            {
                var type = (IArrayTypeReference) call.Value;
                result -= (int)type.Rank;
            }
            return result;
        }

        public static ushort MaxStackSize(ILGenerator gen)
        {
            var result = 0;
            var blockMax = 0;
            var blockCurrent = 0;
            foreach (var op in gen.GetOperations())
            {
                var stackChange = _calls.Contains(op.OperationCode) ? CallStackChange(op) : _opcodeDict[op.OperationCode];
                blockCurrent += stackChange;
                blockMax = Math.Max(blockMax, blockCurrent);
                blockCurrent = Math.Max(blockCurrent, 0);
                if (_endsUncondJmpBlk.Contains(op.OperationCode))
                {
                    result += blockMax;
                    blockMax = 0;
                    blockCurrent = 0;
                }
            }
            return (ushort)result;
        }
    }
}
