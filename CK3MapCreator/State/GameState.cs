using CK3MapCreator.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CK3MapCreator
{
    internal class GameState
    {
        static GameState instance;
        public List<ProvinceCK3> provinces { get; set; }

        [JsonConstructor]
        public GameState(){}

        public static GameState GetGameState()
        {
            if (instance == null)
            {
                instance = new GameState();
            }
            return instance;
        }

        public void updateProvinces(List<ProvinceCK3> p)
        {
            instance.provinces = p;
        }

        public List<ProvinceCK3> getProvinces()
        {
            if (instance.provinces == null)
            {
                Logger.getLogger().Log("Cannot get provinces before they are initialized");
            }
            return instance.provinces;
        }

        public void exportToJson()
        {
            var options = new JsonSerializerOptions() //https://stackoverflow.com/questions/69664644/serialize-deserialize-system-drawing-color-with-system-text-json
            {
                Converters = {
                    new ColorJsonConverter()
                }
            };

            Console.WriteLine("Province number = " + instance.provinces.Count);
            using (FileStream fs = File.Create(FileLoader.getFileLoader().getFilePath("ExportData.json")))
            {
                JsonSerializer.Serialize(fs, this, options);
            }
        }

        public void importFromJson()
        {
            var options = new JsonSerializerOptions() //https://stackoverflow.com/questions/69664644/serialize-deserialize-system-drawing-color-with-system-text-json
            {
                Converters = {
                    new ColorJsonConverter()
                }
            };

            String s = File.ReadAllText(FileLoader.getFileLoader().getFilePath("ExportData.json"));

            //File.WriteAllText("data.json", JsonSerializer.Serialize(test, options));
            //Test test2 = JsonSerializer.Deserialize<Test>(File.ReadAllText("data.json"), options);


            instance = JsonSerializer.Deserialize<GameState>(s, options);

        }

        public void printState()
        {
            if(instance == null)
            {
                Logger.getLogger().Log("There is no data loaded right now");
                return;
            }

            Logger.getLogger().Log("Province Count: " + instance.provinces.Count.ToString());
            foreach (ProvinceCK3 p in instance.provinces)
            {
                Logger.getLogger().Log("Name = " + p.name + " - ID = " + p.ID + " - Color = " + p.color.ToString() + " - Terrain = " + p.terrain);
            }
        }
    }

    public class ColorJsonConverter : JsonConverter<Color> //https://stackoverflow.com/questions/69664644/serialize-deserialize-system-drawing-color-with-system-text-json
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => ColorTranslator.FromHtml(reader.GetString());

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options) => writer.WriteStringValue("#" + value.R.ToString("X2") + value.G.ToString("X2") + value.B.ToString("X2").ToLower());
    }
}
//damian is cute:)