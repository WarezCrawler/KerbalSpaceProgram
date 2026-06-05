using System.Collections.Generic;
using UnityEngine;
using KSP.UI.Screens;

namespace AutoRove
{
    /// <summary>
    /// the main addon class to track behavier across multiple vessels
    /// </summary>
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    class autoRoveMain : MonoBehaviour
    {
        private int frameCounter = 0;
        private int framesPerUpdate = 0;
        private autoRoveGUI.AppButtonWindow window;
        private List<string[]> items = new List<string[]>();
        public List<Vessel> AutoRovers = new List<Vessel>();     //WarezCrawler
        private GameScenes currentScene = HighLogic.LoadedScene;
        private static ApplicationLauncherButton btnLauncher;

        public void toggleAppLauncher()
        {
            if (window == null)
            {
                window = gameObject.AddComponent<AutoRove.autoRoveGUI.AppButtonWindow>();
                window.items = items;
            }
            //else if (window != null)
            else
            {
                window.close();
            }
        }

        void Awake()
        {
            Texture icon = GameDatabase.Instance.GetTexture("AutoRove/appLauncherButton", false);
            //ApplicationLauncher.AppScenes visibility = ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.TRACKSTATION | ApplicationLauncher.AppScenes.FLIGHT;
            ApplicationLauncher.AppScenes visibility = ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.TRACKSTATION;

            //// registering the button
            //if (ApplicationLauncher.Ready)
            //{
            //    Debug.Log(this.name + ": registering AppButton (AutoRove)!");
            //    ApplicationLauncherButton appButton = ApplicationLauncher.Instance.AddModApplication(toggleAppLauncher, toggleAppLauncher, null, null, null, null, visibility, icon);
            //}

            //AppLauncherFlight.Awake();
            if (btnLauncher == null)
                btnLauncher = ApplicationLauncher.Instance.AddModApplication(toggleAppLauncher, toggleAppLauncher, null, null, null, null, visibility, icon);
        }

        void Start()
        {
            DontDestroyOnLoad(this);
            framesPerUpdate = autoRoveConfig.framesPerUpdate;

            //Debug.Log("AutoRove - Start() - add onGameSceneLoadRequested");
            //GameEvents.onGameSceneLoadRequested.Add (CallbackGameSceneLoadRequested);
        }

        //Callback function for scene changes
        //ADDED BY WAREZCRAWLER
        private void CallbackGameSceneLoadRequested(GameScenes scene)
        {
            Debug.Log("AutoRove - CallbackGameSceneLoadRequested() - Run update on scene change --> " + scene);

            //WarezCrawler --> Store rover list
            //if (AutoRovers != null)
            //{
            //    AutoRovers.Clear();
            //}
            //Collect relevant vessels for autoroving #WarezCrawler
            //foreach (Vessel ship in FlightGlobals.Vessels)
            //{
            //    //Check situation first to exclude all in space vessels from further checking
            //    if (ship.situation == Vessel.Situations.LANDED)
            //    {
            //        if (!ship.loaded && !ship.isActiveVessel)
            //        {
            //            //autoRoveUtils.debugMessage("found a rover!");
            //            Rover rover = new Rover().Initialize(ship);
            //            if (rover != null)
            //            {
            //                Debug.Log("AutoRove - CallbackGameSceneLoadRequested() - Adding Ship (" + ship.name + " | " + ship.vesselName + ") to List");
            //                AutoRovers.Add(ship);
            //            }
            //        }
            //    }
            //}
            UpdateAutoRovingRovers();
        }

        void Update()
        //void FixedUpdate()
        {
            frameCounter += 1;
            // update code goes here
            if (frameCounter == framesPerUpdate)
            {
                //Debug.Log("AutoRove - Update() - #Vessels = " + AutoRovers.Count);
                UpdateAutoRovingRovers();
                frameCounter = 0;
                
                // close the appLauncher window on scene change
                if (currentScene != HighLogic.LoadedScene)
                {
                    if (window != null) { window.close(); }
                    currentScene = HighLogic.LoadedScene;
                }
            }
        }

        private void UpdateAutoRovingRovers()
        {
            if (items != null)
            {
                items.Clear();
            }
            try
            {
                
                //Debug.Log("AutoRove - UpdateAutoRovingRovers() - if (AutoRovers != null && (AutoRovers.Count > 0)) --> CHECK");
                //if (AutoRovers != null && (AutoRovers.Count > 0))
                //{
                    //Debug.Log("AutoRove - UpdateAutoRovingRovers() - if (AutoRovers != null && (AutoRovers.Count > 0)) --> TRUE");
                    foreach (Vessel ship in FlightGlobals.Vessels)
                    //foreach (Vessel ship in AutoRovers)
                    {
                        //Check situation first to exclude all in space vessels from further checking
                        if (ship.situation == Vessel.Situations.LANDED)
                        {
                            if (!ship.loaded && !ship.isActiveVessel)
                            {
                            //autoRoveUtils.debugMessage("found a rover!");
                            //Debug.Log("AutoRove - UpdateAutoRovingRovers() - Initialize " + ship.vesselName);
                            Rover rover = new Rover().Initialize(ship);
                                if (rover != null)
                                {
                                    //autoRoveUtils.debugMessage("Rover is not null!");
                                    //if (rover.move()) { autoRoveUtils.debugMessage("moved rover!"); }
                                    Debug.Log("AutoRove - UpdateAutoRovingRovers() - Moving Rover (" + ship.vesselName + ")");
                                    rover.move();

                                    // updating the appLauncher window
                                    string distance = (rover.distanceToTarget / 1000).ToString("F1") + " km";
                                    string[] item = new string[4] { rover.name, rover.body.name, rover.target, distance };
                                    items.Add(item);
                                }
                                //else
                                //{
                                //    Debug.Log("AutoRove - UpdateAutoRovingRovers() - Vessel no a roving rover (" + ship.vesselName + ")");
                                //}
                            }
                        }
                    }
                //}
                //else
                //{
                //    Debug.Log("AutoRove - UpdateAutoRovingRovers() - NOTHING TO MOVE");
                //}
            }
            catch
            {
                //Debug.LogWarning(
                //    "AutoRove - UpdateAutoRovingRovers() - Exception Happend" +
                //    "\nAutoRovers != null = " + AutoRovers != null +
                //    "\n(AutoRovers.Count > 0) = " + (AutoRovers.Count > 0) +
                //    "\n"
                //    );
                Debug.LogWarning(
                    "AutoRove - UpdateAutoRovingRovers() - Exception Happend"
                    );
            }

        }
    }
}
