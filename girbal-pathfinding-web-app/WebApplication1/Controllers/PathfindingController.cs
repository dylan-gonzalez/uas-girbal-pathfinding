using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    using WebApplication1.Helpers;

    [Route("/[controller]")]
    [ApiController]
    public class PathfindingController : ControllerBase
    {
        [HttpPost]
        public ResponseBody<AlgorithmSolution> Find([FromBody] PathfindingRequestBody body)
        {
            Console.WriteLine("API");
 
            var settings = body.ToSettings();

            if (!PathfindingSettings.CheckIfValid(settings))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new ResponseBody<AlgorithmSolution>() { Errors = new[] { ResponseError.InvalidFromAndGoalSettings } };
            }
            try
            {
                var result = AlgorithmCore.Find(settings, body.map);

                Response.StatusCode = 200;

                return new ResponseBody<AlgorithmSolution>() { Data = result };

            }
            catch (InvalidOperationException)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new ResponseBody<AlgorithmSolution>() { };
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return new ResponseBody<AlgorithmSolution>() { };
            }
        }
    }
}
