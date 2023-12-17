using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
namespace NjsFixer
{

    [HarmonyPatch(typeof(BeatmapObjectSpawnMovementData), "Init")]
    internal class SpawnMovementDataUpdatePatch
    {
        public static void Prefix(ref float startNoteJumpMovementSpeed, ref BeatmapObjectSpawnMovementData.NoteJumpValueType noteJumpValueType, float startBpm, ref float noteJumpValue, ref BeatmapObjectSpawnMovementData __instance, ref bool __state)
        {
            bool WillOverride = BS_Utils.Plugin.LevelData.IsSet && !BS_Utils.Gameplay.Gamemode.IsIsolatedLevel 
                && Config.UserConfig.enabled && (BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Standard || BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer) && (Config.UserConfig.enabledInPractice || BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.practiceSettings == null);
            __state = WillOverride;
            if (!WillOverride) return;

            noteJumpValueType = BeatmapObjectSpawnMovementData.NoteJumpValueType.JumpDuration;

            var oneBeat = 60f / startBpm;
            float mapNJS = startNoteJumpMovementSpeed;

            float njs = Config.UserConfig.dontForceNJS ? mapNJS : Config.UserConfig.njs;

            if (njs == 0)
                njs = startNoteJumpMovementSpeed;
            //Change NJS and Offset
            if (!Config.UserConfig.dontForceNJS && BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Standard)
            {
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("NjsFixer");
                float desiredJumpDis = Config.UserConfig.jumpDistance;
                float desiredHalfJumpDuration = desiredJumpDis / njs / 2;
                noteJumpValue = desiredHalfJumpDuration;
                startNoteJumpMovementSpeed = njs;
                Logger.log.Debug($"BPM/NJS/JumpDistance/NoteJumpValue {startBpm}/{njs}/{startNoteJumpMovementSpeed}/{noteJumpValue}");
            }
            else //Change Only Offset
            {
                float desiredJumpDis = Config.UserConfig.jumpDistance;
                if (Config.UserConfig.usePreferredJumpDistanceValues)
                {
                    var pref = Config.UserConfig.preferredValues.FirstOrDefault(x => x.njs == mapNJS);
                    if (pref != null)
                        desiredJumpDis = pref.jumpDistance;
                }
                float desiredHalfJumpDuration = desiredJumpDis / mapNJS / 2;

                noteJumpValue = desiredHalfJumpDuration;
            }



        }

        public static void Postfix(ref float ____jumpDistance, bool __state)
        {
            if(__state)
                Logger.log.Debug("Final Jump Distance: " + ____jumpDistance);
        }
    }
}
