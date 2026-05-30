using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using UnityEngine;
using static GTI.GTIConfig;


namespace GTI.Events
{
    /// <summary>
    /// Creates the GTI Events
    /// </summary>
    

    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class GTI_EventCreator : MonoBehaviour
    {
        //public static EventVoid onThrottleChange;
        public static EventData<float, float> onThrottleChange;  //onThrottleChange<"Current Throttle", "Previous Throttle">

        private void Awake()
        {
            if (onThrottleChange == null)
            {
                GTIDebug.Log("Event 'onThrottleChange' Created", iDebugLevel.Low);
                onThrottleChange = new EventData<float, float>("onThrottleChange");
            } else { GTIDebug.Log("Event 'onThrottleChange' already exists", iDebugLevel.DebugInfo); }

        }
    }

    /// <summary>
    /// Evaluates and raises the GTI Events
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class GTI_Events : MonoBehaviour
    {
        //Threaded Tasks list
        public List<Task> ThreadTasks = new List<Task>();
        //public static EventVoid onThrottleChange;
        private static float savedThrottle;
        //private EventVoid onThrottleChangeEvent;
        private EventData<float, float> onThrottleChangeEvent;
        //private EventData<GameScenes> onSceneChange;
        public static bool EventDetectorRunning = false;

        // Thread-safe hand-off: the worker thread enqueues throttle changes; the main-thread consumer
        // coroutine drains it and fires the event, so subscribers run on the main thread.
        private readonly ConcurrentQueue<ThrottleChange> _throttleChanges = new ConcurrentQueue<ThrottleChange>();
        // How often the main-thread consumer wakes to apply the newest change. Low cadence = low cost. Tunable.
        private const float _consumeInterval = 0.1f;   // seconds (~10x per second)
        private struct ThrottleChange
        {
            public float current;
            public float previous;
            public ThrottleChange(float current, float previous) { this.current = current; this.previous = previous; }
        }

        private void Awake()
        {
            //Load Config
            //EventConfig = GetConfigurationsCFG();
            //if (!bool.TryParse(EventConfig.GetValue("initEvent"), out initEvent)) initEvent = true;

            #region Events
            GTIDebug.Log("GTI_Events find onthrottleChangeEvent on Awake()", iDebugLevel.DebugInfo);
            onThrottleChangeEvent = GameEvents.FindEvent<EventData<float, float>>("onThrottleChange");

            //Starting the thread which will continuously check and raise the Throttle Event if interaction was detected
            startThread();
            #endregion

            // Activate the load fixer, where cheats are temporarily activated to counteract spontaneous explotions on load of scenes.
            if (GTIConfig.ActivateLoadFixer)
                StartCoroutine(SceneLoadFixer());
        }

        //Scene Load Fixer ensures that things does not blow up right after scene load
        internal IEnumerator SceneLoadFixer()
        {
            bool NoCrashDamage = CheatOptions.NoCrashDamage;
            bool UnbreakableJoints = CheatOptions.UnbreakableJoints;

            GTIDebug.Log("Enabled cheat options", "GTI Scene LoadFixer", iDebugLevel.Medium);
            CheatOptions.NoCrashDamage = true;
            CheatOptions.UnbreakableJoints = true;

            //wait 5 seconds
            yield return new WaitForSeconds(5f);

            CheatOptions.NoCrashDamage = NoCrashDamage;
            CheatOptions.UnbreakableJoints = UnbreakableJoints;
        }

        internal void startThread()
        {
            if (GTIConfig.Event.initialize)
            {
                if (!EventDetectorRunning)
                {
                    // Set on the main thread before launching so the consumer coroutine's loop guard is valid.
                    EventDetectorRunning = true;

                    Thread EventThread = new Thread(() => GTI_inFlightEventDetector());
                    GTIDebug.Log("Starting GTI Event thread", iDebugLevel.High);
                    EventThread.Priority = System.Threading.ThreadPriority.BelowNormal;
                    EventThread.IsBackground = true;
                    EventThread.Start();

                    // Drain the queue on the main thread (coroutine, not Update/FixedUpdate).
                    StartCoroutine(ConsumeThrottleChanges());

                    GTIDebug.Log("GTI_inFlightEventDetector Started in new thread", iDebugLevel.DebugInfo);
                    return;
                }
                else GTIDebug.Log("GTI onThrottle event detector allready runnning. New Activation Cancelled.", iDebugLevel.Low);
            }
            else
            {
                GTIDebug.Log("The GTI Event 'onThrottleChange' deactivated. initEvent was set to 'false'", iDebugLevel.Low);
                Destroy(this.gameObject);
            }
        }

        private void GTI_inFlightEventDetector()        //For threaded execution --> Detects the basis for event
        {
            int wait = GTIConfig.Event.CheckFreqIdle;

            //Stopwatch for timing how long the event checking is running
            System.Diagnostics.Stopwatch stopwatch;
            stopwatch = System.Diagnostics.Stopwatch.StartNew();

            //GTIDebug.Log("Event thread GTI_inFlightEventDetector started\n\tEventCheckFreqIdle: " + GTIConfig.Event.CheckFreqIdle + "\n\tEventCheckFreqActive: " + GTIConfig.Event.CheckFreqActive, iDebugLevel.Medium);
            GTIDebug.LogAppend(iDebugLevel.Medium, "Event thread GTI_inFlightEventDetector started\n\tEventCheckFreqIdle: ", GTIConfig.Event.CheckFreqIdle.ToString(), "\n\tEventCheckFreqActive: ", GTIConfig.Event.CheckFreqActive.ToString());
            while (EventDetectorRunning)
            {
                float throttle = FlightInputHandler.state.mainThrottle;
                if (savedThrottle != throttle)
                {
                    // Hand the change to the main-thread consumer - do NOT Fire() or touch KSP objects here.
                    _throttleChanges.Enqueue(new ThrottleChange(throttle, savedThrottle));
                    savedThrottle = throttle;
                    wait = GTIConfig.Event.CheckFreqActive;
                }
                else { wait = GTIConfig.Event.CheckFreqIdle; }

                Thread.Sleep(wait);
            }

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            GTIDebug.Log("GTI_inFlightEventDetector Stopped.\tRuntime = " + elapsedTime, iDebugLevel.Medium);
        }

        /// <summary>
        /// Main-thread consumer. Wakes at a low fixed cadence (not every frame), coalesces the queued
        /// throttle changes down to the newest, and fires onThrottleChange there - so every subscriber
        /// runs on the main thread where touching KSP/Unity objects is safe.
        /// </summary>
        private IEnumerator ConsumeThrottleChanges()
        {
            // Cached once to avoid per-iteration allocation (GC).
            WaitForSeconds wait = new WaitForSeconds(_consumeInterval);
            while (EventDetectorRunning)
            {
                bool any = false;
                ThrottleChange latest = default(ThrottleChange);
                // Coalesce: drain everything queued since last wake, keep only the newest.
                while (_throttleChanges.TryDequeue(out ThrottleChange change)) { latest = change; any = true; }
                if (any) onThrottleChangeEvent?.Fire(latest.current, latest.previous);
                yield return wait;
            }
        }


        #region Event Call functions

        private void EventDebugger(float newThrottle, float OrigThrottle)
        {
            GTIDebug.Log("onThrottle Event Raised", iDebugLevel.DebugInfo);
        }
        #endregion

        private void OnDestroy()
        {
            GTIDebug.Log("GTI_Events destroyed", iDebugLevel.Medium);
            EventDetectorRunning = false;
            if (onThrottleChangeEvent != null)
                onThrottleChangeEvent.Remove(EventDebugger);
            //onSceneChange.Remove(onEventSceneChange);
        }
    }
}