# MiddleWeather - Nordic Weather Aggregator

Ett demo-projekt som visar min approach till väderaggregering från flera källor.

## Varför denna app?

Istället för att bara lita på EN vädertjänst hämtar jag data från flera:
- **SMHI** (svenska väderdata)
- **YR.no** (norska, riktigt bra API)  
- **OpenWeatherMap** (internationell)

Sen räknar jag ut medelvärden från alla tre. Användaren kan också klicka och se vad varje källa säger individuellt.

## Hur det fungerar

```
┌─────────────┐    ┌─────────────┐    ┌─────────────────┐
│   SMHI      │    │   Met.no    │    │ OpenWeatherMap  │
│ (Sverige)   │    │  (Norge)    │    │ (Internationell)│
└─────────────┘    └─────────────┘    └─────────────────┘
       │                  │                     │
       └──────────────────┼─────────────────────┘
                          │
              ┌───────────▼──────────┐
              │ WeatherDataAggregator │
              │ • Hämtar från alla   │
              │ • Räknar medelvärden │
              │ • Sparar individuell │
              └──────────────────────┘
```

## Funktioner

### Flera väder-sources
- **SMHI**: Sveriges meteorologiska institut (gratis API)
- **Met.no**: Norges väderservice (gratis API)
- **OpenWeatherMap**: Global väderdata (kräver API-nyckel)

### Smart aggregering
- Medelvärden för temp, vind, luftfuktighet
- Summerar nederbörd från alla källor  
- Hittar vanligaste väderbeskrivningen
- Beräknar "prognossäkerhet" baserat på hur många som är överens

### Transparens
- Visar både det aggregerade resultatet OCH vad varje källa säger
- Klicka "Visa individuell data" för att se alla providers
- Användaren kan själv bedöma om källorna är överens

## API Endpoints

- `GET /` - Huvud-sidan med sökformulär
- `GET /api/weather/aggregated?location=Stockholm` - All väderdata för dagen
- `GET /api/weather/demo/14?location=Stockholm` - Demo för specifik timme (14:00)

## Setup

1. Klona repot
2. Kör `dotnet run`  
3. Gå till `https://localhost:5001`

**OBS**: SMHI och Met.no fungerar direkt (gratis APIer). OpenWeatherMap kräver att du lägger till API-nyckel i `AppConstants.cs`.

## Exempel på hur det ser ut

När du söker på "Stockholm" får du något som:

**Aggregerat resultat**: 16.2°C, 7.4 m/s vind, Molnigt

**Individuella källor**:
- SMHI: 15.8°C, Molnigt  
- YR.no: 16.4°C, Delvis molnigt
- OpenWeatherMap: 16.4°C, Broken clouds

**Prognossäkerhet**: 60-80% Relativt Säker (2 av 3 överens om beskrivning)

## Teknisk info

- **ASP.NET Core MVC** 
- **Async/await** för alla API-anrop
- **Dependency injection** för att hantera providers
- **Custom aggregering-logik** i `Medelvader.cs`
- **Error handling** om någon källa inte svarar

## Bakgrund

Detta är en förenklad version av ett större projekt jag jobbar på. I den riktiga versionen:
- Databas för historisk data
- Mer avancerade viktnings-algoritmer  
- Kartor och visualiseringar
- Fler väderkällor

Men grundidén är samma: **ta data från flera källor → skapa medelvärden → visa transparens**