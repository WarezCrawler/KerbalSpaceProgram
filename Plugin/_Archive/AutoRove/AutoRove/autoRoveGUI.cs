using System;
using System.Collections.Generic;
using UnityEngine;
using KSP.UI.Screens;

namespace AutoRove
{

    internal class autoRoveGUI : MonoBehaviour
    {


        /// <summary>
        /// a window for readouts
        /// </summary>
        internal class DebugWindow : MonoBehaviour
        {
            private Rect mainWindowRect = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 410, 100);
            private Rect closeButtonRect = new Rect(410 - 16, 3, 13, 12);
            private string windowTitle = "";
            internal Vector3d value = new Vector3d();

            internal void Init(string windowName)
            {
                name = windowName;
            } 

            private void mainWindow(int windowID)
            {
                if (GUI.Button(closeButtonRect, ""))
                {
                    Destroy(this);
                }

                GUILayout.BeginVertical();
                string displayX = "X: " + value.x.ToString();
                string displayY = "Y: " + value.y.ToString();
                string displayZ = "Z: " + value.z.ToString();
                GUILayout.Label(displayX);
                GUILayout.Label(displayY);
                GUILayout.Label(displayZ);
                GUILayout.EndVertical();
                GUI.DragWindow();
            }

            void OnGUI()
            {
                mainWindowRect = GUI.Window(0, mainWindowRect, mainWindow, windowTitle);
            }
        }

        /// <summary>
        /// the window that pops up when the Applauncher butten is pressed
        /// </summary>
        internal class AppButtonWindow : MonoBehaviour
        {
            private Rect mainWindowRect = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 410, 200);
            private Rect closeButtonRect = new Rect(410 - 16, 3, 13, 12);
            private Vector2 scrollPosition = new Vector2(400, 100);

            /// <summary>
            /// the list of items displayed in the window.
            /// every line consits of, in order: string name, string body, string target, string distance
            /// </summary>
            internal List<string[]> items = new List<string[]>();

            /// <summary>
            /// function that gets executed before the window closes
            /// </summary>
            internal Action closeFunction;
            //internal Action clickFunction;


            internal void Init(Action close, Action click)
            {
                closeFunction = close;
                //clickFunction = click;
            }

            /// <summary>
            /// destroys the window and executs the closeFunction
            /// </summary>
            internal void close()
            {
                if (closeFunction != null)
                {
                    closeFunction.Invoke();
                }
                Destroy(this);
            }

            /// <summary>
            /// adding a line to the appLauncher window
            /// </summary>
            /// <param name="name"></param>
            /// <param name="body"></param>
            /// <param name="target"></param>
            /// <param name="distance"></param>
            internal void addItem(string name, string body, string target, string distance)
            {
                string[] item = new string[4] { name, body, target, distance };
                items.Add(item);
            }

            /// <summary>
            /// deletes all entries in the window, leaving an empty window
            /// </summary>
            internal void clean()
            {
                items.Clear();
            }

            private void mainWindow(int windowID)
            {
                GUILayoutOption[] textFieldOptions = { GUILayout.Width(35) };
                GUILayoutOption[] labelOptions = { GUILayout.Width(88) };

                if (GUI.Button(closeButtonRect, ""))
                {
                    this.close();
                }

                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name", labelOptions);
                GUILayout.Label("Body", labelOptions);
                GUILayout.Label("Target", labelOptions);
                GUILayout.Label("Distance", labelOptions);
                GUILayout.EndHorizontal();
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
                foreach (string[] item in items)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(item[0], labelOptions);
                    GUILayout.Label(item[1], labelOptions);
                    GUILayout.Label(item[2], labelOptions);
                    GUILayout.Label(item[3], labelOptions);
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();

                GUI.DragWindow();
            }

            void OnGUI()
            {
                mainWindowRect = GUI.Window(0, mainWindowRect, mainWindow, "AutoRover List");
            }

        }



        /// <summary>
        /// the PopUp window fot the target coordinates
        /// </summary>
        internal class targetSelectionWindow : autoRoveGUI
        {
            private Rect finishedWindowRect = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 165, 100);
            private Rect closeButtonRect = new Rect(165 - 16, 3, 13, 12);
            private string latitude = "";
            private string longitude = "";
            private bool startPressed = false;
            private double latInput = 0;
            private double lonInput = 0;

            internal bool startButton(out double lat, out double lon)
            {
                if (startPressed)
                {
                    lat = latInput;
                    lon = lonInput;
                    return true;
                }
                else
                {
                    lat = 0;
                    lon = 0;
                    return false;
                }
            }

            internal bool close()
            {
                Destroy(this);
                return true;
            }

            private void finishedWindow(int windowID)
            {
                GUILayoutOption[] textFieldOptions = { GUILayout.Width(50) };
                GUILayoutOption[] labelOptions = { GUILayout.Width(70) };

                if (GUI.Button(closeButtonRect, ""))
                {
                    this.close();
                }
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Latitude: ", labelOptions);
                latitude = GUILayout.TextField(latitude, 10, textFieldOptions);
                GUILayout.Label("°N");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Longitude: ", labelOptions);
                longitude = GUILayout.TextField(longitude, 10, textFieldOptions);
                GUILayout.Label("°E");
                GUILayout.EndHorizontal();
                if (GUILayout.Button("Start AutoRove!", GUILayout.ExpandWidth(true)))
                {
                    // start methode exucution here
                    if (Double.TryParse(latitude, out latInput) && Double.TryParse(longitude, out lonInput))
                    {
                        startPressed = true;
                    }
                    else
                    {
                        autoRoveUtils.debugError("could not convert input!");
                        ScreenMessages.PostScreenMessage("AutoRove: could not convert input!", 5, ScreenMessageStyle.UPPER_CENTER);
                    }
                }
                GUILayout.EndVertical();
                GUI.DragWindow();
            }

            void OnGUI()
            {
                finishedWindowRect = GUI.Window(0, finishedWindowRect, finishedWindow, "AutoRove Target");
            }
        }
    }
}
