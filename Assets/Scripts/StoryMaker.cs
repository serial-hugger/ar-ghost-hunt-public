using Google.Common.Geometry;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Mathematics;
using UnityEngine;


public class StoryMaker : MonoBehaviour
{
    public struct Coordinate
    {
        public double lat;
        public double lon;
    }
    Coordinate northCoord = new Coordinate();
    Coordinate eastCoord = new Coordinate();
    Coordinate southCoord = new Coordinate();
    Coordinate westCoord = new Coordinate();

    public struct NotableFeatures
    {
        public int id;
        public string type;
        public int population;
        public string tags;
        public string activity;
        public string name;
        public string wikidata;
    }

    public List<NotableFeatures> features = new List<NotableFeatures>();

    // Start is called before the first frame update
    void Start()
    {
        S2LatLng coord1 = S2LatLng.FromDegrees(38.9868044088541, -94.69668358037313);
        string token = S2CellId.FromLatLng(coord1).ParentForLevel(10).ToToken();
        S2LatLng cellCenter = S2CellId.FromToken(token).ToLatLng();
        northCoord = newLocation(15, 0, cellCenter.LatDegrees, cellCenter.LngDegrees);
        eastCoord = newLocation(15, 90, cellCenter.LatDegrees, cellCenter.LngDegrees);
        southCoord = newLocation(15, 180, cellCenter.LatDegrees, cellCenter.LngDegrees);
        westCoord = newLocation(15, 270, cellCenter.LatDegrees, cellCenter.LngDegrees);
        StartCoroutine(QueryOverpass());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Coordinate newLocation(int distance,double direction, double lat, double lon)
    {
        double R = 6378.1; //Radius of the Earth
        double brng = math.radians(direction); //Bearing is 90 degrees converted to radians.
        int d = distance; //Distance in km

        //lat2  52.20444 - the lat result I'm hoping for
        //lon2  0.36056 - the long result I'm hoping for.

        double lat1 = math.radians(lat); //Current lat point converted to radians
        double lon1 = math.radians(lon); //Current long point converted to radians

        double lat2 = math.asin(math.sin(lat1) * math.cos(d / R) +
             math.cos(lat1) * math.sin(d / R) * math.cos(brng));

        double lon2 = lon1 + math.atan2(math.sin(brng) * math.sin(d / R) * math.cos(lat1),
                     math.cos(d / R) - math.sin(lat1) * math.sin(lat2));

        lat2 = math.degrees(lat2);
        lon2 = math.degrees(lon2);
        Coordinate coord = new Coordinate();
        coord.lat = lat2;
        coord.lon = lon2;
        return coord;
    }
    public NotableFeatures getHome()
    {
        int randIndex = UnityEngine.Random.Range(1,features.Count);
        for (int i = 0; i < features.Count;i++ )
        {
            if (features[(features.Count-1)%randIndex].type == "town"|| features[(features.Count - 1) % randIndex].type == "village"|| features[(features.Count - 1) % randIndex].type == "hamlet")
            {
                if (features[(features.Count - 1) % randIndex].wikidata!= "") {
                    return features[(features.Count - 1) % randIndex];
                }
            }
            randIndex++;
        }
        return new NotableFeatures();
    }
    public NotableFeatures getOccupation()
    {
        int randIndex = UnityEngine.Random.Range(1, features.Count);
        foreach (var t in features)
        {
            //if (features[(features.Count - 1) % randIndex].type == "occupation")
            //{
            return features[(features.Count - 1) % randIndex];
            //}
            //randIndex++;
        }
        return new NotableFeatures();
    }
    public NotableFeatures getHobby()
    {
        int randIndex = UnityEngine.Random.Range(1, features.Count);
        for (int i = 0; i < features.Count; i++)
        {
            //if (features[(features.Count - 1) % randIndex].type == "attendee" || features[(features.Count - 1) % randIndex].type == "area")
            //{
                return features[(features.Count - 1) % randIndex];
            //}
            //randIndex++;
        }
        return new NotableFeatures();
    }
    public NotableFeatures getDeath()
    {
        int randIndex = UnityEngine.Random.Range(1, features.Count);
        for (int i = 0; i < features.Count; i++)
        {
            if (features[(features.Count - 1) % randIndex].type == "town" || features[(features.Count - 1) % randIndex].type == "village" || features[(features.Count - 1) % randIndex].type == "hamlet")
            {
                return features[(features.Count - 1) % randIndex];
            }
            randIndex++;
        }
        return new NotableFeatures();
    }
    private IEnumerator QueryOverpass()
    {
        WWW w = new WWW("https://lz4.overpass-api.de/api/interpreter?data=[out:json];(node(if:count_tags() > 0)(" + southCoord.lat+","+ westCoord.lon + ","+ northCoord.lat + "," + eastCoord.lon  + ");<;);out body;");
        yield return w;
        if (w.error != null)
        {
            Debug.Log("Error .. " + w.error);
            // for example, often 'Error .. 404 Not Found'
        }
        else
        {
            Debug.Log("Found ... ==>" + w.text + "<==");
            // don't forget to look in the 'bottom section'
            // of Unity console to see the full text of
            // multiline console messages.
        }
        int indent = 0;
        List<string> tags = new List<string>();
        string name = "";
        string type = "";
        int population = 0;
        string wikidata = "";
        float lat = 0.0f;
        float lon = 0.0f;
        int id = -1;
        string[] textSplit = w.text.Split('\n');
        for (int i = 0; i < textSplit.Length;i++)
        {
            if (indent == 2)
            {
                if (textSplit[i].Contains("\"id\":"))
                {
                    int.TryParse(Regex.Replace(textSplit[i], "[^0-9]", ""), out id);
                }
                if (textSplit[i].Contains("\"lat\":"))
                {
                    float.TryParse(Regex.Replace(textSplit[i], "[^0-9]", ""), out lat);
                }
                if (textSplit[i].Contains("\"lon\":"))
                {
                    float.TryParse(Regex.Replace(textSplit[i], "[^0-9]", ""), out lon);
                }
            }
            if (indent == 3)
            {
                if (textSplit[i].Contains("name\":"))
                {
                    string nameSplit = textSplit[i].Split(':')[1].Replace("\"", "").Replace(",", "");
                    name = nameSplit;
                }
                if (textSplit[i].Contains("natural\": \"peak"))
                {
                    tags.Add("cliff");
                    
                    type = "area";
                }
                if (textSplit[i].Contains("natural\": \"cliff"))
                {
                    tags.Add("cliff");
                    type = "area";
                }
                if (textSplit[i].Contains("amenity\": \"grave_yard"))
                {
                    tags.Add("graveyard");
                    type = "area";
                }
                if (textSplit[i].Contains("amenity\": \"post_office"))
                {
                    tags.Add("postoffice");
                    type = "occupation";
                }
                if (textSplit[i].Contains("leisure\": \"park"))
                {
                    tags.Add("park");
                    type = "area";
                }
                if (textSplit[i].Contains("amenity\": \"school"))
                {
                    tags.Add("school");
                    if (i%1==0)
                    {
                        type = "occupation";
                    }
                    if (i % 2 == 1)
                    {
                        type = "attendee";
                    }
                }
                if (textSplit[i].Contains("amenity\": \"place_of_worship"))
                {
                    tags.Add("church");
                    type = "attendee";
                }
                if (textSplit[i].Contains("waterway\": \"river") || textSplit[i].Contains("water\": \"pond") || textSplit[i].Contains("water\": \"lake") || textSplit[i].Contains("water\": \"reservoir"))
                {
                    tags.Add("water");
                    type = "area";
                }
                if (textSplit[i].Contains("railway\": \"rail"))
                {
                    tags.Add("railroad");
                    type = "occupation";
                }
                if (textSplit[i].Contains("place\": \"town"))
                {
                    tags.Add("town");
                    type = "town";
                }
                if (textSplit[i].Contains("place\": \"hamlet"))
                {
                    tags.Add("hamlet");
                    type = "hamlet";
                }
                if (textSplit[i].Contains("place\": \"village"))
                {
                    tags.Add("village");
                    type = "village";
                }
                if (textSplit[i].Contains("\"wikidata\":"))
                {
                    string nameSplit = textSplit[i].Split(':')[1].Replace("\"", "").Replace(",","").Replace(" ", "");
                    wikidata = nameSplit;
                }
                if (textSplit[i].Contains("\"population\":"))
                {
                    int.TryParse(Regex.Replace(textSplit[i], "[^0-9]", ""), out population);
                }
            }
            if (textSplit[i].Contains("{"))
            {
                indent += 1;
            }
            if (textSplit[i].Contains("}"))
            {
                indent -= 1;
            }
            if (indent == 1)
            {
                NotableFeatures newFeature = new NotableFeatures();
                newFeature.id = id;
                newFeature.name = name;
                newFeature.population = population;
                newFeature.type = type;
                newFeature.wikidata = wikidata;
                string newTags = "";
                for (int t = 0; t<tags.Count;t++)
                {
                    newTags += tags[t];
                    if (t < tags.Count)
                    {
                        newTags += ";";
                    }
                }
                newFeature.tags = newTags;
                if (name != "" && type!="" && id!=0)
                {
                    features.Add(newFeature);
                }
                id = 0;
                name = "";
                population = 0;
                type = "";
                wikidata = "";
                tags.Clear();
            }
        }
        for (int f = 0; f<features.Count;f++)
        {
            //Debug.Log(features[f].name);
        }
        NotableFeatures home = getHome();
        NotableFeatures hobby = getHobby();
        NotableFeatures occupation = getOccupation();
        NotableFeatures death = getDeath();
        string homeName = home.name;
        string hobbyName = hobby.name;
        string occupationName = occupation.name;
        string deathName = death.name;
        Debug.Log(homeName);
        Debug.Log(hobbyName);
        Debug.Log(occupationName);
        Debug.Log(deathName);
    }
}
