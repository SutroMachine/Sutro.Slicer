﻿using g3;
using gs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Sutro.Core.Decompilers;
using Sutro.Core.Models.GCode;
using Sutro.Core.Settings;

namespace Sutro.Core.UnitTests.Decompilers
{
    [TestClass()]
    public class DecompilerFFFTests : DecompilerFFF
    {
        [TestMethod()]
        public void NewLayerComment()
        {
            var line = new GCodeLine(0, LineType.Comment);
            line.Comment = "layer 99, Z = 33.33";

            var isNewLine = LineIsNewLayerComment(line, out int index, out double height);

            Assert.IsTrue(isNewLine);
            Assert.AreEqual(99, index);
            Assert.AreEqual(33.33, height);
        }

        [TestMethod()]
        public void NewLayerUnknown()
        {
            var line = new GCodeLine(0, LineType.UnknownString);
            line.OriginalString = "; layer 99, Z = 33.33";

            var isNewLine = LineIsNewLayerComment(line, out int index, out double height);

            Assert.IsTrue(isNewLine);
            Assert.AreEqual(99, index);
            Assert.AreEqual(33.33, height);
        }

        [TestMethod()]
        public void NotNewLayer()
        {
            var line = new GCodeLine(0, LineType.GCode);
            line.Code = 1;
            line.Parameters = new GCodeParam[] { GCodeParam.Double(200, "X") };
            line.Comment = "layer";

            var isNewLine = LineIsNewLayerComment(line, out int index, out double height);

            Assert.IsFalse(isNewLine);
        }

    }
}
