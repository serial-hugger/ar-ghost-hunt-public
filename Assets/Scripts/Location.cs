using Google.Common.Geometry;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
#if UNITY_ANDROID
using UnityEngine.Android;
#elif UNITY_IOS
using UnityEngine.iOS;
#endif

public class Location : MonoBehaviour
{
    public string token = "NONE";
    public float trueNorthDirection;
    public float cameraDirection;
    public double currentLat;
    public double currentLon;
    public Controller controlScript;

    private IEnumerator Start()
    {
        // Check if the user has location service enabled.
        if (!IsLocationPermissionGranted())
            yield break;

        // Starts the location service.
        Input.location.Start();

        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            Input.compass.enabled = true;
            S2LatLng coord1 = S2LatLng.FromDegrees(0, 0);
            if (!Application.isEditor)
            {
               coord1  = S2LatLng.FromDegrees(Input.location.lastData.latitude, Input.location.lastData.longitude);
            }

            token = S2CellId.FromLatLng(coord1).ParentForLevel(17).ToToken();
            var s2CellId = S2CellId.FromToken(token);
            var s2Cell = new S2Cell(s2CellId);
            for (int i = 0; i < 4; i++)
            {
                S2LatLng latLng = new S2LatLng(s2Cell.GetVertex(i));
            }
        }

        // List of polygon coordinates in (latitude, longitude)
        List<Tuple<double, double>> polygon = new List<Tuple<double, double>>() {
    Tuple.Create(25.774, -80.190),
    Tuple.Create(18.466, -66.118),
    Tuple.Create(32.321, -64.757),
    Tuple.Create(25.774, -80.190)  // Last point is same as first to close the polygon
};

        // Generate 10 random coordinates within the polygon area
        List<Tuple<double, double>> randomCoordinates = GenerateRandomCoordinates(10, polygon);
    }
    public void SpawnMappingSystem()
    {

        S2LatLng coord1 = S2LatLng.FromDegrees(0, 0);
        if (!Application.isEditor)
        {
            coord1 = S2LatLng.FromDegrees(controlScript.toolDeskScript.locationStartLat, controlScript.toolDeskScript.locationStartLon);
        }

        token = S2CellId.FromLatLng(coord1).ParentForLevel(17).ToToken();
        var s2CellId = S2CellId.FromToken(token);
        var s2Cell = new S2Cell(s2CellId);
        controlScript.center.transform.position = PositionFromCoordinate(new S2LatLng(s2Cell.Center).LatDegrees, new S2LatLng(s2Cell.Center).LngDegrees);
        for (int i = 0; i < 4; i++)
        {
            S2LatLng latLng = new S2LatLng(s2Cell.GetVertex(i));
            if (i==0)
            {
                controlScript.topLeft.transform.position = PositionFromCoordinate(latLng.LatDegrees, latLng.LngDegrees);
            }
            if (i == 1)
            {
                controlScript.topRight.transform.position = PositionFromCoordinate(latLng.LatDegrees, latLng.LngDegrees);
            }
            if (i == 2)
            {
                controlScript.bottomLeft.transform.position = PositionFromCoordinate(latLng.LatDegrees, latLng.LngDegrees);
            }
            if (i == 3)
            {
                controlScript.bottomRight.transform.position = PositionFromCoordinate(latLng.LatDegrees, latLng.LngDegrees);
            }
        }
    }
    public Vector3 PositionFromCoordinate(double lat, double lon)
    {
        double bearing = ((controlScript.locationScript.cameraDirection - controlScript.locationScript.trueNorthDirection) + CalculateBearing(controlScript.toolDeskScript.locationStartLat, controlScript.toolDeskScript.locationStartLon,lat, lon))%360f;
        double distance = GetDistanceInMeters(controlScript.toolDeskScript.locationStartLat, controlScript.toolDeskScript.locationStartLon, lat, lon);
        Quaternion quat = Quaternion.Euler(0f, (float)bearing, 0f);
        return controlScript.toolDeskScript.locationStartPosition + (quat * Vector3.forward) * (float)distance;
    }
    public (double Latitude, double Longitude) CoordinateFromPosition(Vector3 position)
    {
        Vector3 orgPosition = position;
        orgPosition.y = 0f;
        Vector3 anchorPosition = controlScript.toolDeskScript.locationStartPosition;
        anchorPosition.y = 0f;
        Vector3 rot = orgPosition - anchorPosition;
        double bearing = rot.y-trueNorthDirection;
        double distance = Vector3.Distance(orgPosition,anchorPosition);
        Quaternion quat = Quaternion.Euler(0f, (float)bearing, 0f);
        return GetCoordinatesFromDistanceAndBearing(controlScript.toolDeskScript.locationStartLat, controlScript.toolDeskScript.locationStartLon,distance,bearing);
    }

    // Calculates the distance in meters between two sets of coordinates
    public static double GetDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371e3; // Earth's mean radius in meters
        double phi1 = lat1 * Math.PI / 180; // φ, λ in radians
        double phi2 = lat2 * Math.PI / 180;
        double deltaPhi = (lat2 - lat1) * Math.PI / 180;
        double deltaLambda = (lon2 - lon1) * Math.PI / 180;

        // Haversine formula to calculate the distance
        double a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                   Math.Cos(phi1) * Math.Cos(phi2) *
                   Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = R * c; // Distance in meters

        return distance;
    }

    // Calculate new coordinates given the start coordinates, distance in meters, and bearing angle (in degrees)
    public static (double Latitude, double Longitude) GetCoordinatesFromDistanceAndBearing(double startLatitude, double startLongitude, double distanceInMeters, double bearingDegrees)
    {
        double radiusEarthKilometers = 6371.01;
        double distanceKilometers = distanceInMeters / 1000.0;

        // Convert latitude and longitude from degrees to radians
        double startLatitudeRadians = DegreesToRadians(startLatitude);
        double startLongitudeRadians = DegreesToRadians(startLongitude);
        double bearingRadians = DegreesToRadians(bearingDegrees);

        // Calculate the end latitude in radians
        double endLatitudeRadians = Math.Asin(Math.Sin(startLatitudeRadians) * Math.Cos(distanceKilometers / radiusEarthKilometers) +
                                              Math.Cos(startLatitudeRadians) * Math.Sin(distanceKilometers / radiusEarthKilometers) * Math.Cos(bearingRadians));

        // Calculate the end longitude in radians
        double endLongitudeRadians = startLongitudeRadians +
                                     Math.Atan2(Math.Sin(bearingRadians) * Math.Sin(distanceKilometers / radiusEarthKilometers) * Math.Cos(startLatitudeRadians),
                                                Math.Cos(distanceKilometers / radiusEarthKilometers) - Math.Sin(startLatitudeRadians) * Math.Sin(endLatitudeRadians));

        // Convert the end latitude and longitude from radians to degrees
        double endLatitude = RadiansToDegrees(endLatitudeRadians);
        double endLongitude = RadiansToDegrees(endLongitudeRadians);

        // Normalize the longitude to be within the range [-180, 180]
        endLongitude = NormalizeLongitude(endLongitude);

        return (endLatitude, endLongitude);
    }
    public static double CalculateBearing(double lat1, double lon1, double lat2, double lon2)
    {
        var radianLat1 = DegreesToRadians(lat1);
        var radianLat2 = DegreesToRadians(lat2);
        var deltaLongitude = DegreesToRadians(lon2 - lon1);

        var y = Math.Sin(deltaLongitude) * Math.Cos(radianLat2);
        var x = Math.Cos(radianLat1) * Math.Sin(radianLat2) - Math.Sin(radianLat1) * Math.Cos(radianLat2) * Math.Cos(deltaLongitude);
        var bearingRadians = Math.Atan2(y, x);

        // Convert from radians to degrees
        var bearingDegrees = RadiansToDegrees(bearingRadians);

        // Normalize the result to be between 0 and 360 degrees
        return (bearingDegrees + 360) % 360;
    }


    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    private static double RadiansToDegrees(double radians)
    {
        return radians * 180.0 / Math.PI;
    }

    private static double NormalizeLongitude(double longitude)
    {
        while (longitude < -180) longitude += 360;
        while (longitude > 180) longitude -= 360;
        return longitude;
    }
    public bool IsLocationPermissionGranted()
    {
#if UNITY_ANDROID
        return Permission.HasUserAuthorizedPermission(Permission.FineLocation);
#elif UNITY_IOS
        return Input.location.isEnabledByUser;
#endif
        return true;
    }
    public void SetHeading()
    {
        if (Input.location.status == LocationServiceStatus.Running) {
            if (Input.compass.headingAccuracy != 0) {
                trueNorthDirection = Input.compass.trueHeading;
            }
            else
            {
                trueNorthDirection = Input.compass.magneticHeading;
            }
        }
    }
    public static List<Tuple<double, double>> GenerateRandomCoordinates(int count, List<Tuple<double, double>> polygon)
    {
        var randomPoints = new List<Tuple<double, double>>();
        var rnd = new System.Random();
        var minLat = polygon.Min(p => p.Item1);
        var maxLat = polygon.Max(p => p.Item1);
        var minLon = polygon.Min(p => p.Item2);
        var maxLon = polygon.Max(p => p.Item2);

        while (randomPoints.Count < count)
        {
            var latitude = rnd.NextDouble() * (maxLat - minLat) + minLat;
            var longitude = rnd.NextDouble() * (maxLon - minLon) + minLon;

            if (IsPointInPolygon(new Tuple<double, double>(latitude, longitude), polygon))
            {
                randomPoints.Add(new Tuple<double, double>(latitude, longitude));
            }
        }

        return randomPoints;
    }

    private static bool IsPointInPolygon(Tuple<double, double> point, List<Tuple<double, double>> polygon)
    {
        int polygonCount = polygon.Count;
        bool inside = false;

        for (int i = 0, j = polygonCount - 1; i < polygonCount; j = i++)
        {
            var xi = polygon[i].Item1;
            var yi = polygon[i].Item2;
            var xj = polygon[j].Item1;
            var yj = polygon[j].Item2;

            var intersect = ((yi > point.Item1) != (yj > point.Item2))
                && (point.Item1 < (xj - xi) * (point.Item2 - yi) / (yj - yi) + xi);
            if (intersect)
                inside = !inside;
        }

        return inside;
    }
    // Calculates the new position (x, z) of an object after rotating the grid around the origin (0,0)
    public static Tuple<float, float> GetRotatedPosition(float x, float z, float rotationDegrees)
    {
        // Convert degrees to radians for math functions 
        double radians = rotationDegrees * Math.PI / 180.0;

        // Calculate the new position using rotation matrix transformation
        float newX = (float)(x * Math.Cos(radians) - z * Math.Sin(radians));
        float newZ = (float)(x * Math.Sin(radians) + z * Math.Cos(radians));

        return new Tuple<float, float>(newX, newZ);
    }
}
