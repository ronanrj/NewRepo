using System;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mongo.Migration.Demo.Model.Pkg
{
    [RuntimeVersion("0.0.2")]
    [StartUpVersion("0.0.1")]
    [CollectionLocation("Aplicabilidades_Teste", "SAC_PlataformaComunicacao")]
    public class Aplicabilidades_Teste : IDocument
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid _Id { get; set; }
        public string Nome { get; set; }
        public string PontoControle { get; set; }
        public string[] TipoPagamentos { get; set; }
        public Tipoentrega[] TipoEntregas { get; set; }
        public bool MarketPlace { get; set; }
        public bool Online { get; set; }
        public int TempoEsperaAplicabilidade { get; set; }
        public Tipoaplicabilidade TipoAplicabilidade { get; set; }
        public bool Ativo { get; set; }
        public Tipovenda TipoVenda { get; set; }
        public DocumentVersion Version { get; set; }

    }

    public class Tipoaplicabilidade
    {
        public string Value { get; set; }
    }

    public class Tipovenda
    {
        public string Value { get; set; }
    }

    public class Tipoentrega
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid _Id { get; set; }
        public int SGPTypeDeliveryId { get; set; }
        public string Name { get; set; }
    }
}
