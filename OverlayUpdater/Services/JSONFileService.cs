using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OverlayUpdater.Models;
using System.IO;
using System.Threading.Tasks;

namespace OverlayUpdater.Services
{
    public class JSONFileService
    {
        public async Task<ProgressBarJSON> ReadFile(string folderPath)
        {
            using var reader = new StreamReader($"{folderPath}/data.json");
            string json = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<ProgressBarJSON>(json);
        }

        public async Task WriteFile(string folderPath, ProgressBarJSON json)
        {
            using var writer = new StreamWriter($"{folderPath}/data.json");
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var serializer = JsonSerializer.Create(serializerSettings);
            await Task.Run(()=>serializer.Serialize(writer, json));
        }
    }
}
