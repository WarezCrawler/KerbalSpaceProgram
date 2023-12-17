using System;

namespace AutoRove
{
    /// <summary>
    /// the module attached to a rover part to enable AutoRove behavier
    /// </summary>
    public class AutoRoveModule : PartModule
    {
        //private autoRoveConfig config = autoRoveConfig.load();
        private double gravityScaleFactor = autoRoveConfig.gravityScaleFactor;
        private autoRoveGUI.targetSelectionWindow popUp;

        [KSPField(isPersistant = true)]
        public double maxCharge;

        [KSPField(isPersistant = true)]
        public double generatorCharge;

        [KSPField(isPersistant = true)]
        public double solarCharge;

        [KSPField(isPersistant = true)]
        public double lastUpdate;

        [KSPField(isPersistant = true)]
        public bool isAutoRoveOn;
        
        [KSPField(isPersistant = true)]
        public double targetLatitude;

        [KSPField(isPersistant = true)]
        public double targetLongitude;

        [KSPField(isPersistant = true)]
        public double maxSpeed;

        [KSPField(isPersistant = true, guiName = "Speed", guiUnits = "m/s", guiActive = false)]
        public double speedDisplay;

        [KSPField(guiName = "Target", guiActive = false, isPersistant = true)]
        public string target;

        [KSPEvent(guiActive = true, guiName = "start AutoRove")]
        public void autoRoveButton()
        {
            // turn off
            if (isAutoRoveOn == true)
            {
                isAutoRoveOn = false;
                Events["autoRoveButton"].guiName = "stop AutoRove";        //Fix 08-05-2016
            }
            // turn on
            else if (!isAutoRoveOn)
            {
                // creating the PopUp window
                popUp = gameObject.AddComponent<AutoRove.autoRoveGUI.targetSelectionWindow>();
            }
            else
            {
                autoRoveUtils.debugError("couldn't toggle AutoRove");
            }
        }

        private void turnOnAutoRove(double latitude, double longitude)
        {
            double speed;
            double charge;
            double gCharge;
            double sCharge;
            double gravityFactor = this.vessel.mainBody.GeeASL / FlightGlobals.GetHomeBody().GeeASL;
            if (vessel.situation != Vessel.Situations.LANDED)
            {
                ScreenMessages.PostScreenMessage("AutoRove functions only for landed Rover!", 5, ScreenMessageStyle.UPPER_CENTER);
            }
            if (this.wheelCheck(out speed, out charge))
            {
                isAutoRoveOn = true;
                Events["autoRoveButton"].guiName = "stop AutoRove";
                Fields["target"].guiActive = true;
                Fields["speedDisplay"].guiActive = true;
                targetLatitude = latitude;
                targetLongitude = longitude;
                target = targetLatitude + "°N; " + targetLongitude + "°E";
                lastUpdate = Planetarium.GetUniversalTime();

                // apply gravity limitations if needed
                if (gravityFactor < 1 && gravityScaleFactor > 0 && gravityScaleFactor < 0)
                {
                    double deltaSpeed = (speed - speed * (double)gravityFactor) * gravityScaleFactor;
                    maxSpeed = speed - deltaSpeed;
                    speedDisplay = Math.Round(maxSpeed, 2);
                    maxCharge = charge * maxSpeed / speed;
                }
                else
                {
                    maxSpeed = speed;
                    speedDisplay = Math.Round(maxSpeed, 2);
                    maxCharge = charge;
                }

                // get all energy values
                if (this.generatorCheck(out gCharge)) { generatorCharge = gCharge; }
                if(this.solarCheck(out sCharge)) { solarCharge = sCharge; }
            }
            else
            {
                ScreenMessages.PostScreenMessage("AutoRove: no working wheels found!", 5, ScreenMessageStyle.UPPER_CENTER);
            }
        }

        /// <summary>
        /// retruns true if the rover has working wheels
        /// </summary>
        /// <param name="speed"> the max speed the rover can reach </param>
        /// <param name="charge"> the maxiumum charge the rover would consum for driving </param>
        /// <returns></returns>
        private bool wheelCheck(out double speed, out double charge)
        {
            charge = 0;
            speed = 0;

            foreach (Part part in this.vessel.Parts)
            {

                //ModuleWheel wheel = part.FindModuleImplementing<ModuleWheel>();
                ModuleWheels.ModuleWheelMotor wheel = part.FindModuleImplementing<ModuleWheels.ModuleWheelMotor>();
                if (wheel != null && wheel.motorEnabled)                // && wheel.inputResource == "ElectricCharge")
                {
                    //autoRoveUtils.debugWarning(String.Format("Wheel: {0}, speed: {1}, Charge: {2}", wheel, wheel.overSpeedDamage, wheel.resourceConsumptionRate));
                    //charge += wheel.resourceConsumptionRate;

                    // choose the smallest possible speed
                    //if (speed == 0 || speed > wheel.overSpeedDamage)
                    speed = 10f;     //Hardcode low speed for now -- WarezCrawler
                    charge = 1f;    //Hardcode charge for now -- WarezCrawler
                    //{
                    //    speed = wheel.overSpeedDamage;
                    //}
                }
            }
            //if (charge > 0 && speed > 0)
            //{
            //    autoRoveUtils.debugMessage("WheelData: " + speed + "m/s at " + charge + "ec/s");
                return true;
            //}
            //return false;
        }

        //possibly needs more checks for active generators
        /// <summary>
        /// checks if the vessel has electric generators that can produce charge
        /// </summary>
        /// <param name="generatorCharge"> the maximum chargerate for the vessel </param>
        /// <returns></returns>
        private bool generatorCheck(out double generatorCharge)
        {
            //********************* Commented out because of bug
            //double charge = 0;
            //foreach (Part part in this.vessel.Parts)
            //{
            //    ModuleGenerator generator = part.FindModuleImplementing<ModuleGenerator>();
            //    if (generator != null && generator.outputList.Exists(resource => resource.name == "ElectricCharge"))
            //    {
            //        //ModuleGenerator.GeneratorResource gr =  generator.outputList.Find(resource => resource.name == "ElectricCharge");
            //       ModuleResource gr = generator.outputList.Find(resource => resource.name == "ElectricCharge");
            //        autoRoveUtils.debugMessage(String.Format("found Generator: {0}, rate: {1}, Charge: {2}", generator, gr.rate, gr.name));
            //        charge += gr.rate;
            //    }
            //    if (charge > 0)
            //    {
            //        generatorCharge = charge;
            //        return true;
            //    }
            //}
            //generatorCharge = charge;
            //return false;
            generatorCharge = 100;
            return true;
        }

        /// <summary>
        /// checks the vessel for all solar panels and summs up the maxiumum charge
        /// </summary>
        /// <param name="solarCharge"> maximum possible charge befor celestial body power factor </param>
        /// <returns> true if vessel implements ModuleDeployableSolarPanel </returns>
        private bool solarCheck(out double solarCharge)
        {
        //    //********************* Commented out because of bug
        //    double sCharge = 0;
        //    foreach (Part part in this.vessel.Parts)
        //    {
        //        ModuleDeployableSolarPanel panel = part.FindModuleImplementing<ModuleDeployableSolarPanel>();
        //        if (panel != null && panel.panelState == ModuleDeployableSolarPanel.panelStates.EXTENDED)
        //        {
        //            sCharge += panel.chargeRate;
        //        }
        //        if (sCharge > 0)
        //        {
        //            solarCharge = sCharge;
        //            return true;
        //        }
        //    }
        //    solarCharge = sCharge;
        //    return false;
            solarCharge = 10;
            return true;
        }

        public override void OnLoad(ConfigNode node)
        {
            autoRoveUtils.debugMessage("Doing OnLoad...");
            // in case autoRove was turned off change all stats accordingly
            if (isAutoRoveOn == false)
            {
                //if (this.Events["autoRoveButton"].guiName == "stop AutoRove")
                //{
                    this.Events["autoRoveButton"].guiName = "start AutoRove";
                //}
                //if (Fields["target"].guiActive == true)
                //{
                    Fields["target"].guiActive = false;
                //}
                //if (Fields["speedDisplay"].guiActive == true)
                //{
                    Fields["speedDisplay"].guiActive = false;
                //}
                //if (target != "")
                //{
                    target = "";
                //}
                //if (maxSpeed != 0)
                //{
                    maxSpeed = 0;
                //}
                //if (lastUpdate != 0)
                //{
                    lastUpdate = 0;
                //}
            }

            // sometimes the UI disapears and we need to turn it back on
            if (isAutoRoveOn == true)
            {
                //if (Fields["target"].guiActive == false)
                //{
                    Fields["target"].guiActive = true;
                //}
                //if (Fields["speedDisplay"].guiActive == false)
                //{
                    Fields["speedDisplay"].guiActive = true;
                //}
            }
            //base.OnLoad(node);
        }

        void OnGUI()
        {
            double inputLatitude = 0;
            double inputLongitude = 0;
            if (this.isAutoRoveOn == false && popUp != null && popUp.startButton(out inputLatitude, out inputLongitude))
            {
                this.turnOnAutoRove(autoRoveUtils.sanitizeDegrees(inputLatitude), autoRoveUtils.sanitizeDegrees(inputLongitude));
                popUp.close();
            }
        }

    }
}
