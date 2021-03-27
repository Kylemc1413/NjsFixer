using IPA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BeatSaberMarkupLanguage;
using TMPro;
using HarmonyLib;
namespace NjsFixer
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {

        [OnStart]
        public void OnApplicationStart()
        {
            Config.Read();
            var harmony = new Harmony("com.kyle1413.BeatSaber.NjsFixer");
            harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
            BS_Utils.Utilities.BSEvents.gameSceneLoaded += BSEvents_gameSceneLoaded;
            BeatSaberMarkupLanguage.GameplaySetup.GameplaySetup.instance.AddTab("NjsFixer", "NjsFixer.UI.BSML.modifierUI.bsml", UI.ModifierUI.instance, BeatSaberMarkupLanguage.GameplaySetup.MenuType.Solo);
            BeatSaberMarkupLanguage.GameplaySetup.GameplaySetup.instance.AddTab("NjsFixerOnline", "NjsFixer.UI.BSML.modifierOnlineUI.bsml", UI.ModifierUI.instance, BeatSaberMarkupLanguage.GameplaySetup.MenuType.Online);
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
        {
            Config.Write();
        }

        [Init]
        public void Init(IPA.Logging.Logger logger)
        {
            Logger.log = logger;
        }
        private void BSEvents_gameSceneLoaded()
        {
            bool WillOverride = BS_Utils.Plugin.LevelData.IsSet && !BS_Utils.Gameplay.Gamemode.IsIsolatedLevel
                && Config.UserConfig.enabled && (BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Standard || BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer) && BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.practiceSettings == null;
            if (WillOverride && BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer)
            {
                Config.UserConfig.dontForceNJS = true;
            }
            if(WillOverride && !Config.UserConfig.dontForceNJS)
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("NjsFixer");

        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Config.Write();
        }

    }
}
