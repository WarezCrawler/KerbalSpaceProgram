using System;

namespace AutoRove
{
    /// <summary>
    /// the data construct to represent a rover implementing autoRove
    /// </summary>
    internal class Rover
    {
        // the name of the module we are looking for
        private string myModuleName = "AutoRoveModule";
        private ConfigNode autoRoveModule;
        private ConfigNode protoVesselConfigNode = new ConfigNode();
        private Vessel roverVessel;
        internal Vessel vessel
        {
            get { return roverVessel; }
        }

        // the names of the module values as displayed in the ConfigNote
        private string toggleAutoRove = "isAutoRoveOn";
        private string speed = "maxSpeed";
        private string targetLat = "targetLatitude";
        private string targetLong = "targetLongitude";
        private string updateTime = "lastUpdate";
        private string maxCharge = "maxCharge";
        private string solarCharge = "solarCharge";
        private string generatorCharge = "generatorCharge";
        private string targetString = "target";

        // atributes of the rover as set in AutoRoveModule
        internal Guid vesselID
        {
            get { return roverVessel.id; }
        }

        /// <summary>
        /// the distance in meters from current location to the target
        /// </summary>
        internal double distanceToTarget
        {
            get { return autoRoveUtils.distanceBetweenPoints(this.currentLatitude, this.currentLongitude, this.targetLatitude, this.targetLongitude, this.body.Radius + this.currentAltitude); ; }
        }

        /// <summary>
        /// the Celestial Body the rover is on
        /// </summary>
        internal CelestialBody body
        {
            get { return this.vessel.mainBody; }
        }


        /// <summary>
        /// the target string as set in the module
        /// </summary>
        internal string target
        {
            get { return autoRoveModule.GetValue(targetString); }
        }

        internal string name
        {
            get { return this.roverVessel.vesselName; }
        }

        internal double targetLatitude
        {
            get { return Convert.ToDouble(autoRoveModule.GetValue(targetLat)); }
        }

        internal double targetLongitude
        {
            get { return Convert.ToDouble(autoRoveModule.GetValue(targetLong)); }
        }

        internal double roveSpeed
        {
            get { return Convert.ToDouble(autoRoveModule.GetValue(speed)); }
        }

        internal double lastUpdate
        {
            get { return Convert.ToDouble(autoRoveModule.GetValue(updateTime)); }
            set
            {
                bool updateModule = autoRoveModule.SetValue(updateTime, value.ToString(), false);
                if (!updateModule)
                {
                    autoRoveUtils.debugError(String.Format("Failed Updating {0}: {1} - {2}", roverVessel.vesselName, myModuleName, updateTime));
                }
            }
        }

        internal double maxWheelCharge
        {
            get { return Convert.ToDouble(autoRoveModule.GetValue(maxCharge)); }
        }

        internal double solarPower
        {
            get { return Convert.ToDouble(autoRoveModule.GetValue(solarCharge)); }
        }

        internal double generatorPower
        {
            get { return Convert.ToDouble(autoRoveModule.GetValue(generatorCharge)); }
        }

        internal double currentLatitude
        {
            get { return roverVessel.latitude; }
            set { roverVessel.latitude = value; this.protoVesselConfigNode.SetValue("lat", value.ToString()); }
        }

        internal double currentLongitude
        {
            get { return roverVessel.longitude; }
            set { roverVessel.longitude = value; this.protoVesselConfigNode.SetValue("lon", value.ToString()); }
        }

        internal double currentAltitude
        {
            get { return roverVessel.protoVessel.altitude; }
            set { roverVessel.altitude = value; this.protoVesselConfigNode.SetValue("alt", value.ToString()); }
        }

        internal string landedAt
        {
            get { return roverVessel.protoVessel.landedAt; }
            set { roverVessel.landedAt = value; this.protoVesselConfigNode.SetValue("landedAt", value); }
        }

        /// <summary>
        /// constructor, take a vessel and extract all usefull information
        /// if AutoRoveModule is present and activ, returns null otherwise
        /// </summary>
        /// <param name="vessel"> the unloaded Vessel of the Rover </param>
        internal Rover Initialize (Vessel vessel)
        {
            vessel.protoVessel.Save(protoVesselConfigNode);            
            ConfigNode module = autoRoveUtils.queryNode(protoVesselConfigNode, "MODULE", "name", myModuleName);

            if (module != null && Convert.ToBoolean(module.GetValue(this.toggleAutoRove)))
            {
                autoRoveModule = module;
                this.roverVessel = vessel;
                return this;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///  serializing all rover data to a string and prints it to the console for debugging
        /// </summary>
        internal void print()
        {
            String message = String.Format(
                "Rover Data - vesselID: {0}, targetLatitude: {1}, targetLongitude: {2}, roveSpeed: {3}, lastUpdate: {4}, currentLatitude: {5}, currentLongitude: {6}, currentAltitude: {7}",
                vesselID, targetLatitude, targetLongitude, roveSpeed, lastUpdate, currentLatitude, currentLongitude, currentAltitude);
            autoRoveUtils.debugMessage(message);
        }

        /// <summary>
        /// synches the vessels protoVessel with the protoVesselConfigNode
        /// </summary>
        internal void update()
        {
            vessel.protoVessel = new ProtoVessel(protoVesselConfigNode, HighLogic.CurrentGame);
        }

        /// <summary>
        /// moves the rover to its target coordinates by a distance acordining to its speed
        /// and updates the vessel if succesful
        /// </summary>
        /// <returns> true if the move was sucessfull, false if it would end in water</returns>
        internal bool move()
        {
            double timeNow = Planetarium.GetUniversalTime();
            CelestialBody body = this.vessel.mainBody;
            double[] newPosition = new double[2];

            // calculating the driven distance
            double distanceTraveled = roveSpeed * (timeNow - this.lastUpdate);

            // calculating the distance from old position to target position
            double toTargetDistance = autoRoveUtils.distanceBetweenPoints(this.currentLatitude, this.currentLongitude, this.targetLatitude, this.targetLongitude, body.Radius + this.currentAltitude);

            // if the rover would have reached its target than set it on the target and turn autoRove off
            if (toTargetDistance < distanceTraveled)
            {
                newPosition[0] = this.targetLatitude;
                newPosition[1] = this.targetLongitude;
                autoRoveUtils.debugMessage(String.Format("Rover {0} reached its destination, turning AutoRove off!", this.vessel.name));
                this.turnAutoRoveOff();
            }
            else // calculating the new position
            {
                double brgDegrees = autoRoveUtils.bearingDegrees(this.currentLatitude, this.currentLongitude, this.targetLatitude, this.targetLongitude);
                newPosition = autoRoveUtils.newPosition(this.currentLatitude, this.currentLongitude, brgDegrees, distanceTraveled, body.Radius + this.currentAltitude);
            }

            double newAltitude = autoRoveUtils.surfaceHeight(newPosition[0], newPosition[1], body);

            // moving the rover to the new position or denying the move
            if (newAltitude >= 0)
            {
                this.currentAltitude = newAltitude;
                this.currentLatitude = newPosition[0];
                this.currentLongitude = newPosition[1];
                this.lastUpdate = timeNow;
                this.landedAt = "";
                this.update();
                return true;
            }
            else if (newAltitude < 0)
            {
                autoRoveUtils.debugMessage(String.Format("Disabeling autoRove for {0} / {1} - would have been splashed", this.name, this.body));
                this.turnAutoRoveOff();
                this.update();
                return false;
            }
            else
            {
                autoRoveUtils.debugError("Something went wrong while moving " + this.name + " on " + this.body);
                return false;
            }
        }


        /// <summary>
        /// sets the module value for isAutoRoveOn to false
        /// </summary>
        /// <returns> true if the value has been set</returns>
        internal void turnAutoRoveOff()
        {
            this.autoRoveModule.SetValue(toggleAutoRove, "False");

            if (Convert.ToBoolean(autoRoveModule.GetValue(toggleAutoRove)))
            {
                autoRoveUtils.debugError("Could not turn AutoRove off!");
            }
        }
    }
}
