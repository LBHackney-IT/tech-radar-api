using System;

namespace TechRadarApi.V1.Boundary.Response
{
    public class TechnologyResponseObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Technique { get; set; }

    }
}
