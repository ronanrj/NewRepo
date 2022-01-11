using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mongo.Migration.Demo.Model.Pkg;
using Mongo.Migration.Demo.MongoMigrations.Pkg.Aplicabilidades.V00;
using Mongo.Migration.Documents;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;
using Mongo.Migration.Startup.Static;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Demo.WebCore.Pgk
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        private IMongoClient _client;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton<IMongoClient>(_client = new MongoClient(_configuration.GetSection("MongoDb:ConnectionString").Value));

            var runner = MongoDbRunner.Start();

            services.AddMigration(new MongoMigrationSettings()
            {
                ConnectionString = runner.ConnectionString,
                Database = _configuration.GetSection("MongoDb:Database").Value,
                VersionFieldName = "0.0.1",
                DatabaseMigrationVersion = new DocumentVersion(2, 0, 0)
            });

            #region inserindo lista de Bson
            var bsonList = GetDocumentsBsonList();
            InsertAplicabilidadeList(bsonList);
            #endregion

            #region inserindo arquivo via Objeto

            var aplicabilidade = GetDocumentsObjeto();
            InsertAplicabilidade(aplicabilidade);

            #endregion

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.Run(
                async context =>
                {
                    // Migrate old version to current version by reading collection
                    var typedCollection = _client.GetDatabase("SAC_PlataformaComunicacao").GetCollection<Aplicabilidades_Teste>("Aplicabilidades_Teste");

                    // Create new car and add it with current version number into MongoDB
                    //var id = ObjectId.GenerateNewId();
                    //var type = "Test" + id;
                    //var car = new Car {Doors = 2, Type = type};

                    //typedCollection.InsertOne(car);
                    //var test = typedCollection.FindAsync(Builders<Car>.Filter.Eq(c => c.Type, type)).Result.Single();

                    //var aggregate = typedCollection.Aggregate()
                    //    .Match(new BsonDocument {{"Dors", 3}});
                    //var results = aggregate.ToListAsync().Result;

                    //var result = typedCollection.FindAsync(_ => true).Result.ToListAsync().Result;

                    //var response = "";
                    //result.ForEach(
                    //    d =>
                    //    {
                    //         response += d.ToBsonDocument().ToString() + "\n";            
                    //    });

                    //await context.Response.WriteAsync(response);
                });
        }

        #region Metodos Privados
        private void InsertAplicabilidade(Aplicabilidades_Teste aplicabilidade)
        {
            try
            {
                var typedCollection = GetTypedCollection();
                typedCollection.InsertOne(aplicabilidade);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private IMongoCollection<Aplicabilidades_Teste> GetTypedCollection()
        {
            var typedCollection = _client.GetDatabase("SAC_PlataformaComunicacao")
                .GetCollection<Aplicabilidades_Teste>("Aplicabilidades_Teste");
            return typedCollection;
        }

        private Aplicabilidades_Teste GetDocumentsObjeto()
        {
            return new Aplicabilidades_Teste
            {
                _Id = Guid.NewGuid(),
                Nome = "PV3-021",
                PontoControle = "TDS",
                TipoPagamentos = new[] { "Debito" },
                TipoEntregas = new Tipoentrega[]
                {
                    new Tipoentrega()
                    {
                        _Id = Guid.NewGuid(),
                        Name = "Coleta",
                        SGPTypeDeliveryId = 15
                    }
                },
                Online = true,
                MarketPlace = false,
                Ativo = true,
                TempoEsperaAplicabilidade = 0,
                TipoAplicabilidade = new Tipoaplicabilidade()
                {
                    Value = "Cancelamento"
                },
                TipoVenda = new Tipovenda()
                {
                    Value = "B2C"
                }

            };
        }

        private List<BsonDocument> GetDocumentsBsonList()
        {
            var aplicabilidadeList = new List<BsonDocument>();
            try
            {
                aplicabilidadeList.Add(
                    new BsonDocument
                        {
                            {"Id", ObjectId.GenerateNewId()},
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
                    );
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new NotImplementedException();
            }

            return aplicabilidadeList;
        }

        private void InsertAplicabilidadeList(List<BsonDocument> bsonList)
        {
            try
            {
                var bsonCollection = GetMongoCollection();
                bsonCollection.InsertManyAsync(bsonList).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private IMongoCollection<BsonDocument> GetMongoCollection()
        {
            var bsonCollection = _client.GetDatabase("SAC_PlataformaComunicacao")
                .GetCollection<BsonDocument>("Aplicabilidades_Teste");
            var resultList = bsonCollection.FindAsync(_ => true).Result.ToListAsync().Result;
            return bsonCollection;
        }
        #endregion

    }
}