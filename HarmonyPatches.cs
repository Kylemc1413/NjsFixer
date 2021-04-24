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
        public static void Prefix(ref float startNoteJumpMovementSpeed, float startBpm, ref float noteJumpStartBeatOffset, ref BeatmapObjectSpawnMovementData __instance, ref bool __state)
        {
            bool WillOverride = BS_Utils.Plugin.LevelData.IsSet && !BS_Utils.Gameplay.Gamemode.IsIsolatedLevel 
                && Config.UserConfig.enabled && (BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Standard || BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer) && BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.practiceSettings == null;
            __state = WillOverride;
            if (!WillOverride) return;

          //  BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("NjsFixer");
            float mapNJS = startNoteJumpMovementSpeed;

            float njs = Config.UserConfig.dontForceNJS ? mapNJS : Config.UserConfig.njs;
            //   float bpm = 124f;
            //  float njs = 18;
            if (njs == 0)
                njs = startNoteJumpMovementSpeed;
            float simOffset = 0;
            //Change NJS and Offset
            if (!Config.UserConfig.dontForceNJS && BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Standard)
            {
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("NjsFixer");
                float numCurr = 60f / startBpm;
                float num2Curr = 4f;
                while (njs * numCurr * num2Curr > 18f)
                    num2Curr /= 2f;
                //      num2Curr += spawnMovementData.GetPrivateField<float>("_noteJumpStartBeatOffset");
                if (num2Curr < 1f)
                    num2Curr = 1f;
                float jumpDurCurr = num2Curr * numCurr * 2f;
                float jumpDisCurr = njs * jumpDurCurr;

                //SimBPM Calc
                float simBPM = Config.UserConfig.bpm > 0 ? Config.UserConfig.bpm : startBpm;
                float numSim = 60f / simBPM;
                float num2Sim = 4f;
                while (njs * numSim * num2Sim > 18f)
                    num2Sim /= 2f;
                var num2SimOffset = num2Sim + Config.UserConfig.spawnOffset;
                if (num2Sim < 1f)
                    num2Sim = 1f;
                if (num2SimOffset < 1f)
                    num2SimOffset = 1f;
                float jumpDurSim = num2SimOffset * numSim * 2f;
                float jumpDisSim = njs * jumpDurSim;

                float jumpDurMul = jumpDurSim / jumpDurCurr;


                simOffset = (num2Curr * jumpDurMul) - num2Curr;
                Logger.log.Debug($"BPM/NJS/Offset {startBpm}/{startNoteJumpMovementSpeed}/{noteJumpStartBeatOffset}");
                Logger.log.Debug($"Sim BPM/NJS/Offset {Config.UserConfig.bpm}/{njs}/{Config.UserConfig.spawnOffset}");
                Logger.log.Debug($"HalfJumpCurrent: {num2Curr} | HalfJumpSimulated {num2SimOffset} | SimJumpDis {jumpDisSim} | CurrJumpDis {jumpDisCurr} | JumpDurMul {jumpDurMul} | Simulated Offset {simOffset}");
                startNoteJumpMovementSpeed = njs;
            }
            else //Change Only Offset
            {
                float numCurr = 60f / startBpm;
                float num2Curr = 4f;
                while (njs * numCurr * num2Curr > 18f)
                    num2Curr /= 2f;
                //      num2Curr += spawnMovementData.GetPrivateField<float>("_noteJumpStartBeatOffset");
                if (num2Curr < 1f)
                    num2Curr = 1f;
                float jumpDurCurr = num2Curr * numCurr * 2f;
                float jumpDisCurr = njs * jumpDurCurr;

                float desiredJumpDis = Config.UserConfig.jumpDistance;
                if(Config.UserConfig.usePreferredJumpDistanceValues)
                {
                    var pref = Config.UserConfig.preferredValues.FirstOrDefault(x => x.njs == mapNJS);
                    if (pref != null)
                        desiredJumpDis = pref.jumpDistance;
                }
                float desiredJumpDur = desiredJumpDis / njs;
                float desiredHalfJumpDur = desiredJumpDur / 2f / num2Curr;
                float jumpDurMul = desiredJumpDur / jumpDurCurr;
                simOffset = (num2Curr * jumpDurMul) - num2Curr;
                Logger.log.Debug($"BPM/NJS/Offset {startBpm}/{startNoteJumpMovementSpeed}/{noteJumpStartBeatOffset}");
                Logger.log.Debug($"HalfJumpCurrent: {num2Curr} | DesiredHalfJump {desiredHalfJumpDur} | DesiredJumpDis {desiredJumpDis} | CurrJumpDis {jumpDisCurr} | Simulated Offset {simOffset}");
            }



            noteJumpStartBeatOffset = simOffset;



        }

        public static void Postfix(ref float ____jumpDistance, bool __state)
        {
            if(__state)
                Logger.log.Debug("Final Jump Distance: " + ____jumpDistance);
        }
    }
}
