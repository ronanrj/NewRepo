using System;
using System.Collections.Generic;
using Mongo.Migration.Demo.Model.Pkg;
using Mongo.Migration.Demo.MongoMigrations.Pkg.Aplicabilidades.V00;
using Mongo.Migration.Demo.MongoMigrations.Pkg.Cars.M00;
using Mongo.Migration.Migrations.Adapters;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.Static;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Demo.Core.Pkg
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Init MongoDB
            var runner = MongoDbRunner.StartForDebugging();
            var client = new MongoClient(runner.ConnectionString);

            // Init MongoMigration
            MongoMigrationClient.Initialize(client, new MongoMigrationSettings()
                {
                    ConnectionString = runner.ConnectionString,
                    Database = "SAC_PlataformaComunicacao"
            },
                new LightInjectAdapter(new LightInject.ServiceContainer()));

            #region
            //client.GetDatabase("TestCars").DropCollection("Car");
            //client.GetDatabase("SAC_PlataformaComunicacao").GetCollection<Aplicabilidades_Teste>("Aplicabilidades_Teste");

            // Insert old and new version of cars into MongoDB
            //var cars = new List<BsonDocument>
            //{
            //    new BsonDocument {{"Dors", 3}, {"Type", "Cabrio"}, {"UnnecessaryField", ""}},
            //    new BsonDocument {{"Dors", 5}, {"Type", "Combi"}, {"UnnecessaryField", ""}},
            //    new BsonDocument {{"Doors", 3}, {"Type", "Truck"}, {"UnnecessaryField", ""}, {"Version", "0.0.1"}},
            //    new BsonDocument {{"Doors", 5}, {"Type", "Van"}, {"Version", "0.1.1"}}
            //};

            //var bsonCollection =
            //    client.GetDatabase("TestCars").GetCollection<BsonDocument>("Car");
            #endregion


            var aplicabilidadeList = new List<BsonDocument>
            {
                new BsonDocument
                {
                    //{"Id", ObjectId.GenerateNewId()},
                    {"Nome", "PV3-020"},
                    {"PontoControle", "TDS"},
                    {"TipoPagamentos", new BsonArray {"Debito"}},
                    {
                        "TipoEntregas", new BsonArray
                        {
                            new BsonDocument
                                {{"Id", ObjectId.GenerateNewId()}, {"Name", "Coleta"}, {"SGPTypeDeliveryId", 15}}
                        }

                    },
                    {"Online", true},
                    {"MarketPlace", false},
                    {"Ativo", true},
                    {"TempoEsperaAplicabilidade", 0},
                    {"TipoAplicabilidade", new BsonDocument {{"Value", "Cancelamento"}}},
                    {"TipoVenda", new BsonDocument {{"Value", "B2C"}}}


                }
            };

            var bsonCollection = client.GetDatabase("SAC_PlataformaComunicacao").GetCollection<BsonDocument>("Aplicabilidades_Teste");
            var bsonCollectionteste = client.GetDatabase("SAC_PlataformaComunicacao").GetCollection<Aplicabilidades_Teste>("Aplicabilidades_Teste");
            var result2 = bsonCollectionteste.FindAsync(_ => true).Result.ToListAsync().Result;

            bsonCollection.InsertManyAsync(aplicabilidadeList).Wait();

            Console.WriteLine("Migrate from:");
            aplicabilidadeList.ForEach(c => Console.WriteLine(c.ToBsonDocument() + "\n"));

            // Migrate old version to current version by reading collection
            var typedCollection = client.GetDatabase("TestCars").GetCollection<Car>("Car");
            var result = typedCollection.FindAsync(_ => true).Result.ToListAsync().Result;

            Console.WriteLine("To:");
            result.ForEach(r => Console.WriteLine(r.ToBsonDocument() + "\n"));

            // Create new car and add it with current version number into MongoDB
            var id = ObjectId.GenerateNewId();
            var type = "Test" + id;
            var car = new Car {Doors = 2, Type = type};

            typedCollection.InsertOne(car);
            var test = typedCollection.FindAsync(Builders<Car>.Filter.Eq(c => c.Type, type)).Result.Single();

            var aggregate = typedCollection.Aggregate()
                .Match(new BsonDocument {{"Dors", 3}});
            var results = aggregate.ToListAsync().Result;

            Console.WriteLine("New Car was created with version: " + test.Version);
            Console.WriteLine("\n");

            Console.WriteLine("\n");
            Console.WriteLine("Press any Key to exit...");
            Console.Read();

            client.GetDatabase("TestCars").DropCollection("Car");
        }
    }
}