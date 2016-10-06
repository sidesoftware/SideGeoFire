using System;
using System.Data.Spatial;
using System.Net;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SideGeoFire.Exceptions;

namespace SideGeoFire.Tests
{
    [TestClass]
    public class GeoFireTests
    {
        private readonly IFirebaseClient _client;
        private const double Latitude = 43.6344415;
        private const double Longitude = -116.4032208;
        private const string GeoFirePath = "geofire";
        private const string Key = "9B6BDA56-B019-4D8E-B9A0-F931B45FE41E";
        private const string ApplicationName=  "[APPLICATION NAME]";
        private const string AuthSecret = "[AUTH SECRET]";

        public GeoFireTests()
        {
            // Create new firebase client
            _client = new FirebaseClient(new FirebaseConfig
            {
                AuthSecret = AuthSecret,
                BasePath = $"https://{ApplicationName}.firebaseio.com/"
            });
        }

        [TestMethod]
        public void TestBuildGeoHashFromDbGeography()
        {
            // Create a DBGeography object from a Latitude and Longitude
            var geography = GenerateLocation(Latitude, Longitude);

            // Build the geohash
            var geoHash = GeoFire.BuildGeoHash(geography, Key);

            // Deserialize the geohash into an object
            var deserializedResult = JsonConvert.DeserializeObject(geoHash);

            // Make sure the object isn't null
            Assert.IsNotNull(deserializedResult);
        }

        [TestMethod]
        public void TestBuildGeoHashFromLongitudeAndLatitude()
        {
            // Build the geohash
            var geoHash = GeoFire.BuildGeoHash(Latitude, Longitude);

            // Deserialize the geohash into an object
            var deserializedResult = JsonConvert.DeserializeObject(geoHash);

            // Make sure the object isn't null
            Assert.IsNotNull(deserializedResult);
        }

        [TestMethod]
        public void SetGeoFire()
        {
            // Delete any that exist
            Delete(GeoFirePath);

            // Build the geohash
            var geoHash = GeoFire.BuildGeoHash(Latitude, Longitude);

            // Deserialize the geohash into an object
            var deserializedResult = JsonConvert.DeserializeObject(geoHash);

            // Set data
            var response = _client.Set($"{GeoFirePath}/{Key}", deserializedResult);

            // Make sure we have an object
            Assert.IsNotNull(response);

            // Make sure we have a good status
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }



        [TestMethod]
        public void GetGeoFire()
        {
            var response = _client.Get($"{GeoFirePath}/{Key}/l");

            Assert.IsNotNull(response);

            // Deserialize JSON data
            var json = JsonConvert.DeserializeObject(response.Body);

            Assert.IsNotNull(json);

            // Convert to an array
            var jarray = (JArray)json;

            Assert.IsFalse(jarray.Count != 2, "get returned an object that does not contain both latitude and longitude");

            // Extract coordinates
            var latitude = jarray[0];
            var longitude = jarray[1];

            double outLatitude;
            Assert.IsTrue(double.TryParse(latitude.ToString(), out outLatitude), "invalid latitude returned");

            double outLongitude;
            Assert.IsTrue(double.TryParse(longitude.ToString(), out outLongitude), "invalid longitude returned");

            // Final validation
            var latitudeResult = GeoFire.ValidateLatitude(outLatitude);

            Assert.IsTrue(latitudeResult, "Latitude is < -90 or > 90");

            // Final validation
            var longitudeResult = GeoFire.ValidateLongitude(outLongitude);

            Assert.IsTrue(longitudeResult, "Longitude is < -180 or > 180");

            // Return DbGeography object
            var dbGeography = DbGeography.FromText($"POINT({outLongitude} {outLatitude})");

            Assert.IsNotNull(dbGeography, "Unable to parse DbGeography object");

            Console.Write(dbGeography);
        }

        private void Delete(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new GeoFireException("delete path must be specified");

            _client.Delete(path);
        }

        /// <summary>
        /// Generate a DbGeography object <seealso cref="DbGeography"/>
        /// </summary>
        /// <param name="latitude">The latitude to test</param>
        /// <param name="longitude">The longitude to test</param>
        /// <returns><seealso cref="DbGeography"/></returns>
        internal static DbGeography GenerateLocation(double latitude, double longitude)
        {
            return DbGeography.FromText($"POINT({longitude} {latitude})");
        }
    }
}
