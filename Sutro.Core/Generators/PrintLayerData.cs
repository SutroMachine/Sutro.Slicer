﻿using g3;
using Sutro.Core.Settings;
using Sutro.Core.Slicing;
using Sutro.Core.Toolpathing;
using Sutro.Core.Toolpaths;
using Sutro.Core.Utility;
using System.Collections.Generic;

namespace Sutro.Core.Generators
{
    /// <summary>
    /// PrintLayerData is set of information for a single print layer
    /// </summary>
    public class PrintLayerData
    {
        public int layer_i;
        public PlanarSlice Slice;
        public IPrintProfileFFF Settings;

        public PrintLayerData PreviousLayer;

        public ToolpathSetBuilder PathAccum;
        public IFillPathScheduler2d Scheduler;

        public List<IShellsFillPolygon> ShellFills;
        public List<GeneralPolygon2d> SupportAreas;

        public TemporalPathHash Spatial;

        public PrintLayerData(int layer_i, PlanarSlice slice, IPrintProfileFFF settings)
        {
            this.layer_i = layer_i;
            Slice = slice;
            Settings = settings;
            Spatial = new TemporalPathHash();
        }
    }
}