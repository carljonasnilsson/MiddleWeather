using WeatherAggregatorDemo.Models;

namespace WeatherAggregatorDemo.Services
{
    /// <summary>
    /// Min egen logik för att skapa medelvärden från väderdata
    /// Baserad på samma princip som pissvader men enklare
    /// Jonas Nilsson 2024
    /// </summary>
    public class Medelvader
    {
        /// <summary>
        /// Huvudfunktion som tar väderdata och skapar medelvärden
        /// Funkar för alla providers - smhi, metno, owm osv
        /// </summary>
        public static AggregatedWeatherData ProcessWeatherData(List<WeatherData> weatherData)
        {
            if (!weatherData.Any())
            {
                return new AggregatedWeatherData();
            }

            var validData = weatherData.Where(w => w != null).ToList();
            if (!validData.Any())
            {
                return new AggregatedWeatherData();
            }

            return new AggregatedWeatherData
            {
                Hour = validData.First().Hour,
                Timestamp = validData.First().Timestamp,
                
                // Medelvärden
                AverageTemperature = Math.Round(validData.Average(d => d.Temperature), 1),
                AverageWindSpeed = Math.Round(validData.Average(d => d.WindSpeed), 1),
                AverageHumidity = Math.Round(validData.Average(d => d.Humidity), 0),
                AverageAirPressure = Math.Round(validData.Average(d => d.AirPressure), 0),
                
                // Summa för nederbörd (eftersom det är kumulativt)
                TotalPrecipitation = Math.Round(validData.Sum(d => d.Precipitation), 2),
                
                // Mest vanliga beskrivningen
                MostCommonDescription = GetMostCommonDescription(validData),
                
                PrognosSakerhet = CalculatePrognosSakerhet(validData),
                
                // Behåll individuell data
                ProviderData = validData.OrderBy(d => d.Provider).ToList()
            };
        }

        /// <summary>
        /// Hittar den vanligaste väderbeskrivningen bland providers
        /// </summary>
        private static string GetMostCommonDescription(List<WeatherData> data)
        {
            if (!data.Any()) return "Okänt väder";

            // Gruppera beskrivningar och hitta den vanligaste
            var mostCommon = data
                .Where(d => !string.IsNullOrEmpty(d.Description))
                .GroupBy(d => NormalizeDescription(d.Description))
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            return mostCommon?.Key ?? "Varierat väder";
        }

        private static string CalculatePrognosSakerhet(List<WeatherData> data)
        {
            if (!data.Any()) return "0% Ingen data";

            var totalProviders = data.Count;
            
            // gruppera beskrivningar och hitta den vanligaste
            var descriptionGroups = data
                .GroupBy(d => NormalizeDescription(d.Description))
                .OrderByDescending(g => g.Count())
                .ToList();

            var mostCommonCount = descriptionGroups.FirstOrDefault()?.Count() ?? 0;

            // samma logik som pissvader - baserat på antal providers som är överens
            switch (totalProviders)
            {
                case 3: // tre providers
                    switch (mostCommonCount)
                    {
                        case 3: return "80-100% Mycket Säker"; // alla överens
                        case 2: return "60-80% Relativt Säker"; // 2 av 3 överens
                        default: return "0-60% Osäker"; // ingen överensstämmelse
                    }
                    
                case 4: // fyra providers (om jag lägger till fler senare)
                    switch (mostCommonCount)
                    {
                        case 4: return "80-100% Mycket Säker"; // alla överens
                        case 3: return "60-80% Relativt Säker"; // 3 av 4 överens
                        case 2: return "40-50% Ganska säker"; // 2 av 4 överens
                        default: return "0-40% Mycket Osäker"; // 1 av 4 överens
                    }
                    
                default: // fallback för andra antal
                    double ratio = (double)mostCommonCount / totalProviders;
                    if (ratio >= 1.0) return "80-100% Mycket Säker";
                    else if (ratio >= 0.66) return "60-80% Relativt Säker";
                    else if (ratio >= 0.33) return "40-60% Ganska säker";
                    else return "0-40% Mycket Osäker";
            }
        }

        /// <summary>
        /// Normaliserar beskrivningar från olika providers för bättre gruppering
        /// </summary>
        private static string NormalizeDescription(string description)
        {
            if (string.IsNullOrEmpty(description)) return "Okänt";

            var normalized = description.ToLower().Trim();

            // Översättningar och normaliseringar
            var mappings = new Dictionary<string, string>
            {
                // Soligt/klart
                { "clear sky", "Klart" },
                { "clear", "Klart" },
                { "soligt", "Klart" },
                { "klar", "Klart" },
                
                // Molnigt
                { "few clouds", "Lätt molnigt" },
                { "scattered clouds", "Molnigt" },
                { "broken clouds", "Molnigt" },
                { "overcast clouds", "Mulet" },
                { "molnigt", "Molnigt" },
                { "halvklart", "Lätt molnigt" },
                { "mulet", "Mulet" },
                
                // Regn
                { "light rain", "Lätt regn" },
                { "moderate rain", "Regn" },
                { "heavy rain", "Kraftigt regn" },
                { "shower rain", "Regnskurar" },
                { "regn", "Regn" },
                { "duggregn", "Lätt regn" },
                
                // Snö
                { "snow", "Snö" },
                { "light snow", "Lätt snöfall" },
                { "snö", "Snö" },
                
                // Dimma
                { "mist", "Dimma" },
                { "fog", "Dimma" },
                { "dimma", "Dimma" },
                { "dis", "Dimma" }
            };

            foreach (var mapping in mappings)
            {
                if (normalized.Contains(mapping.Key))
                {
                    return mapping.Value;
                }
            }

            // Fallback - kapitalisera första bokstaven
            return char.ToUpper(normalized[0]) + normalized.Substring(1);
        }
    }
}