using System;

namespace TechRadarApi.V1.Boundary.Request
{
    public class CreateTechnologyRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Technique { get; set; }
    }
}
