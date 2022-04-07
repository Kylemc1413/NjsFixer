﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HMUI;
using IPA.Utilities;
using BeatSaberMarkupLanguage;
namespace NjsFixer.UI
{

    public class PreferencesFlowCoordinator : FlowCoordinator
    {
        public FlowCoordinator ParentFlow { get; set; }
        private PreferencesListViewController _prefListView;
        public void Awake()
        {
            if (_prefListView == null)
                _prefListView = BeatSaberMarkupLanguage.BeatSaberUI.CreateViewController<PreferencesListViewController>();
        }
        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if(firstActivation)
            {
                showBackButton = true;
                SetTitle("NjsFixer Preferences");
                ProvideInitialViewControllers(_prefListView);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            ParentFlow?.DismissFlowCoordinator(this, null, ViewController.AnimationDirection.Horizontal, false);
        }
    }
}
