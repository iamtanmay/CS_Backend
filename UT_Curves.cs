using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utilities
{
    [TestClass]
    public class UT_Curves
    {
        public CurveLibrary curves;
        [TestMethod]
        public void CreateCurveLibrary()
        {
            curves = new CurveLibrary();
            curves.LoadCurves(".\\Data\\CurvesControlPoints\\", "Lagrange");
        }

        [TestMethod]
        public void CompareCurveLibrary()
        {
        }
    }
}
