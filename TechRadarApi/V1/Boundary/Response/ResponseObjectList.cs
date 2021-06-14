using System.Collections.Generic;

namespace TechRadarApi.V1.Boundary.Response
{
    //TODO: Rename to represent to object you will be returning eg. ResidentInformationList
    public class ResponseObjectList
    {
        //TODO: Rename field to match the name of the response object
        //TODO: add xml comments containing information that will be included in the auto generated swagger docs (https://github.com/LBHackney-IT/lbh-tech-radar-api/wiki/Controllers-and-Response-Objects)
        public List<ResponseObject> ResponseObjects { get; set; }
    }
}
