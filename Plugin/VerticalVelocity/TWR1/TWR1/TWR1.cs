using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Timers;
using KSP.UI.Screens;

namespace VerticalVelocity
{
    //Begin Vertical Velocity Control Mod by Diazo. (Originally Thrust to Weight Ratio 1 mod, hence the TWR1 references everywhere.)
    //Released under the GPL 3 license (http://www.gnu.org/licenses/gpl-3.0.html)
    //This means you may modify and distribute this code as long as you include the source code showing the changes you made and also release your changes under the GPL 3 license.
    //https://forum.kerbalspaceprogram.com/index.php?/topic/46647-13jun0417-automate-vertical-velocity-and-altitude-control/

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class TWR1 : MonoBehaviour
    {
        public static TWR1 thisModule;
        ApplicationLauncherButton TWR1StockButton = null; //stock toolbar button instance
        private bool buttonCreated = false;
        private bool TWR1KeyDown = false; //Is the TWR1 being held down at the moment?
        //private bool curVsl.TWR1Engaged = false; //Is TWR1 (Thrust Weight Ratio 1) mod engaged and auto-controlling?
        //private double TWR1ThrustUp = 0f; //thrust straight up needed for desired accel
        //private double TWR1ThrustUpAngle = 0f; //thrust needed for desired accel, compensation for vessel angel
        //private double TWR1DesiredAccel = 0f; //desired acceleration, includes planet grav so this can not be negative
        //private double TWR1DesiredAccelThrust = 0f; //thrust needed for desired accel
        //private Vessel TWR1Vessel; //Our active vessel
        //private double TWR1Mass = 0f; //Vessel's mass
        //private double TWR1MassLast = 0;
        //private double TWR1MaxThrust = 0f; //Max thrust contolled by throttle
        //private double TWR1MinThrust = 0f; //Min thrust controlled by throttle, not necessarily zero if solid rocket boosters are firing
        //private ModuleEngines TWR1EngineModule; //part of check for solid rocket booster
        //private ModuleEnginesFX TWR1EngineModuleFX;
        //private CelestialBody TWR1SOI; //what are we orbiting?
        //private double TWR1GravHeight = 0f; //distance from center of body for gravity force
        //private double TWR1GravForce = 0f; //acceleration down due to gravity at this moment
        //private double TWR1ThrottleRead = 0f; //current throttle at start of update in %
        //private double TWR1VesselPitch; //pitch of vessel from horizon
        //private Vector3 TWR1Up; //vessel up angle
        //private Vector3 TWR1CoM; //vessel Center of mass
        //private double TWR1OffsetVert = 0f; //vessel's offset from vertical in degrees
        //private double TWR1OffsetVertRadian = 0f; //vessel's offset from vertical in radians
        //private double TWR1OffsetVertRatio = 0f; //cosine of vessel's offset from vertical (return as unitless nubmer, so not degress or radians)
        private ConfigNode TWR1Node; //config node used to load keybind
        private string TWR1KeyCodeString = "Z"; //set the TWR1, default to Z if it can't load from TWR1.cfg
        private KeyCodeExtended TWR1KeyCode;
        //private double TWR1VelocitySetpoint = 0f; //vessel vertical velocity setpoint
        //private double TWR1VelocityCurrent = 0f; //vessel's current vertical velocity
        //private double TWR1VelocityDiff = 0f; //velocity difference between setpoint and current
        private static Rect TWR1WinPos = new Rect(100, 100, 195, 80); //window size
        private int TWR1WinPosHeight = 100;
        private int TWR1WinPosWidth = 100;
        private static GUISkin TWR1Skin;
        private static GUIStyle TWR1WinStyle = null; //window style
        private static GUIStyle TWR1LblStyle = null; //window style
        private static GUIStyle TWR1BtnStyle = null; //window style
        private static GUIStyle TWR1FldStyle = null; //window style
        //private bool TWR1HeightCtrl = false; //control to height engaged?
        //private double TWR1HC80Thrust; //80% thrust accel
        //private double TWR1HCTarget = 0f; //target height
        //private double TWR1HCToGround; //height about ground/building/sea
        //private double TWR1HCDistance; //distance from current heigh to target, this is absolute value so always postive
        //private double TWR1HC5Thrust; //accel at 5% thrust
        //private double TWR1HC1Thrust; //accel at 5% thrust
        //private double TWR1HCDistToTarget; //Distance to HC target, always positive
        private Color TWR1ContentColor; //Default content color
        //private string TWR1HCTargetString; //Height Control target height in string format for GUI text entry
        //private bool TWR1HCOrbitDrop = false; //Are we orbit dropping?
        private IButton TWR1Btn; //blizzy's toolbar button
        //WarezCrawler - Make static and available publicly for the module to use
        public static bool TWR1Show { get; private set; } = false; //show GUI?
        //private double TWR1HCThrustWarningTime = 0; //gametime saved for thrust warning check
        //private bool TWR1OrbitDropAllow = false; //are we high enough to offer Orbit Drop as an option?
        //private double TWR1OrbitDropHeightNeeded = 0f; //how much height needed for Orbit Drop
        //private double TWR1OrbitDropTimeNeeded = 0f; //how much time needed to orbit drop
        private TextAnchor TWR1DefautTextAlign; //store default text alignment to reset it after GUI frame draws
        private TextAnchor TWR1DefaultTextFieldAlign; //same^
        //private bool TWR1KASDetect = false; //is KAS installed?
        public static float TWR1SpeedStep = 1f; //Size of speed change per tap, default to 1m/s
        public string TWR1SpeedStepString; //speed step as string for GUI text entry
        private Texture2D TWR1SettingsIcon = new Texture2D(20, 22, TextureFormat.ARGB32, false); //toolbar icon texture
        private Rect TWR1SettingsWin = new Rect(500, 500, 200, 145);  //settings window position
        private bool TWR1SettingsShow = false; //show settings window?
        private bool TWR1SelectingKey = false; //are we selecting a new key?
        //private double TWR1LastVel; //vessel vertical velocity last physics frame
        //private double TWR1DesiredAccelThrustLast = 0; //desired thrust last physics frame
        //private double TWR1ThrustDiscrepancy; //difference in kN between thrusts last frame
        //private double TWR1LastFrameActualThrust; //actual "thrust" last frame, includes both engine and aerodynamic lift
        //private Queue<double> TWR1ThrustQueue; //last 5 frames of thrust to average out, it's too bouncy to use just last frame
        //private float ThrustUnderRun = 0; //difference between requested and actual thrust, hello jet engines
        GameObject lineObj = new GameObject("Line");
        LineRenderer theLine = new LineRenderer();
        private Timer showLineTime;
        private static bool timerElapsed = false;
        private bool timerRunning = false;
        //Vector3 TWR1ControlUp;
        //public static int ControlDirection = 0; //control direction for up, 0 is for rockets, 1 for cockpits, 2 through 5 the other directions.
        //private Part LastVesselRoot; //saved vessel last update pass, use rootpart for check
        public TWR1Data curVsl; //find our current vessel
        ITargetable lastTarget;
        bool mouseOverWindow = false;
        bool lockSetHeight = false;
        bool lockSetStep = false;
        bool showMyUI = true;

        public class VslTime
        {
            public Vessel vsl;
            public double time;
            public bool landed;
            public bool stable;
        } //class for 15second landed delay
        public static List<VslTime> SCVslList; //vessel list
        private bool TWR1VesselActive = true; //do we have a vessel to control?
        //TWR1Data rootTWR1Data;

        public void DummyVoid()
        {

        }

        public void onStockToolbarClick()
        {
            TWR1Show = !TWR1Show;
        }

        public void Start() //Start runs on mod start, after all other mods loaded
        {
            try
            {

                Debug.Log("Vertical Veloctiy 1.33 Loaded");
                thisModule = this;
                TWR1SettingsIcon = GameDatabase.Instance.GetTexture("Diazo/TWR1/TWR1Settings", false); //load toolbar icon
                                                                                                       //GameEvents.onVesselChange.Add(TWR1VesselChange);
                                                                                                       //GameEvents.onUndock.Add(TWR1VesselUnDock);
                                                                                                       //SCVslList = new List<VslTime>(); //initialize SkyCrane vesse list
                                                                                                       // TWR1ThrustQueue = new Queue<double>();  // initilize ThrustQueue for lift compensation
                                                                                                       //if (!CompatibilityChecker.IsCompatible()) //run compatiblity check
                                                                                                       //{
                                                                                                       //    TWR1ControlOffText = "Control Off (Mod Outdated)"; //if mod outdated, display it
                                                                                                       //}
                                                                                                       //else
                                                                                                       //{
                                                                                                       //    TWR1ControlOffText = "Control Off";
                                                                                                       //}

                //RenderingManager.AddToPostDrawQueue(0, OnDraw); //add call to GUI routing
                if (System.IO.File.Exists((string)KSPUtil.ApplicationRootPath + "GameData/Diazo/TWR1/TWR1.settings"))
                {
                    //Debug.Log("Twr1 case 1");
                    TWR1Node = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/Diazo/TWR1/TWR1.settings"); //load .cfg file
                }
                else if ((System.IO.File.Exists((string)KSPUtil.ApplicationRootPath + "GameData/Diazo/TWR1/TWR1.cfg")))
                {
                    //Debug.Log("Twr1 case 2");
                    TWR1Node = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/Diazo/TWR1/TWR1.cfg"); //load .cfg file
                    TWR1Node.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/TWR1/TWR1.settings");
                    System.IO.File.Delete(KSPUtil.ApplicationRootPath + "GameData/Diazo/TWR1/TWR1.cfg");
                }
                else
                {
                    //Debug.Log("Twr1 case 3");
                    TWR1Node = new ConfigNode();
                    TWR1Node.AddValue("TWR1Key", "None");
                    TWR1Node.AddValue("TWR1WinX", "100");
                    TWR1Node.AddValue("TWR1WinY", "100");
                    TWR1Node.AddValue("TWR1KASDisable", "false");
                    TWR1Node.AddValue("TWR1KASForce", "false");
                    TWR1Node.AddValue("TWR1Step", "1");
                    TWR1Node.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/TWR1/TWR1.settings");
                }
                TWR1KeyCodeString = TWR1Node.GetValue("TWR1Key"); //read keybind from .cfg, no functionality to set keybind from ingame exists yet
                //TWR1KeyCode.code = (KeyCode)Enum.Parse(typeof(KeyCode), TWR1KeyCodeString); //convert from text string to KeyCode item
                TWR1KeyCode = new KeyCodeExtended(TWR1KeyCodeString);
                TWR1SpeedStep = (float)Convert.ToDecimal(TWR1Node.GetValue("TWR1Step")); //load speed step size from file

                TWR1Skin = (GUISkin)MonoBehaviour.Instantiate(HighLogic.Skin);
                TWR1WinStyle = new GUIStyle(TWR1Skin.window); //GUI skin style
                TWR1LblStyle = new GUIStyle(TWR1Skin.label);
                TWR1FldStyle = new GUIStyle(TWR1Skin.textField);
                TWR1FldStyle.fontStyle = FontStyle.Normal;
                TWR1FldStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
                //Font fontTest = Font("calibri");
                //TWR1LblStyle.font = UnityEngine.Font("calibri");
                TWR1LblStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
                TWR1LblStyle.wordWrap = false;
                TWR1BtnStyle = new GUIStyle(TWR1Skin.button);
                TWR1BtnStyle.fontStyle = FontStyle.Normal;
                TWR1BtnStyle.alignment = TextAnchor.MiddleCenter;

                try
                {
                    TWR1WinPosHeight = Convert.ToInt32(TWR1Node.GetValue("TWR1WinY")); //get saved window position
                }
                catch
                {
                    TWR1WinPosHeight = (int)(Screen.height * .1);
                }
                try
                {
                    TWR1WinPosWidth = Convert.ToInt32(TWR1Node.GetValue("TWR1WinX")); //get saved window position
                }
                catch
                {
                    TWR1WinPosWidth = (int)(Screen.height * .1); //set window to 10% from top if fail
                }


                TWR1WinPos = new Rect(TWR1WinPosWidth, TWR1WinPosHeight, 215, 180); //set window position
                TWR1SettingsWin = new Rect(TWR1WinPosWidth + 218, TWR1WinPosHeight, 200, 180); //set settings window position to just next to main window
                if (ToolbarManager.ToolbarAvailable) //check if toolbar available, load if it is
                {


                    TWR1Btn = ToolbarManager.Instance.add("TWR1", "TWR1Btn");
                    TWR1Btn.TexturePath = "Diazo/TWR1/icon_button";
                    TWR1Btn.ToolTip = "Vertical Velocity Control";
                    TWR1Btn.OnClick += (e) =>
                    {
                        onStockToolbarClick();
                    };
                }
                else
                {
                    //AGXShow = true; //toolbar not installed, show AGX regardless
                    //now using stock toolbar as fallback
                    //TWR1StockButton = ApplicationLauncher.Instance.AddModApplication(onStockToolbarClick, onStockToolbarClick, DummyVoid, DummyVoid, DummyVoid, DummyVoid, ApplicationLauncher.AppScenes.FLIGHT, (Texture)GameDatabase.Instance.GetTexture("Diazo/TWR1/icon_button", false));
                    StartCoroutine("AddButtons");
                }
                showLineTime = new System.Timers.Timer(3000);
                //showLineTime.Interval = 3;
                showLineTime.Elapsed += new ElapsedEventHandler(LineTimeOut);
                showLineTime.AutoReset = false;


                theLine = lineObj.AddComponent<LineRenderer>();
                theLine.material = new Material(Shader.Find("Particles/Additive"));
                //theLine.SetColors(Color.red, Color.red);
                theLine.startColor = Color.red;
                theLine.endColor = Color.red;
                //theLine.SetWidth(0, 0);
                theLine.startWidth = 0f;
                theLine.endWidth = 0f;
                //theLine.SetVertexCount(2);
                theLine.positionCount = 2;
                theLine.useWorldSpace = false;
                GameEvents.onHideUI.Add(onHideMyUI);
                GameEvents.onShowUI.Add(onShowMyUI);
                //LastVesselRoot = new Part();
            }
            catch(Exception e)
            {
                Debug.Log("TWR1 Start Fail " + e);
            }

        }

        public void onShowMyUI()
        {
           // Debug.Log("TWR1 Show UI");
            showMyUI = true;
        }

        public void onHideMyUI()
        {
           // Debug.Log("TWR1 Show UI");
            showMyUI = false;
        }

        IEnumerator AddButtons()
        {
            while (!ApplicationLauncher.Ready)
            {
                yield return null;
            }
            if (!buttonCreated)
            {
                TWR1StockButton = ApplicationLauncher.Instance.AddModApplication(onStockToolbarClick, onStockToolbarClick, DummyVoid, DummyVoid, DummyVoid, DummyVoid, ApplicationLauncher.AppScenes.FLIGHT, (Texture)GameDatabase.Instance.GetTexture("Diazo/TWR1/icon_button", false));
                //GameEvents.onGUIApplicationLauncherReady.Remove(AddButtons);
                //CLButton.onLeftClick(StockToolbarClick);
                //CLButton.onRightClick = (Callback)Delegate.Combine(CLButton.onRightClick, rightClick); //combine delegates together
                buttonCreated = true;
            }
        }

        public void LineTimeOut(System.Object source, ElapsedEventArgs e)
        {
            timerElapsed = true; //timer runs in seperate thread, set static bool to true to get back to our main thread
        }

        public void ShowLine()
        {
            showLineTime.Start();
            timerRunning = true;
            //theLine.SetWidth(1, 0);
            theLine.startWidth = 1f;
            theLine.endWidth = 0f;


        }

        public void HideLine()
        {
            //theLine.SetWidth(0, 0);
            theLine.startWidth = 0f;
            theLine.endWidth = 0f;
        }

        public void OnDisable()
        {

            if (ToolbarManager.ToolbarAvailable) //if toolbar loaded, destroy button on leaving flight scene
            {
                TWR1Btn.Destroy();
            }
            else
            {
                ApplicationLauncher.Instance.RemoveModApplication(TWR1StockButton);
            }
            TWR1Node.SetValue("TWR1WinX", TWR1WinPos.x.ToString()); //save window position
            TWR1Node.SetValue("TWR1WinY", TWR1WinPos.y.ToString());//same^
            TWR1Node.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/TWR1/TWR1.settings");//same^
            GameEvents.onHideUI.Remove(onHideMyUI);
            GameEvents.onShowUI.Remove(onShowMyUI);

        }


        public void OnGUI()
        {
            if (TWR1Show && showMyUI) //show window?
            {
                TWR1WinPos = GUI.Window(673467798, TWR1WinPos, OnWindow, "Vertical Velocity (Key:" + TWR1KeyCode.ToString() + ")", TWR1WinStyle);
                if(TWR1WinPos.Contains(Mouse.screenPos))
                {
                    mouseOverWindow = true;
                }
                else
                {
                    mouseOverWindow = false;
                }
                if (TWR1SettingsShow) //show settings window?
                {
                    TWR1SettingsWin = GUI.Window(673467799, TWR1SettingsWin, OnSettingsWindow, "Settings", TWR1WinStyle);
                }
            }
            else
            {
                mouseOverWindow = false;
            }
        }

        public void OnSettingsWindow(int WindowID)
        {
            if (curVsl == null)
            {
                GUI.Label(new Rect(10, 30, 150, 25), "No vessel", TWR1LblStyle);
            }
            else
            {
                TWR1ContentColor = GUI.contentColor; //set defaults to reset them at end
                TWR1DefautTextAlign = GUI.skin.label.alignment; //same^
                TWR1DefaultTextFieldAlign = GUI.skin.textField.alignment;//same^
                TWR1LblStyle.alignment = TextAnchor.MiddleLeft;
                if (TWR1SelectingKey) //are we selecting a new key binding?
                {
                    if (Event.current.keyCode != KeyCode.None) //wait for keypress
                    {
                        if (Event.current.keyCode == KeyCode.Escape)
                        {
                            TWR1KeyCode.code = KeyCode.None;
                        }
                        else
                        {
                            TWR1KeyCode.code = Event.current.keyCode;
                        }

                        TWR1SelectingKey = false; //no longer selecting a new key binding
                        TWR1KeyCodeString = TWR1KeyCode.code.ToString(); //save new keybinding
                        TWR1Node.SetValue("TWR1Key", TWR1KeyCodeString);//same^
                        TWR1Node.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/TWR1/TWR1.settings");//same^
                    }
                    GUI.Label(new Rect(10, 30, 150, 25), "Press New Key\nESC to unbind", TWR1LblStyle); //change GUI to indicate we are waiting for key press
                    if (GUI.Button(new Rect(110, 30, 100, 25), "Cancel", TWR1BtnStyle)) //cancel key change
                    {
                        TWR1SelectingKey = false;
                    }
                }
                else  //not selecting a new key so display normal settings window
                {
                    GUI.Label(new Rect(10, 30, 150, 25), "Key: " + TWR1KeyCode.ToString(), TWR1LblStyle); //current key and option to change
                    if (GUI.Button(new Rect(80, 30, 90, 25), "Change Key", TWR1BtnStyle))//select new key?
                    {
                        TWR1SelectingKey = true;
                    }
                }
                //GUI.Label(new Rect(10, 60, 150, 25), "Scycrane Mode:", TWR1LblStyle); //skycrane mode settings

                //if (TWR1Node.GetValue("TWR1KASDisable") == "false" && TWR1Node.GetValue("TWR1KASForce") == "false") //in auto, so green
                //{

                //    GUI.contentColor = Color.green;
                //}
                //if (GUI.Button(new Rect(10, 80, 57, 25), "Auto", TWR1BtnStyle)) //change to auto mode
                //{
                //    TWR1KASDetect = false;
                //    foreach (AssemblyLoader.LoadedAssembly Asm in AssemblyLoader.loadedAssemblies) //run auto mode check
                //    {
                //        if (Asm.dllName == "KAS")
                //        {
                //            TWR1KASDetect = true;
                //        }

                //    }
                //    TWR1Node.SetValue("TWR1KASDisable", "false"); //save change
                //    TWR1Node.SetValue("TWR1KASForce", "false");//same^
                //    TWR1Node.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/TWR1/TWR1.cfg");//same^
                //}

                //GUI.contentColor = TWR1ContentColor; //reset color
                //if (TWR1Node.GetValue("TWR1KASDisable") == "false" && TWR1Node.GetValue("TWR1KASForce") == "true") //skycrane forced on? green text
                //{
                //    GUI.contentColor = Color.green;
                //}
                //if (GUI.Button(new Rect(67, 80, 57, 25), "On", TWR1BtnStyle)) //force skycrane mode on
                //{
                //    TWR1KASDetect = true;
                //    TWR1Node.SetValue("TWR1KASDisable", "false"); //save change
                //    TWR1Node.SetValue("TWR1KASForce", "true");//same^
                //    TWR1Node.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/TWR1/TWR1.cfg");//same^
                //}
                //GUI.contentColor = TWR1ContentColor;//reset color
                //if (TWR1Node.GetValue("TWR1KASDisable") == "true" && TWR1Node.GetValue("TWR1KASForce") == "false") //skycrane forced off? green text
                //{
                //    GUI.contentColor = Color.green;
                //}
                //if (GUI.Button(new Rect(124, 80, 57, 25), "Off", TWR1BtnStyle)) //force skycrane mode off
                //{
                //    TWR1KASDetect = false;
                //    TWR1Node.SetValue("TWR1KASDisable", "true"); //save change
                //    TWR1Node.SetValue("TWR1KASForce", "false");//same^
                //    TWR1Node.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/TWR1/TWR1.cfg");//same^
                //}
                GUI.contentColor = TWR1ContentColor; //reset color

                GUI.Label(new Rect(10, 110, 150, 25), "Velocity Step Size:", TWR1LblStyle);
                TWR1SpeedStepString = TWR1SpeedStep.ToString(); //text box requires a string, not a number
                //GUI.skin.label.alignment = TextAnchor.MiddleRight; //these lines are for that conversion back and forth
                TWR1LblStyle.alignment = TextAnchor.MiddleRight;
                //GUI.skin.textField.alignment = TextAnchor.MiddleRight;//same^
                TWR1FldStyle.alignment = TextAnchor.MiddleRight;
                GUI.SetNextControlName("TWR1StepSize");
                TWR1SpeedStepString = GUI.TextField(new Rect(130, 110, 50, 25), TWR1SpeedStepString, 5, TWR1FldStyle);//same^

                try //try converting characters in text box to string
                {
                    TWR1SpeedStep = (float)Convert.ToDecimal(TWR1SpeedStepString); //conversion ok, apply and save change
                    TWR1Node.SetValue("TWR1Step", TWR1SpeedStep.ToString());
                    TWR1Node.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/TWR1/TWR1.settings");
                }
                catch //fail converting characters
                {
                    TWR1SpeedStepString = TWR1SpeedStep.ToString(); //undo any changes
                    GUI.FocusControl(""); //non-number key was pressed, give focus back to ship control
                }

                if (GUI.GetNameOfFocusedControl() == "TWR1StepSize" && !lockSetStep)
                {
                    ControlLockCalls.SetControlLock("TWR1StepSizeLock");
                    lockSetStep = true;
                }
                else if (GUI.GetNameOfFocusedControl() != "TWR1StepSize" && lockSetStep)
                {
                    ControlLockCalls.ReleaseControlLock("TWR1StepSizeLock");
                    lockSetStep = false;//lockSetStep
                }
                if (GUI.Button(new Rect(10, 140, 70, 25), "Direction", TWR1BtnStyle)) //force skycrane mode off
                {
                    ShowLine();
                }
                if (GUI.Button(new Rect(80, 140, 23, 20), "U", TWR1BtnStyle)) //force skycrane mode off
                {
                    curVsl.controlDirection = 0;
                    ShowLine();
                }
                if (GUI.Button(new Rect(80, 160, 23, 20), "D", TWR1BtnStyle)) //force skycrane mode off
                {
                    curVsl.controlDirection = 2;
                    ShowLine();
                }
                if (GUI.Button(new Rect(103, 140, 23, 25), "F", TWR1BtnStyle)) //force skycrane mode off
                {
                    curVsl.controlDirection = 3;
                    ShowLine();
                }
                if (GUI.Button(new Rect(126, 140, 23, 25), "B", TWR1BtnStyle)) //force skycrane mode off
                {
                    curVsl.controlDirection = 1;
                    ShowLine();
                }
                if (GUI.Button(new Rect(103, 160, 23, 25), "L", TWR1BtnStyle)) //force skycrane mode off
                {
                    curVsl.controlDirection = 5;
                    ShowLine();
                }
                if (GUI.Button(new Rect(126, 160, 23, 25), "R", TWR1BtnStyle)) //force skycrane mode off
                {
                    curVsl.controlDirection = 4;
                    ShowLine();
                }

                //GUI.skin.textField.alignment = TWR1DefaultTextFieldAlign; //reset GUI skin stuff
                TWR1FldStyle.alignment = TWR1DefaultTextFieldAlign;
                //GUI.skin.label.alignment = TWR1DefautTextAlign;//same^
                TWR1LblStyle.alignment = TWR1DefautTextAlign;
                GUI.contentColor = TWR1ContentColor;//same^
                GUI.DragWindow(); //window is draggable
                GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            }
        }

        public void OnWindow(int WindowID) //main VertVel window
        {
            if (curVsl == null)
            {
                GUI.Label(new Rect(10, 30, 150, 25), "No vessel", TWR1LblStyle);
            }
            else
            {

                TWR1ContentColor = GUI.contentColor; //grab defaults
                TWR1DefautTextAlign = GUI.skin.label.alignment;//same^
                TWR1DefaultTextFieldAlign = GUI.skin.textField.alignment;//same^
                //GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                TWR1LblStyle.alignment = TextAnchor.MiddleLeft;

                GUI.Label(new Rect(10, 40, 150, 20), "Velocity Setpoint(m/s): ", TWR1LblStyle);
                GUI.Label(new Rect(10, 25, 150, 20), "Current Velocity(m/s): ", TWR1LblStyle);
                //GUI.skin.label.alignment = TextAnchor.MiddleRight;
                TWR1LblStyle.alignment = TextAnchor.MiddleRight;
                GUI.Label(new Rect(145, 25, 60, 20), curVsl.TWR1VelocityCurrent.ToString("##0.00"), TWR1LblStyle);
                //velocity setpoint value changes format depending on mode
                if (curVsl.TWR1HeightCtrl) //in height control mode, display "Auto"
                {
                    GUI.Label(new Rect(145, 40, 60, 20), "Auto", TWR1LblStyle);
                }
                else if (curVsl.TWR1Engaged) //in velocity control, display velocity setpoint
                {
                    GUI.Label(new Rect(145, 40, 60, 20), curVsl.TWR1VelocitySetpoint.ToString("##0.00"), TWR1LblStyle);
                }
                else //mod off, display "---.--"
                {
                    GUI.Label(new Rect(145, 40, 60, 20), "---.--", TWR1LblStyle);
                }
                if (GUI.Button(new Rect(7, 65, 50, 40), "Off", TWR1BtnStyle)) //button to turn mod off
                {
                    curVsl.TWR1Engaged = false;
                    curVsl.TWR1HeightCtrl = false;

                }
                if (GUI.Button(new Rect(57, 65, 50, 40), "Zero\nVel.", TWR1BtnStyle)) //button to zero velocity
                {
                    curVsl.TWR1Engaged = true;
                    curVsl.TWR1HeightCtrl = false;
                    curVsl.TWR1VelocitySetpoint = 0f;
                }
                if (GUI.Button(new Rect(107, 65, 50, 40), "+" + TWR1SpeedStep, TWR1BtnStyle)) //button to increase velocity, display value of change (SpeedStep)
                {
                    if (curVsl.TWR1Engaged) //if mod is engaged already, add speedstep to velocity setpoint
                    {
                        curVsl.TWR1HeightCtrl = false;
                        curVsl.TWR1VelocitySetpoint += TWR1SpeedStep;
                    }
                    else //if mod is not engaged, add speedstep to current velocity and make that the velocity setpoint
                    {
                        curVsl.TWR1Engaged = true;
                        curVsl.TWR1HeightCtrl = false;
                        curVsl.TWR1VelocitySetpoint = curVsl.TWR1VelocityCurrent + TWR1SpeedStep;
                    }
                }

                if (GUI.Button(new Rect(157, 65, 50, 40), "-" + TWR1SpeedStep, TWR1BtnStyle))//button to decrease velocity, display value of change (SpeedStep)
                {
                    if (curVsl.TWR1Engaged)//if mod is engaged already, subtract speedstep from velocity setpoint
                    {
                        curVsl.TWR1HeightCtrl = false;
                        curVsl.TWR1VelocitySetpoint -= TWR1SpeedStep;
                    }
                    else//if mod is not engaged, subtract speedstep from current velocity and make that the velocity setpoint
                    {
                        curVsl.TWR1Engaged = true;
                        curVsl.TWR1HeightCtrl = false;
                        curVsl.TWR1VelocitySetpoint = curVsl.TWR1VelocityCurrent - TWR1SpeedStep;
                    }
                }

                //height control button
                if (curVsl.TWR1HC1Thrust >= 0f) //1% thrust is a TWR of greater then 1, probably SRBs. Can not enable Height Control
                {
                    GUI.contentColor = Color.red;
                    //GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                    TWR1LblStyle.alignment = TextAnchor.MiddleCenter;
                    GUI.Label(new Rect(107, 110, 100, 40), "TWR\nHIGH", TWR1LblStyle);
                    GUI.contentColor = TWR1ContentColor;
                }
                else if (curVsl.TWR1HC80Thrust <= 0f) //80% thrust is not a TWR of greater then 1. Can not enable Height Control
                {
                    GUI.contentColor = Color.red;
                    //GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                    TWR1LblStyle.alignment = TextAnchor.MiddleCenter;
                    GUI.Label(new Rect(107, 110, 100, 40), "TWR\nLOW", TWR1LblStyle);
                    GUI.contentColor = TWR1ContentColor;
                }
                else if (curVsl.TWR1VesselPitch <= 55f && curVsl.TWR1HCOrbitDrop == false && curVsl.TWR1Engaged == true) //vessel is angled a long way off vertical, warn player mod may not work
                {
                    GUI.contentColor = Color.yellow;
                    //GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                    TWR1LblStyle.alignment = TextAnchor.MiddleCenter;
                    GUI.Label(new Rect(107, 110, 100, 40), "OVER\nPITCH", TWR1LblStyle);
                    GUI.contentColor = TWR1ContentColor;
                }
                else if (curVsl.TWR1HeightCtrl) //are we in height control mode?
                {
                    if (curVsl.TWR1HCOrbitDrop == true) //orbit drop in progress
                    {
                        if (curVsl.TWR1HCThrustWarningTime == 0) //have not hit ThrustWarning altitude yet
                        {
                            GUI.contentColor = Color.green;
                            //GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                            TWR1LblStyle.alignment = TextAnchor.MiddleCenter;
                            GUI.Label(new Rect(107, 110, 100, 40), "Orbit Drop\nControls Free", TWR1LblStyle);
                            GUI.contentColor = TWR1ContentColor;
                        }
                        else if (curVsl.TWR1HCThrustWarningTime != 0) //hit ThrustWarning altitude, warn player they have to upright their ship
                        {
                            GUI.contentColor = Color.yellow;
                            //GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                            TWR1LblStyle.alignment = TextAnchor.MiddleCenter;
                            GUI.Label(new Rect(107, 110, 100, 40), "THRUST\nWARNING", TWR1LblStyle);
                            GUI.contentColor = TWR1ContentColor;
                        }
                    }
                    else //orbit drop is not in progress, height control mode engaged
                    {
                        GUI.contentColor = Color.green;
                        if (GUI.Button(new Rect(107, 110, 100, 40), "In\nAuto", TWR1BtnStyle))
                        {
                            curVsl.TWR1HeightCtrl = false;
                            curVsl.TWR1VelocitySetpoint = 0f;
                        }
                        GUI.contentColor = TWR1ContentColor;
                    }


                }

                else if (!curVsl.TWR1HeightCtrl) //not in height control mode
                {
                    if (curVsl.TWR1OrbitDropAllow == true) //can enter orbitdrop mode
                    {
                        if (GUI.Button(new Rect(107, 110, 100, 40), "Enter\nOrbit Drop", TWR1BtnStyle))
                        {
                            curVsl.TWR1HeightCtrl = true;
                            curVsl.TWR1Engaged = true;
                            curVsl.TWR1HCOrbitDrop = true;
                            curVsl.TWR1HCThrustWarningTime = 0;

                        }
                    }
                    else //too low for orbit drop, but can enter normal height control mode
                    {
                        if (GUI.Button(new Rect(107, 110, 100, 40), "Auto Height", TWR1BtnStyle))
                        {
                            curVsl.TWR1HeightCtrl = true;
                            curVsl.TWR1Engaged = true;
                            curVsl.TWR1HCOrbitDrop = false;
                        }
                    }
                }


                //GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                TWR1LblStyle.alignment = TextAnchor.MiddleLeft;
                if (curVsl.vessel.Landed)
                {
                    Color txtClr = TWR1LblStyle.normal.textColor;
                    TWR1LblStyle.normal.textColor = Color.green;
                    GUI.Label(new Rect(7, 110, 50, 20), "LANDED", TWR1LblStyle);
                    TWR1LblStyle.normal.textColor = txtClr;
                }
                else
                {
                    GUI.Label(new Rect(7, 110, 50, 20), "Altitude:", TWR1LblStyle);
                }
                //GUI.skin.label.alignment = TextAnchor.MiddleRight;
                TWR1LblStyle.alignment = TextAnchor.MiddleRight;
                if (curVsl.TWR1HCToGround > 50000) //are we really high? disaply orbit due to character limit concerns
                {
                    GUI.Label(new Rect(47, 110, 49, 20), "Orbit", TWR1LblStyle);
                }
                else
                {
                    GUI.Label(new Rect(47, 110, 49, 20), curVsl.TWR1HCToGround.ToString("#####0"), TWR1LblStyle);
                }
                //GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                TWR1LblStyle.alignment = TextAnchor.MiddleLeft;
                GUI.Label(new Rect(7, 130, 40, 20), "Fly to:", TWR1LblStyle); //fly to altitude, GUI text box requires string, not number
                curVsl.TWR1HCTargetString = curVsl.TWR1HCTarget.ToString();//same^
                GUI.skin.label.alignment = TextAnchor.MiddleRight;
                //GUI.skin.textField.alignment = TextAnchor.MiddleRight;
                TWR1FldStyle.alignment = TextAnchor.MiddleRight;
                GUI.SetNextControlName("TWR1TargetHeight");
                curVsl.TWR1HCTargetString = GUI.TextField(new Rect(47, 130, 50, 20), curVsl.TWR1HCTargetString, 5, TWR1FldStyle);//same^
                try//same^
                {
                    curVsl.TWR1HCTarget = Convert.ToInt32(curVsl.TWR1HCTargetString); //convert string to number
                }
                catch//same^
                {
                    curVsl.TWR1HCTargetString = curVsl.TWR1HCTarget.ToString(); //conversion failed, reset change
                    GUI.FocusControl(""); //non-number key pressed, return control focus to vessel
                }

                //bottom button displaying mode mod is in
                if (!TWR1VesselActive)
                {
                    if (GUI.Button(new Rect(10, 153, 175, 25), "No Active Vessel", TWR1BtnStyle))
                    {
                        curVsl.TWR1HeightCtrl = false;
                        curVsl.TWR1Engaged = false;
                        TWR1Show = false;
                    }
                }

                else if (curVsl.TWR1HeightCtrl)
                {
                    TWR1BtnStyle.fontStyle = FontStyle.Bold;
                    GUI.contentColor = Color.green;
                    if (GUI.Button(new Rect(10, 153, 175, 25), "Height Control", TWR1BtnStyle))
                    {
                        curVsl.TWR1HeightCtrl = false;
                    }
                    GUI.contentColor = TWR1ContentColor;
                    TWR1BtnStyle.fontStyle = FontStyle.Normal;
                }
                else if (curVsl.TWR1Engaged)
                {
                    TWR1BtnStyle.fontStyle = FontStyle.Bold;
                    GUI.contentColor = Color.green;
                    if (GUI.Button(new Rect(10, 153, 175, 25), "Velocity Control", TWR1BtnStyle))
                    {
                        curVsl.TWR1HeightCtrl = false;
                        curVsl.TWR1Engaged = false;
                    }
                    GUI.contentColor = TWR1ContentColor;
                    TWR1BtnStyle.fontStyle = FontStyle.Normal;
                }
                else //if (!TWR1HCArmed)
                {
                    TWR1BtnStyle.fontStyle = FontStyle.Bold;
                    if (GUI.Button(new Rect(10, 153, 175, 25), "Control Off", TWR1BtnStyle)) //text displayed changes if mod is out of date
                    {
                        curVsl.TWR1Engaged = false;
                        TWR1Show = false;
                    }
                    TWR1BtnStyle.fontStyle = FontStyle.Normal;

                }

                if (GUI.Button(new Rect(185, 153, 25, 25), TWR1SettingsIcon, TWR1BtnStyle)) //settings button
                {
                    TWR1SettingsShow = !TWR1SettingsShow;
                    TWR1SettingsWin.x = TWR1WinPos.x + 218;
                    TWR1SettingsWin.y = TWR1WinPos.y;
                }

                //GUI.skin.textField.alignment = TWR1DefaultTextFieldAlign; //reset text defaults
                TWR1FldStyle.alignment = TWR1DefaultTextFieldAlign;
                //GUI.skin.label.alignment = TWR1DefautTextAlign; //same^
                TWR1LblStyle.alignment = TWR1DefautTextAlign;
                GUI.contentColor = TWR1ContentColor;//same^
                GUI.DragWindow(); //window is draggable
                GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            }
            //print(Time.time + " control " + GUI.GetNameOfFocusedControl());
            if (GUI.GetNameOfFocusedControl() == "TWR1TargetHeight" && !lockSetHeight)
            {
                ControlLockCalls.SetControlLock("TWR1HeightSetPoint");
                lockSetHeight = true;
            }
            else if (GUI.GetNameOfFocusedControl() != "TWR1TargetHeight" && lockSetHeight)
            {
                ControlLockCalls.ReleaseControlLock("TWR1HeightSetPoint");
                lockSetHeight = false;//lockSetStep
            }
        }

        public bool TWR1DataPresent(Vessel vsl)
        {
            foreach(Part p in vsl.Parts)
            {
                if(p.Modules.Contains("TWR1Data"))
                {
                    return true;
                }
            }
            return false;
        }

        public void Update()
        {
            try
            {
                try
                {
                    //if(curVsl != null)
                    //{
                    //    if (!FlightGlobals.ActiveVessel.parts.Contains(curVsl.part))
                    //    {
                    //        Debug.Log("part not contained");
                    //    }
                    //}
                     

                    if(!TWR1DataPresent(FlightGlobals.ActiveVessel) && curVsl != null)
                    {
                        curVsl = null;
                    }

                    if (curVsl == null  && TWR1DataPresent(FlightGlobals.ActiveVessel) || curVsl.vessel.rootPart != FlightGlobals.ActiveVessel.rootPart || !FlightGlobals.ActiveVessel.parts.Contains(curVsl.part) || !curVsl.masterModule)
                    {
                        List<TWR1Data> dataModules = new List<TWR1Data>();
                        foreach (Part p in FlightGlobals.ActiveVessel.parts)
                        {
                            //foreach (TWR1Data td in p.Modules.OfType<TWR1Data>())
                            //{
                                dataModules.AddRange(p.Modules.OfType<TWR1Data>());
                            //}
                        }
                        if (dataModules.Count == 0)
                        {
                            curVsl = null;
                        }
                        else if(dataModules.Where(pm => pm.masterModule == true).Count() > 0)
                        {
                            curVsl = dataModules.Where(pm => pm.masterModule == true).First();
                        }
                        else
                        {
                            curVsl = dataModules.First();
                        }
                        foreach(TWR1Data tdata in dataModules)
                        {
                            if(tdata == curVsl) //make sure our master is set
                            {
                                curVsl.masterModule = true;
                            }
                            else //all other modules are ignored
                            {
                                tdata.masterModule = false;
                                tdata.TWR1Engaged = false;;
                            }
                        }
                    }
                    //foreach(Part p in FlightGlobals.ActiveVessel.parts.Where(p2 => p2.Modules.Contains("TWR1Data")))
                    //{
                    //    Debug.Log("MM " + p.Modules["TWR1Data"].Fields.GetValue("masterModule") + " " + p.flightID + " " + p.Modules.OfType<TWR1Data>().First().masterModule);
                    //}
                }
                catch
                {
                    //print("hit catch");
                    curVsl = null;
                }
            //partFound:
                //print(HighLogic.Skin.font);
                if (timerElapsed)
                {
                    HideLine();
                    timerElapsed = false;
                    timerRunning = false;
                }
                if (timerRunning)
                {
                    theLine.transform.parent = curVsl.vessel.rootPart.transform;
                    theLine.transform.localPosition = Vector3.zero;
                    theLine.transform.rotation = Quaternion.identity;
                    theLine.SetPosition(0, new Vector3(0, 0, 0));
                    theLine.SetPosition(1, curVsl.TWR1ControlUp * 50);
                }


                if (ExtendedInput.GetKeyDown(TWR1KeyCode) == true) //Does the Z key get pressed, enabling this mod? Note this is only true on the first Update cycle the key is pressed.
                {

                    InputLockManager.SetControlLock(ControlTypes.THROTTLE | ControlTypes.THROTTLE_CUT_MAX, "TWR1Throttle"); //set control lock so pressing throttle keys doesn't pulse the engine.
                    //InputLockManager.SetControlLock(ControlTypes.All, "TWR1Throttle");
                    TWR1Show = true;
                    if (curVsl.TWR1Engaged == false) //TWR1 not engaged when Z pressed so input our current velocity into velocity setpoint
                    {
                        curVsl.TWR1VelocitySetpoint = (float)curVsl.vessel.verticalSpeed;
                    }
                    TWR1KeyDown = true; //TWR1 key is down
                    curVsl.TWR1Engaged = true; //TWR1 Engaged, auto control on
                    //TWR1ThrottleWhileKeyDown = false; //Z key was just pressed so throttle key can not have been pressed yet

                    curVsl.TWR1HeightCtrl = false; //turn off height control if engaged
                    curVsl.TWR1HCOrbitDrop = false;
                }
                if (ExtendedInput.GetKeyUp(TWR1KeyCode) == true) //TWR1 Key just got released.
                {
                    InputLockManager.RemoveControlLock("TWR1Throttle"); //remove throttle control lock
                    TWR1KeyDown = false; //TWR1 key is no longer held down
                }

                if (curVsl != null)
                {
                    if (GameSettings.THROTTLE_UP.GetKeyDown() == true && curVsl.TWR1Engaged == true) //throtlle up key pressed, TWR1 engaged
                    {
                        if (curVsl.TWR1HCOrbitDrop) //orbit drop in progress
                        {
                            //do nothing, allow KSP to control throttle
                        }
                        else if (TWR1KeyDown == false) //Z key not down, disengage TWR1
                        {

                            curVsl.TWR1Engaged = false; //disengage TWR1 

                        }

                        else if (TWR1KeyDown == true) //Z key down, increse desired accel by 1 m/s^2
                        {
                            curVsl.TWR1VelocitySetpoint += TWR1SpeedStep;


                            //TWR1ThrottleWhileKeyDown = true; //throttle key pressed while TWR1 keydown
                            curVsl.TWR1HeightCtrl = false;
                        }
                    }
                    if (GameSettings.THROTTLE_DOWN.GetKeyDown() == true && curVsl.TWR1Engaged == true) //throtlle down key pressed, TWR1 engaged
                    {
                        if (curVsl.TWR1HCOrbitDrop) //orbit drop in progress
                        {
                            //do nothing, allow KSP to control throttle
                        }
                        else if (TWR1KeyDown == false) ////Z key not down, disengage TWR1
                        {

                            curVsl.TWR1Engaged = false; //disengage TWR1

                        }
                        else if (TWR1KeyDown == true)//Z key down, decrease desired accel by 1 m/s^2
                        {
                            curVsl.TWR1VelocitySetpoint -= TWR1SpeedStep;

                            //TWR1ThrottleWhileKeyDown = true; //throttle key pressed while TWR1 keydown
                            curVsl.TWR1HeightCtrl = false;

                        }
                    }
                    if (GameSettings.THROTTLE_CUTOFF.GetKeyDown() == true && curVsl.TWR1Engaged == true)
                    {
                        if (curVsl.TWR1HCOrbitDrop) //height control engaged and doing orbit drop
                        {
                            //do nothing, allow KSP to control throttle
                        }

                        else if (TWR1KeyDown == false) ////Z key not down, disengage TWR1
                        {
                            curVsl.TWR1Engaged = false; //disengage TWR1
                            FlightInputHandler.state.mainThrottle = 0f; //throttle cut off hit, player will not necessarily hold it down like they will throttle up/down so zero the throttle

                        }
                        else if (TWR1KeyDown == true)//Z key down, set vert vel to 0
                        {
                            curVsl.TWR1VelocitySetpoint = 0f;
                            //TWR1ThrottleWhileKeyDown = true; //throttle key pressed while TWR1 keydown
                            curVsl.TWR1HeightCtrl = false;

                        }

                    }
                }
                if(mouseOverWindow)
                {
                    if (lastTarget==null)
                    {
                        lastTarget = FlightGlobals.fetch.VesselTarget;
                    }
                    if(lastTarget != FlightGlobals.fetch.VesselTarget)
                    {
                        FlightGlobals.fetch.SetVesselTarget(lastTarget);
                    }
                }
                else
                {
                    lastTarget = null;
                }

                //Debug.Log(curVsl.TWR1Vessel.rootPart.transform.position + "||" + curVsl.);

            }
            catch(Exception e)
            {
                Debug.Log("TWR1 Update Fail! " + e);
            }
        }
    }
}
