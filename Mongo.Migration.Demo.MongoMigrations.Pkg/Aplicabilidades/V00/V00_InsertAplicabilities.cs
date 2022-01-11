using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Mongo.Migration.Demo.Model.Pkg;
using Mongo.Migration.Migrations.Document;
using MongoDB.Bson;
using Mongo.Migration;
using Mongo.Migration.Migrations.Database;
using MongoDB.Driver;

namespace Mongo.Migration.Demo.MongoMigrations.Pkg.Aplicabilidades.V00
{
    public class V00_InsertAplicabilities : DocumentMigration<Aplicabilidades_Teste>
    {
        private readonly IMongoClient _client;
        public V00_InsertAplicabilities(IMongoClient client) : base("0.0.1")
        {
            _client = client;
        }

        public override void Down(BsonDocument document)
        {

        }

        public override void Up(BsonDocument document)
        {
            var bsonCollection = _client.GetDatabase("SAC_PlataformaComunicacao").GetCollection<BsonDocument>("Aplicabilidades_Teste");
            var resultList = bsonCollection.FindAsync(_ => true).Result.ToListAsync().Result;
        }
    }
}
