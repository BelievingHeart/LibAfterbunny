using System.Collections.Generic;
using Lib_VP.Rectifier;

namespace Lib_VP.Tests
{
    public static class MeasuredData
    {
        public static DataColumn X1_Pixel { get; } = new DataColumn("X1_Pixel", new List<double> {2384.196, 2379.415});
        public static DataColumn X2_Pixel { get; } = new DataColumn("X2_Pixel", new List<double> {2382.608, 2381.753});

        public static DataColumn Y1_WeightedByX_Pixel { get; } =
            new DataColumn("Y1_WeightedByX_Pixel", new List<double> {47.201, 55.69});

        public static DataColumn Y2_WeightedByX_Pixel { get; } =
            new DataColumn("Y2_WeightedByX_Pixel", new List<double> {59.194, 45.826});

        public static DataColumn Angle_Pixel { get; } = new DataColumn("Angle", new List<double> {89.894, 90.212});

        public static DataColumn X1 { get; } = new DataColumn("X1", new List<double> {16.05, 16.018});
        public static DataColumn X2 { get; } = new DataColumn("X2", new List<double> {16.039, 16.034});

        public static DataColumn Y1_WeightedByX { get; } =
            new DataColumn("Y1_WeightedByX", new List<double> {0.354, 0.411});

        public static DataColumn Y2_WeightedByX { get; } =
            new DataColumn("Y2_WeightedByX", new List<double> {0.428, 0.339});

        public static DataColumn Angle { get; } = new DataColumn("Angle", new List<double> {89.894, 90.212});
    }

    public static class OMMData
    {
        public static DataColumn X1 { get; } = new DataColumn("X1", new List<double> {16.048, 16.0235});
        public static DataColumn X2 { get; } = new DataColumn("X2", new List<double> {16.0431, 16.0414});

        public static DataColumn Y1_WeightedByX { get; } =
            new DataColumn("Y1_WeightedByX", new List<double> {0.3395, 0.4176});

        public static DataColumn Y2_WeightedByX { get; } =
            new DataColumn("Y2_WeightedByX", new List<double> {0.4463, 0.3391});

        public static DataColumn Angle { get; } = new DataColumn("Angle", new List<double> {89.7607, 90.2146});
    }

    public static class Weights
    {
        public static double X { get; } = 0.0067334379046892;
        public static double Angle { get; } = 0.999272981705799;
    }

    public static class Biases
    {
        public static double X1 { get; } = -0.0019894352972124;
        public static double X2 { get; } = 0.0019855254885292;
        public static double Y1 { get; } = 0.0321449202743118;
        public static double Y2 { get; } = 0.0391271756247701;
        public static double Angle { get; } = 0.00012017844768;
    }

    public static class Estimates
    {
        public static DataColumn X1 { get; } = new DataColumn("MillimeterColumnEstimate",
            new List<double> {16.05184628331116, 16.01965371668884});

        public static DataColumn X2 { get; } = new DataColumn("MillimeterColumnEstimate",
            new List<double> {16.04512854470425, 16.03937145529575});

        public static DataColumn Y1 { get; } = new DataColumn("MillimeterColumnEstimate",
            new List<double> {0.3499699228135467, 0.4071300771864533});

        public static DataColumn Y2 { get; } = new DataColumn("MillimeterColumnEstimate",
            new List<double> {0.4377062989549426, 0.3476937010450574});

        public static DataColumn Angle { get; } = new DataColumn("MillimeterColumnEstimate",
            new List<double> {89.82876559590878, 90.14653440409122});
    }
}