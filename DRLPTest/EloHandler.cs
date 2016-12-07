using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using DRNumberCrunchers;

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

            if (this.HasData == false)
            {
                CurrentEloPlayerData = new EloPlayerCollection();
                CurrentEloPlayerData.EloPlayers = new Dictionary<string, EloPlayerData>();
            }
        }

        public bool HasData
        {
            get
            {
                return CurrentEloPlayerData != null && CurrentEloPlayerData.EloPlayers != null && CurrentEloPlayerData.EloPlayers.Count > 0;
            }
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
            if (this.HasData == false)
                return;

            var serializer = new DataContractJsonSerializer(typeof(EloPlayerCollection));

            using (var stream = new StreamWriter(jsonDataStore, false))
                serializer.WriteObject(stream.BaseStream, CurrentEloPlayerData);
        }

        public void NewMatch(Dictionary<int, string> matchData)
        {
            var eloMatch = new ELOMatch();

            // for each player in the dictionary, see if there is an entry already
            // if so, get the current ranking, if not, use 1500
            foreach(var kvp in matchData)
            {
                var place = kvp.Key;
                var playerName = kvp.Value;

                if (CurrentEloPlayerData.EloPlayers.Keys.Contains(playerName))
                    eloMatch.addPlayer(playerName, place, CurrentEloPlayerData.EloPlayers[playerName].CurrentRating);
                else
                    eloMatch.addPlayer(playerName, place, 1500);
            }

            // run the matches
            eloMatch.calculateELOs();

            // update the data cache
            foreach (var kvp in matchData)
            {
                var playerName = kvp.Value;
                var newRanking = eloMatch.getELO(playerName);
                var eloChange = eloMatch.getELOChange(playerName);

                if (CurrentEloPlayerData.EloPlayers.Keys.Contains(playerName))
                {
                    var oldPlayerData = CurrentEloPlayerData.EloPlayers[playerName];
                    var newPlayerData = new EloPlayerData();

                    newPlayerData.PlayerName = oldPlayerData.PlayerName;
                    newPlayerData.CurrentRating = newRanking;
                    newPlayerData.LastEloChange = eloChange;
                    newPlayerData.NumMatches = oldPlayerData.NumMatches + 1;
                    newPlayerData.NumConsecutiveMissed = 0;
                    newPlayerData.ChangeFromLastMatch = newRanking - oldPlayerData.CurrentRating;

                    CurrentEloPlayerData.EloPlayers[playerName] = newPlayerData;
                }
                else
                {
                    var newPlayerData = new EloPlayerData();
                    newPlayerData.PlayerName = playerName;
                    newPlayerData.CurrentRating = newRanking;
                    newPlayerData.LastEloChange = eloChange;
                    newPlayerData.NumMatches = 1;
                    newPlayerData.NumConsecutiveMissed = 0;
                    newPlayerData.ChangeFromLastMatch = newRanking - 1500;

                    CurrentEloPlayerData.EloPlayers.Add(playerName, newPlayerData);
                }
            }

            // Now increment the rallies missed counter for players not in the match
            foreach (var kvp in CurrentEloPlayerData.EloPlayers)
            {
                if (!matchData.Values.Contains(kvp.Key))
                {
                    kvp.Value.NumConsecutiveMissed++;
                    kvp.Value.ChangeFromLastMatch = 0;
                }
            }
        }
    }

    [DataContract]
    class EloPlayerCollection
    {
        [DataMember]
        public Dictionary<string, EloPlayerData> EloPlayers { get; set; }
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
        
        [DataMember]
        public int ChangeFromLastMatch { get; set; }

        [DataMember]
        public int LastEloChange { get; set; }

        [DataMember]
        public int NumConsecutiveMissed { get; set; }
    }
}
