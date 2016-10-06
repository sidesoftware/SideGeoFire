# SideGeoFire for Firebase
SideGeoFire is a .net component for generating a firebase geofire object. For a list of firebase supported libraries, visit https://github.com/firebase/geofire

This library does not write to firebase. The reccommended library for writing to firebase is (https://github.com/ziyasal/FireSharp)

#### Installation (NuGet)
```csharp
//**Install**
Install-Package SideGeoFire
```

#### Usage
***

The GeoFire object contains a function called BuildGeoHash which has two overloads. The first accepts a System.Data.Spatial.DbGeography object. The second accepts both latitude and longitude.

**string BuildGeoHash(DbGeography, Key)**

```csharp
// Create a DBGeography object from a Latitude and Longitude
var geography = GenerateLocation(latitude, longitude);

// Build the geohash
var geoHash = GeoFire.BuildGeoHash(geography, Key);
```

**string BuildGeoHash(Latitude, Longitude, Key)**

```csharp
// Build the geohash
var geoHash = GeoFire.BuildGeoHash(latitude, longitude);
```

**Returns**
The result of the BuildGeoHash is a JSON object formatted to the firebase specification. The **g** object is the geohash and the **l** is the latitude and longitude.
```json
{
	"g" : "9rv0ppmv03",
	"l" : {
		"0" : 43.6344415,
		"1" : -116.4032208
	}
}
```

## License
Code and documentation are available according to the *MIT* License (see [LICENSE](https://github.com/sidesoftware/SideGeoFire/blob/master/LICENSE.md)).