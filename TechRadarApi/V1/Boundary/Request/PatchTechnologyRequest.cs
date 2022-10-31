using System;
using Microsoft.AspNetCore.Mvc;

namespace TechRadarApi.V1.Boundary.Request 
{
    public class PatchTechnologyRequest
    {   public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Technique{ get; set; }
    }

}