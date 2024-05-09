using FluentAssertions;
using ThermalSim.Domain.Extensions;

namespace ThermalSim.Tests
{
    public class MathExtensionsTest
    {
        [Theory]
        [InlineData(0.0, 0.0, 0.1, 0.1, 51610.0)]
        public void CalcDistance_ShouldReturnCorrectDistance_GivenGPSCoordinates(double x1, double y1, double x2, double y2, double expectedDistance)
        {
            double distance = MathExtensions.CalcDistance(x1, y1, x2, y2);

            distance.Should().BeApproximately(expectedDistance, 100.0);
        }
    }
}