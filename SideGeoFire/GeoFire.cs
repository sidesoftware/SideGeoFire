
using System.Data.Spatial;
using SideGeoFire.Exceptions;

namespace SideGeoFire
{
    public static class GeoFire
    {
        /// <summary>
        /// Builds a firebase compatible GeoFire location entry
        /// </summary>
        /// <param name="geography">geography object containing location</param>
        /// <returns>Key stored in geofire</returns>
        public static string BuildGeoHash(DbGeography geography)
        {
            if (geography == null) throw new GeoFireException("geography argument is null");
            if (geography.Latitude == null || geography.Longitude == null)
                throw new GeoFireException("geography argument is null");

            return BuildGeoHash(geography.Latitude.Value, geography.Longitude.Value);
        }

        /// <summary>
        /// Builds a firebase compatible GeoFire location entry
        /// </summary>
        /// <param name="latitude">latitude of location</param>
        /// <param name="longitude">longitude of location</param>
        /// <returns>Key stored in geofire</returns>
        public static string BuildGeoHash(double latitude, double longitude)
        {
            // Validate location
            ValidateLocation(latitude, longitude);

            // Generate a new geohash
            var geohash = Geohash.Encode(latitude, longitude);

            // Build location object
            return BuildLocationWithPriority(latitude, longitude, geohash);
        }

        /// <summary>
        /// Builds a firebase location priority object
        /// </summary>
        /// <param name="latitude">latitude of location</param>
        /// <param name="longitude">longitude of location</param>
        /// <param name="geohash">An encoded geohash object</param>
        /// <param name="includePriority">When set to true, the priority is set in the object. Legacy</param>
        /// <returns></returns>
        private static string BuildLocationWithPriority(double latitude, double longitude, string geohash, bool includePriority = false)
        {
            return
                $"{{'g': '{geohash}', 'l': {{'0': {latitude}, '1': {longitude}}}{(includePriority ? ", '.priority': '{0}'}" : "")}}}";
        }

        /// <summary>
        /// Validates the key to ensure it is not empty, does not contain more characters than 755
        /// UTF-8 encoded, cannot contain . $ # [ ] / or ASCII control characters 0-31 or 127
        /// Limit = 768 bytes
        /// </summary>
        public static void ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new GeoFireException("key cannot be the empty string");
            if ((11 + key.Length) > 755) throw new GeoFireException("key is too long to be stored in Firebase");
            if (key.IndexOfAny(new[] { '.', '#', '$', ']', '[', '/' }) != -1) throw new GeoFireException(
                $"key '{key}' cannot contain any of the following characters: . # $ ] [ /");
        }

        /// <summary>
        /// Validates the location ensuring that the latitude and longitude are valid
        /// </summary>
        public static void ValidateLocation(double latitude, double longitude)
        {
            if (!ValidateLatitude(latitude))
                throw new GeoFireException("latitude must be within the range [-90, 90]");
            if (!ValidateLongitude(longitude))
                throw new GeoFireException("longitude must be within the range [-180, 180]");
        }

        /// <summary>
        /// Validates the latitude is not less than -90 and not greater than 90
        /// </summary>
        /// <param name="latitude">The latitude to test</param>
        /// <returns>True/False</returns>
        public static bool ValidateLatitude(double latitude)
        {
            return !(latitude < -90) && !(latitude > 90);
        }

        /// <summary>
        /// Validates the longitude is not less than -180 and not greater than 180
        /// </summary>
        /// <param name="longitude">The longitude to test</param>
        /// <returns>True/False</returns>
        public static bool ValidateLongitude(double longitude)
        {
            return !(longitude < -180) && !(longitude > 180);
        }
    }
}
