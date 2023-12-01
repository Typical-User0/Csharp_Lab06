using Newtonsoft.Json.Linq;

struct Weather
{
    public string Country { get; set; }
    public string Name { get; set; }
    public double Temp { get; set; }
    public string Description { get; set; }
}

class Program
{
    static async Task Main(string[] args)
    {
        string apiKey = "***";

        List<Weather> weatherData = new List<Weather>();
        Random random = new Random();

        using (var httpClient = new HttpClient())
        {
            for (int i = 0; i < 50; i++)
            {
                double latitude = random.NextDouble() * (180.0) - 90.0;
                double longitude = random.NextDouble() * (360.0) - 180.0;
                string url =
                    $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}";
                
                var response = await httpClient.GetStringAsync(url);
                var jsonData = JObject.Parse(response);

                var countryOption = jsonData["sys"]?["country"];

                string country = "Unknown";
                if (countryOption != null)
                {
                    country = countryOption.ToString();
                }

                string name = jsonData["name"].ToString();
                double temp = Convert.ToDouble(jsonData["main"]["temp"]);
                string description = jsonData["weather"][0]["description"].ToString();

                weatherData.Add(new Weather { Country = country, Name = name, Temp = temp, Description = description });
            }
        }

        // LINQ queries
        string countryWithMaxTemp = weatherData.MaxBy(w => w.Temp).Country;
        string countryWithMinTemp = weatherData.MinBy(w => w.Temp).Country;
        double averageTemp = weatherData.Average(w => w.Temp);
        int distinctCountriesCount = weatherData.Select(w => w.Country).Distinct().Count();

        var specificDescriptions = new string[] { "clear sky", "rain", "few clouds" };
        var firstMatch = weatherData.FirstOrDefault(w => specificDescriptions.Contains(w.Description));

        if (!firstMatch.Equals(default(Weather)))
        {
            string specificCountry = firstMatch.Country;
            string specificLocation = firstMatch.Name;
            string specificDescription = firstMatch.Description;

            Console.WriteLine($"Country with the highest temperature: {countryWithMaxTemp}");
            Console.WriteLine($"Country with the lowest temperature: {countryWithMinTemp}");
            Console.WriteLine($"Average temperature in the world: {averageTemp:F2}");
            Console.WriteLine($"Number of distinct countries: {distinctCountriesCount}");
            Console.WriteLine($"First country and location with specific description ({specificDescription}): {specificCountry}, {specificLocation}");
        }
        else
        {
            Console.WriteLine("No data with specific descriptions found.");
        }
    }
}
