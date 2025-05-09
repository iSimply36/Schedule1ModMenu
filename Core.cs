using Il2CppScheduleOne.Combat;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.Networking;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.PlayerScripts.Health;
using Il2CppScheduleOne.Stealth;
using Il2CppScheduleOne.UI;
using MelonLoader;
using S1API.Money;
using System.Collections;
using UnityEngine;

[assembly: MelonInfo(typeof(Schedule1ModMenu.Core), "Schedule1ModMenu", "1.0.0", "iSimply", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace Schedule1ModMenu
{
    public class Core : MelonMod
    {
        public static Core Instance { get; private set; }

        private const string LOBBY_OBJECT_NAME = "@Lobby";
        private const string MONEY_OBJECT_NAME = "@Money";
        private const string CAMERA_CONTAINER_NAME = "CameraContainer";
        private const string MAIN_CAMERA_TAG = "MainCamera";

        public const KeyCode moneyButton = KeyCode.Keypad1;
        public string steamID;

        private bool moneySpamDelay = false;


        delegate void ButtonCallback();
        public override void OnInitializeMelon()
        {
            Instance = this;
            MelonCoroutines.Start(WaitForLobbyObject());

        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {

            if (buildIndex == 1)
            {
                LoggerInstance.Msg("Main Scene Was Loaded, intializing resources");
                LoggerInstance.Msg("Waiting for Player To Load...");
                MelonCoroutines.Start(DelayGetResources(3));

            }

        }

        public override void OnLateUpdate()
        {
            if (PlayerContext.instance.isInvincible)
            {
                if (PlayerContext.instance.health.CurrentHealth < 100)
                {
                    PlayerContext.instance.health.SetHealth(100);
                }
            }
            if (PlayerContext.instance.moneySpam && !moneySpamDelay)
            {
                Money.ChangeCashBalance(1000, true, true);
                moneySpamDelay = true;
                MelonCoroutines.Start(DelayMoneySpam());
            }
        }

        private IEnumerator WaitForLobbyObject()
        {
            GameObject lobbyObj = null;

            // Wait until the object is found
            while (lobbyObj == null)
            {
                lobbyObj = GameObject.Find(LOBBY_OBJECT_NAME);
                yield return new WaitForSeconds(1); // wait one frame
            }

            LoggerInstance.Msg("Initializing...");
            Lobby lobby = lobbyObj.GetComponent<Lobby>();
            if (lobby != null)
            {
                steamID = lobby.LocalPlayerID.ToString();
                LoggerInstance.Msg("steam id: " + steamID);
            }
            else
            {
                LoggerInstance.Error("Lobby component not found on object");
            }

        }

        public void SetPlayerInput(bool isMenuOpen)
        {
            bool state = !isMenuOpen && !PlayerContext.instance.gameplayMenu.IsOpen;

            LoggerInstance.Msg("setting player input to: " + state);

            if (PlayerContext.instance.cameraScript != null)
                if (PlayerContext.instance.cameraScript.canLook != state)
                    PlayerContext.instance.cameraScript.SetCanLook(state);
            if (PlayerContext.instance.punchScript != null)
                if (PlayerContext.instance.punchScript.enabled != state)
                    PlayerContext.instance.punchScript.enabled = state;

            UnityEngine.Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
            UnityEngine.Cursor.visible = !state;

        }


        private IEnumerator DelayGetResources(float time)
        {
            //LoggerInstance.Msg("waiting for " + time + " seconds");
            yield return new WaitForSeconds(time);
            GetResources();
        }

        public void GetResources()
        {

            GameObject[] tempcameraObjs = GameObject.FindGameObjectsWithTag(MAIN_CAMERA_TAG);
            //LoggerInstance.Msg("cameraObjs found: " + tempcameraObjs.Length);

            if (tempcameraObjs.Length > 0)
            {
                foreach (var camera in tempcameraObjs)
                {
                    if (camera.name == CAMERA_CONTAINER_NAME)
                    {
                        var potentialPlayer = camera.transform.parent.parent.gameObject;
                        //LoggerInstance.Msg("Checking: " + potentialPlayer.name);

                        if (potentialPlayer.name.Trim().Contains(steamID.Trim()))
                        {
                            LoggerInstance.Msg("Found local player");
                            PlayerContext.instance.gameObj = potentialPlayer;
                            PlayerContext.instance.health = potentialPlayer.GetComponent<PlayerHealth>();
                            PlayerContext.instance.cameraScript = camera.GetComponent<PlayerCamera>();
                            PlayerContext.instance.punchScript = camera.GetComponentInChildren<PunchController>();
                            PlayerContext.instance.gameplayMenu = camera.GetComponentInChildren<GameplayMenu>();

                            break;
                        }
                        else
                        {
                            LoggerInstance.Msg(potentialPlayer.name + " does not contain " + steamID);
                        }
                    }
                }
            }   
            PlayerContext.instance.movementScript = PlayerContext.instance.gameObj.GetComponentInChildren<PlayerMovement>();
            PlayerContext.instance.visibilityScript = PlayerContext.instance.gameObj.GetComponentInChildren<PlayerVisibility>();
            CheckInitialized();
        }
        public void CheckInitialized()
        {
            LoggerInstance.Msg("Checking resources...");

            var requiredComponents = new Dictionary<string, object>
            {
                { "PlayerCamera", PlayerContext.instance.cameraScript },
                { "Player Object", PlayerContext.instance.gameObj },
                { "PunchController", PlayerContext.instance.punchScript },
                { "PlayerMovementScript", PlayerContext.instance.movementScript },
                { "PlayerVisibilityScript", PlayerContext.instance.visibilityScript }
            };

            bool errorDetected = false;

            foreach (var comp in requiredComponents)
            {
                if (comp.Value == null)
                {
                    LoggerInstance.Error($"{comp.Key} is null");
                    errorDetected = true;
                }
            }

            if (!errorDetected)
            {
                ModState.CurrentStatus = ModState.ModStatus.Ready;
                LoggerInstance.Msg("Resources successfully initialized!");
            }
            else
            {
                LoggerInstance.Error("Mod failed to initialize: One or more resources are missing.");
            }
        }


        private IEnumerator DelayMoneySpam()
        {
            yield return new WaitForSeconds(0.15f);
            moneySpamDelay = false;
        }

    }
}