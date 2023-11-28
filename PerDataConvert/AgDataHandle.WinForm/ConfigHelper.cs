using System.Text.RegularExpressions;

namespace AgDataHandle.WinForm
{
    internal class ConfigHelper
    {
        string configFile { get; set; }

        public static ConfigHelper Instance=new ConfigHelper();
        ConfigHelper() 
        {
            configFile= Path.Combine(Directory.GetCurrentDirectory(), "config.ini");
            if (!File.Exists(configFile))
            {
                File.Create(configFile).Close();
            }
        }
        static ConfigHelper() { }

        internal string Read(string key)
        {
            var fileTxt = File.ReadAllText(configFile);
            var mc = Regex.Match(fileTxt, key + @"=(?<value>[\s\S]*?)\r\n");
            if (mc.Success)
            {
                return mc.Groups["value"].Value;
            }
            return null;
        }

        internal void Write(string key, string value)
        {
            var fileTxt = File.ReadAllText(configFile);
            var mc = Regex.Match(fileTxt, $"{key}=(?<value>[\\s\\S]*?)\r\n");
            if (mc.Success)
            {
                fileTxt= Regex.Replace(fileTxt, key + @"=(?<value>[\s\S]*?)\r\n", key + "=" + value + "\r\n");
            }
            else
            {
                fileTxt += key + "=" + value + "\r\n";
            }
            File.WriteAllText(configFile, fileTxt);
        }

    }
}
