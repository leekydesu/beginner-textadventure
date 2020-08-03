using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpGameExample
{
    static class Character
    {
        public static string playerName = "Default Dan";
        public static int luck = 5;
        public static int experience = 0;
        public static List<string> BodyParts
        {
            get; set;
        } = new List<string> { "Right Hand", "Left Hand", "Right Foot", "Left Foot" };

        public static bool DamageBody(string bodyPart)
        {
            bool damageResult = false;
            if (BodyParts.Contains(bodyPart))
            {
                BodyParts.Remove(bodyPart);
                damageResult = true;
            }
            return damageResult;
        }

        public static List<Item> Inventory
        {
            get; set;
        } = new List<Item>();

        public static string Flags
        {
            get; set;
        } = "";

        public static void SetFlags(string[] newFlags)
        {
            foreach (string newFlag in newFlags)
            {
                Flags = $"{newFlag};{Flags}";
            }
        }
        public static void SetFlags(string newFlag)
        {
            Flags = $"{newFlag};{Flags}";
        }
    }
}
