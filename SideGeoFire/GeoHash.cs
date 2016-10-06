namespace SideGeoFire
{
    public static class Geohash
    {
        private const string Base32 = "0123456789bcdefghjkmnpqrstuvwxyz";
        private static readonly int[] Bits = {16, 8, 4, 2, 1};

        /// <summary>
        ///     Implimentation of GeoFire's encodeGeohash function
        /// </summary>
        public static string Encode(double latitude, double longitude, int precision = 10)
        {
            // Local variables
            var even = true;
            var bits = 0;
            var hashValue = 0;
            var geohash = "";

            // Define ranges
            double[] latitudeRange = {-90.0, 90.0};
            double[] longitudeRange = {-180.0, 180.0};

            // Ensure we have a valid precision
            if (precision < 1 || precision > 20) precision = 10;

            // Loop while our geohash length is less than our precision
            while (geohash.Length < precision)
            {
                // Check if we are even or odd
                var mid = even ? (longitudeRange[0] + longitudeRange[1])/2 : (latitudeRange[0] + latitudeRange[1])/2;

                if (even)
                {
                    if (longitude > mid)
                    {
                        hashValue |= Bits[bits];
                        longitudeRange[0] = mid;
                    }
                    else
                    {
                        longitudeRange[1] = mid;
                    }
                }
                else
                {
                    if (latitude > mid)
                    {
                        hashValue |= Bits[bits];
                        latitudeRange[0] = mid;
                    }
                    else
                    {
                        latitudeRange[1] = mid;
                    }
                }

                // Flip bit
                even = !even;

                if (bits < 4) bits++;
                else
                {
                    geohash += Base32[hashValue];
                    bits = 0;
                    hashValue = 0;
                }
            }

            return geohash;
        }
    }
}