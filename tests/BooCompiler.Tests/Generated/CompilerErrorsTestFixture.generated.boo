namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class CompilerErrorsTestFixture(AbstractCompilerErrorsTestFixture):

	[Test]
	def @BCE0000_1():
		RunCompilerTestCase("BCE0000-1.boo")

	[Test]
	def @BCE0004_1():
		RunCompilerTestCase("BCE0004-1.boo")

	[Test]
	def @BCE0004_2():
		RunCompilerTestCase("BCE0004-2.boo")

	[Test]
	def @BCE0004_3():
		RunCompilerTestCase("BCE0004-3.boo")

	[Test]
	def @BCE0005_1():
		RunCompilerTestCase("BCE0005-1.boo")

	[Test]
	def @BCE0005_2():
		RunCompilerTestCase("BCE0005-2.boo")

	[Test]
	def @BCE0005_named_argument():
		RunCompilerTestCase("BCE0005-named-argument.boo")

	[Test]
	def @BCE0006_1():
		RunCompilerTestCase("BCE0006-1.boo")

	[Test]
	def @BCE0007_1():
		RunCompilerTestCase("BCE0007-1.boo")

	[Test]
	def @BCE0017_1():
		RunCompilerTestCase("BCE0017-1.boo")

	[Test]
	def @BCE0017_2():
		RunCompilerTestCase("BCE0017-2.boo")

	[Test]
	def @BCE0017_3():
		RunCompilerTestCase("BCE0017-3.boo")

	[Test]
	def @BCE0017_4():
		RunCompilerTestCase("BCE0017-4.boo")

	[Test]
	def @BCE0018_1():
		RunCompilerTestCase("BCE0018-1.boo")

	[Test]
	def @BCE0018_2():
		RunCompilerTestCase("BCE0018-2.boo")

	[Test]
	def @BCE0018_3():
		RunCompilerTestCase("BCE0018-3.boo")

	[Test]
	def @BCE0018_4():
		RunCompilerTestCase("BCE0018-4.boo")

	[Test]
	def @BCE0019_1():
		RunCompilerTestCase("BCE0019-1.boo")

	[Test]
	def @BCE0019_2():
		RunCompilerTestCase("BCE0019-2.boo")

	[Test]
	def @BCE0020_1():
		RunCompilerTestCase("BCE0020-1.boo")

	[Test]
	def @BCE0020_2():
		RunCompilerTestCase("BCE0020-2.boo")

	[Test]
	def @BCE0020_3():
		RunCompilerTestCase("BCE0020-3.boo")

	[Test]
	def @BCE0021_1():
		RunCompilerTestCase("BCE0021-1.boo")

	[Test]
	def @BCE0022_1():
		RunCompilerTestCase("BCE0022-1.boo")

	[Test]
	def @BCE0022_10():
		RunCompilerTestCase("BCE0022-10.boo")

	[Test]
	def @BCE0022_11():
		RunCompilerTestCase("BCE0022-11.boo")

	[Test]
	def @BCE0022_12():
		RunCompilerTestCase("BCE0022-12.boo")

	[Test]
	def @BCE0022_13():
		RunCompilerTestCase("BCE0022-13.boo")

	[Test]
	def @BCE0022_14():
		RunCompilerTestCase("BCE0022-14.boo")

	[Test]
	def @BCE0022_15_strict():
		RunCompilerTestCase("BCE0022-15-strict.boo")

	[Test]
	def @BCE0022_16_strict():
		RunCompilerTestCase("BCE0022-16-strict.boo")

	[Test]
	def @BCE0022_2():
		RunCompilerTestCase("BCE0022-2.boo")

	[Test]
	def @BCE0022_3():
		RunCompilerTestCase("BCE0022-3.boo")

	[Test]
	def @BCE0022_4():
		RunCompilerTestCase("BCE0022-4.boo")

	[Test]
	def @BCE0022_5():
		RunCompilerTestCase("BCE0022-5.boo")

	[Test]
	def @BCE0022_6():
		RunCompilerTestCase("BCE0022-6.boo")

	[Test]
	def @BCE0022_8():
		RunCompilerTestCase("BCE0022-8.boo")

	[Test]
	def @BCE0022_9():
		RunCompilerTestCase("BCE0022-9.boo")

	[Test]
	def @BCE0023_1():
		RunCompilerTestCase("BCE0023-1.boo")

	[Test]
	def @BCE0024_1():
		RunCompilerTestCase("BCE0024-1.boo")

	[Test]
	def @BCE0024_2():
		RunCompilerTestCase("BCE0024-2.boo")

	[Test]
	def @BCE0024_3():
		RunCompilerTestCase("BCE0024-3.boo")

	[Test]
	def @BCE0024_4():
		RunCompilerTestCase("BCE0024-4.boo")

	[Test]
	def @BCE0032_1():
		RunCompilerTestCase("BCE0032-1.boo")

	[Test]
	def @BCE0035_1():
		RunCompilerTestCase("BCE0035-1.boo")

	[Test]
	def @BCE0035_2():
		RunCompilerTestCase("BCE0035-2.boo")

	[Test]
	def @BCE0035_3():
		RunCompilerTestCase("BCE0035-3.boo")

	[Test]
	def @BCE0035_4():
		RunCompilerTestCase("BCE0035-4.boo")

	[Test]
	def @BCE0038_1():
		RunCompilerTestCase("BCE0038-1.boo")

	[Test]
	def @BCE0045_1():
		RunCompilerTestCase("BCE0045-1.boo")

	[Test]
	def @BCE0045_2():
		RunCompilerTestCase("BCE0045-2.boo")

	[Test]
	def @BCE0045_3():
		RunCompilerTestCase("BCE0045-3.boo")

	[Test]
	def @BCE0045_4():
		RunCompilerTestCase("BCE0045-4.boo")

	[Test]
	def @BCE0046_1():
		RunCompilerTestCase("BCE0046-1.boo")

	[Test]
	def @BCE0047_1():
		RunCompilerTestCase("BCE0047-1.boo")

	[Test]
	def @BCE0047_2():
		RunCompilerTestCase("BCE0047-2.boo")

	[Test]
	def @BCE0047_3():
		RunCompilerTestCase("BCE0047-3.boo")

	[Test]
	def @BCE0047_4():
		RunCompilerTestCase("BCE0047-4.boo")

	[Test]
	def @BCE0047_5():
		RunCompilerTestCase("BCE0047-5.boo")

	[Test]
	def @BCE0049_1():
		RunCompilerTestCase("BCE0049-1.boo")

	[Test]
	def @BCE0051_1():
		RunCompilerTestCase("BCE0051-1.boo")

	[Test]
	def @BCE0051_2():
		RunCompilerTestCase("BCE0051-2.boo")

	[Test]
	def @BCE0053_1():
		RunCompilerTestCase("BCE0053-1.boo")

	[Test]
	def @BCE0053_2():
		RunCompilerTestCase("BCE0053-2.boo")

	[Test]
	def @BCE0053_3():
		RunCompilerTestCase("BCE0053-3.boo")

	[Test]
	def @BCE0057_1():
		RunCompilerTestCase("BCE0057-1.boo")

	[Test]
	def @BCE0058_1():
		RunCompilerTestCase("BCE0058-1.boo")

	[Test]
	def @BCE0060_1():
		RunCompilerTestCase("BCE0060-1.boo")

	[Test]
	def @BCE0060_2():
		RunCompilerTestCase("BCE0060-2.boo")

	[Test]
	def @BCE0060_3():
		RunCompilerTestCase("BCE0060-3.boo")

	[Test]
	def @BCE0061_1():
		RunCompilerTestCase("BCE0061-1.boo")

	[Test]
	def @BCE0063_1():
		RunCompilerTestCase("BCE0063-1.boo")

	[Test]
	def @BCE0063_2():
		RunCompilerTestCase("BCE0063-2.boo")

	[Test]
	def @BCE0063_3():
		RunCompilerTestCase("BCE0063-3.boo")

	[Test]
	def @BCE0063_4():
		RunCompilerTestCase("BCE0063-4.boo")

	[Test]
	def @BCE0063_5():
		RunCompilerTestCase("BCE0063-5.boo")

	[Test]
	def @BCE0063_6():
		RunCompilerTestCase("BCE0063-6.boo")

	[Test]
	def @BCE0063_7():
		RunCompilerTestCase("BCE0063-7.boo")

	[Test]
	def @BCE0063_8():
		RunCompilerTestCase("BCE0063-8.boo")

	[Test]
	def @BCE0065_1():
		RunCompilerTestCase("BCE0065-1.boo")

	[Test]
	def @BCE0065_2():
		RunCompilerTestCase("BCE0065-2.boo")

	[Test]
	def @BCE0067_1():
		RunCompilerTestCase("BCE0067-1.boo")

	[Test]
	def @BCE0067_2():
		RunCompilerTestCase("BCE0067-2.boo")

	[Test]
	def @BCE0067_3():
		RunCompilerTestCase("BCE0067-3.boo")

	[Test]
	def @BCE0070_1():
		RunCompilerTestCase("BCE0070-1.boo")

	[Test]
	def @BCE0070_2():
		RunCompilerTestCase("BCE0070-2.boo")

	[Test]
	def @BCE0070_3():
		RunCompilerTestCase("BCE0070-3.boo")

	[Test]
	def @BCE0071_1():
		RunCompilerTestCase("BCE0071-1.boo")

	[Test]
	def @BCE0071_2():
		RunCompilerTestCase("BCE0071-2.boo")

	[Test]
	def @BCE0072_1():
		RunCompilerTestCase("BCE0072-1.boo")

	[Test]
	def @BCE0072_2():
		RunCompilerTestCase("BCE0072-2.boo")

	[Test]
	def @BCE0073_1():
		RunCompilerTestCase("BCE0073-1.boo")

	[Test]
	def @BCE0079_1():
		RunCompilerTestCase("BCE0079-1.boo")

	[Test]
	def @BCE0080_1():
		RunCompilerTestCase("BCE0080-1.boo")

	[Test]
	def @BCE0081_1():
		RunCompilerTestCase("BCE0081-1.boo")

	[Test]
	def @BCE0081_2():
		RunCompilerTestCase("BCE0081-2.boo")

	[Test]
	def @BCE0081_3():
		RunCompilerTestCase("BCE0081-3.boo")

	[Test]
	def @BCE0081_4():
		RunCompilerTestCase("BCE0081-4.boo")

	[Test]
	def @BCE0081_5():
		RunCompilerTestCase("BCE0081-5.boo")

	[Test]
	def @BCE0082_1():
		RunCompilerTestCase("BCE0082-1.boo")

	[Test]
	def @BCE0083_1():
		RunCompilerTestCase("BCE0083-1.boo")

	[Test]
	def @BCE0084_1():
		RunCompilerTestCase("BCE0084-1.boo")

	[Test]
	def @BCE0085_1():
		RunCompilerTestCase("BCE0085-1.boo")

	[Test]
	def @BCE0085_3():
		RunCompilerTestCase("BCE0085-3.boo")

	[Test]
	def @BCE0086_1():
		RunCompilerTestCase("BCE0086-1.boo")

	[Test]
	def @BCE0087_1():
		RunCompilerTestCase("BCE0087-1.boo")

	[Test]
	def @BCE0089_1():
		RunCompilerTestCase("BCE0089-1.boo")

	[Test]
	def @BCE0089_10():
		RunCompilerTestCase("BCE0089-10.boo")

	[Test]
	def @BCE0089_11():
		RunCompilerTestCase("BCE0089-11.boo")

	[Test]
	def @BCE0089_12():
		RunCompilerTestCase("BCE0089-12.boo")

	[Test]
	def @BCE0089_13():
		RunCompilerTestCase("BCE0089-13.boo")

	[Test]
	def @BCE0089_14():
		RunCompilerTestCase("BCE0089-14.boo")

	[Test]
	def @BCE0089_15():
		RunCompilerTestCase("BCE0089-15.boo")

	[Test]
	def @BCE0089_16():
		RunCompilerTestCase("BCE0089-16.boo")

	[Test]
	def @BCE0089_2():
		RunCompilerTestCase("BCE0089-2.boo")

	[Test]
	def @BCE0089_3():
		RunCompilerTestCase("BCE0089-3.boo")

	[Test]
	def @BCE0089_4():
		RunCompilerTestCase("BCE0089-4.boo")

	[Test]
	def @BCE0089_5():
		RunCompilerTestCase("BCE0089-5.boo")

	[Test]
	def @BCE0089_6():
		RunCompilerTestCase("BCE0089-6.boo")

	[Test]
	def @BCE0089_7():
		RunCompilerTestCase("BCE0089-7.boo")

	[Test]
	def @BCE0089_8():
		RunCompilerTestCase("BCE0089-8.boo")

	[Test]
	def @BCE0089_9():
		RunCompilerTestCase("BCE0089-9.boo")

	[Test]
	def @BCE0090_1():
		RunCompilerTestCase("BCE0090-1.boo")

	[Test]
	def @BCE0090_2():
		RunCompilerTestCase("BCE0090-2.boo")

	[Test]
	def @BCE0090_3():
		RunCompilerTestCase("BCE0090-3.boo")

	[Test]
	def @BCE0091_1():
		RunCompilerTestCase("BCE0091-1.boo")

	[Test]
	def @BCE0092_1():
		RunCompilerTestCase("BCE0092-1.boo")

	[Test]
	def @BCE0093_1():
		RunCompilerTestCase("BCE0093-1.boo")

	[Test]
	def @BCE0093_2():
		RunCompilerTestCase("BCE0093-2.boo")

	[Test]
	def @BCE0094_1():
		RunCompilerTestCase("BCE0094-1.boo")

	[Test]
	def @BCE0095_1():
		RunCompilerTestCase("BCE0095-1.boo")

	[Test]
	def @BCE0095_2():
		RunCompilerTestCase("BCE0095-2.boo")

	[Test]
	def @BCE0096_1():
		RunCompilerTestCase("BCE0096-1.boo")

	[Test]
	def @BCE0097_1():
		RunCompilerTestCase("BCE0097-1.boo")

	[Test]
	def @BCE0097_2():
		RunCompilerTestCase("BCE0097-2.boo")

	[Test]
	def @BCE0099_1():
		RunCompilerTestCase("BCE0099-1.boo")

	[Test]
	def @BCE0099_2():
		RunCompilerTestCase("BCE0099-2.boo")

	[Test]
	def @BCE0100_1():
		RunCompilerTestCase("BCE0100-1.boo")

	[Test]
	def @BCE0101_1():
		RunCompilerTestCase("BCE0101-1.boo")

	[Test]
	def @BCE0101_2():
		RunCompilerTestCase("BCE0101-2.boo")

	[Test]
	def @BCE0102_1():
		RunCompilerTestCase("BCE0102-1.boo")

	[Test]
	def @BCE0103_1():
		RunCompilerTestCase("BCE0103-1.boo")

	[Test]
	def @BCE0103_2():
		RunCompilerTestCase("BCE0103-2.boo")

	[Test]
	def @BCE0103_3():
		RunCompilerTestCase("BCE0103-3.boo")

	[Test]
	def @BCE0103_cannot_extend_array():
		RunCompilerTestCase("BCE0103-cannot-extend-array.boo")

	[Test]
	def @BCE0104_1():
		RunCompilerTestCase("BCE0104-1.boo")

	[Test]
	def @BCE0105_1():
		RunCompilerTestCase("BCE0105-1.boo")

	[Test]
	def @BCE0107_1():
		RunCompilerTestCase("BCE0107-1.boo")

	[Test]
	def @BCE0108_1():
		RunCompilerTestCase("BCE0108-1.boo")

	[Test]
	def @BCE0111_1():
		RunCompilerTestCase("BCE0111-1.boo")

	[Test]
	def @BCE0112_1():
		RunCompilerTestCase("BCE0112-1.boo")

	[Test]
	def @BCE0114_1():
		RunCompilerTestCase("BCE0114-1.boo")

	[Test]
	def @BCE0115_1():
		RunCompilerTestCase("BCE0115-1.boo")

	[Test]
	def @BCE0116_1():
		RunCompilerTestCase("BCE0116-1.boo")

	[Test]
	def @BCE0117_1():
		RunCompilerTestCase("BCE0117-1.boo")

	[Test]
	def @BCE0120_1():
		RunCompilerTestCase("BCE0120-1.boo")

	[Test]
	def @BCE0120_2():
		RunCompilerTestCase("BCE0120-2.boo")

	[Test]
	def @BCE0120_3():
		RunCompilerTestCase("BCE0120-3.boo")

	[Test]
	def @BCE0120_4():
		RunCompilerTestCase("BCE0120-4.boo")

	[Test]
	def @BCE0120_5():
		RunCompilerTestCase("BCE0120-5.boo")

	[Test]
	def @BCE0120_6():
		RunCompilerTestCase("BCE0120-6.boo")

	[Test]
	def @BCE0121_1():
		RunCompilerTestCase("BCE0121-1.boo")

	[Test]
	def @BCE0122_1():
		RunCompilerTestCase("BCE0122-1.boo")

	[Test]
	def @BCE0123_1():
		RunCompilerTestCase("BCE0123-1.boo")

	[Test]
	def @BCE0123_2():
		RunCompilerTestCase("BCE0123-2.boo")

	[Test]
	def @BCE0124_1():
		RunCompilerTestCase("BCE0124-1.boo")

	[Test]
	def @BCE0125_1():
		RunCompilerTestCase("BCE0125-1.boo")

	[Test]
	def @BCE0126_1():
		RunCompilerTestCase("BCE0126-1.boo")

	[Test]
	def @BCE0126_2():
		RunCompilerTestCase("BCE0126-2.boo")

	[Test]
	def @BCE0126_3():
		RunCompilerTestCase("BCE0126-3.boo")

	[Test]
	def @BCE0126_4():
		RunCompilerTestCase("BCE0126-4.boo")

	[Test]
	def @BCE0126_5():
		RunCompilerTestCase("BCE0126-5.boo")

	[Test]
	def @BCE0126_6():
		RunCompilerTestCase("BCE0126-6.boo")

	[Test]
	def @BCE0126_7():
		RunCompilerTestCase("BCE0126-7.boo")

	[Test]
	def @BCE0127_1():
		RunCompilerTestCase("BCE0127-1.boo")

	[Test]
	def @BCE0127_2():
		RunCompilerTestCase("BCE0127-2.boo")

	[Test]
	def @BCE0128_1():
		RunCompilerTestCase("BCE0128-1.boo")

	[Test]
	def @BCE0129_1():
		RunCompilerTestCase("BCE0129-1.boo")

	[Test]
	def @BCE0130_1():
		RunCompilerTestCase("BCE0130-1.boo")

	[Test]
	def @BCE0131_1():
		RunCompilerTestCase("BCE0131-1.boo")

	[Test]
	def @BCE0131_2():
		RunCompilerTestCase("BCE0131-2.boo")

	[Test]
	def @BCE0132_1():
		RunCompilerTestCase("BCE0132-1.boo")

	[Test]
	def @BCE0132_2():
		RunCompilerTestCase("BCE0132-2.boo")

	[Test]
	def @BCE0132_3():
		RunCompilerTestCase("BCE0132-3.boo")

	[Test]
	def @BCE0132_4():
		RunCompilerTestCase("BCE0132-4.boo")

	[Test]
	def @BCE0132_5():
		RunCompilerTestCase("BCE0132-5.boo")

	[Test]
	def @BCE0133_1():
		RunCompilerTestCase("BCE0133-1.boo")

	[Test]
	def @BCE0133_2():
		RunCompilerTestCase("BCE0133-2.boo")

	[Test]
	def @BCE0134_1():
		RunCompilerTestCase("BCE0134-1.boo")

	[Test]
	def @BCE0134_2():
		RunCompilerTestCase("BCE0134-2.boo")

	[Test]
	def @BCE0136_1():
		RunCompilerTestCase("BCE0136-1.boo")

	[Test]
	def @BCE0137_1():
		RunCompilerTestCase("BCE0137-1.boo")

	[Test]
	def @BCE0137_2():
		RunCompilerTestCase("BCE0137-2.boo")

	[Test]
	def @BCE0137_3():
		RunCompilerTestCase("BCE0137-3.boo")

	[Test]
	def @BCE0141():
		RunCompilerTestCase("BCE0141.boo")

	[Test]
	def @BCE0142():
		RunCompilerTestCase("BCE0142.boo")

	[Test]
	def @BCE0143_1():
		RunCompilerTestCase("BCE0143-1.boo")

	[Test]
	def @BCE0145_1():
		RunCompilerTestCase("BCE0145-1.boo")

	[Test]
	def @BCE0150_1():
		RunCompilerTestCase("BCE0150-1.boo")

	[Test]
	def @BCE0151_1():
		RunCompilerTestCase("BCE0151-1.boo")

	[Test]
	def @BCE0151_2():
		RunCompilerTestCase("BCE0151-2.boo")

	[Test]
	def @BCE0152_1():
		RunCompilerTestCase("BCE0152-1.boo")

	[Test]
	def @BCE0153_1():
		RunCompilerTestCase("BCE0153-1.boo")

	[Test]
	def @BCE0153_2():
		RunCompilerTestCase("BCE0153-2.boo")

	[Test]
	def @BCE0154_1():
		RunCompilerTestCase("BCE0154-1.boo")

	[Test]
	def @BCE0154_2():
		RunCompilerTestCase("BCE0154-2.boo")

	[Test]
	def @BCE0155_1():
		RunCompilerTestCase("BCE0155-1.boo")

	[Test]
	def @BCE0156_1():
		RunCompilerTestCase("BCE0156-1.boo")

	[Test]
	def @BCE0157_1():
		RunCompilerTestCase("BCE0157-1.boo")

	[Test]
	def @BCE0158_1():
		RunCompilerTestCase("BCE0158-1.boo")

	[Test]
	def @BCE0165_1():
		RunCompilerTestCase("BCE0165-1.boo")

	[Test]
	def @BCE0166_1():
		RunCompilerTestCase("BCE0166-1.boo")

	[Test]
	def @BCE0167_1():
		RunCompilerTestCase("BCE0167-1.boo")

	[Test]
	def @BCE0169_1():
		RunCompilerTestCase("BCE0169-1.boo")

	[Test]
	def @BCE0170_1():
		RunCompilerTestCase("BCE0170-1.boo")

	[Test]
	def @BCE0171_1():
		RunCompilerTestCase("BCE0171-1.boo")

	[Test]
	def @BCE0173_1():
		RunCompilerTestCase("BCE0173-1.boo")

	[Test]
	def @BCE0175():
		RunCompilerTestCase("BCE0175.boo")

	[Test]
	def @BCE0176_1():
		RunCompilerTestCase("BCE0176-1.boo")

	[Test]
	def @BCE0183_1():
		RunCompilerTestCase("BCE0183-1.boo")

	[Test]
	def @BCE0183_2():
		RunCompilerTestCase("BCE0183-2.boo")

	[Test]
	def @BCE0183_3():
		RunCompilerTestCase("BCE0183-3.boo")

	[Test]
	def @BCE0184_1():
		RunCompilerTestCase("BCE0184-1.boo")

	[Test]
	def @BCE0185_1():
		RunCompilerTestCase("BCE0185-1.boo")

	[Test]
	def @BCE0185_2():
		RunCompilerTestCase("BCE0185-2.boo")

	[Test]
	def @BCE0187_1():
		RunCompilerTestCase("BCE0187-1.boo")

	[Test]
	def @BCE0188_1():
		RunCompilerTestCase("BCE0188-1.boo")

	[Test]
	def @CannotConvertFooToInt():
		RunCompilerTestCase("CannotConvertFooToInt.boo")

	[Test]
	def @cannot_convert_enum_to_single():
		RunCompilerTestCase("cannot-convert-enum-to-single.boo")

	[Test]
	def @docstring_written_twice():
		RunCompilerTestCase("docstring-written-twice.boo")

	[Test]
	def @error_doesnt_cascade_to_cast():
		RunCompilerTestCase("error-doesnt-cascade-to-cast.boo")

	[Test]
	def @error_doesnt_cascade_to_raise():
		RunCompilerTestCase("error-doesnt-cascade-to-raise.boo")

	[Test]
	def @invalid_array_slice():
		RunCompilerTestCase("invalid-array-slice.boo")

	[Test]
	def @invalid_generic_extension_1():
		RunCompilerTestCase("invalid-generic-extension-1.boo")

	[Test]
	def @invalid_generic_extension_2():
		RunCompilerTestCase("invalid-generic-extension-2.boo")

	[Test]
	def @invalid_matrix_index():
		RunCompilerTestCase("invalid-matrix-index.boo")

	[Test]
	def @mismatched_collection_initializers():
		RunCompilerTestCase("mismatched-collection-initializers.boo")

	[Test]
	def @selective_import_2():
		RunCompilerTestCase("selective-import-2.boo")

	[Test]
	def @selective_import():
		RunCompilerTestCase("selective-import.boo")

	[Test]
	def @single_error_on_missing_import_namespace():
		RunCompilerTestCase("single-error-on-missing-import-namespace.boo")

	[Test]
	def @strict_1():
		RunCompilerTestCase("strict-1.boo")

	[Test]
	def @warnaserror_1():
		RunCompilerTestCase("warnaserror-1.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "errors"
