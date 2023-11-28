namespace NetTopologySuite.IO.Postgis
{
    public class SdeConnectstringParam
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Databese { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        //public string SdePath { get { return $"PG:dbname={Databese} host={Host} port={Port} user={Username} password={Password}"; } }
    }
}
