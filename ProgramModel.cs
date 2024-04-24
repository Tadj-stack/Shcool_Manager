using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBDemoApp2
{
    public class ProgramModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
       public string NumProg { get; set; }
        public string Nomprog { get; set; }
        public string Duree { get; set; }




    }
}
