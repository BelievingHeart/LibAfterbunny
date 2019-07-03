using System.Collections.Generic;
using Lib_VP.Rectifier;
using Lib_VP.Tests;
using NUnit.Framework;

namespace Lib_VP.IntegrationTests
{
    [TestFixture]
    public class RectifierIntegrationTests
    {
        private Rectifier.Rectifier CreateRectifier()
        {
            return new Rectifier.Rectifier(new OMMDataParserStub());
        }

        [Test]
        public void Rectify_WhenDataGridCollectedEnoughData_EditBlockInputsCorrespondingly()
        {
            var rectifier = CreateRectifier();
            var dataGrid = new DataGrid(new List<string>());
            dataGrid["X1_Pixel"] = MeasuredData.X1_Pixel;
            dataGrid["X2_Pixel"] = MeasuredData.X2_Pixel;
            dataGrid["Y1_WeightedByX_Pixel"] = MeasuredData.Y1_WeightedByX_Pixel;
            dataGrid["Y2_WeightedByX_Pixel"] = MeasuredData.Y2_WeightedByX_Pixel;
            dataGrid["Angle_Pixel"] = MeasuredData.Angle_Pixel;
            dataGrid["X1"] = MeasuredData.X1;
            dataGrid["X2"] = MeasuredData.X2;
            dataGrid["Y1_WeightedByX"] = MeasuredData.Y1_WeightedByX;
            dataGrid["Y2_WeightedByX"] = MeasuredData.Y2_WeightedByX;
            dataGrid["Angle"] = MeasuredData.Angle;
            dataGrid.MaxRows = 2;
            dataGrid.RowsFixed = true;
            rectifier.DataGrid = dataGrid;

            rectifier.Rectify(new CogToolBlockMock());
        }
    }
}