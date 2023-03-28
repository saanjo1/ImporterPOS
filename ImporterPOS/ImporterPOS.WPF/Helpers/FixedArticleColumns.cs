using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.WPF.Helpers
{
    public class FixedArticleColumns
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Storage { get; set; }
        public string BarCode { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
        public string PricePerUnit { get; set; }
        public string Item { get; set; }
        public string ColorDescription { get; set; }
        public string ItemSize { get; set; }


        public FixedArticleColumns()
        {
            var json = File.ReadAllText("appconfigsettings.json");
            var settings = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);

            Name = settings["Articles"]["Name"];
            Category = settings["Articles"]["Category"];
            Storage = settings["Articles"]["Storage"];
            BarCode = settings["Articles"]["BarCode"];
            Price = settings["Articles"]["Price"];
            Quantity = settings["Articles"]["Quantity"];
            PricePerUnit = settings["Articles"]["PricePerUnit"];
            ItemSize = settings["Articles"]["ItemSize"];
            Item = settings["Articles"]["Item"];
            ColorDescription = settings["Articles"]["ColorDescription"];
        }
    }
}
