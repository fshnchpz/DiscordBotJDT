using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordBotJDT.config
{
    public class JSONReader
    {
        public string token { get; set; }
        public string prefix { get; set; }

        public async Task ReadJSON()
        {
            using (StreamReader sr = new StreamReader("config.json"))
            {
                string jsonCfg = await sr.ReadToEndAsync();
                JSONStructure data = JsonConvert.DeserializeObject<JSONStructure>(jsonCfg);

                this.token = data.token;
                this.prefix = data.prefix;
            }
        }

        internal sealed class JSONStructure
        {
            public string token { get; set; }
            public string prefix { get; set; }
        }
    }
}
