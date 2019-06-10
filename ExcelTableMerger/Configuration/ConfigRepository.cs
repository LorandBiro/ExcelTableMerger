using Newtonsoft.Json;
using System;
using System.IO;

namespace ExcelTableMerger.Configuration
{
    public sealed class ConfigRepository : IConfigRepository
    {
        public static readonly IConfigRepository Instance = new ConfigRepository("config.json");

        private readonly string fileName;
        private Config cache;

        public ConfigRepository(string fileName)
        {
            this.fileName = fileName ?? throw new System.ArgumentNullException(nameof(fileName));
        }

        public Config Get()
        {
            if (this.cache == null)
            {
                if (File.Exists(this.fileName))
                {
                    this.cache = JsonConvert.DeserializeObject<Config>(File.ReadAllText(this.fileName));
                }
                else
                {
                    this.cache = new Config();
                }
            }
            
            return this.cache;
        }

        public void Set(Config config)
        {
            this.cache = config ?? throw new ArgumentNullException(nameof(config));
            File.WriteAllText(this.fileName, JsonConvert.SerializeObject(config));
        }
    }
}
