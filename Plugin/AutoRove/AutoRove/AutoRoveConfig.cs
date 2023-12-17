using KSP;
using UnityEngine;
using System;

namespace AutoRove
{
    internal static class autoRoveConfig
    {
        private static string configName = "autoRoveConfig";
        private static double defaultGravityScaleFactor = 1;
        private static int defaultFramesPerUpdate = 60;

        /// <summary>
        /// the configNode from the config file
        /// </summary>
        private static ConfigNode loadedNode;
        private static ConfigNode node
        {
            get
            {
                if (loadedNode == null)
                {
                    load();
                    return loadedNode;
                }
                else
                {
                    return loadedNode;
                }
            }
        }
        
        /// <summary>
        /// factor determining how much the gravity difference matters
        /// </summary>
        internal static double gravityScaleFactor
        {
            get
            {
                if (node == null)
                {
                    load();
                }
                double gsf = Convert.ToDouble(node.GetValue("gravityScaleFactor"));
                if (gsf >= 0 && gsf <= 1)
                {
                    return gsf;
                }
                else
                {
                    autoRoveUtils.debugWarning("gravityScaleFactor not found defaulting to " + defaultGravityScaleFactor);
                    return defaultGravityScaleFactor;
                }
            }
        }

        /// <summary>
        /// the update rate for the main behaviour
        /// </summary>
        internal static int framesPerUpdate
        {
            get
            {
                if (node == null)
                {
                    load();
                }
                int fpu = fpu = Convert.ToInt16(node.GetValue("framesPerUpdate"));
                if (fpu > 0)
                {
                    return fpu;
                }
                else
                {
                    autoRoveUtils.debugWarning("framesPerUpdate not found defaulting to " + defaultFramesPerUpdate);
                    return defaultFramesPerUpdate;
                }
            }
        }



        /// <summary>
        /// parses the the config file into the data structure
        /// </summary>
        internal static void load()
        {
            autoRoveUtils.debugMessage("loading AutoRove config");
            foreach (UrlDir.UrlFile item in GameDatabase.Instance.root.AllConfigFiles)
            {
                if (item.name == configName)
                {
                    loadedNode = item.GetConfig("AUTOROVE").config;
                }
            }
        }

    }
}
