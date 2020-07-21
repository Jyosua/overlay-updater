using Newtonsoft.Json;
using OverlayUpdater.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OverlayUpdater.Services
{
    public class JSONFileService
    {
        public async Task<ProgressBarJSON> ReadFile(string filepath)
        {
            using var reader = new StreamReader(filepath);
            string json = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<ProgressBarJSON>(json);
        }

        public async Task WriteFile(string filepath, ProgressBarJSON json)
        {
            using var writer = new StreamWriter(filepath);
            var serializer = new JsonSerializer();
            await Task.Run(()=>serializer.Serialize(writer, json));
        }
    }
}
