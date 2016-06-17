using DRNumberCrunchers;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace DRLPTest
{
    static class RacenetApiParser
    {
        /// <summary>
        /// Creates the URIs and parses responses into Rally objects
        /// </summary>
        public static Rally GetRallyData(string eventId, IProgress<int> progress)
        {
            try
            {
                // example string for reference
                //string apiUrl = "https://www.dirtgame.com/uk/api/event?assists=any&eventId=95576&group=all&leaderboard=true&nameSearch=&noCache=1463699433315&number=10&page=1&stageId=0&wheel=any";

                string apiUrl = "https://www.dirtgame.com/uk/api/event?assists=any&group=all&leaderboard=true&nameSearch=&number=10&wheel=any";

                var uriBuilder = new UriBuilder(apiUrl);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["eventId"] = eventId;
                query["noCache"] = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds.ToString();

                // get the overall results and stage count
                query["page"] = "1";
                query["stageId"] = "0";                 // overall results
                uriBuilder.Query = query.ToString();
                var requestUri = uriBuilder.ToString();

                var result = ExecuteRequest(requestUri).Result;

                var totalStages = result.TotalStages;
                var rallyData = new Rally();

                // for each stage
                for (int i = 1; i <= totalStages; i++)
                {
                    var currentPage = 1;
                    var totalPages = 99;
                    var leaderboardTotal = 99;
                    var stageData = new Stage();

                    // for each page in stage
                    var retryCount = 0;
                    while (currentPage <= totalPages)
                    {
                        if (retryCount >= 10)
                            throw new Exception("Racenet returned no entry data");

                        query["page"] = currentPage.ToString();
                        query["stageId"] = i.ToString();        // stage number
                        uriBuilder.Query = query.ToString();
                        requestUri = uriBuilder.ToString();

                        result = ExecuteRequest(requestUri).Result;

                        // sometimes we don't get entries, if so, retry the request
                        if (result == null || result.Entries == null || result.Entries.Count < 1)
                        {
                            retryCount++;
                            continue;
                        }

                        leaderboardTotal = result.LeaderboardTotal;
                        totalPages = result.Pages;

                        if (totalPages == 0 || leaderboardTotal == 0)
                            break;

                        // for each driver in page
                        foreach (var entry in result.Entries)
                            stageData.AddDriver(new DriverTime(entry.Position, entry.Name, entry.VehicleName, entry.Time, entry.DiffFirst));

                        // increment page
                        currentPage++;
                    }

                    if (stageData.Count != leaderboardTotal)
                            throw new Exception("Racenet data incomplete for stage " + i);

                    rallyData.AddStage(stageData);
                    progress.Report(rallyData.StageCount);
                }

                return rallyData;
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        /// <summary>
        /// Performs the web requests
        /// </summary>
        private static async Task<RacenetRallyData> ExecuteRequest(string requestUri)
        {
            using (var client = new HttpClient())
            {
                var serializer = new DataContractJsonSerializer(typeof(RacenetRallyData));

                var response = client.GetAsync(requestUri).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseStream = await response.Content.ReadAsStreamAsync();
                    var racenetStageData = serializer.ReadObject(responseStream) as RacenetRallyData;
                    return racenetStageData;
                }
            }

            return null;
        }



        // JSON classes for deserialization
        [DataContract]
        public class Restriction
        {
            [DataMember]
            public List<string> VehicleClass { get; set; }
        }

        [DataContract]
        public class Entry
        {
            [DataMember]
            public int Position { get; set; }
            [DataMember]
            public string NationalityImage { get; set; }
            [DataMember]
            public bool IsFounder { get; set; }
            [DataMember]
            public bool IsVIP { get; set; }
            [DataMember]
            public bool HasGhost { get; set; }
            [DataMember]
            public int PlayerId { get; set; }
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public string VehicleName { get; set; }
            [DataMember]
            public string Time { get; set; }
            [DataMember]
            public string DiffFirst { get; set; }
            [DataMember]
            public int PlayerDiff { get; set; }
            [DataMember]
            public int TierID { get; set; }
            [DataMember]
            public string ProfileUrl { get; set; }
        }

        [DataContract]
        public class RacenetRallyData
        {
            [DataMember]
            public string EventName { get; set; }
            [DataMember]
            public int TotalStages { get; set; }
            [DataMember]
            public bool ShowStageInfo { get; set; }
            [DataMember]
            public object LocationName { get; set; }
            [DataMember]
            public object LocationImage { get; set; }
            [DataMember]
            public object StageName { get; set; }
            [DataMember]
            public object StageImage { get; set; }
            [DataMember]
            public object TimeOfDay { get; set; }
            [DataMember]
            public object WeatherImageUrl { get; set; }
            [DataMember]
            public object WeatherImageAltUrl { get; set; }
            [DataMember]
            public object WeatherText { get; set; }
            [DataMember]
            public Restriction Restriction { get; set; }
            [DataMember]
            public bool EventRestart { get; set; }
            [DataMember]
            public bool StageRetry { get; set; }
            [DataMember]
            public bool HasServiceArea { get; set; }
            [DataMember]
            public bool AllowCareerEngineers { get; set; }
            [DataMember]
            public bool OnlyOwnedVehicles { get; set; }
            [DataMember]
            public bool AllowVehicleTuning { get; set; }
            [DataMember]
            public bool IsCheckpoint { get; set; }
            [DataMember]
            public int Page { get; set; }
            [DataMember]
            public int Pages { get; set; }
            [DataMember]
            public int LeaderboardTotal { get; set; }
            [DataMember]
            public object PlayerEntry { get; set; }
            [DataMember]
            public List<Entry> Entries { get; set; }
            [DataMember]
            public bool FiltersEnabled { get; set; }
            [DataMember]
            public bool IsWagerEvent { get; set; }
        }
    }
}
