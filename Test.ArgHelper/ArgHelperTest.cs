using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArgHelper;

namespace Test.ArgHelper
{
    [TestClass]
    public class ArgHelperTest
    {
        [TestMethod]
        public void TestBuildMethod()
        {
            string[] args = new[] 
            {
                "-path", @"D:/sample",
                "-date", "2020/01/01",
                "-doubleValue", "10.0",
                "-intValue", "1",
                "-floatValue", "100.0",
                "-enumValue", "Sample",
            };
            SampleArg arg = Arg.Build<SampleArg>(args);

            Assert.AreEqual(@"D:/sample", arg.Path);
            Assert.AreEqual(new DateTime(2020, 1, 1), arg.Date);
            Assert.AreEqual(10.0, arg.DoubleValue);
            Assert.AreEqual(1, arg.IntValue);
            Assert.AreEqual(100.0, arg.FloatValue);
            Assert.AreEqual(ESampleEnum.Sample, arg.EnumValue);
        }

        private class SampleArg
        {
            [ArgAttribute(key: "path", isMandatory: true, description: "sample path")]
            public string Path { get; set; }
            [ArgAttribute(key: "date", isMandatory: true, description: "sample date")]
            public DateTime Date { get; set; }
            [ArgAttribute(key: "doubleValue", isMandatory: true, description: "sample double value")]
            public double DoubleValue { get; set; }
            [ArgAttribute(key: "intValue", isMandatory: true, description: "sample int value")]
            public int IntValue { get; set; }
            [ArgAttribute(key: "floatValue", isMandatory: true, description: "sample float value")]
            public float FloatValue { get; set; }
            [ArgAttribute(key: "enumValue", isMandatory: true, description: "sample enum value")]
            public ESampleEnum EnumValue { get; set; }
        }

        private enum ESampleEnum
        {
            Sample,
        }
    }
}
