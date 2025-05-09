using S1API.Internal.Utils;
using S1API.PhoneApp;
using S1API.UI;
using UnityEngine;
using UnityEngine.UI;
using S1API.Money;
namespace Schedule1ModMenu
{

    public class PhoneModMenu : PhoneApp
    {
        protected override string AppName => "ModMenu";
        protected override string AppTitle => "Mod Menu";
        protected override string IconLabel => "Mod Menu";
        protected override string IconFileName => null;

        private Button handButton;
        private Button bankButton;



        protected override void OnCreatedUI(UnityEngine.GameObject container)
        {
            var bg = UIFactory.Panel("MainPanel", container.transform, Color.gray, fullAnchor: true);
            //UIFactory.TopBar("TopBar", bg.transform, "Mod Menu", 150f, 10, 1, 75, 75);


            var leftPanel = UIFactory.Panel("LeftPanel", bg.transform, new Color(0.1f, 0.1f, 0.1f),
                new Vector2(0.02f, 0f), new Vector2(0.49f, 0.82f));
            var separator = UIFactory.Panel("Separator", bg.transform, new Color(0.2f, 0.2f, 0.2f),
                new Vector2(0.485f, 0f), new Vector2(0.487f, 0.82f));
            var buttonRow = UIFactory.ButtonRow("ActionButtons", leftPanel.transform, 12, TextAnchor.MiddleLeft);

            var (handbuttonGO, handBtn, handLbl) = UIFactory.RoundedButtonWithLabel("handButton", "Add To Hand", buttonRow.transform, new Color(0.2f, 0.6f, 0.2f), 125, 75, 16, Color.white);
            handButton = handBtn;

            var (bankbuttonGO, bankBtn, bankLbl) = UIFactory.RoundedButtonWithLabel("bankButton", "Add To Bank", buttonRow.transform, new Color(0.2f, 0.6f, 0.2f), 125, 75, 16, Color.white);
            bankButton = bankBtn;

            ButtonUtils.AddListener(handButton, () => { Money.ChangeCashBalance(1000, true, true); });
            ButtonUtils.AddListener(bankButton, () => { 
                Money.CreateOnlineTransaction("hippity hoppity", 1000, 1, "");  
                
            });

        }

    }
}