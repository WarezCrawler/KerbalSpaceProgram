/*GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007*/
/*All credit goes to mic-e and linuxgurugamer*/
using UnityEngine;
using KSP.UI.Screens.Flight;
using KSP.IO;
using static GTI.Utilities;

namespace GTI
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class GTI_NavBallDockingAlignmentIndicator : MonoBehaviour
    {
        private NavBall navBall;

        private GameObject indicator;
        private Material indicatorMaterial;
        private PluginConfiguration cfg;
        private Color color;

        private void Start()
        {
            GTIDebug.Log("GTI_NavBallDockingAlignmentIndicator Starting", "GTI - DAI", GTIConfig.iDebugLevel.DebugInfo);
            if (!GTIConfig.NavBallDockingIndicator.Activate || PluginExists("NavBallDockingAlignmentIndicatorCE"))
            {
                if(PluginExists("NavBallDockingAlignmentIndicatorCE") && GTIConfig.NavBallDockingIndicator.Activate)
                    GTIDebug.Log("NavBallDockingAlignmentIndicatorCE Detected -- Disabling GTI Alignment indicator", "GTI - DAI", GTIConfig.iDebugLevel.DebugInfo);
                Destroy(this.gameObject);
                return;
            }

            GTIDebug.Log(" ======== AWAKE  ======== ", GTIConfig.iDebugLevel.Low);
            this.cfg = KSP.IO.PluginConfiguration.CreateForType<GTI_NavBallDockingAlignmentIndicator>();
            this.cfg.load();
            Vector3 tmp = cfg.GetValue<Vector3>("alignmentmarkercolor", new Vector3(1f, 0f, 0f)); // default: red
            this.color = new Color(tmp.x, tmp.y, tmp.z);
            this.cfg.save();
        }

        private void OnDestroy()
        {
            GTIDebug.Log("OnDestroy", "GTI - DAI", GTIConfig.iDebugLevel.DebugInfo);
            if (this.indicator != null)
                Destroy(this.indicator);
        }

        private void LateUpdate()
        {
            // Active is driven by the 250 ms background detector (BackgroundDetectors.cs);
            // it owns that flag, so we only read it here.
            if (!GTIConfig.NavBallDockingIndicator.Active)
            {
                HideIndicator();
                return;
            }

            if (this.navBall == null)
                this.navBall = FindObjectOfType<NavBall>();

            // Re-verify the target this frame: the detector only repolls every 250 ms,
            // so the target can disappear between polls and would otherwise NRE below.
            if (FlightGlobals.fetch != null
                && FlightGlobals.ready
                && FlightGlobals.fetch.activeVessel != null
                && FlightGlobals.fetch.VesselTarget != null
                && FlightGlobals.fetch.VesselTarget.GetTargetingMode() == VesselTargetModes.DirectionVelocityAndOrientation)
            {
                /// Targeted a Port if I am not mistaken o__o

                if (this.indicator == null)
                    SetupIndicator();

                #region "legacy" Code
                ITargetable targetPort = FlightGlobals.fetch.VesselTarget;
                Transform targetTransform = targetPort.GetTransform();
                Transform selfTransform = FlightGlobals.ActiveVessel.ReferenceTransform;

                // Position
                Vector3 targetPortOutVector = targetTransform.forward.normalized;
                Vector3 rotatedTargetPortInVector = navBall.attitudeGymbal * -targetPortOutVector;
                this.indicator.transform.localPosition = rotatedTargetPortInVector * navBall.progradeVector.localPosition.magnitude;

                // Rotation
                Vector3 v1 = Vector3.Cross(selfTransform.up, -targetTransform.up);
                Vector3 v2 = Vector3.Cross(selfTransform.up, selfTransform.forward);
                float ang = Vector3.Angle(v1, v2);
                if (Vector3.Dot(selfTransform.up, Vector3.Cross(v1, v2)) < 0)
                    ang = -ang;
                this.indicator.transform.rotation = Quaternion.Euler(90 + ang, 90, 270);
                #endregion

                // Set opacity
                float value = Vector3.Dot(indicator.transform.localPosition.normalized, Vector3.forward);
                value = Mathf.Clamp01(value);
                this.indicatorMaterial.SetFloat("_Opacity", value);

                this.indicator.SetActive(indicator.transform.localPosition.z > 0f);
            }
            else
            {
                HideIndicator();
            }
        }

        private void HideIndicator()
        {
            if (this.indicator != null)
                this.indicator.SetActive(false);
        }

        private void SetupIndicator()
        {
            this.indicator = GameObject.Instantiate(navBall.progradeVector.gameObject);
            this.indicator.transform.parent = navBall.progradeVector.parent;
            this.indicator.transform.position = navBall.progradeVector.position;
            // Cache the instanced material once. Reading Renderer.materials each frame
            // allocates a new array and instantiates material copies (GC churn + leak).
            this.indicatorMaterial = this.indicator.GetComponent<MeshRenderer>().materials[0];
            this.indicatorMaterial.SetColor("_TintColor", this.color);
        }
    }
}