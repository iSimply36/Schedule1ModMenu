
namespace Schedule1ModMenu
{
    internal static class ModState
    {
        public enum ModStatus
        {
            Uninitialized,
            Ready,
            MenuOpen,
            Error
        }

        public static ModStatus CurrentStatus { get; set; } = ModStatus.Uninitialized;

        public static bool IsMenuOpen => CurrentStatus == ModStatus.MenuOpen;
        public static bool IsReady => CurrentStatus == ModStatus.Ready;
        public static bool MenuInitialized { get; set; } = false;

        public static bool isInvincible = false;

    }

}