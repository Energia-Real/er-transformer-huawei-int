namespace er_transformer_huawei_int.Models
{
    public class Data
    {
        public List<Plant> List { get; set; }
        public int PageCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
    }

    public class PlantList
    {
        public Data Data { get; set; }
        public int FailCode { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
