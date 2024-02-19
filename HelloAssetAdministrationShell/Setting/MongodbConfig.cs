namespace HelloAssetAdministrationShell.Setting
{
    public class MongodbConfig
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public string AasCollection { get; set; }
        public string AasSubmodelCollection { get; set; }
    }
}