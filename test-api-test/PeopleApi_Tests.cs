using System;
using System.Threading.Tasks;
using Xunit;
using TestApi.Function;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Http;

namespace TestApi.UnitTests
{
    public class PeoplApi_Tests
    {
        // Centre of London - Charring Cross
        const double LONDON_LAT = 51.50853;
        const double LONDON_LONG = -0.12574;
        const int DISTANCE_FROM_CENTER = 50;
        const string ALL_USERS_ENDPOINT = "/users";       
        const string CTY_USERS_ENDPOINT = "/city/London/users";       
        private readonly Locale _london;

        public PeoplApi_Tests()
        {
            _london = new Locale(LONDON_LAT, LONDON_LONG, DISTANCE_FROM_CENTER);
        }

        [Fact]
        public void Test_LatLimits()
        {
            double lat = 90;
            double lng = 180;
            ushort rad = 50;

            Action act = () => {
                var t = new Locale(lat, lng, rad);
                };
            
            act();
            lat=91;
            var exception = Assert.Throws<ArgumentOutOfRangeException>(act);
            lat=-91; ;
            exception = Assert.Throws<ArgumentOutOfRangeException>(act);
        }

        [Fact]
        public void Test_LngLimits()
        {
            double lat = 90;
            double lng = 180;
            ushort rad = 50;

            Action act = () => {
                var t = new Locale(lat, lng, rad);
                };
            
            act();
            lng=181;
            var exception = Assert.Throws<ArgumentOutOfRangeException>(act);
            lng=-181;
            exception = Assert.Throws<ArgumentOutOfRangeException>(act);
        }

        [Fact]
        public void Test_RadiusLimits()
        {
            double lat = 90;
            double lng = 180;
            ushort rad = 50;

            Action act = () => {
                var t = new Locale(lat, lng, rad);
                };
            
            act();
            rad=0;
            var exception = Assert.Throws<ArgumentOutOfRangeException>(act);
            rad=12451;
            exception = Assert.Throws<ArgumentOutOfRangeException>(act);
        }

        [Fact]
        public void Test_InsideLocale()
        {
            // 49.76 miles from Charring X
            Assert.True(_london.IsLocationInLocale(51.398139, -1.257332) );
        }
        [Fact]
        public void Test_OutsideLocale()
        {
            // 51.71 miles from Charring X
            Assert.False(_london.IsLocationInLocale( 51.394712, -1.315010) );
        }

        [Fact]
        public async void Test_InnerApiCityAndAllReturnUsers()
        {
            var mockLog = new Mock<ILogger>();

            string cdata = "[{\"id\": 266,\"first_name\": \"Ancell\",\"last_name\": \"Garnsworthy\",\"email\": \"agarnsworthy7d@seattletimes.com\",\"ip_address\": \"67.4.69.137\",\"latitude\": 51.6553959,\"longitude\": 0.057255}]";
            string adata = "[{\"id\": 366,\"first_name\": \"Ancell\",\"last_name\": \"Garnsworthy\",\"email\": \"agarnsworthy7d@seattletimes.com\",\"ip_address\": \"67.4.69.137\",\"latitude\": 51.6553959,\"longitude\": 0.057255}]";
            var mock = new Mock<IRestApi>();
            mock.Setup(m => m.GetEndPointAsync(It.Is<string>(s => s.Equals(ALL_USERS_ENDPOINT)))).Returns(Task.FromResult(adata));
            mock.Setup(m => m.GetEndPointAsync(It.Is<string>(s => s.Equals(CTY_USERS_ENDPOINT)))).Returns(Task.FromResult(cdata));

            var api = new PeopleApi();

            var res =  await api.GetLondonPeople(mock.Object, mockLog.Object);

            var okObjectResult = res as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var json = okObjectResult.Value as string;
            JArray a = JArray.Parse(json);

            Assert.True(a.Count==2);
        }

        [Fact]
        public async void Test_InnerApiCityAndAllReturnDuplicateUsers()
        {
            var mockLog = new Mock<ILogger>();

            string cdata = "[{\"id\": 266,\"first_name\": \"Ancell\",\"last_name\": \"Garnsworthy\",\"email\": \"agarnsworthy7d@seattletimes.com\",\"ip_address\": \"67.4.69.137\",\"latitude\": 51.6553959,\"longitude\": 0.057255}]";
            string adata = "[{\"id\": 266,\"first_name\": \"Ancell\",\"last_name\": \"Garnsworthy\",\"email\": \"agarnsworthy7d@seattletimes.com\",\"ip_address\": \"67.4.69.137\",\"latitude\": 51.6553959,\"longitude\": 0.057255}]";
            var mock = new Mock<IRestApi>();
            mock.Setup(m => m.GetEndPointAsync(It.Is<string>(s => s.Equals(ALL_USERS_ENDPOINT)))).Returns(Task.FromResult(adata));
            mock.Setup(m => m.GetEndPointAsync(It.Is<string>(s => s.Equals(CTY_USERS_ENDPOINT)))).Returns(Task.FromResult(cdata));

            var api = new PeopleApi();

            var res =  await api.GetLondonPeople(mock.Object, mockLog.Object);

            var okObjectResult = res as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var json = okObjectResult.Value as string;
            JArray a = JArray.Parse(json);

            Assert.True(a.Count==1);
        }

        [Fact]
        public async void Test_InnerApiCityReturnsUsers()
        {
            var mockLog = new Mock<ILogger>();

            string cdata = "[{\"id\": 266,\"first_name\": \"Ancell\",\"last_name\": \"Garnsworthy\",\"email\": \"agarnsworthy7d@seattletimes.com\",\"ip_address\": \"67.4.69.137\",\"latitude\": 51.6553959,\"longitude\": 0.057255}]";
            string adata = "[]";
            var mock = new Mock<IRestApi>();
            mock.Setup(m => m.GetEndPointAsync(It.Is<string>(s => s.Equals(ALL_USERS_ENDPOINT)))).Returns(Task.FromResult(adata));
            mock.Setup(m => m.GetEndPointAsync(It.Is<string>(s => s.Equals(CTY_USERS_ENDPOINT)))).Returns(Task.FromResult(cdata));

            var api = new PeopleApi();

            var res =  await api.GetLondonPeople(mock.Object, mockLog.Object);

            var okObjectResult = res as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var json = okObjectResult.Value as string;
            JArray a = JArray.Parse(json);

            Assert.True(a.Count==1);
        }

        [Fact]
        public async void Test_InnerApiAllReturnsUsers()
        {
            var mockLog = new Mock<ILogger>();

            string cdata = "[]";
            string adata = "[{\"id\": 266,\"first_name\": \"Ancell\",\"last_name\": \"Garnsworthy\",\"email\": \"agarnsworthy7d@seattletimes.com\",\"ip_address\": \"67.4.69.137\",\"latitude\": 51.6553959,\"longitude\": 0.057255}]";

            var mock = new Mock<IRestApi>();
            mock.Setup(m => m.GetEndPointAsync(It.Is<string>(s => s.Equals(ALL_USERS_ENDPOINT)))).Returns(Task.FromResult(adata));
            mock.Setup(m => m.GetEndPointAsync(It.Is<string>(s => s.Equals(CTY_USERS_ENDPOINT)))).Returns(Task.FromResult(cdata));

            var api = new PeopleApi();

            var res =  await api.GetLondonPeople(mock.Object, mockLog.Object);

            var okObjectResult = res as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var json = okObjectResult.Value as string;
            JArray a = JArray.Parse(json);

            Assert.True(a.Count==1);
        }

        [Fact]
        public async void Test_InnerApiNoUsers()
        {
            var mockLog = new Mock<ILogger>();

            string cdata = "[]";
            string adata = "[]";
 
            var mock = new Mock<IRestApi>();
            mock.Setup(m => m.GetEndPointAsync(It.Is<string>(s => s.Equals(ALL_USERS_ENDPOINT)))).Returns(Task.FromResult(adata));
            mock.Setup(m => m.GetEndPointAsync(It.Is<string>(s => s.Equals(CTY_USERS_ENDPOINT)))).Returns(Task.FromResult(cdata));

            var api = new PeopleApi();

            var res =  await api.GetLondonPeople(mock.Object, mockLog.Object);

            var okObjectResult = res as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var json = okObjectResult.Value as string;
            JArray a = JArray.Parse(json);

            Assert.True(a.Count==0);
        }
        [Fact]
        public async void Test_InnerApiWrongFormat()
        {
            var mockLog = new Mock<ILogger>();

            string cdata = "[]";
            string adata = "{\"id\": 266,\"first_name\": \"Ancell\",\"last_name\": \"Garnsworthy\",\"email\": \"agarnsworthy7d@seattletimes.com\",\"ip_address\": \"67.4.69.137\",\"latitude\": 51.6553959,\"longitude\": 0.057255}";
 
            var mock = new Mock<IRestApi>();
            mock.Setup(m => m.GetEndPointAsync(It.Is<string>(s => s.Equals(ALL_USERS_ENDPOINT)))).Returns(Task.FromResult(adata));
            mock.Setup(m => m.GetEndPointAsync(It.Is<string>(s => s.Equals(CTY_USERS_ENDPOINT)))).Returns(Task.FromResult(cdata));

            var api = new PeopleApi();

            var res =  await api.GetLondonPeople(mock.Object, mockLog.Object);

            var exceptionResult = res as ExceptionResult;
            Assert.NotNull(exceptionResult);

        }
    }
}
