using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace TestApi.Function
{
    public static class AzureFunction
    {
        // Base Url for source API
        const string BASE_URL = "https://bpdts-test-app.herokuapp.com";
        // REST API for users
        const string ALL_USERS_ENDPOINT = "/users";       
        const string CTY_USERS_ENDPOINT = "/city/London/users";       
        // Centre of London - Charring Cross
        const double LONDON_LAT = 51.50853;
        const double LONDON_LONG = -0.12574;
        const int DISTANCE_FROM_CENTER = 50;

        [FunctionName("GetLondonPeople")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var innerApi = new RestApi(BASE_URL);
            var peopleApl = new PeopleApi();
            
            return await peopleApl.GetLondonPeople(innerApi, log);                
 
        }
    }
}

