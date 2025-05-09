using Il2CppScheduleOne.Combat;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.PlayerScripts.Health;
using Il2CppScheduleOne.Stealth;
using Il2CppScheduleOne.UI;
using UnityEngine;

namespace Schedule1ModMenu
{
    public class PlayerContext
    {
        public GameObject gameObj;
        public PlayerHealth health;
        public PlayerCamera cameraScript;
        public PunchController punchScript;
        public PlayerMovement movementScript;
        public PlayerVisibility visibilityScript;
        public GameplayMenu gameplayMenu;

        public bool isInvincible = false;
        public bool moneySpam = false;
    }
}
