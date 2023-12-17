using System.Collections.Generic;
using System.IO;
using System;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine;
namespace NjsFixer
{
    public class NjsPref
    {
        public float njs = 12f;
        public float jumpDistance = 24f;

        public NjsPref(float njs, float jumpDistance)
        {
            this.njs = njs;
            this.jumpDistance = jumpDistance;
        }
    }

    public class NjsFixerConfig
    {
        public float njs = 0;
        public bool enabled = false;
        public bool enabledInPractice = false;
        public bool dontForceNJS = false;
        public float jumpDistance = 24f;
        public int minJumpDistance = 20;
        public int maxJumpDistance = 30;
        public bool usePreferredJumpDistanceValues = false;
        public List<NjsPref> preferredValues = new List<NjsPref>();

        public NjsFixerConfig()
        {

        }
        [JsonConstructor]
        public NjsFixerConfig(float njs, bool enabled, bool enabledInPractice, bool dontForceNJS, float jumpDistance, int minJumpDistance, int maxJumpDistance, bool usePreferredJumpDistanceValues, List<NjsPref> preferredValues)
        {
            this.njs = njs;
            this.enabled = enabled;
            this.enabledInPractice = enabledInPractice;
            this.dontForceNJS = dontForceNJS;
            this.jumpDistance = jumpDistance;
            this.minJumpDistance = minJumpDistance;
            this.maxJumpDistance = maxJumpDistance;
            this.usePreferredJumpDistanceValues = usePreferredJumpDistanceValues;
            this.preferredValues = preferredValues;
        }
    }

    public class Config
    {
        public static NjsFixerConfig UserConfig { get; private set; }
        public static string ConfigPath { get; private set; } = Path.Combine(IPA.Utilities.UnityGame.UserDataPath, "NjsFixer.json");

        private static bool CheckForOldConfig()
        {
            return File.Exists(Path.Combine(IPA.Utilities.UnityGame.UserDataPath, "NjsFixer.ini")) && !File.Exists(Path.Combine(IPA.Utilities.UnityGame.UserDataPath, "NjsFixer.json"));
        }
        public static void Read()
        {
            if (!File.Exists(ConfigPath))
            {
                if (CheckForOldConfig())
                {
                    var oldConfig = new BS_Utils.Utilities.Config("NjsFixer");
                    UserConfig = new NjsFixerConfig();
                    UserConfig.njs = oldConfig.GetFloat("NjsFixer", "njs", 0, true);
                    UserConfig.enabled = oldConfig.GetBool("NjsFixer", "Enabled", false, true);
                    UserConfig.dontForceNJS = oldConfig.GetBool("NjsFixer", "DontForceNJS", false, true);
                    UserConfig.jumpDistance = oldConfig.GetFloat("NjsFixer", "DesiredJumpDistance", 24f, true);
                    UserConfig.minJumpDistance = oldConfig.GetInt("NjsFixer", "minJumpDistance", 15, true);
                    UserConfig.maxJumpDistance = oldConfig.GetInt("NjsFixer", "maxJumpDistance", 30, true);
                    try
                    {
                        File.Delete(Path.Combine(IPA.Utilities.UnityGame.UserDataPath, "NjsFixer.ini"));
                    }
                    catch (Exception ex)
                    {
                        Logger.log.Warn($"Failed to delete old NjsFixer Config file {ex}");
                    }

                }
                else
                {
                    UserConfig = new NjsFixerConfig();
                }
                Write();
            }
            else
            {
                UserConfig = JsonConvert.DeserializeObject<NjsFixerConfig>(File.ReadAllText(ConfigPath));
            }
            UserConfig.preferredValues = UserConfig.preferredValues.OrderByDescending(x => x.njs).ToList();
        }

        public static void Write()
        {
            UserConfig.preferredValues = UserConfig.preferredValues.OrderByDescending(x => x.njs).ToList();
            Logger.log.Debug("Writing Config to file");
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(UserConfig, Formatting.Indented));
        }
    }
}