using Boo.Lang.Compiler.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Boo.Lang.Compiler.Steps.Ecma335
{
    internal class MaxStackAnalyzer
    {
        private static readonly ILOpCode[] ZERO = new[]
        {
            ILOpCode.Nop, ILOpCode.Break, ILOpCode.Jmp, ILOpCode.Call, ILOpCode.Calli,
            ILOpCode.Ret, ILOpCode.Br_s, ILOpCode.Br, ILOpCode.Ldind_i1, ILOpCode.Ldind_u1,
            ILOpCode.Ldind_i2, ILOpCode.Ldind_u2, ILOpCode.Ldind_i4, ILOpCode.Ldind_u4,
            ILOpCode.Ldind_i8, ILOpCode.Ldind_i, ILOpCode.Ldind_r4, ILOpCode.Ldind_r8,
            ILOpCode.Ldind_ref, ILOpCode.Neg, ILOpCode.Not, ILOpCode.Conv_i1, ILOpCode.Conv_i2,
            ILOpCode.Conv_i4, ILOpCode.Conv_i8, ILOpCode.Conv_r4, ILOpCode.Conv_r8,
            ILOpCode.Conv_u4, ILOpCode.Conv_u8, ILOpCode.Callvirt, ILOpCode.Ldobj,
            ILOpCode.Castclass, ILOpCode.Isinst, ILOpCode.Conv_r_un, ILOpCode.Unbox,
            ILOpCode.Ldfld, ILOpCode.Ldflda, ILOpCode.Conv_ovf_i1_un, ILOpCode.Conv_ovf_i2_un,
            ILOpCode.Conv_ovf_i4_un, ILOpCode.Conv_ovf_i8_un, ILOpCode.Conv_ovf_u1_un,
            ILOpCode.Conv_ovf_u2_un, ILOpCode.Conv_ovf_u4_un, ILOpCode.Conv_ovf_u8_un,
            ILOpCode.Conv_ovf_i_un, ILOpCode.Conv_ovf_u_un, ILOpCode.Box,
            ILOpCode.Ldlen, ILOpCode.Stelem, ILOpCode.Unbox_any, ILOpCode.Conv_ovf_i1,
            ILOpCode.Conv_ovf_u1, ILOpCode.Conv_ovf_i2, ILOpCode.Conv_ovf_u2, ILOpCode.Conv_ovf_i4,
            ILOpCode.Conv_ovf_u4, ILOpCode.Conv_ovf_i8, ILOpCode.Conv_ovf_u8, ILOpCode.Refanyval,
            ILOpCode.Ckfinite, ILOpCode.Mkrefany, ILOpCode.Conv_u2, ILOpCode.Conv_u1,
            ILOpCode.Conv_i, ILOpCode.Conv_ovf_i, ILOpCode.Conv_ovf_u, ILOpCode.Endfinally,
            ILOpCode.Leave, ILOpCode.Leave_s, ILOpCode.Conv_u, ILOpCode.Ldvirtftn,
            ILOpCode.Localloc, ILOpCode.Unaligned, ILOpCode.Volatile, ILOpCode.Tail,
            ILOpCode.Constrained, ILOpCode.Rethrow, ILOpCode.Refanytype, ILOpCode.Readonly,
        };

        private static readonly ILOpCode[] PLUS_ONE = new[]
        {
            ILOpCode.Ldarg_0, ILOpCode.Ldarg_1,
            ILOpCode.Ldarg_2, ILOpCode.Ldarg_3, ILOpCode.Ldloc_0, ILOpCode.Ldloc_1,
            ILOpCode.Ldloc_2, ILOpCode.Ldloc_3, ILOpCode.Ldarg_s, ILOpCode.Ldarga_s,
            ILOpCode.Ldloc_s, ILOpCode.Ldloca_s, ILOpCode.Ldnull, ILOpCode.Ldc_i4_m1,
            ILOpCode.Ldc_i4_0, ILOpCode.Ldc_i4_1, ILOpCode.Ldc_i4_2, ILOpCode.Ldc_i4_3,
            ILOpCode.Ldc_i4_4, ILOpCode.Ldc_i4_5, ILOpCode.Ldc_i4_6, ILOpCode.Ldc_i4_7,
            ILOpCode.Ldc_i4_8, ILOpCode.Ldc_i4_s, ILOpCode.Ldc_i4, ILOpCode.Ldc_i8,
            ILOpCode.Ldc_r4, ILOpCode.Ldc_r8, ILOpCode.Dup, ILOpCode.Ldstr, ILOpCode.Newobj,
            ILOpCode.Ldsfld, ILOpCode.Ldsflda, ILOpCode.Ldtoken, ILOpCode.Arglist,
            ILOpCode.Ldftn, ILOpCode.Ldarg, ILOpCode.Ldarga, ILOpCode.Ldloc, ILOpCode.Ldloca,
            ILOpCode.Sizeof, ILOpCode.Newarr
        };

        private static readonly ILOpCode[] MINUS_ONE = new[]
        {
            ILOpCode.Stloc_0, ILOpCode.Stloc_1, ILOpCode.Stloc_2, ILOpCode.Stloc_3,
            ILOpCode.Starg_s, ILOpCode.Stloc_s, ILOpCode.Pop, ILOpCode.Brfalse_s,
            ILOpCode.Brtrue_s, ILOpCode.Brfalse, ILOpCode.Brtrue, ILOpCode.Add, ILOpCode.Sub,
            ILOpCode.Mul, ILOpCode.Div, ILOpCode.Div_un, ILOpCode.Rem, ILOpCode.Rem_un,
            ILOpCode.And, ILOpCode.Or, ILOpCode.Xor, ILOpCode.Shl, ILOpCode.Shr,
            ILOpCode.Shr_un, ILOpCode.Throw, ILOpCode.Stsfld, ILOpCode.Ldelema,
            ILOpCode.Ldelem_i1, ILOpCode.Ldelem_u1, ILOpCode.Ldelem_i2, ILOpCode.Ldelem_u2,
            ILOpCode.Ldelem_i4, ILOpCode.Ldelem_u4, ILOpCode.Ldelem_i8, ILOpCode.Ldelem_i,
            ILOpCode.Ldelem_r4, ILOpCode.Ldelem_r8, ILOpCode.Ldelem_ref, ILOpCode.Ldelem,
            ILOpCode.Add_ovf, ILOpCode.Add_ovf_un, ILOpCode.Mul_ovf, ILOpCode.Mul_ovf_un,
            ILOpCode.Sub_ovf, ILOpCode.Sub_ovf_un, ILOpCode.Ceq, ILOpCode.Cgt, ILOpCode.Cgt_un,
            ILOpCode.Clt, ILOpCode.Clt_un, ILOpCode.Starg, ILOpCode.Stloc, ILOpCode.Endfilter,
            ILOpCode.Initobj, ILOpCode.Switch,
        };

        private static readonly ILOpCode[] MINUS_TWO = new[]
        {
            ILOpCode.Beq_s, ILOpCode.Bge_s, ILOpCode.Bgt_s, ILOpCode.Ble_s, ILOpCode.Blt_s,
            ILOpCode.Bne_un_s, ILOpCode.Bge_un_s, ILOpCode.Bgt_un_s, ILOpCode.Ble_un_s,
            ILOpCode.Blt_un_s, ILOpCode.Beq, ILOpCode.Bge, ILOpCode.Bgt, ILOpCode.Ble,
            ILOpCode.Blt, ILOpCode.Bne_un, ILOpCode.Bge_un, ILOpCode.Bgt_un, ILOpCode.Ble_un,
            ILOpCode.Blt_un, ILOpCode.Stind_ref, ILOpCode.Stind_i1,
            ILOpCode.Stind_i2, ILOpCode.Stind_i4, ILOpCode.Stind_i8, ILOpCode.Stind_r4,
            ILOpCode.Stind_r8, ILOpCode.Cpobj, ILOpCode.Stfld, ILOpCode.Stobj, ILOpCode.Stind_i
        };

        private static readonly ILOpCode[] MINUS_THREE = new[]
        {
            ILOpCode.Stelem_i, ILOpCode.Stelem_i1, ILOpCode.Stelem_i2, ILOpCode.Stelem_i4,
            ILOpCode.Stelem_i8, ILOpCode.Stelem_r4, ILOpCode.Stelem_r8, ILOpCode.Stelem_ref,
            ILOpCode.Cpblk, ILOpCode.Initblk
        };

        private static readonly Dictionary<ILOpCode, int> _opcodeDict;

        private static readonly HashSet<ILOpCode> _endsUncondJmpBlk = new()
        {
            ILOpCode.Jmp, ILOpCode.Ret, ILOpCode.Br_s, ILOpCode.Br, ILOpCode.Throw,
            ILOpCode.Endfinally, ILOpCode.Leave, ILOpCode.Leave_s, ILOpCode.Endfilter,
            ILOpCode.Rethrow
        };

        static MaxStackAnalyzer()
        {
            var dict = new Dictionary<ILOpCode, int>();
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
            foreach (ILOpCode op in Enum.GetValues(typeof(ILOpCode)))
                if (!dict.ContainsKey(op))
                    throw new Exception(string.Format("Opcode {0} not found in list", op));
            _opcodeDict = dict;
        }

        private int _total;
        private int _current;
        private int _blockMax;

        private readonly TypeSystemServices _tss;

        public int Total => _total;

        public MaxStackAnalyzer(TypeSystemServices tss)
        {
            _tss = tss;
        }

        private void Adjust(int value, ILOpCode op)
        {
            _current += value;
            System.Diagnostics.Debug.WriteLine($"OpCode: {op}, Adjustment: {value}, Current stack depth: {_current}");
            _blockMax = Math.Max(_blockMax, _current);
            if (_endsUncondJmpBlk.Contains(op))
            {
                _total += _blockMax;
                _blockMax = 0;
                _current = 0;
            }
        }

        public void Operation(ILOpCode op)
        {
            var stackChange = _opcodeDict[op];
            Adjust(stackChange, op);
        }

        public void Call(ILOpCode op, IMethodBase method, int varArgCount = 0)
        {
            var result = 0;
            if (method.Type != _tss.VoidType || op == ILOpCode.Newobj)
                ++result;
            result -= method.GetParameters().Length;
            if (method.AcceptVarArgs)
                result -= varArgCount;
            if (op != ILOpCode.Newobj && !method.IsStatic)
                --result;
            if (op == ILOpCode.Calli)
                --result; //1 for the function pointer
            Adjust(result, op);
        }

        internal void CallArrayMethod(int length, bool hasReturnValue)
        {
            var result = hasReturnValue ? 1 : 0;
            result -= length;
            Adjust(result, ILOpCode.Call);
        }
    }
}
