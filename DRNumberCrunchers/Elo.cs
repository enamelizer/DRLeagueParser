// Code from https://github.com/FigBug/Multiplayer-ELO

//Copyright(c) 2015, Roland Rabien / Unknown(http://elo-norsak.rhcloud.com/index.php)
//All rights reserved.

//Redistribution and use in source and binary forms, with or without
//modification, are permitted provided that the following conditions are met:

//* Redistributions of source code must retain the above copyright notice, this
// list of conditions and the following disclaimer.


//* Redistributions in binary form must reproduce the above copyright notice,
// this list of conditions and the following disclaimer in the documentation

// and/or other materials provided with the distribution.


//* Neither the name of Multiplayer-ELO nor the names of its

// contributors may be used to endorse or promote products derived from

// this software without specific prior written permission.


//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//DISCLAIMED.IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
//FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
//DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
//CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
//OR TORT(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;

namespace DRNumberCrunchers
{
    class ELOPlayer
    {
        public string name;

        public int place = 0;
        public int eloPre = 0;
        public int eloPost = 0;
        public int eloChange = 0;
    }

    class ELOMatch
    {
        private List<ELOPlayer> players = new List<ELOPlayer>();

        public void addPlayer(string name, int place, int elo)
        {
            ELOPlayer player = new ELOPlayer();

            player.name = name;
            player.place = place;
            player.eloPre = elo;

            players.Add(player);
        }

        public int getELO(string name)
        {
            foreach (ELOPlayer p in players)
            {
                if (p.name == name)
                    return p.eloPost;
            }
            return 1500;
        }

        public int getELOChange(string name)
        {
            foreach (ELOPlayer p in players)
            {
                if (p.name == name)
                    return p.eloChange;
            }
            return 0;
        }

        public void calculateELOs()
        {
            int n = players.Count;
            float K = 32 / (float)(n - 1);

            for (int i = 0; i < n; i++)
            {
                int curPlace = players[i].place;
                int curELO = players[i].eloPre;

                for (int j = 0; j < n; j++)
                {
                    if (i != j)
                    {
                        int opponentPlace = players[j].place;
                        int opponentELO = players[j].eloPre;

                        //work out S
                        float S;
                        if (curPlace < opponentPlace)
                            S = 1.0F;
                        else if (curPlace == opponentPlace)
                            S = 0.5F;
                        else
                            S = 0.0F;

                        //work out EA
                        float EA = 1 / (1.0f + (float)Math.Pow(10.0f, (opponentELO - curELO) / 400.0f));

                        //calculate ELO change vs this one opponent, add it to our change bucket
                        //I currently round at this point, this keeps rounding changes symetrical between EA and EB, but changes K more than it should
                        players[i].eloChange += (int)Math.Round(K * (S - EA));
                    }
                }
                //add accumulated change to initial ELO for final ELO   
                players[i].eloPost = players[i].eloPre + players[i].eloChange;
            }
        }
    }
}