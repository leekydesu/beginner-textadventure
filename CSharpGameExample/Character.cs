using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpGameExample
{
    static class Character
    {
        public static string playerName = "Default Dan";
        public static int luck = 5;
        public static int experience = 0;
        private static List<string> bodyParts = new List<string> { "Right Hand", "Left Hand", "Right Foot", "Left Foot" };
        public static List<string> inventory = new List<string>();

        public static bool DamageBody(string bodyPart)
        {
            bool damageResult = false;
            if (bodyParts.Contains(bodyPart))
            {
                bodyParts.Remove(bodyPart);
                damageResult = true;
            }
            return damageResult;
        }
    }
}
