using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoneWandererGame.Progression
{
    public static class PlayerScore
    {
        public delegate void OnAction();
        public delegate void OnXpAction(int x);
        public static int Score { get; set; } = 0;
        public static string Name { get; set; }
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

        public static void LoadName()
        {
            string filePath = "name"; // Replace with the actual path to your JSON file

            try
            {
                // Check if the file exists
                if (File.Exists(filePath))
                {
                    // Read the JSON data from the file
                    string jsonData = File.ReadAllText(filePath);

                    // Deserialize the JSON data into a Person object
                    Name = jsonData;
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        public static void WriteToFile()
        {
            string filePath = "name"; // Replace with the actual path to your JSON file

            try
            {
                // Write the JSON data to the file
                File.WriteAllText(filePath, Name);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
