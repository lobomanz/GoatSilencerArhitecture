using System.Text.Json;
using System.IO;

namespace GoatSilencerArchitecture.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

        public string GetProjectsMainPageLayoutType()
        {
            var json = File.ReadAllText(_appSettingsPath);
            var jsonObj = JsonSerializer.Deserialize<JsonElement>(json);
            if (jsonObj.TryGetProperty("ProjectsMainPageLayoutType", out var layoutTypeElement))
            {
                return layoutTypeElement.GetString();
            }
            return null;
        }

        public void SetProjectsMainPageLayoutType(string layoutType)
        {
            var json = File.ReadAllText(_appSettingsPath);
            var jsonObj = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            jsonObj["ProjectsMainPageLayoutType"] = layoutType;
            var updatedJson = JsonSerializer.Serialize(jsonObj, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_appSettingsPath, updatedJson);
        }
    }
}
