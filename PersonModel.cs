using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBDemoApp
{
    public class PersonModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string NomStage { get; set; }
        public string PrenomStage { get; set; }
        public string NumeroEtudiant { get; set; }
        public string Programme { get; set; }
        public string DateNaissance { get; set; }
        public string Sexe { get; set; }



    }
}
