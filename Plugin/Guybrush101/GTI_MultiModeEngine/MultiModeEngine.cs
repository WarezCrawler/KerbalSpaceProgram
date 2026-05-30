using static GTI.GTIConfig;
using static GTI.Utilities;
using static GTI.PhysicsUtilities;

using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace GTI
{
    /// <summary>
    /// Rich per-mode data for an engine: propellants, ratios, thrust and the various KSP curves.
    /// This is a fuller alternative to the plain MultiMode type. It is NOT currently wired into the
    /// active GTI_MultiModeEngineFX (which uses MultiMode); it is kept for a future engine module that
    /// also rewrites propellants/curves/thrust per mode. Most setters take raw cfg strings and parse them.
    /// </summary>
    public class GTI_MultiModeEngine : IMultiMode
    {
        public int moduleIndex { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }

        // Comma-separated propellant names. The setter also caches the split array for fast access.
        private string _propellants;
        private string[] _propellantsArray;
        public string propellants
        {
            get => _propellants;
            set
            {
                _propellants = value;
                _propellantsArray = value.Split(',');
            }
        }
        public string[] GetPropellants => _propellantsArray;

        // Comma-separated propellant ratios, parallel to propellants.
        private string _propRatios;
        private string[] _propRatiosArray;
        public string propRatios
        {
            get => _propRatios;
            set
            {
                _propRatios = value;

                // Cache the split array, and warn early if any ratio isn't a number so cfg typos are caught here.
                _propRatiosArray = value.Split(',');
                try
                {
                    foreach (string item in _propRatiosArray)
                    {
                        if (!Single.TryParse(item, out float numvalue))
                            GTIDebug.LogWarning("CustomTypes.PropellantList -> Could not parse propellant ratio " + item + " into integer.", iDebugLevel.Low);
                    }
                }
                catch (Exception e) { GTIDebug.LogError("CustomTypes.PropellantList -> Could not parse propellant ratio into integer.\n" + value + "\nError trown:\n" + e); throw e; }
            }
        }
        public string[] GetPropellantRatios
        {
            get
            {
                return _propRatiosArray;
            }
        }


        public string propIgnoreForISP { get; set; }
        public string propDrawGauge { get; set; }

        // Resource flow mode. The setter whitelists the valid KSP values; anything else becomes empty (= use stock default).
        private string _resourceFlowMode;
        public string resourceFlowMode
        {
            get => _resourceFlowMode;
            set
            {
                switch (value)
                {
                    case "ALL_VESSEL":
                        _resourceFlowMode = value;
                        break;
                    case "ALL_VESSEL_BALANCE":
                        _resourceFlowMode = value;
                        break;
                    case "NO_FLOW":
                        _resourceFlowMode = value;
                        break;
                    case "NULL":
                        _resourceFlowMode = value;
                        break;
                    case "STACK_PRIORITY_SEARCH":
                        _resourceFlowMode = value;
                        break;
                    case "STAGE_PRIORITY_FLOW":
                        _resourceFlowMode = value;
                        break;
                    case "STAGE_PRIORITY_FLOW_BALANCE":
                        _resourceFlowMode = value;
                        break;
                    case "STAGE_STACK_FLOW":
                        _resourceFlowMode = value;
                        break;
                    case "STAGE_STACK_FLOW_BALANCE":
                        _resourceFlowMode = value;
                        break;
                    default:
                        _resourceFlowMode = string.Empty;
                        break;
                }
            }
        }

        // maxThrust is the real value; SetMaxThrust is a string-input helper for parsing it straight from the cfg.
        public float maxThrust { get; set; }
        public string SetMaxThrust
        {
            set
            {
                Single.TryParse(value, out float outMaxThrust);
                maxThrust = outMaxThrust;
            }
        }
        public string heatProduction { get; set; }
        public string engineType { get; set; }
        public string atmChangeFlow { get; set; }
        public string useEngineResponseTime { get; set; }
        public string engineAccelerationSpeed { get; set; }
        public string engineDecelerationSpeed { get; set; }

        // ISP-vs-atmosphere curve. Setting the ConfigNode also loads it into a ready-to-evaluate FloatCurve.
        private ConfigNode _atmosphereCurve = new ConfigNode();
        public FloatCurve atmosphereFloatCurve { get; private set; } = new FloatCurve();
        public ConfigNode atmosphereCurve
        {
            get => _atmosphereCurve;
            set
            {
                _atmosphereCurve = value;

                if (value != null)
                {
                    GTIDebug.Log("Before load of atmosphereFloatCurve", iDebugLevel.DebugInfo);
                    atmosphereFloatCurve.Load(value);
                }
            }
        }
        public string useVelCurve { get; set; }
        public ConfigNode velCurve { get; set; }
        public string useAtmCurve { get; set; }
        public ConfigNode atmCurve { get; set; }

        // Optional custom GTI throttle-vs-ISP curve. Setting the node loads the FloatCurve; a null node clears it.
        public bool useGTIthrottleISPCurve { get; set; } = false;
        private ConfigNode _GTIthrottleISPCurve = new ConfigNode();
        public FloatCurve GTIthrottleISPFloatCurve { get; private set; } = new FloatCurve();
        public ConfigNode GTIthrottleISPCurve
        {
            get => _GTIthrottleISPCurve;
            set
            {
                _GTIthrottleISPCurve = value;
                if (value != null)
                {
                    GTIDebug.Log("Before load of throttleISPFloatCurve", iDebugLevel.DebugInfo);
                    GTIthrottleISPFloatCurve.Load(value);
                }
                else
                {
                    GTIthrottleISPFloatCurve = null;
                }
            }
        }
        // String-input helper: parse the on/off flag from the cfg, defaulting to false on bad input.
        public string SetUseGTIthrottleISPCurve
        {
            set
            {
                bool result;
                if (bool.TryParse(value, out result)) useGTIthrottleISPCurve = result; else useGTIthrottleISPCurve = false;
            }
        }
    }
}
