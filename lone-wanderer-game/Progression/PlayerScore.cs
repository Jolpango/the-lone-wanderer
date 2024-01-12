using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoneWandererGame.Progression
{
    public static class PlayerScore
    {
        public delegate void OnAction();
        public delegate void OnXpAction(int x);
        public static int Score { get; set; } = 0;
        public static int XP
        {
            get
            {
                return xp;
            }
            set
            {
                xp = value;
                if (xp >= RequiredXP)
                {
                    xp = 0;
                    RequiredXP *= 2;
                    Level++;
                }
            }
        }
        public static int RequiredXP { get; set; } = 5;
        private static int xp = 0;
        public static int Level { get; set; } = 1;
        public static OnAction OnLevelUp { get; set; }
        public static OnXpAction OnGainXp { get; set; }

        public static void GainXP(int newXP)
        {
            xp += newXP;
            OnGainXp(newXP);
            if (xp >= RequiredXP)
            {
                xp = 0;
                RequiredXP = RequiredXP + (int)(RequiredXP * 0.666f) + 4;
                Level++;
                OnLevelUp();
            }
        }
    }
}
