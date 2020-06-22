namespace ConvertConfigToAzureSettings
{

    public class AzureAppSetting
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public bool SlotSetting { get; set; } = true;
    }

}