﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GuigleAPI
{
    public static class Helper
    {
        public static double EarhRadius { get; set; } = 6371;

        /// <summary>
        /// Gets the rounded distance in kilometers
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lat2"></param>
        /// <param name="lng1"></param>
        /// <param name="lng2"></param>
        /// <returns></returns>
        public static int GetDistance(double lat1, double lat2, double lng1, double lng2)
        {
            return Convert.ToInt32(Math.Round(GetPreciseDistance(lat1, lat2, lng1, lng2)));
        }

        /// <summary>
        /// Gets the precise distance in kilometers
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lat2"></param>
        /// <param name="lng1"></param>
        /// <param name="lng2"></param>
        /// <returns></returns>
        public static double GetPreciseDistance(double lat1, double lat2, double lng1, double lng2)
        {
            var distance = new Coordinates(lat1, lng1)
                .DistanceTo(
                    new Coordinates(lat2, lng2),
                    UnitOfLength.Kilometers
                );

            return distance;
        }

        public static Tuple<double, double> GetMiddleCoordinates(double lat1, double lat2, double lng1, double lng2)
        {
            double lat;
            double lng;

            double dLon = DegreesToRadians(lng2 - lng1);
            double Bx = Math.Cos(DegreesToRadians(lat2)) * Math.Cos(dLon);
            double By = Math.Cos(DegreesToRadians(lat2)) * Math.Sin(dLon);

            lat = RadiansToDegrees(Math.Atan2(
                Math.Sin(DegreesToRadians(lat1)) + Math.Sin(DegreesToRadians(lat2)),
                Math.Sqrt(
                    (Math.Cos(DegreesToRadians(lat1)) + Bx) *
                    (Math.Cos(DegreesToRadians(lat1)) + Bx) + By * By)));

            lng = lng1 + RadiansToDegrees(Math.Atan2(By, Math.Cos(DegreesToRadians(lat1)) + Bx));

            return new Tuple<double, double>(lat, lng);
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        private static double RadiansToDegrees(double radians)
        {
            return radians * (180.0 / Math.PI);
        }
    }


    public class Coordinates
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public Coordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }

    public static class CoordinatesDistanceExtensions
    {
        public static double DistanceTo(this Coordinates baseCoordinates, Coordinates targetCoordinates)
        {
            return DistanceTo(baseCoordinates, targetCoordinates, UnitOfLength.Kilometers);
        }

        public static double DistanceTo(this Coordinates baseCoordinates, Coordinates targetCoordinates, UnitOfLength unitOfLength)
        {
            var baseRad = Math.PI * baseCoordinates.Latitude / 180;
            var targetRad = Math.PI * targetCoordinates.Latitude / 180;
            var theta = baseCoordinates.Longitude - targetCoordinates.Longitude;
            var thetaRad = Math.PI * theta / 180;

            double dist =
                Math.Sin(baseRad) * Math.Sin(targetRad) + Math.Cos(baseRad) *
                Math.Cos(targetRad) * Math.Cos(thetaRad);
            dist = Math.Acos(dist);

            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return unitOfLength.ConvertFromMiles(dist);
        }
    }

    public class UnitOfLength
    {
        public static UnitOfLength Kilometers = new UnitOfLength(1.609344);
        public static UnitOfLength NauticalMiles = new UnitOfLength(0.8684);
        public static UnitOfLength Miles = new UnitOfLength(1);

        private readonly double _fromMilesFactor;

        private UnitOfLength(double fromMilesFactor)
        {
            _fromMilesFactor = fromMilesFactor;
        }

        public double ConvertFromMiles(double input)
        {
            return input * _fromMilesFactor;
        }
    }
}
