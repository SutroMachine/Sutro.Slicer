using gs;
using gs.info;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gsCore.UnitTests
{
    internal class SubSettingsGood : Settings
    {
        public int SubFieldX = 0;
        public int SubFieldY = 0;
    }

    internal class SubSettingsBad
    {
        public int SubFieldX = 0;
        public int SubFieldY = 0;
    }

    internal class SettingsA : Settings
    {
        public int IntegerFieldA = 0;
        public int IntegerPropertyA { get; set;} = 0;
        public string StringFieldA = "";
    }

    internal class SettingsB : SettingsA
    {
        public int IntegerFieldB = 0;
    }

    internal class SettingsC : SettingsA
    {
        public int IntegerFieldC = 0;
    }

    internal class SettingsD : SettingsA
    {
        public SubSettingsGood SubSettings = new SubSettingsGood();
    }

    internal class SettingsE : SettingsA
    {
        public SubSettingsBad SubSettings = new SubSettingsBad();
    }

    internal enum EnumNumbers { Zero = 0, One = 1, Two = 2};
    internal class SettingsF : SettingsA
    {
        public EnumNumbers Enum;
    }

    internal enum EnumColors { Blue = 0, Green = 1, Red = 2};
    internal class SettingsG : SettingsA
    {
        public EnumColors Enum;
    }

    [TestClass]
    public class SettingsTests
    {
        [TestMethod]
        public void CopyFromSameType_Field_ValueType()
        {
            var orig = new SettingsA();
            var copy = new SettingsA();
            orig.IntegerFieldA = 3;
            
            copy.CopyValuesFrom(orig);

            Assert.AreEqual(3, copy.IntegerFieldA);
        }

        [TestMethod]
        public void CopyFromSameType_Property_ValueType()
        {
            var orig = new SettingsA();
            var copy = new SettingsA();
            orig.IntegerPropertyA = 5;

            copy.CopyValuesFrom(orig);

            Assert.AreEqual(5, copy.IntegerPropertyA);
        }

        [TestMethod]
        public void CopyFromChild_Field_ValueType()
        {
            var orig = new SettingsB();
            var copy = new SettingsA();
            orig.IntegerFieldA = 7;

            copy.CopyValuesFrom(orig);

            Assert.AreEqual(7, copy.IntegerFieldA);
        }

        [TestMethod]
        public void CopyFromParent_Field_ValueType()
        {
            var orig = new SettingsA();
            var copy = new SettingsB();
            orig.IntegerFieldA = 8;

            copy.CopyValuesFrom(orig);

            Assert.AreEqual(8, copy.IntegerFieldA);
        }

        [TestMethod]
        public void CopyFromSibling_Field_ValueType()
        {
            var orig = new SettingsB();
            var copy = new SettingsC();
            orig.IntegerFieldA = 9;

            copy.CopyValuesFrom(orig);

            Assert.AreEqual(9, copy.IntegerFieldA);
        }

        [TestMethod]
        public void CloneFromParent()
        {
            var orig = new SettingsA();
            orig.IntegerFieldA = 4;

            var copy = orig.CloneAs<SettingsB>();

            Assert.IsNotNull(copy);
            Assert.AreEqual(4, copy.IntegerFieldA);
        }

        [TestMethod]
        public void CloneFromChild()
        {
            var orig = new SettingsB();
            orig.IntegerFieldA = 4;

            var copy = orig.CloneAs<SettingsA>();

            Assert.IsNotNull(copy);
            Assert.AreEqual(4, copy.IntegerFieldA);
        }

        [TestMethod]
        public void Clone_StringsIndependant()
        {
            var orig = new SettingsA();
            orig.StringFieldA = "hello";

            var copy = orig.CloneAs<SettingsA>();
            orig.StringFieldA = "world";

            Assert.AreEqual("hello", copy.StringFieldA);
        }

        [TestMethod]
        public void Clone_ReferenceTypesIndependant()
        {
            var orig = new SettingsD();
            orig.SubSettings.SubFieldX = 1;

            var copy = orig.CloneAs<SettingsD>();
            orig.SubSettings.SubFieldX = 2;

            Assert.AreEqual(1, copy.SubSettings.SubFieldX);
        }

        [TestMethod]
        public void Clone_ReferenceTypes_ExceptionWhenNotDerivedFromSettings()
        {
            var orig = new SettingsE();
            orig.SubSettings.SubFieldX = 1;

            Assert.ThrowsException<SettingsContainsReferenceType>(() => 
            {
                var copy = orig.CloneAs<SettingsE>();
            });
        }

        [TestMethod]
        public void Clone_FieldsWithDifferentEnums()
        {
            var orig = new SettingsF();
            orig.Enum = EnumNumbers.One;

            var copy = new SettingsG();
            copy.Enum = EnumColors.Red;

            copy.CopyValuesFrom(orig);

            Assert.AreEqual(EnumColors.Red, copy.Enum);
        }

        [TestMethod]
        public void Clone_FieldsWithSameEnums()
        {
            var orig = new SettingsF();
            orig.Enum = EnumNumbers.One;

            var copy = new SettingsF();
            copy.Enum = EnumNumbers.Two;

            copy.CopyValuesFrom(orig);

            Assert.AreEqual(EnumNumbers.One, copy.Enum);
        }
    }
}
