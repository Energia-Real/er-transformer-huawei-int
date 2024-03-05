namespace er_transformer_huawei_int.Models
{
    public class DeviceData
    {
        public List<Device> data { get; set; }
        public int failCode { get; set; }
        public string message { get; set; }
        public Params @params { get; set; }
        public bool success { get; set; }
    }
}
