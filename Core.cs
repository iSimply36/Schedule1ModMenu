using Il2CppScheduleOne.Combat;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.Networking;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.PlayerScripts.Health;
using Il2CppScheduleOne.Stealth;
using Il2CppScheduleOne.UI;
using MelonLoader;
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

        private PlayerContext playerContext = new PlayerContext();
        private MoneyManager moneyScript;
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
                GameObject tempmoneyObj = GameObject.Find(MONEY_OBJECT_NAME);
                if (tempmoneyObj != null)
                {
                    moneyScript = tempmoneyObj.GetComponent<MoneyManager>();
                }
                else
                {
                    LoggerInstance.Error("Failed to Get MoneyObject");
                }

                LoggerInstance.Msg("Waiting for Player To Load...");
                MelonCoroutines.Start(DelayGetResources(3));

            }

        }

        public override void OnLateUpdate()
        {
            if (playerContext.isInvincible)
            {
                if (playerContext.health.CurrentHealth < 100)
                {
                    playerContext.health.SetHealth(100);
                }
            }
            if (playerContext.moneySpam && !moneySpamDelay)
            {
                moneyScript.ChangeCashBalance(1000, true, true);
                moneySpamDelay = true;
                MelonCoroutines.Start(DelayMoneySpam());
            }

            if (Input.GetKeyDown(moneyButton))
            {
                UIModMenu.Instance.ToggleMenu();
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
            bool state = !isMenuOpen && !playerContext.gameplayMenu.IsOpen;

            LoggerInstance.Msg("setting player input to: " + state);

            if (playerContext.cameraScript != null)
                if (playerContext.cameraScript.canLook != state)
                    playerContext.cameraScript.SetCanLook(state);
            if (playerContext.punchScript != null)
                if (playerContext.punchScript.enabled != state)
                    playerContext.punchScript.enabled = state;

            UnityEngine.Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
            UnityEngine.Cursor.visible = !state;

        }

        //public override void OnGUI()
        //{
        //    if (ModState.IsReady && !ModState.MenuInitialized)
        //    {
        //        try
        //        {
        //            modMenu.Initialize(playerContext, moneyScript);
        //        }
        //        catch (Exception e)
        //        {
        //            MelonLogger.Error("Error in OnGUI: " + e.Message + "\n" + e.StackTrace);
        //            ModState.CurrentStatus = ModState.ModStatus.Error;
        //        }
        //    }

        //    if (ModState.CurrentStatus == ModState.ModStatus.MenuOpen)
        //    {
        //        modMenu.Draw();
        //    }
        //}


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
                            playerContext.gameObj = potentialPlayer;
                            playerContext.health = potentialPlayer.GetComponent<PlayerHealth>();
                            playerContext.cameraScript = camera.GetComponent<PlayerCamera>();
                            playerContext.punchScript = camera.GetComponentInChildren<PunchController>();
                            playerContext.gameplayMenu = camera.GetComponentInChildren<GameplayMenu>();

                            break;
                        }
                        else
                        {
                            LoggerInstance.Msg(potentialPlayer.name + " does not contain " + steamID);
                        }
                    }
                }
            }
            playerContext.movementScript = playerContext.gameObj.GetComponentInChildren<PlayerMovement>();
            playerContext.visibilityScript = playerContext.gameObj.GetComponentInChildren<PlayerVisibility>();
            CheckInitialized();
        }
        public void CheckInitialized()
        {
            LoggerInstance.Msg("Checking resources...");

            var requiredComponents = new Dictionary<string, object>
            {
                { "MoneyManager", moneyScript },
                { "PlayerCamera", playerContext.cameraScript },
                { "Player Object", playerContext.gameObj },
                { "PunchController", playerContext.punchScript },
                { "PlayerMovementScript", playerContext.movementScript },
                { "PlayerVisibilityScript", playerContext.visibilityScript }
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
                UIModMenu.Instance.LoadMenu(playerContext);
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