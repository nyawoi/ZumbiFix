using UnityEngine;

namespace AetharNet.Mods.ZumbiBlocks2.ZumbiFix.MonoBehaviours;

public class EscapeHandler : MonoBehaviour
{
    public void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        // Developer console takes highest priority as it is highest visually
        if (ZBMain.instance.console.show)
        {
            ZBMain.instance.DisplayConsole(false);
        }
        // BossfightCameraSequence takes priority over escape menu
        // This is so the sequence can be skipped without having to close an escape menu
        else if (BossfightCameraSequence.instance.active) 
        {
            BossfightCameraSequence.instance.ClearCameraSequence();
            PlayerHUD.instance.bossName.DisableAll();
        }
        // GameHUD takes priority over other in-game menus (inventory, map)
        // This allows those menus to be closed with escape,
        // with a fallback to the escape menu
        else if (MenuController.instance.curMenuID == BaseMenu.ID.GameHUD)
        {
            MenuController.instance.inGameMenus.GoToMenu(BaseMenu.ID.EscapeMenu);
        }
        // Other in-game menus take priority over start screen menus
        // There's no real reason for this
        else if (MenuController.instance.curMenu.isInGameMenu)
        {
            MenuController.instance.inGameMenus.GoToMenu(BaseMenu.ID.GameHUD);
        }
        // Handle start screen menus
        else
        {
            MenuController.instance.OnPressEscape();
        }
    }
}
