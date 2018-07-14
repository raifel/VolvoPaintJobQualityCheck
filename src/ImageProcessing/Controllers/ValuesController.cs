using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ImageProcessing.Services;
using Microsoft.Azure;
using Swashbuckle.Swagger.Annotations;

namespace ImageProcessing.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [SwaggerOperation("GetAll")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [SwaggerOperation("GetById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public string Get(int id, int id2)
        {
            BlobStorageService df = new BlobStorageService();
            var motionDetector = new MotionDetector();
            motionDetector.ProcessFrame(df.ReadImageContent(CloudConfigurationManager.GetSetting("referenceImageFile"), CloudConfigurationManager.GetSetting("referenceImageContainer")));
            var result = new ImageComparisionResult();
            result.UploadImageAfterProcess(motionDetector.ProcessFrame(df.ReadImageContent(CloudConfigurationManager.GetSetting("actualImageFile"),"actualImageContainer")));
            return "Completed the Process";
        }
        
 
    
        // POST api/values
        [SwaggerOperation("Create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [SwaggerOperation("Update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [SwaggerOperation("Delete")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void Delete(int id)
        {
        }

    }
}
