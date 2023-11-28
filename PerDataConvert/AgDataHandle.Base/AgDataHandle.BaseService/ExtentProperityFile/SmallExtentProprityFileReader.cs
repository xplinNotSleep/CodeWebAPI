using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.ExtentProperityFile
{
    public class SmallExtentProprityFileReader : ExtentProprityFileReaderBase
    {
        public override void AddExtentProperityFile(string extentProperityPath)
        {
            if (File.Exists(extentProperityPath) == false)
            {
                LoadStatue = false;
                return;
            }
            GetGltfSourceInfo(extentProperityPath);

            string JsonPath = Path.ChangeExtension(extentProperityPath, "json");
            StreamReader sr = new StreamReader(JsonPath);
            var jr = new JsonSerializer().Deserialize(new JsonTextReader(sr)) as JObject;
            sr.Close();

            M_ComponentCount = jr.Count;

            var objects = jr["objects"];
            if (objects == null)
            {
                LoadStatue = false;
                return;
            }
            jr = (JObject)objects;

            var components = jr["components"] as JArray;
            if (components == null)
            {
                LoadStatue = false;
                return;
            }

            if (components.Count == 0)
            {
                LoadStatue = false;
                return;
            }
            bool hasUUID = false;
            foreach (JObject item in components)
            {
                if (item["uuid"] != null)
                {
                    hasUUID = true;
                }
                else
                    continue;
                if (!FiledNames.Contains(item["uuid"].ToString()))
                {
                    FiledNames.Add(item["uuid"].ToString());
                }
            }
            if (hasUUID == false)
            {
                LoadStatue = false;
                return;
            }
            foreach (JObject o in components)
            {
                if (!ExtentProperities.ContainsKey(o["uuid"].ToString()))
                {
                    ExtentProperities.Add(o["uuid"].ToString(), o.ToString());
                }
            }
            var building = jr["building"];
            if (building != null && building["Baselines"] != null && building["location"] != null && building["Height"] != null)
            {
                BuildingProperities.Add("0", building.ToString());
            }
            LoadStatue = true;
        }
        public override string GetProperity(string uuid)
        {
            if (ExtentProperities.ContainsKey(uuid))
                return ExtentProperities[uuid];
            else
                return null;
        }
    }
}
