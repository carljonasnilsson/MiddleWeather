namespace WeatherAggregatorDemo
{
    public static class AppConstants
    {
        // OpenWeatherMap API - lägg till din egen nyckel här för att testa
        public static readonly string OpenWeatherMapApiUrl = "https://api.openweathermap.org/data/2.5/forecast";
        public static readonly string OpenWeatherMapApiKey = "DIN_API_NYCKEL_HÄR"; // ersätt med riktig nyckel

        public static readonly string GeocodeApiUrl = "https://api.opencagedata.com/geocode/v1/json";
        public static readonly string GeocodeApiKey = "DEMO_REPLACE_WITH_REAL_KEY";

        // Dessa kräver inga API-nycklar
        public static readonly string SmhiApiUrl = "https://opendata-download-metfcst.smhi.se/api/category/pmp3g/version/2/geotype/point";
        public static readonly string MetNoApiUrl = "https://api.met.no/weatherapi/locationforecast/2.0/compact";
        
        // User agent för Met.no (krävs enligt deras API-regler)
        public static readonly string UserAgent = "WeatherAggregatorDemo/1.0 (https://github.com/yourrepo/demo)";
    }
}