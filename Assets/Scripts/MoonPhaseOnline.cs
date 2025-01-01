using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;


class Program
{

    public void Run()
    {
        DateTime currentDate = DateTime.Now;
        MoonPhaseOnline calculator = new MoonPhaseOnline();
        MoonPhaseOnline.MoonPhaseType phase = calculator.GetPhase(currentDate).GetAwaiter().GetResult();
        Console.WriteLine($"Current Moon Phase: {phase}");

        // Additional code or logic can be added here
    }
}
public class MoonPhaseOnline:MonoBehaviour
{
    public void Start()
    {
        new Program().Run();
    }
    public enum MoonPhaseType
    {
        NewMoon,
        WaxingCrescent,
        FirstQuarter,
        WaxingGibbous,
        FullMoon,
        WaningGibbous,
        LastQuarter,
        WaningCrescent
    }

    public async Task<MoonPhaseType> GetPhase(DateTime date)
    {
        try
        {
            string apiUrl = $"https://api.sunrise-sunset.org/json?lat=0&lng=0&date={date:yyyy-MM-dd}&formatted=0";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    var moonPhase = (Newtonsoft.Json.Linq.JValue)Newtonsoft.Json.Linq.JObject.Parse(data)["moon_phase"]["phaseofMoon"];
                    switch (moonPhase.Value<string>())
                    {
                        case "New Moon":
                            return MoonPhaseType.NewMoon;
                        case "First Quarter":
                            return MoonPhaseType.FirstQuarter;
                        case "Full Moon":
                            return MoonPhaseType.FullMoon;
                        case "Last Quarter":
                            return MoonPhaseType.LastQuarter;
                        default:
                            return MoonPhaseType.WaxingCrescent;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred while fetching moon phase: {ex.Message}");
        }

        return MoonPhaseType.NewMoon; // Default to new moon if unable to fetch accurate data
    }
}
