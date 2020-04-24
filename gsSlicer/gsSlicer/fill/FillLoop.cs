﻿using g3;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gs
{
    /// <summary>
    /// Additive polygon fill curve
    /// </summary>
    public class FillLoop<TSegmentInfo> :
        FillBase<TSegmentInfo>
        where TSegmentInfo : IFillSegment, new()
    {
        private FillLoop()
        {
        }

        public FillLoop<TSegmentInfo> CloneBare()
        {
            var loop = new FillLoop<TSegmentInfo>();
            loop.CopyProperties(this);
            return loop;
        }

        public FillCurve<TSegmentInfo> CloneBareAsCurve()
        {
            var curve = new FillCurve<TSegmentInfo>();
            curve.CopyProperties(this);
            return curve;
        }

        public bool IsClockwise { get { throw new NotImplementedException("FIX"); } }
        public double LoopPerimeter { get { throw new NotImplementedException("FIX"); } }

        public Vector2d EntryExitPoint => elements[0].NodeStart.xy;

        public FillLoop(IList<Vector2d> vertices)
        {
            elements.Capacity = vertices.Count;
            for (int i = 1; i < vertices.Count; i++)
            {
                elements.Add(new FillElement<TSegmentInfo>(vertices[i - 1], vertices[i], new TSegmentInfo()));
            }
            elements.Add(new FillElement<TSegmentInfo>(vertices[^1], vertices[0], new TSegmentInfo()));
        }

        public FillLoop(IEnumerable<FillElement<TSegmentInfo>> elements)
        {
            this.elements = elements.ToList();
        }

        public FillLoop<TSegmentInfo> RollToVertex(int startIndex)
        {
            // TODO: Add range checking for startIndex
            var rolledLoop = new FillLoop<TSegmentInfo>();
            rolledLoop.CopyProperties(this);

            for (int i = 0; i < elements.Count; i++)
            {
                rolledLoop.elements.Add(elements[(i + startIndex) % elements.Count]);
            }

            return rolledLoop;
        }

        public FillLoop<TSegmentInfo> RollBetweenVertices(int elementIndex, double elementParameterizedDistance, double tolerance = 0.001)
        {
            if (!ElementShouldSplit(elementParameterizedDistance, tolerance, elements[elementIndex].GetSegment2d().Length))
            {
                if (elementParameterizedDistance > 0.5)
                {
                    ++elementIndex;
                    if (elementIndex >= elements.Count)
                    {
                        elementIndex = 0;
                    }
                }

                return RollToVertex(elementIndex);
            }

            var rolledElements = new List<FillElement<TSegmentInfo>>(elements.Count + 1);

            var elementToSplit = elements[elementIndex];

            var interpolatedVertex = Vector3d.Lerp(elementToSplit.NodeStart, elementToSplit.NodeEnd, elementParameterizedDistance);

            var splitSegmentData = elementToSplit.Edge == null ?
                Tuple.Create((IFillSegment)new TSegmentInfo(), (IFillSegment)new TSegmentInfo()) :
                elementToSplit.Edge.Split(elementParameterizedDistance);

            // Add the second half of the split element
            rolledElements.Add(new FillElement<TSegmentInfo>(
                    interpolatedVertex,
                    elementToSplit.NodeEnd,
                    (TSegmentInfo)splitSegmentData.Item2));

            // Add all elements after the split element
            for (int i = elementIndex + 1; i < elements.Count; ++i)
                rolledElements.Add(elements[i]);

            // Add all elements before the split element
            for (int i = 0; i < elementIndex; ++i)
                rolledElements.Add(elements[i]);

            // Add the first half of the split element
            rolledElements.Add(new FillElement<TSegmentInfo>(
                    elementToSplit.NodeStart,
                    interpolatedVertex,
                    (TSegmentInfo)splitSegmentData.Item1));

            var rolledLoop = new FillLoop<TSegmentInfo>(rolledElements);
            rolledLoop.CopyProperties(this);
            return rolledLoop;
        }

        private static bool ElementShouldSplit(double parameterizedSplitDistance, double tolerance, double segmentLength)
        {
            double toleranceParameterized = tolerance / segmentLength;
            return parameterizedSplitDistance > toleranceParameterized && parameterizedSplitDistance < (1 - toleranceParameterized);
        }

        public List<FillLoop<TSegmentInfo>> SplitAtDistances(double[] v)
        {
            throw new NotImplementedException();
        }

        public FillCurve<TSegmentInfo> ConvertToCurve()
        {
            var curve = new FillCurve<TSegmentInfo>(elements);
            curve.CopyProperties(this);
            return curve;
        }
    }
}