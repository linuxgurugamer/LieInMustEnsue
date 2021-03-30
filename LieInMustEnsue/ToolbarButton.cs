﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP.UI;
using KSP.UI.Screens;
using UnityEngine;
using UnityEngine.UI;

namespace LieInMustEnsue
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class ToolbarButton : MonoBehaviour
    {
        // button textures
        public Texture limeOn;
        public Texture limeOff;
        public Texture limeHover;

        // the toolbar button
        public static ApplicationLauncherButton limeBtn;

        // is button pressed?
        public bool btnIsPressed = false;

        // does the button exist?
        public bool btnIsPresent = false;

        // menu selection id
        public static int selGridInt = 1;

        // menu options
        public static string[] selString = new string[] { "Sunrise (Stock)", "Sunny", "Sunset", "Midnight" };

        // close button on menu
        public static bool closeBtn;

        // menu position reference, set for middle of the screen
        private Vector2 menuPR = new Vector2((Screen.width / 2) - 100, (Screen.height / 2) - 93);

        // menu size reference
        private Vector2 menuSR = new Vector2(200, 250);

        // the menu position holder
        private static Rect menuPos;

        
        

        public void Awake()
        {
            // register game events
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER && limeBtn == null)
            {
                GameEvents.onGUIApplicationLauncherReady.Add(AddButton);
                GameEvents.onGUIApplicationLauncherUnreadifying.Add(RemoveButton);
            }
        }

        private void RemoveButton(GameScenes gameScenes)
        {
            // remove the button

            ApplicationLauncher.Instance.RemoveModApplication(limeBtn);
            btnIsPressed = false;
            btnIsPresent = false;
            
        }
        private void AddButton()
        {
            // add the button

            if (!btnIsPresent)
            {
                limeBtn = ApplicationLauncher.Instance.AddModApplication(onTrue, onFalse, onHover, onHoverOut, onEnable, onDisable,
                    ApplicationLauncher.AppScenes.SPACECENTER, limeOff);

                btnIsPresent = true;
            }
        }

        private static void ItsLimeTime()
        { 

            // instantiate the menu

            menuPos = GUILayout.Window(123456, menuPos, MenuWindow,
                "LIME Time Options", new GUIStyle(HighLogic.Skin.window));  

        }     

        private static void MenuWindow(int windowID)
        {
            // menu defs
         
            GUILayout.BeginVertical();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();

            selGridInt = GUI.SelectionGrid(new Rect(20, 50, 200, 186), selGridInt, selString, 1, new GUIStyle(HighLogic.Skin.toggle));
            
            GUILayout.EndHorizontal();
            GUILayout.Space(25);

            GUILayout.BeginHorizontal();

            closeBtn = GUI.Button(new Rect(20, 200, 160, 25), "Close", new GUIStyle(HighLogic.Skin.button));

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            
            GUI.DragWindow();

        }

        // long winded way (but causes bugs otherwise) of invoking onFalse
        public void CloseMenu()
        {
            onFalse();
            onDisable();
            btnIsPresent = false;
            AddButton();
            btnIsPresent = true;
        }


        public void Start()
        {
            // get the icons from file, preload menu position

            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {

                limeOn = GameDatabase.Instance.GetTexture("FruitKocktail/LIME/Icons/limeon", false);
                limeOff = GameDatabase.Instance.GetTexture("FruitKocktail/LIME/Icons/limeoff", false);
                limeHover = GameDatabase.Instance.GetTexture("FruitKocktail/LIME/Icons/limehover", false);

                menuPos = new Rect(menuPR, menuSR);
            }

            else
            {
                onDisable();
            }
            
            
        }


        public void Update()
        {
            
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                // handles change of mode by player

                if (btnIsPresent)
                {
                    LIME.newMode = selGridInt;
                }

                // handles close button being pressed on menu

                if (closeBtn)
                {
                    CloseMenu();
                    closeBtn = false;
                }
            }
        }

        public void OnGUI()
        {
            // handles GUI event (ie button clicked)

            if (btnIsPressed)
            {
                ItsLimeTime();
            }
        }

        // button callbacks

        public void onTrue()
        {
            // ie when clicked on
            limeBtn.SetTexture(limeOn);
            btnIsPressed = true;      

        }

        public void onFalse()
        {
            // ie when clicked off
            limeBtn.SetTexture(limeOff);
            btnIsPressed = false;
        }

        public void onHover()
        {
            // ie on hover when not currently on

            if (!btnIsPressed)
            {
                limeBtn.SetTexture(limeHover);
            }
        }

        public void onHoverOut()
        {
            // ie when leave button when not currently on

            if (!btnIsPressed)
            {
                limeBtn.SetTexture(limeOff);
            }
        }

        public void onEnable()
        {
            GameEvents.onGUIApplicationLauncherReady.Add(AddButton);
            GameEvents.onGUIApplicationLauncherUnreadifying.Add(RemoveButton);
        }

        public void onDisable()
        {
            // ie when button is disabled / leave scene

            GameEvents.onGUIApplicationLauncherReady.Remove(AddButton);
            GameEvents.onGUIApplicationLauncherUnreadifying.Remove(RemoveButton);
            ApplicationLauncher.Instance.RemoveModApplication(limeBtn);
           
        }


    

      



    }
}