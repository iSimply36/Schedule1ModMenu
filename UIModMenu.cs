using MelonLoader;
using UnityEngine;
using UnityEngine.UIElements;

namespace Schedule1ModMenu
{
    internal class UIModMenu
    {
        public static UIModMenu Instance { get; private set; } = new UIModMenu();

        GameObject uiMod = null;
        GameObject uiObj;
        bool menuOpen = false;

        private PlayerContext playerContext;

        public void LoadMenu(PlayerContext pContext)
        {
            try
            {
                playerContext = pContext;
                MelonLogger.Msg("Attempting to Load Menu...");

                if (Assets.modmenu == null || Assets.modmenu.Length == 0)
                {
                    MelonLogger.Error("Assets.modmenu is null or empty!");
                    return;
                }

                var ab = Il2CppAssetBundleManager.LoadFromMemory(Assets.modmenu);
                if (ab == null)
                {
                    MelonLogger.Error("Failed to load asset bundle from memory.");
                    return;
                }

                MelonLogger.Msg("Loading menu prefab...");
                uiMod = ab.LoadAsset<GameObject>("Assets/ModMenu.prefab");

                if (uiMod != null)
                {
                    MelonLogger.Msg("Instantiating Menu...");
                    //instantiate the mod menu in the scene
                    uiObj = GameObject.Instantiate(uiMod);

                    var uiDoc = uiObj.GetComponent<UIDocument>();

                    if (uiDoc != null)
                    {
                        MelonLogger.Msg("Getting RootVisualElement");
                        var rootVisual = uiDoc.rootVisualElement;

                        //set modmenu size
                        rootVisual.style.width = 400;
                        rootVisual.style.height = 300;
                        rootVisual.style.marginLeft = StyleKeyword.Auto;
                        rootVisual.style.marginRight = StyleKeyword.Auto;
                        rootVisual.style.marginTop = 50;

                        var modContainer = rootVisual.Q<VisualElement>("ModMenu(Clone)-container");

                        if (modContainer != null)
                        {

                            MelonLogger.Msg("Setting Player Name...");

                            // Now get the PlayerName label from inside ModContainer
                            Label playername = modContainer.Q<Label>();

                            if (playername != null)
                            {
                                playername.text = "Player: " + playerContext.gameObj.name;
                            }
                            else
                            {
                                MelonLogger.Error("PlayerName label not found inside ModContainer.");
                            }

                            Button button = modContainer.Q<Button>("HandButton", "unity-button");
                            button.RegisterCallback<ClickEvent>((EventCallback<ClickEvent>)OnButtonClick);
                        }
                        else
                        {
                            MelonLogger.Error("Error Getting ModContainer");
                        }
                    }
                }
                else
                {
                    MelonLogger.Error("Kill me");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error("Unhandled exception in LoadMenu: " + ex.Message + "\n" + ex.StackTrace);
            }
        }



        private void OnButtonClick(ClickEvent evt)
        {
            MelonLogger.Msg("Button clicked!");
        }


        public void ToggleMenu()
        {
            if (!menuOpen)
            {
                uiObj.SetActive(true);
                menuOpen = true;
            }
            else
            {
                uiObj.SetActive(false);
                menuOpen = false;
            }
        }

    }
}
