namespace er_transformer_huawei_int.Data
{
    using er_transformer_huawei_int.Models;
    using er_transformer_huawei_int.Models.Dto;
    using MongoDB.Driver;

    public class MongoService : IMongoRepository
    {
        private readonly IConfiguration _configuration;

        private readonly MongoClient _MongoClient;
        private IMongoDatabase _database;

        public MongoService(IConfiguration configuration)
        {
            _configuration = configuration;
            var connectionString = _configuration["ConnectionStrings:MongoGW"];
            var dataBase = _configuration["DataBases:MongoGWDataBase"];

            _MongoClient = new MongoClient(connectionString);
            _database = _MongoClient.GetDatabase(dataBase);
            _configuration = configuration;
        }

        public async Task<TokenDto> SetToken(string token, string user)
        {
            try
            {
                var collection = _database.GetCollection<TokenDto>("Auth");

                // Verificar si el índice existe en el campo devDn
                var indexKeysDefinition = Builders<TokenDto>.IndexKeys.Ascending(x => x.Name);
                var indexModel = new CreateIndexModel<TokenDto>(indexKeysDefinition);
                var indexExists = await collection.Indexes.CreateOneAsync(indexModel);

                // Si el índice no existe, crearlo
                if (string.IsNullOrEmpty(indexExists))
                {
                    await collection.Indexes.CreateOneAsync(indexModel);
                }

                // Crear un filtro para la consulta
                var filtro = Builders<TokenDto>.Filter.Eq("Name", user);
                var datetime = DateTime.Now.Kind;
                // Crear un objeto de actualización para especificar los campos que se van a modificar
                var updateDefinition = Builders<TokenDto>.Update
                    .Set(x => x.Value, token)
                    .Set(x => x.Date, DateTime.Now);

                var options = new FindOneAndUpdateOptions<TokenDto>
                {
                    IsUpsert = true, // Insertará el documento si no se encuentra uno con el filtro especificado
                    ReturnDocument = ReturnDocument.After // Devolverá el documento actualizado
                };

                // Realizar la consulta y actualizar el documento si se encuentra, o insertar uno nuevo si no se encuentra
                var resultado = await collection.FindOneAndUpdateAsync(filtro, updateDefinition, options);

                if (resultado is null)
                {
                    // No se encontró un documento, entonces insertamos uno nuevo
                    var newToken = new TokenDto { Name = user, Value = token, Date = DateTime.Now };
                    await collection.InsertOneAsync(newToken);
                    resultado = newToken;
                }

                return resultado;
            }
            catch (Exception ex)
            {
                // Manejar la excepción y devolver un resultado vacío o lanzarla nuevamente según sea necesario
                Console.WriteLine($"Error al conectar con la base de datos: {ex.Message}");
                return new TokenDto();
            }
        }

        public async Task<List<TokenDto>> GetToken(string user)
        {
            try
            {
                var collection = _database.GetCollection<TokenDto>("Auth");

                // Verificar si el índice existe en el campo devDn
                var indexKeysDefinition = Builders<TokenDto>.IndexKeys.Ascending(x => x.Name);
                var indexModel = new CreateIndexModel<TokenDto>(indexKeysDefinition);
                var indexExists = await collection.Indexes.CreateOneAsync(indexModel);

                // Si el índice no existe, crearlo
                if (string.IsNullOrEmpty(indexExists))
                {
                    await collection.Indexes.CreateOneAsync(indexModel);
                }

                // Crear un filtro para la consulta
                var filtro = Builders<TokenDto>.Filter.Eq("Name", user);

                // Realizar la consulta y obtener el primer resultado que cumpla con el filtro
                var resultado = await collection.Find(filtro).ToListAsync();

                return resultado;
            }
            catch (Exception ex)
            {
                // Manejar la excepción y devolver un resultado vacío o lanzarla nuevamente según sea necesario
                Console.WriteLine($"Error al conectar con la base de datos: {ex.Message}");
                return new List<TokenDto>();
            }
        }
    }
}
