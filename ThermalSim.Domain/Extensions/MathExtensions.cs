using ThermalSim.Domain.Position;
using ThermalSim.Domain.Thermals;

namespace ThermalSim.Domain.Extensions
{
    public static class MathExtensions
    {
        public static double ToRadians(this double angleInDegrees)
        {
            return (Math.PI / 180) * angleInDegrees;
        }

        public static double CalcDistance(this IThermalModel model, double latitude, double longitude)
        {
            return CalcDistance(model.Properties.Latitude, model.Properties.Longitude, latitude, longitude);
        }

        public static double CalcDistance(this AircraftPositionState position, double latitude, double longitude)
        {
            return CalcDistance(position.Latitude, position.Longitude, latitude, longitude);
        }

        public static double CalcDistance(this AircraftPositionState position, IThermalModel model)
        {
            return CalcDistance(position.Latitude, position.Longitude, model.Properties.Latitude, model.Properties.Longitude);
        }

        public static double CalcDistance(this IThermalModel model, AircraftPositionState position)
        {
            return CalcDistance(position.Latitude, position.Longitude, model.Properties.Latitude, model.Properties.Longitude);
        }

        public static double CalcDistance(this IThermalModel model1, IThermalModel model2)
        {
            return CalcDistance(model1.Properties.Latitude, model1.Properties.Longitude, model2.Properties.Latitude, model2.Properties.Longitude);
        }


        //Calculates the distance in feet between 2 points (a1,a2) and (b1,b2)
        public static double CalcDistance(double a1, double a2, double b1, double b2)
        {
            var a1Rad = a1.ToRadians();
            var a2Rad = a2.ToRadians();
            var b1Rad = b1.ToRadians();
            var b2Rad = b2.ToRadians();

            return 20930000 * (Math.Acos(
                Math.Cos(a1Rad) * Math.Cos(a2Rad) * Math.Cos(b1Rad) * Math.Cos(b2Rad) +
                Math.Cos(a1Rad) * Math.Sin(a2Rad) * Math.Cos(b1Rad) * Math.Sin(b2Rad) +
                Math.Sin(a1Rad) * Math.Sin(b1Rad)));
        }
    }
}
