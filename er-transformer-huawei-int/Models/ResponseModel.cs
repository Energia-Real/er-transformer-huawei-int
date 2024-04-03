namespace er_transformer_huawei_int.Models
{
    public class ResponseModel<T>
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorCode { get; set; }
        public T Data { get; set; }
    }
}
