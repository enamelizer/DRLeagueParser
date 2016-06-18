using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace DRLPTest
{
    /// <summary>
    /// Persistance for ELO data and interface for the ELO cruncher
    /// Handles the IO for ELO persistance and calls into DRNumberCruncher
    /// </summary>
    class EloHandler
    {
        private string jsonDataStore { get; set; }
        public EloPlayerCollection CurrentEloPlayerData { get; private set; }

        public EloHandler()
        {
            jsonDataStore = Path.Combine(Directory.GetCurrentDirectory(), "EloPlayerData.json");
            LoadEloData();
        }

        private void LoadEloData()
        {
            if (File.Exists(jsonDataStore))
            {
                var serializer = new DataContractJsonSerializer(typeof(EloPlayerCollection));

                using (var stream = new StreamReader(jsonDataStore))
                    CurrentEloPlayerData = serializer.ReadObject(stream.BaseStream) as EloPlayerCollection;
            }
        }

        public void SaveEloPlayerData()
        {
            var serializer = new DataContractJsonSerializer(typeof(EloPlayerCollection));

            using (var stream = new StreamWriter(jsonDataStore, false))
                serializer.WriteObject(stream.BaseStream, CurrentEloPlayerData);
        }

        public void NewMatch(Dictionary<int, string> matchData)
        {
            // for each player in the dictionary, see if there is an entry already

            // create the elo player data, either using persisted data if they already exist,
            // or new data if not

            // run the matches

            // update the data cache

            // save the data?
        }
    }

    [DataContract]
    class EloPlayerCollection
    {
        [DataMember]
        public List<EloPlayerData> EloPlayers { get; set; }
    }

    [DataContract]
    class EloPlayerData
    {
        [DataMember]
        public string PlayerName { get; set; }

        [DataMember]
        public int CurrentRating { get; set; }

        [DataMember]
        public int NumMatches { get; set; }
    }
}
