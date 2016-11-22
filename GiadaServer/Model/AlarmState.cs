using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace GiadaServer.Model
{
    public class AlarmState
    {
        public ObjectId Id { get; set; }
        public bool Armed { get; set; }
        public DateTime UpdateTime { get; set; }
        public List<Pir> Pirs { get; set; }
    }

    public class Pir
    {
        public string Location { get; set; }
        public bool Alarm { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    public class Camera
    {
        public string Name { get; set; }
        public string ConnectionUri { get; set; }
    }
}
