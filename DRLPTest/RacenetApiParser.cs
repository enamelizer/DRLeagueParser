using DRTimeCruncher;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace DRLPTest
{
    static class RacenetApiParser
    {
        public static Rally GetRallyData(string eventId)
        {
            using (var client = new HttpClient())
            {
                // the untouched string for reference
                //string apiUrl = "https://www.dirtgame.com/uk/api/event?assists=any&eventId=95576&group=all&leaderboard=true&nameSearch=&noCache=1463699433315&number=10&page=1&stageId=0&wheel=any";

                string apiUrl = "https://www.dirtgame.com/uk/api/event?assists=any&group=all&leaderboard=true&nameSearch=&number=10&wheel=any";

                var uriBuilder = new UriBuilder(apiUrl);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["eventId"] = eventId;
                query["noCache"] = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds.ToString();
                query["page"] = "1";
                query["stageId"] = "0";
                uriBuilder.Query = query.ToString();

                var requestUri = uriBuilder.ToString();

                var response = client.GetAsync(requestUri).Result;


                return null;
            }
        }
    }
}
