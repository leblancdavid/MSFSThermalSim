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

        public static double ToDegrees(this double angleInRad)
        {
            return (angleInRad / Math.PI * 180.0);
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
        public static double CalcDistance(double lat1, double lon1, double lat2, double lon2)
        {
            if ((lat1 == lat2) && (lon1 == lon2))
            {
                return 0.0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(lat1.ToRadians()) * Math.Sin(lat2.ToRadians()) + Math.Cos(lat1.ToRadians()) * Math.Cos(lat2.ToRadians()) * Math.Cos(theta.ToRadians());
                dist = Math.Acos(dist);
                dist = dist.ToDegrees();
                dist = dist * 60.0 * 1.1515 * 5280;
                return (dist);
            }
        }

        public static void CalcApplyWindDrift(this IThermalModel model, double windDirection, double windSpeed) 
        {
            //We want to subtract 180 because the wind direction is where it's coming from, not where it's blowing to
            //or do we?
            double adjustedDirection = windDirection - 180.0;
            double distanceKm = windSpeed * Constants.DEFAULT_TIME_FACTOR * Constants.FEET_TO_KM;
            double latitudeDistance = distanceKm * Math.Sin(adjustedDirection.ToRadians());
            double longitudeDistance = distanceKm * Math.Cos(adjustedDirection.ToRadians());

            double changeInLatitude = latitudeDistance / Constants.DEGREE_LATITUDE_KM;
            double changeInLongitude = longitudeDistance / (Constants.DEGREE_LONGITUDE_KM * Math.Cos(model.Properties.Latitude.ToRadians()));

            model.Properties.Longitude += changeInLongitude;
            model.Properties.Latitude += changeInLatitude;
        }

        public static double GetRelativeDirection(this IThermalModel thermal, AircraftPositionState position)
        {
            var headingRadian = position.HeadingIndicator.ToRadians();
            var thermalX = thermal.Properties.Longitude - position.Longitude;
            var thermalY = thermal.Properties.Latitude - position.Latitude;

            //rotate the vector by the heading
            var rotatedX = Math.Cos(headingRadian) * thermalX - Math.Sin(headingRadian) * thermalY;
            var rotatedY = Math.Sin(headingRadian) * thermalX + Math.Cos(headingRadian) * thermalY;

            var angleToThermalCore = Math.Atan2(rotatedY, rotatedX);

            if (angleToThermalCore < 0)
            {
                angleToThermalCore = Math.Abs(angleToThermalCore - Math.PI * 0.5);
            }
            else
            {
                angleToThermalCore = Math.PI * 2.5 - angleToThermalCore;
                if (angleToThermalCore > Math.PI * 2.0)
                {
                    angleToThermalCore -= Math.PI * 2.0;
                }
            }

            return angleToThermalCore.ToDegrees();

        }

    }
}
