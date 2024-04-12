namespace er_transformer_huawei_int.Models.Dto
{
    using MongoDB.Bson;

    public class TokenDto
    {
        public ObjectId _id { get; set; }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
