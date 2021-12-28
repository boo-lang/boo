
namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class RegressionTestFixture : AbstractCompilerTestCase
	{


		[Test]
		public void array_ldelem()
		{
			RunCompilerTestCase(@"array_ldelem.boo");
		}
		
		[Test]
		public void BOO_1005_1()
		{
			RunCompilerTestCase(@"BOO-1005-1.boo");
		}
		
		[Test]
		public void BOO_1005_2()
		{
			RunCompilerTestCase(@"BOO-1005-2.boo");
		}
		
		[Test]
		public void BOO_1006_1()
		{
			RunCompilerTestCase(@"BOO-1006-1.boo");
		}
		
		[Test]
		public void BOO_1008_1()
		{
			RunCompilerTestCase(@"BOO-1008-1.boo");
		}
		
		[Test]
		public void BOO_1009_1()
		{
			RunCompilerTestCase(@"BOO-1009-1.boo");
		}
		
		[Test]
		public void BOO_1013_1()
		{
			RunCompilerTestCase(@"BOO-1013-1.boo");
		}
		
		[Test]
		public void BOO_1013_2()
		{
			RunCompilerTestCase(@"BOO-1013-2.boo");
		}
		
		[Test]
		public void BOO_1016_1()
		{
			RunCompilerTestCase(@"BOO-1016-1.boo");
		}
		
		[Test]
		public void BOO_1016_2()
		{
			RunCompilerTestCase(@"BOO-1016-2.boo");
		}
		
		[Test]
		public void BOO_1027_1()
		{
			RunCompilerTestCase(@"BOO-1027-1.boo");
		}
		
		[Test]
		public void BOO_1031_1()
		{
			RunCompilerTestCase(@"BOO-1031-1.boo");
		}
		
		[Test]
		public void BOO_1031_2()
		{
			RunCompilerTestCase(@"BOO-1031-2.boo");
		}
		
		[Test]
		public void BOO_1031_3()
		{
			RunCompilerTestCase(@"BOO-1031-3.boo");
		}
		
		[Test]
		public void BOO_1031_4()
		{
			RunCompilerTestCase(@"BOO-1031-4.boo");
		}
		
		[Test]
		public void boo_1032_1()
		{
			RunCompilerTestCase(@"boo-1032-1.boo");
		}
		
		[Test]
		public void boo_1032_2()
		{
			RunCompilerTestCase(@"boo-1032-2.boo");
		}
		
		[Test]
		public void BOO_1035_1()
		{
			RunCompilerTestCase(@"BOO-1035-1.boo");
		}
		
		[Test]
		public void BOO_104_1()
		{
			RunCompilerTestCase(@"BOO-104-1.boo");
		}
		
		[Test]
		public void BOO_1040_1()
		{
			RunCompilerTestCase(@"BOO-1040-1.boo");
		}
		
		[Test]
		public void BOO_1047()
		{
			RunCompilerTestCase(@"BOO-1047.boo");
		}
		
		[Test]
		public void boo_1051()
		{
			RunCompilerTestCase(@"boo-1051.boo");
		}
		
		[Test]
		public void BOO_1069_1()
		{
			RunCompilerTestCase(@"BOO-1069-1.boo");
		}
		
		[Test]
		public void BOO_1070_1()
		{
			RunCompilerTestCase(@"BOO-1070-1.boo");
		}
		
		[Test]
		public void BOO_1071()
		{
			RunCompilerTestCase(@"BOO-1071.boo");
		}
		
		[Test]
		public void BOO_1076()
		{
			RunCompilerTestCase(@"BOO-1076.boo");
		}
		
		[Ignore("broken by SRE bug")][Test]
		public void BOO_1078_1()
		{
			RunCompilerTestCase(@"BOO-1078-1.boo");
		}
		
		[Test]
		public void BOO_1080_1()
		{
			RunCompilerTestCase(@"BOO-1080-1.boo");
		}
		
		[Test]
		public void BOO_1082()
		{
			RunCompilerTestCase(@"BOO-1082.boo");
		}
		
		[Test]
		public void BOO_1085()
		{
			RunCompilerTestCase(@"BOO-1085.boo");
		}
		
		[Test]
		public void BOO_1086()
		{
			RunCompilerTestCase(@"BOO-1086.boo");
		}

#if !NET
		[Test]
		public void BOO_1088_1()
		{
			RunCompilerTestCase(@"BOO-1088-1.boo");
		}
#endif

		[Test]
		public void BOO_1091_1()
		{
			RunCompilerTestCase(@"BOO-1091-1.boo");
		}
		
		[Test]
		public void BOO_1095_1()
		{
			RunCompilerTestCase(@"BOO-1095-1.boo");
		}
		
		[Test]
		public void BOO_1111_1()
		{
			RunCompilerTestCase(@"BOO-1111-1.boo");
		}
		
		[Test]
		public void BOO_1127_1()
		{
			RunCompilerTestCase(@"BOO-1127-1.boo");
		}
		
		[Test]
		public void BOO_1130_1()
		{
			RunCompilerTestCase(@"BOO-1130-1.boo");
		}
		
		[Test]
		public void BOO_1135_1()
		{
			RunCompilerTestCase(@"BOO-1135-1.boo");
		}
		
		[Test]
		public void BOO_1147_1()
		{
			RunCompilerTestCase(@"BOO-1147-1.boo");
		}
		
		[Test]
		public void BOO_1147_2()
		{
			RunCompilerTestCase(@"BOO-1147-2.boo");
		}
		
		[Test]
		public void BOO_1154_1()
		{
			RunCompilerTestCase(@"BOO-1154-1.boo");
		}
		
		[Test]
		public void BOO_1160_1()
		{
			RunCompilerTestCase(@"BOO-1160-1.boo");
		}
		
		[Test]
		public void BOO_1162_1()
		{
			RunCompilerTestCase(@"BOO-1162-1.boo");
		}
		
		[Test]
		public void BOO_1170()
		{
			RunCompilerTestCase(@"BOO-1170.boo");
		}
		
		[Test]
		public void BOO_1171_1()
		{
			RunCompilerTestCase(@"BOO-1171-1.boo");
		}
		
		[Test]
		public void BOO_1176_1()
		{
			RunCompilerTestCase(@"BOO-1176-1.boo");
		}
		
		[Test]
		public void BOO_1177_1()
		{
			RunCompilerTestCase(@"BOO-1177-1.boo");
		}
		
		[Test]
		public void BOO_1206_1()
		{
			RunCompilerTestCase(@"BOO-1206-1.boo");
		}
		
		[Test]
		public void BOO_121_1()
		{
			RunCompilerTestCase(@"BOO-121-1.boo");
		}
		
		[Test]
		public void BOO_1210_1()
		{
			RunCompilerTestCase(@"BOO-1210-1.boo");
		}
		
		[Test]
		public void BOO_1217_1()
		{
			RunCompilerTestCase(@"BOO-1217-1.boo");
		}
		
		[Test]
		public void BOO_122_1()
		{
			RunCompilerTestCase(@"BOO-122-1.boo");
		}
		
		[Test]
		public void BOO_1220_1()
		{
			RunCompilerTestCase(@"BOO-1220-1.boo");
		}
		
		[Test]
		public void BOO_123_1()
		{
			RunCompilerTestCase(@"BOO-123-1.boo");
		}
		
		[Test]
		public void BOO_1256()
		{
			RunCompilerTestCase(@"BOO-1256.boo");
		}
		
		[Test]
		public void BOO_1261_1()
		{
			RunCompilerTestCase(@"BOO-1261-1.boo");
		}
		
		[Ignore("WIP")][Test]
		public void BOO_1264()
		{
			RunCompilerTestCase(@"BOO-1264.boo");
		}
		
		[Test]
		public void BOO_1288()
		{
			RunCompilerTestCase(@"BOO-1288.boo");
		}
		
		[Test]
		public void BOO_129_1()
		{
			RunCompilerTestCase(@"BOO-129-1.boo");
		}
		
		[Test]
		public void BOO_1290()
		{
			RunCompilerTestCase(@"BOO-1290.boo");
		}
		
		[Test]
		public void BOO_1306()
		{
			RunCompilerTestCase(@"BOO-1306.boo");
		}
		
		[Test]
		public void BOO_1307()
		{
			RunCompilerTestCase(@"BOO-1307.boo");
		}
		
		[Test]
		public void BOO_1308()
		{
			RunCompilerTestCase(@"BOO-1308.boo");
		}
		
		[Test]
		public void BOO_138_1()
		{
			RunCompilerTestCase(@"BOO-138-1.boo");
		}
		
		[Test]
		public void BOO_145_1()
		{
			RunCompilerTestCase(@"BOO-145-1.boo");
		}
		
		[Test]
		public void BOO_176_1()
		{
			RunCompilerTestCase(@"BOO-176-1.boo");
		}
		
		[Test]
		public void BOO_178_1()
		{
			RunCompilerTestCase(@"BOO-178-1.boo");
		}
		
		[Test]
		public void BOO_178_2()
		{
			RunCompilerTestCase(@"BOO-178-2.boo");
		}
		
		[Test]
		public void BOO_189_1()
		{
			RunCompilerTestCase(@"BOO-189-1.boo");
		}
		
		[Test]
		public void BOO_189_2()
		{
			RunCompilerTestCase(@"BOO-189-2.boo");
		}
		
		[Test]
		public void BOO_195_1()
		{
			RunCompilerTestCase(@"BOO-195-1.boo");
		}
		
		[Test]
		public void BOO_203_1()
		{
			RunCompilerTestCase(@"BOO-203-1.boo");
		}
		
		[Test]
		public void BOO_210_1()
		{
			RunCompilerTestCase(@"BOO-210-1.boo");
		}
		
		[Test]
		public void BOO_226_1()
		{
			RunCompilerTestCase(@"BOO-226-1.boo");
		}
		
		[Test]
		public void BOO_226_2()
		{
			RunCompilerTestCase(@"BOO-226-2.boo");
		}
		
		[Test]
		public void BOO_227_1()
		{
			RunCompilerTestCase(@"BOO-227-1.boo");
		}
		
		[Test]
		public void BOO_231_1()
		{
			RunCompilerTestCase(@"BOO-231-1.boo");
		}
		
		[Test]
		public void BOO_241_1()
		{
			RunCompilerTestCase(@"BOO-241-1.boo");
		}
		
		[Test]
		public void BOO_248_1()
		{
			RunCompilerTestCase(@"BOO-248-1.boo");
		}
		
		[Test]
		public void BOO_260_1()
		{
			RunCompilerTestCase(@"BOO-260-1.boo");
		}
		
		[Test]
		public void BOO_265_1()
		{
			RunCompilerTestCase(@"BOO-265-1.boo");
		}
		
		[Test]
		public void BOO_270_1()
		{
			RunCompilerTestCase(@"BOO-270-1.boo");
		}
		
		[Test]
		public void BOO_281_1()
		{
			RunCompilerTestCase(@"BOO-281-1.boo");
		}
		
		[Test]
		public void BOO_281_2()
		{
			RunCompilerTestCase(@"BOO-281-2.boo");
		}
		
		[Test]
		public void BOO_301_1()
		{
			RunCompilerTestCase(@"BOO-301-1.boo");
		}
		
		[Test]
		public void BOO_301_2()
		{
			RunCompilerTestCase(@"BOO-301-2.boo");
		}
		
		[Test]
		public void BOO_308_1()
		{
			RunCompilerTestCase(@"BOO-308-1.boo");
		}
		
		[Test]
		public void BOO_308_2()
		{
			RunCompilerTestCase(@"BOO-308-2.boo");
		}
		
		[Test]
		public void BOO_313_1()
		{
			RunCompilerTestCase(@"BOO-313-1.boo");
		}
		
		[Test]
		public void BOO_313_10()
		{
			RunCompilerTestCase(@"BOO-313-10.boo");
		}
		
		[Test]
		public void BOO_313_11()
		{
			RunCompilerTestCase(@"BOO-313-11.boo");
		}
		
		[Test]
		public void BOO_313_12()
		{
			RunCompilerTestCase(@"BOO-313-12.boo");
		}
		
		[Test]
		public void BOO_313_2()
		{
			RunCompilerTestCase(@"BOO-313-2.boo");
		}
		
		[Test]
		public void BOO_313_3()
		{
			RunCompilerTestCase(@"BOO-313-3.boo");
		}
		
		[Test]
		public void BOO_313_4()
		{
			RunCompilerTestCase(@"BOO-313-4.boo");
		}
		
		[Test]
		public void BOO_313_5()
		{
			RunCompilerTestCase(@"BOO-313-5.boo");
		}
		
		[Test]
		public void BOO_313_6()
		{
			RunCompilerTestCase(@"BOO-313-6.boo");
		}
		
		[Test]
		public void BOO_313_7()
		{
			RunCompilerTestCase(@"BOO-313-7.boo");
		}
		
		[Test]
		public void BOO_313_8()
		{
			RunCompilerTestCase(@"BOO-313-8.boo");
		}
		
		[Test]
		public void BOO_313_9()
		{
			RunCompilerTestCase(@"BOO-313-9.boo");
		}
		
		[Test]
		public void BOO_327_1()
		{
			RunCompilerTestCase(@"BOO-327-1.boo");
		}
		
		[Test]
		public void BOO_338_1()
		{
			RunCompilerTestCase(@"BOO-338-1.boo");
		}
		
		[Test]
		public void BOO_356_1()
		{
			RunCompilerTestCase(@"BOO-356-1.boo");
		}
		
		[Test]
		public void BOO_357_1()
		{
			RunCompilerTestCase(@"BOO-357-1.boo");
		}
		
		[Test]
		public void BOO_357_2()
		{
			RunCompilerTestCase(@"BOO-357-2.boo");
		}
		
		[Test]
		public void BOO_366_1()
		{
			RunCompilerTestCase(@"BOO-366-1.boo");
		}
		
		[Test]
		public void BOO_366_2()
		{
			RunCompilerTestCase(@"BOO-366-2.boo");
		}
		
		[Test]
		public void BOO_367_1()
		{
			RunCompilerTestCase(@"BOO-367-1.boo");
		}
		
		[Test]
		public void BOO_368_1()
		{
			RunCompilerTestCase(@"BOO-368-1.boo");
		}
		
		[Test]
		public void BOO_369_1()
		{
			RunCompilerTestCase(@"BOO-369-1.boo");
		}
		
		[Test]
		public void BOO_369_2()
		{
			RunCompilerTestCase(@"BOO-369-2.boo");
		}
		
		[Test]
		public void BOO_372_1()
		{
			RunCompilerTestCase(@"BOO-372-1.boo");
		}
		
		[Test]
		public void BOO_390_1()
		{
			RunCompilerTestCase(@"BOO-390-1.boo");
		}
		
		[Test]
		public void BOO_396_1()
		{
			RunCompilerTestCase(@"BOO-396-1.boo");
		}
		
		[Test]
		public void BOO_398_1()
		{
			RunCompilerTestCase(@"BOO-398-1.boo");
		}
		
		[Test]
		public void BOO_40_1()
		{
			RunCompilerTestCase(@"BOO-40-1.boo");
		}
		
		[Test]
		public void BOO_407_1()
		{
			RunCompilerTestCase(@"BOO-407-1.boo");
		}
		
		[Test]
		public void BOO_408_1()
		{
			RunCompilerTestCase(@"BOO-408-1.boo");
		}
		
		[Test]
		public void BOO_411_1()
		{
			RunCompilerTestCase(@"BOO-411-1.boo");
		}
		
		[Test]
		public void BOO_417_1()
		{
			RunCompilerTestCase(@"BOO-417-1.boo");
		}
		
		[Test]
		public void BOO_420_1()
		{
			RunCompilerTestCase(@"BOO-420-1.boo");
		}
		
		[Test]
		public void BOO_440_1()
		{
			RunCompilerTestCase(@"BOO-440-1.boo");
		}
		
		[Test]
		public void BOO_440_2()
		{
			RunCompilerTestCase(@"BOO-440-2.boo");
		}
		
		[Test]
		public void BOO_440_3()
		{
			RunCompilerTestCase(@"BOO-440-3.boo");
		}
		
		[Test]
		public void BOO_441_1()
		{
			RunCompilerTestCase(@"BOO-441-1.boo");
		}
		
		[Test]
		public void BOO_441_2()
		{
			RunCompilerTestCase(@"BOO-441-2.boo");
		}
		
		[Test]
		public void BOO_46_1()
		{
			RunCompilerTestCase(@"BOO-46-1.boo");
		}
		
		[Test]
		public void BOO_46_2()
		{
			RunCompilerTestCase(@"BOO-46-2.boo");
		}
		
		[Test]
		public void BOO_464_1()
		{
			RunCompilerTestCase(@"BOO-464-1.boo");
		}
		
		[Test]
		public void BOO_474_1()
		{
			RunCompilerTestCase(@"BOO-474-1.boo");
		}
		
		[Test]
		public void BOO_540_1()
		{
			RunCompilerTestCase(@"BOO-540-1.boo");
		}
		
		[Test]
		public void BOO_540_2()
		{
			RunCompilerTestCase(@"BOO-540-2.boo");
		}
		
		[Test]
		public void BOO_549_1()
		{
			RunCompilerTestCase(@"BOO-549-1.boo");
		}
		
		[Category("FailsOnMono")][Test]
		public void BOO_569_1()
		{
			RunCompilerTestCase(@"BOO-569-1.boo");
		}
		
		[Test]
		public void BOO_585_1()
		{
			RunCompilerTestCase(@"BOO-585-1.boo");
		}
		
		[Test]
		public void BOO_590_1()
		{
			RunCompilerTestCase(@"BOO-590-1.boo");
		}
		
		[Test]
		public void BOO_603_1()
		{
			RunCompilerTestCase(@"BOO-603-1.boo");
		}
		
		[Test]
		public void BOO_605_1()
		{
			RunCompilerTestCase(@"BOO-605-1.boo");
		}
		
		[Test]
		public void BOO_608_1()
		{
			RunCompilerTestCase(@"BOO-608-1.boo");
		}
		
		[Test]
		public void BOO_612_1()
		{
			RunCompilerTestCase(@"BOO-612-1.boo");
		}
		
		[Test]
		public void BOO_612_2()
		{
			RunCompilerTestCase(@"BOO-612-2.boo");
		}
		
		[Test]
		public void BOO_632_1()
		{
			RunCompilerTestCase(@"BOO-632-1.boo");
		}
		
		[Test]
		public void BOO_642_1()
		{
			RunCompilerTestCase(@"BOO-642-1.boo");
		}
		
		[Test]
		public void BOO_650_1()
		{
			RunCompilerTestCase(@"BOO-650-1.boo");
		}
		
		[Test]
		public void BOO_651_1()
		{
			RunCompilerTestCase(@"BOO-651-1.boo");
		}
		
		[Test]
		public void BOO_656_1()
		{
			RunCompilerTestCase(@"BOO-656-1.boo");
		}
		
		[Test]
		public void BOO_662_1()
		{
			RunCompilerTestCase(@"BOO-662-1.boo");
		}
		
		[Test]
		public void BOO_662_2()
		{
			RunCompilerTestCase(@"BOO-662-2.boo");
		}
		
		[Test]
		public void BOO_684_1()
		{
			RunCompilerTestCase(@"BOO-684-1.boo");
		}
		
		[Test]
		public void BOO_685_1()
		{
			RunCompilerTestCase(@"BOO-685-1.boo");
		}
		
		[Test]
		public void BOO_697_1()
		{
			RunCompilerTestCase(@"BOO-697-1.boo");
		}
		
		[Test]
		public void BOO_698_1()
		{
			RunCompilerTestCase(@"BOO-698-1.boo");
		}
		
		[Test]
		public void BOO_705_1()
		{
			RunCompilerTestCase(@"BOO-705-1.boo");
		}

#if !NET
		[Category("FailsOnMono")][Test]
		public void BOO_707_1()
		{
			RunCompilerTestCase(@"BOO-707-1.boo");
		}
#endif

		[Category("FailsOnMono")][Test]
		public void BOO_709_1()
		{
			RunCompilerTestCase(@"BOO-709-1.boo");
		}
		
		[Test]
		public void BOO_710_1()
		{
			RunCompilerTestCase(@"BOO-710-1.boo");
		}
		
		[Test]
		public void BOO_714_1()
		{
			RunCompilerTestCase(@"BOO-714-1.boo");
		}
		
		[Test]
		public void BOO_719_1()
		{
			RunCompilerTestCase(@"BOO-719-1.boo");
		}
		
		[Test]
		public void BOO_719_2()
		{
			RunCompilerTestCase(@"BOO-719-2.boo");
		}
		
		[Test]
		public void BOO_723_1()
		{
			RunCompilerTestCase(@"BOO-723-1.boo");
		}
		
		[Test]
		public void BOO_724_1()
		{
			RunCompilerTestCase(@"BOO-724-1.boo");
		}
		
		[Test]
		public void BOO_724_2()
		{
			RunCompilerTestCase(@"BOO-724-2.boo");
		}
		
		[Test]
		public void BOO_725_1()
		{
			RunCompilerTestCase(@"BOO-725-1.boo");
		}
		
		[Test]
		public void BOO_729_1()
		{
			RunCompilerTestCase(@"BOO-729-1.boo");
		}
		
		[Test]
		public void BOO_736_1()
		{
			RunCompilerTestCase(@"BOO-736-1.boo");
		}
		
		[Test]
		public void BOO_739_1()
		{
			RunCompilerTestCase(@"BOO-739-1.boo");
		}
		
		[Test]
		public void BOO_739_2()
		{
			RunCompilerTestCase(@"BOO-739-2.boo");
		}
		
		[Test]
		public void BOO_747_1()
		{
			RunCompilerTestCase(@"BOO-747-1.boo");
		}
		
		[Test]
		public void BOO_747_2()
		{
			RunCompilerTestCase(@"BOO-747-2.boo");
		}
		
		[Test]
		public void BOO_748_1()
		{
			RunCompilerTestCase(@"BOO-748-1.boo");
		}
		
		[Test]
		public void BOO_75_1()
		{
			RunCompilerTestCase(@"BOO-75-1.boo");
		}
		
		[Test]
		public void BOO_753_1()
		{
			RunCompilerTestCase(@"BOO-753-1.boo");
		}
		
		[Test]
		public void BOO_753_2()
		{
			RunCompilerTestCase(@"BOO-753-2.boo");
		}
		
		[Test]
		public void BOO_76_1()
		{
			RunCompilerTestCase(@"BOO-76-1.boo");
		}
		
		[Test]
		public void BOO_76_2()
		{
			RunCompilerTestCase(@"BOO-76-2.boo");
		}
		
		[Test]
		public void BOO_77_1()
		{
			RunCompilerTestCase(@"BOO-77-1.boo");
		}
		
		[Test]
		public void BOO_770_1()
		{
			RunCompilerTestCase(@"BOO-770-1.boo");
		}
		
		[Ignore("Preference for generic not complete")][Test]
		public void BOO_779_1()
		{
			RunCompilerTestCase(@"BOO-779-1.boo");
		}
		
		[Ignore("Preference for generic not complete")][Test]
		public void BOO_779_2()
		{
			RunCompilerTestCase(@"BOO-779-2.boo");
		}
		
		[Ignore("Non-IEnumerable definitions of GetEnumerator() not yet supported")][Test]
		public void BOO_779_3()
		{
			RunCompilerTestCase(@"BOO-779-3.boo");
		}
		
		[Test]
		public void BOO_779_4()
		{
			RunCompilerTestCase(@"BOO-779-4.boo");
		}
		
		[Test]
		public void BOO_792_1()
		{
			RunCompilerTestCase(@"BOO-792-1.boo");
		}
		
		[Test]
		public void BOO_792_2()
		{
			RunCompilerTestCase(@"BOO-792-2.boo");
		}
		
		[Test]
		public void BOO_799_1()
		{
			RunCompilerTestCase(@"BOO-799-1.boo");
		}
		
		[Test]
		public void BOO_806_1()
		{
			RunCompilerTestCase(@"BOO-806-1.boo");
		}
		
		[Test]
		public void BOO_809_1()
		{
			RunCompilerTestCase(@"BOO-809-1.boo");
		}
		
		[Test]
		public void BOO_809_2()
		{
			RunCompilerTestCase(@"BOO-809-2.boo");
		}
		
		[Test]
		public void BOO_809_3()
		{
			RunCompilerTestCase(@"BOO-809-3.boo");
		}
		
		[Test]
		public void BOO_809_4()
		{
			RunCompilerTestCase(@"BOO-809-4.boo");
		}
		
		[Test]
		public void BOO_813_1()
		{
			RunCompilerTestCase(@"BOO-813-1.boo");
		}
		
		[Test]
		public void BOO_826()
		{
			RunCompilerTestCase(@"BOO-826.boo");
		}
		
		[Test]
		public void BOO_835_1()
		{
			RunCompilerTestCase(@"BOO-835-1.boo");
		}
		
		[Test]
		public void BOO_844_1()
		{
			RunCompilerTestCase(@"BOO-844-1.boo");
		}
		
		[Test]
		public void BOO_844_2()
		{
			RunCompilerTestCase(@"BOO-844-2.boo");
		}
		
		[Test]
		public void BOO_85_1()
		{
			RunCompilerTestCase(@"BOO-85-1.boo");
		}
		
		[Test]
		public void BOO_860_1()
		{
			RunCompilerTestCase(@"BOO-860-1.boo");
		}
		
		[Test]
		public void BOO_861_1()
		{
			RunCompilerTestCase(@"BOO-861-1.boo");
		}
		
		[Test]
		public void BOO_862_1()
		{
			RunCompilerTestCase(@"BOO-862-1.boo");
		}
		
		[Test]
		public void BOO_862_2()
		{
			RunCompilerTestCase(@"BOO-862-2.boo");
		}
		
		[Test]
		public void BOO_864_1()
		{
			RunCompilerTestCase(@"BOO-864-1.boo");
		}
		
		[Test]
		public void BOO_865_1()
		{
			RunCompilerTestCase(@"BOO-865-1.boo");
		}
		
		[Test]
		public void BOO_88_1()
		{
			RunCompilerTestCase(@"BOO-88-1.boo");
		}
		
		[Test]
		public void BOO_882()
		{
			RunCompilerTestCase(@"BOO-882.boo");
		}
		
		[Test]
		public void BOO_893_1()
		{
			RunCompilerTestCase(@"BOO-893-1.boo");
		}
		
		[Test]
		public void BOO_90_1()
		{
			RunCompilerTestCase(@"BOO-90-1.boo");
		}
		
		[Test]
		public void BOO_913_1()
		{
			RunCompilerTestCase(@"BOO-913-1.boo");
		}
		
		[Test]
		public void BOO_935_1()
		{
			RunCompilerTestCase(@"BOO-935-1.boo");
		}
		
		[Test]
		public void BOO_935_2()
		{
			RunCompilerTestCase(@"BOO-935-2.boo");
		}
		
		[Test]
		public void BOO_948_1()
		{
			RunCompilerTestCase(@"BOO-948-1.boo");
		}
		
		[Test]
		public void BOO_949_1()
		{
			RunCompilerTestCase(@"BOO-949-1.boo");
		}
		
		[Test]
		public void BOO_949_2()
		{
			RunCompilerTestCase(@"BOO-949-2.boo");
		}
		
		[Test]
		public void BOO_952_1()
		{
			RunCompilerTestCase(@"BOO-952-1.boo");
		}
		
		[Test]
		public void BOO_955_1()
		{
			RunCompilerTestCase(@"BOO-955-1.boo");
		}
		
		[Test]
		public void BOO_958_1()
		{
			RunCompilerTestCase(@"BOO-958-1.boo");
		}
		
		[Test]
		public void BOO_960()
		{
			RunCompilerTestCase(@"BOO-960.boo");
		}
		
		[Test]
		public void BOO_973_1()
		{
			RunCompilerTestCase(@"BOO-973-1.boo");
		}
		
		[Test]
		public void BOO_974_1()
		{
			RunCompilerTestCase(@"BOO-974-1.boo");
		}
		
		[Test]
		public void BOO_975_1()
		{
			RunCompilerTestCase(@"BOO-975-1.boo");
		}
		
		[Test]
		public void BOO_977_1()
		{
			RunCompilerTestCase(@"BOO-977-1.boo");
		}
		
		[Test]
		public void BOO_979_1()
		{
			RunCompilerTestCase(@"BOO-979-1.boo");
		}
		
		[Test]
		public void BOO_979_2()
		{
			RunCompilerTestCase(@"BOO-979-2.boo");
		}
		
		[Test]
		public void BOO_982_1()
		{
			RunCompilerTestCase(@"BOO-982-1.boo");
		}
		
		[Test]
		public void BOO_983_1()
		{
			RunCompilerTestCase(@"BOO-983-1.boo");
		}
		
		[Test]
		public void BOO_983_2()
		{
			RunCompilerTestCase(@"BOO-983-2.boo");
		}
		
		[Test]
		public void BOO_986_1()
		{
			RunCompilerTestCase(@"BOO-986-1.boo");
		}
		
		[Test]
		public void BOO_99_1()
		{
			RunCompilerTestCase(@"BOO-99-1.boo");
		}
		
		[Test]
		public void BOO_992_1()
		{
			RunCompilerTestCase(@"BOO-992-1.boo");
		}
		
		[Test]
		public void BOO_992_2()
		{
			RunCompilerTestCase(@"BOO-992-2.boo");
		}
		
		[Test]
		public void BOO_999_1()
		{
			RunCompilerTestCase(@"BOO-999-1.boo");
		}
		
		[Test]
		public void complex_iterators_1()
		{
			RunCompilerTestCase(@"complex-iterators-1.boo");
		}
		
		[Test]
		public void duck_default_member_overload()
		{
			RunCompilerTestCase(@"duck-default-member-overload.boo");
		}
		
		[Test]
		public void duck_default_setter_overload()
		{
			RunCompilerTestCase(@"duck-default-setter-overload.boo");
		}
		
		[Test]
		public void for_re_Split()
		{
			RunCompilerTestCase(@"for-re-Split.boo");
		}
		
		[Test]
		public void generators_1()
		{
			RunCompilerTestCase(@"generators-1.boo");
		}
		
		[Test]
		public void method_with_type_inference_rule_as_statement()
		{
			RunCompilerTestCase(@"method-with-type-inference-rule-as-statement.boo");
		}
		
		[Test]
		public void nullables_and_generators()
		{
			RunCompilerTestCase(@"nullables-and-generators.boo");
		}
		
		[Test]
		public void override_inference()
		{
			RunCompilerTestCase(@"override-inference.boo");
		}

		[Test]
		public void linq_filter_error()
		{
			RunCompilerTestCase(@"linq-filter-error.boo");
		}

		[Test]
		public void linq_filter_error_2()
		{
			RunCompilerTestCase(@"linq-filter-error-2.boo");
		}

		[Test]
		public void delegate_overload()
		{
			RunCompilerTestCase(@"delegate-overload.boo");
		}


		override protected string GetRelativeTestCasesPath()
		{
			return "regression";
		}
	}
}
