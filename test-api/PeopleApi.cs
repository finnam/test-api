using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Http;


namespace TestApi.Function
{
    public class PeopleApi
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

        public async Task<IActionResult> GetLondonPeople(
            IRestApi innerApi,
            ILogger log)
        {
            try
            {
                //RestApi innerApi = new RestApi(BASE_URL);
                var allTask = innerApi.GetEndPointAsync(ALL_USERS_ENDPOINT);
                var ctyTask = innerApi.GetEndPointAsync(CTY_USERS_ENDPOINT); 

                JArray cityUsers = new JArray();
                JToken[] ldnUsers = new JToken[0];

                var allTasks = new List<Task>{allTask, ctyTask};
                while (allTasks.Any())
                {
                    Task finished = await Task.WhenAny(allTasks);
                    if (finished == allTask)
                    {
                        if (finished.IsCompletedSuccessfully)
                        {
                            // Filter users on location
                            var users = JArray.Parse(allTask.Result);
                            var london = new Locale(LONDON_LAT, LONDON_LONG, DISTANCE_FROM_CENTER);
                            ldnUsers = users.Where(u => london.IsLocationInLocale((double)u["latitude"], (double)u["longitude"])).ToArray();
                        }
                        else
                        {
                            return new ObjectResult(new ObjectResult(new {statusCode = System.Convert.ToInt32(allTask.Result), message = "Http Error"}));
                        }
                    }
                    else if (finished == ctyTask)
                    {
                        if (finished.IsCompletedSuccessfully)
                        {
                            cityUsers = JArray.Parse(ctyTask.Result);
                        }
                        else
                        {
                            return new ObjectResult(new ObjectResult(new {statusCode = System.Convert.ToInt32(ctyTask.Result), message = "Http Error"}));
                        }
                    }
                    allTasks.Remove(finished);
                }
                var res = ldnUsers.Concat(cityUsers);
                // Remove Duplicates
                res = res.GroupBy(j => j["id"]).Select(g => g.First());

                return new OkObjectResult(JsonConvert.SerializeObject(res));
            }
            catch(Exception ex)
            {
                return new ExceptionResult(ex, true);
            }
        }
    }
}
