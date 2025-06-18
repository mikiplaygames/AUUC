using MikiHeadDev.Core.Singletons;
#if !DISABLESTEAMWORKS
using Steamworks;
#endif
namespace MikiHeadDev.Helpers.Steamworks
{
    public static class SteamDataHelper
    {
        public static void IncrementStat(string statName, int increment = 1)
        {
#if !DISABLESTEAMWORKS
            if (!SteamManager.Initialized) return;
            SteamUserStats.GetStat(statName, out int statValue);
            SteamUserStats.SetStat(statName, statValue + increment);
            SteamUserStats.StoreStats();
#endif
        }
        public static bool SetStat(string statName, int value)
        {
#if !DISABLESTEAMWORKS
            if (!SteamManager.Initialized) return false;
            var answer = SteamUserStats.SetStat(statName, value);
            SteamUserStats.StoreStats();
            return answer;
#else
        return false;
#endif
        }
        public static bool GetStat(string statName, out int value)
        {
            value = 0;
#if !DISABLESTEAMWORKS
            if (!SteamManager.Initialized) return false;
            return SteamUserStats.GetStat(statName, out value);
#else
        return false;
#endif
        }
        public static void UnlockAchievement(string achievementName)
        {
#if !DISABLESTEAMWORKS
            if (!SteamManager.Initialized) return;
            SteamUserStats.SetAchievement(achievementName);
            SteamUserStats.StoreStats();
#endif
        }
    }
}