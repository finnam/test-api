using System;
using GeoCoordinatePortable;

namespace TestApi.Function
{
    public class Locale
    {
       const double METERS_IN_A_MILE=1609.34;
       private readonly GeoCoordinate _center;

       double _radius;
        public Locale(double lat, double lng,  ushort radius )
        {
           // Circumference of earth at equator is 24901, so furthest distance from a point is aprox 12450
           if ( radius > 0 && radius < 12450 )
           {
           _center = new GeoCoordinate(lat, lng);
           _radius = radius*METERS_IN_A_MILE;
           }
           else
           {
              throw new ArgumentOutOfRangeException("radius", "radius must be >0 and < 12450.  " );
           }
        }

        public bool IsLocationInLocale(double lat, double lng)
        {
           return  _center.GetDistanceTo(new GeoCoordinate(lat, lng)) <= _radius;
        }

    }
}