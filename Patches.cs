using HarmonyLib;
using Il2CppScheduleOne.PlayerScripts;

namespace Schedule1ModMenu
{
    [HarmonyPatch(typeof(PlayerMovement), "Update")]

    internal class Patch_PlayerMovement
    {
        static bool Prefix()
        {
            //MelonLogger.Msg("Prefix called!");
            if (ModState.CurrentStatus == ModState.ModStatus.MenuOpen)
            {
                // Eat the input and skip the original method
                return false;
            }
            return true;
        }

    }
}