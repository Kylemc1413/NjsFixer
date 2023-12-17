using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using HMUI;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Attributes;
using UnityEngine;
using BeatSaberMarkupLanguage.MenuButtons;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Util;

namespace NjsFixer.UI
{
    public class ModifierUI : NotifiableSingleton<ModifierUI>
    {
        private PreferencesFlowCoordinator _prefFlow;
        [UIValue("minJump")]
        private int minJump => Config.UserConfig.minJumpDistance;
        [UIValue("maxJump")]
        private int maxJump => Config.UserConfig.maxJumpDistance;
        [UIComponent("njsSlider")]
        private SliderSetting njsSlider;
        [UIValue("njsValue")]
        public float njsValue
        {
            get => Config.UserConfig.njs;
            set
            {
                if (value < 0.05f) value = 0;
                Config.UserConfig.njs = value;
            }
        }
        [UIAction("setNjs")]
        void SetNjs(float value)
        {
            njsValue = value;
        }
        //[UIComponent("spawnOffsetSlider")]
        //private SliderSetting spawnOffsetSlider;
        //[UIValue("spawnOffset")]
        //public float spawnOffset
        //{
        //    get => Config.UserConfig.spawnOffset;
        //    set
        //    {
        //        Config.UserConfig.spawnOffset = value;
        //    }
        //}
        //[UIAction("setSpawnOffset")]
        //void setSpawnOffset(float value)
        //{
        //    spawnOffset = value;
        //}
        //[UIComponent("bpmSlider")]
        //private SliderSetting bpmSlider;
        //[UIValue("bpmValue")]
        //public float bpmValue
        //{
        //    get => Config.UserConfig.bpm;
        //    set
        //    {
        //        Config.UserConfig.bpm = value;
        //    }
        //}
        //[UIAction("setBPM")]
        //void SetBPM(float value)
        //{
        //    bpmValue = value;
        //}

        [UIValue("enabled")]
        public bool modEnabled
        {
            get => Config.UserConfig.enabled;
            set
            {
                Config.UserConfig.enabled = value;
            }
        }
        [UIAction("setEnabled")]
        void SetEnabled(bool value)
        {
            modEnabled = value;
        }
        [UIValue("practiceEnabled")]
        public bool practiceEnabled
        {
            get => Config.UserConfig.enabledInPractice;
            set
            {
                Config.UserConfig.enabledInPractice = value;
            }
        }
        [UIAction("setPracticeEnabled")]
        void SetPracticeEnabled(bool value)
        {
            practiceEnabled = value;
        }
        [UIValue("showNJS")]
        public bool showNJS
        {
            get => !Config.UserConfig.dontForceNJS;

            set
            {
                NotifyPropertyChanged();
            }
        }
        [UIValue("showJump")]
        public bool showJump
        {
            get => Config.UserConfig.dontForceNJS;
            set
            {
                NotifyPropertyChanged();
            }
        }

        [UIValue("forceNJS")]
        public bool forceNJS
        {
            get => !Config.UserConfig.dontForceNJS;
            set
            {
                Config.UserConfig.dontForceNJS = !value;
                showNJS = value;
                showJump = !value;
            }
        }
        [UIAction("setForceNJS")]
        void SetForceNJS(bool value)
        {
            forceNJS = value;
        }

        [UIComponent("jumpDisSlider")]
        private SliderSetting jumpDisSlider;
        [UIValue("jumpDisValue")]
        public float jumpDisValue
        {
            get => Config.UserConfig.jumpDistance;
            set
            {
                Config.UserConfig.jumpDistance = value;
            }
        }
        [UIAction("setJumpDis")]
        void SetJumpDis(float value)
        {
            jumpDisValue = value;
        }
        [UIAction("jumpDisFormatter")]
        public string HalfJumpDistanceForJumpDistance(float value)
        {
            return value.ToString("F2");
        }
        [UIValue("usePrefJumpValues")]
        public bool usePrefJumpValues
        {
            get => Config.UserConfig.usePreferredJumpDistanceValues;
            set
            {
                Config.UserConfig.usePreferredJumpDistanceValues = value;
            }
        }
        [UIAction("setUsePrefJumpValues")]
        void SetUsePrefJumpValues(bool value)
        {
            usePrefJumpValues = value;
        }
        [UIAction("prefButtonClicked")]
        void PrefButtonClicked()
        {
            if (_prefFlow == null)
                _prefFlow = BeatSaberUI.CreateFlowCoordinator<PreferencesFlowCoordinator>();
            var ActiveFlowCoordinator = DeepestChildFlowCoordinator(BeatSaberUI.MainFlowCoordinator);
            _prefFlow.ParentFlow = ActiveFlowCoordinator;
            ActiveFlowCoordinator.PresentFlowCoordinator(_prefFlow, null, ViewController.AnimationDirection.Horizontal, true);
        }

        [UIComponent("leftButton")]
        private RectTransform leftButton;
        [UIComponent("rightButton")]
        private RectTransform rightButton;
        [UIAction("#post-parse")]
        void PostParse()
        {
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), njsSlider, 0.1f);
            //SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), spawnOffsetSlider, 0.1f);
            //SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), bpmSlider, 1);
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), jumpDisSlider, 0.1f);
            GameObject.Destroy(leftButton.gameObject);
            GameObject.Destroy(rightButton.gameObject);
        }

        public static FlowCoordinator DeepestChildFlowCoordinator(FlowCoordinator root)
        {
            var flow = root.childFlowCoordinator;
            if (flow == null) return root;
            if (flow.childFlowCoordinator == null || flow.childFlowCoordinator == flow)
            {
                return flow;
            }
            return DeepestChildFlowCoordinator(flow);
        }
    }
}
