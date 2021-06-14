using System.Collections.Generic;
using System.Linq;
using TechRadarApi.V1.Boundary.Response;
using TechRadarApi.V1.Domain;

namespace TechRadarApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static TechnologyResponseObject ToResponse(this Technology domain)
        {
            if (domain == null) return null;
            return new TechnologyResponseObject
            {
                Id = domain.Id,
                Name = domain.Name,
                Description = domain.Description,
                Category = domain.Category,
                Technique = domain.Technique
            };
        }

        public static List<TechnologyResponseObject> ToResponse(this IEnumerable<Technology> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
    }
}
