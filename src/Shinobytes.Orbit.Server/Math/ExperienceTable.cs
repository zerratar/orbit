using System;
using System.Collections.Generic;
using System.Text;

namespace Shinobytes.Orbit.Server
{
    public static class ExperienceTable
    {

        public static int[] ExpMultiplier = { 20, 25, 30, 40, 50, 70, 90, 95, 120, 150, 180, 200 };

        public static int MaxLevel = 65;

        public static long GetExperienceForLevel(int level)
        {
            var lvpow = 2415;
            var minlevel = 1;
            var points = Math.Floor(level + lvpow * Math.Pow(2, level / 7.0));
            if (level >= minlevel)
            {
                var output = Math.Floor(points / 4);
                return (long)output;
            }

            return 0;
        }

        public static int GetLevelFromExperience(long exp)
        {
            var retLevel = 1;
            for (int lvl = 1; lvl < (MaxLevel + 1); lvl++)
            {
                var expForLevel = GetExperienceForLevel(lvl);
                if (exp > expForLevel)
                    retLevel = lvl;
            }

            return retLevel;
        }


        public static int CalcExperienceGain(int nodeLevel, int userLevel)
        {
            return (int)(ExpMultiplier[nodeLevel - 1] + (userLevel * 10));
        }

        public static int CalcCurrencyGain(int nodeLevel, int userLevel)
        {
            var exp = CalcExperienceGain(nodeLevel, userLevel);
            return (int)(exp * 8.75);
        }
    }
}
