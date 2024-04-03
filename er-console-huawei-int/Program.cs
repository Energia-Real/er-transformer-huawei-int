using er_console_huawei_int.Services;

var mongoDataService = new MongoDataService("mongodb://localhost:27017", "myDatabase", "myCollection");
var dataProcessor = new DataProcessor(mongoDataService);



dataProcessor.ProcessData("Datos a procesar");