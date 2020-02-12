using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LEA_for_ED
{
    public static class StatusReader
    {
        #region Private Internal Fields
        private static FileSystemWatcher statusWatcher;
        private static string statusFilePath;
        private static RawStatus rawStatus;
        private static Stopwatch stopwatch = new Stopwatch();
        private static object _ProcessStatusFileLock = new object();
        #endregion

        #region Event Handling
        public static event EventHandler<StatusChangedEventArgs> StatusChanged;
        public static void OnValueChanged(Property propertyName, Type propertyType, object obj, DateTime timestamp)
        {
            if (SubscribedEvents.Contains(propertyName))
            {
                EventHandler<StatusChangedEventArgs> handler = StatusChanged;
                if (handler != null)
                {
                    handler(obj, new StatusChangedEventArgs(propertyName, propertyType, obj, timestamp));
                }
            }
        }
        public static List<Property> SubscribedEvents = new List<Property>();
        #endregion

        #region Private Fields
        private static bool docked;
        private static bool landed;
        private static bool landingGearDown;
        private static bool shieldsUp;
        private static bool supercruise;
        private static bool flightAssistOff;
        private static bool hardpointsDeployed;
        private static bool inWing;
        private static bool lightsOn;
        private static bool cargoScoopDeployed;
        private static bool silentRunning;
        private static bool scoopingFuel;
        private static bool srvHandbrake;
        private static bool srvUsingTurretView;
        private static bool srvTurretRetracted;
        private static bool srvDriveAssist;
        private static bool fsdMassLocked;
        private static bool fsdCharging;
        private static bool fsdCooldown;
        private static bool lowFuel;
        private static bool overHeating;
        private static bool hasLatLong;
        private static bool isInDanger;
        private static bool beingInterdicted;
        private static bool inMainShip;
        private static bool inFighter;
        private static bool inSRV;
        private static bool hudInAnalysisMode;
        private static bool nightVision;
        private static bool altitudeFromAverageRadius;
        private static bool fsdJump;
        private static bool srvHighBeam;
        private static int engPips;
        private static int sysPips;
        private static int wepPips;
        private static int fireGroup;
        private static Gui guiFocus;
        private static double fuelMain;
        private static double fuelReservoir;
        private static double cargo;
        private static string legalState;
        private static double latitude;
        private static double altitude;
        private static double longitude;
        private static double heading;
        private static string bodyName;
        private static double planetRadius;
        #endregion

        #region Properies

        public static DateTime TimeStamp { get; set; }
        public static bool Docked
        {
            get => docked;
            set
            {
                if (docked != value)
                {
                    docked = value;
                    OnValueChanged(Property.Docked, typeof(bool), Docked, TimeStamp);
                }
            }
        }
        public static bool Landed
        {
            get => landed;
            set
            {
                if (landed != value)
                {
                    landed = value;
                    OnValueChanged(Property.Landed, typeof(bool), Landed, TimeStamp);
                }
            }
        }
        public static bool LandingGearDown
        {
            get => landingGearDown;
            set
            {
                if (landingGearDown != value)
                {
                    landingGearDown = value;
                    OnValueChanged(Property.LandingGearDown, typeof(bool), LandingGearDown, TimeStamp);
                }
            }
        }
        public static bool ShieldsUp
        {
            get => shieldsUp;
            set
            {
                if (shieldsUp != value)
                {
                    shieldsUp = value;
                    OnValueChanged(Property.ShieldsUp, typeof(bool), ShieldsUp, TimeStamp);
                }
            }
        }
        public static bool Supercruise
        {
            get => supercruise;
            set
            {
                if (supercruise != value)
                {
                    supercruise = value;
                    OnValueChanged(Property.Supercruise, typeof(bool), Supercruise, TimeStamp);
                }
            }
        }
        public static bool FlightAssistOff
        {
            get => flightAssistOff;
            set
            {
                if (flightAssistOff != value)
                {
                    flightAssistOff = value;
                    OnValueChanged(Property.FlightAssistOff, typeof(bool), FlightAssistOff, TimeStamp);
                }
            }
        }
        public static bool HardpointsDeployed
        {
            get => hardpointsDeployed;
            set
            {
                if (hardpointsDeployed != value)
                {
                    hardpointsDeployed = value;
                    OnValueChanged(Property.HardpointsDeployed, typeof(bool), HardpointsDeployed, TimeStamp);
                }
            }
        }
        public static bool InWing
        {
            get => inWing;
            set
            {
                if (inWing != value)
                {
                    inWing = value;
                    OnValueChanged(Property.InWing, typeof(bool), InWing, TimeStamp);
                }
            }
        }
        public static bool LightsOn
        {
            get => lightsOn;
            set
            {
                if (lightsOn != value)
                {
                    lightsOn = value;
                    OnValueChanged(Property.LightsOn, typeof(bool), LightsOn, TimeStamp);
                }
            }
        }
        public static bool CargoScoopDeployed
        {
            get => cargoScoopDeployed;
            set
            {
                if (cargoScoopDeployed != value)
                {
                    cargoScoopDeployed = value;
                    OnValueChanged(Property.CargoScoopDeployed, typeof(bool), CargoScoopDeployed, TimeStamp);
                }
            }
        }
        public static bool SilentRunning
        {
            get => silentRunning;
            set
            {
                if (silentRunning != value)
                {
                    silentRunning = value;
                    OnValueChanged(Property.SilentRunning, typeof(bool), SilentRunning, TimeStamp);
                }
            }
        }
        public static bool ScoopingFuel
        {
            get => scoopingFuel;
            set
            {
                if (scoopingFuel != value)
                {
                    scoopingFuel = value;
                    OnValueChanged(Property.ScoopingFuel, typeof(bool), ScoopingFuel, TimeStamp);
                }
            }
        }
        public static bool SrvHandbrake
        {
            get => srvHandbrake;
            set
            {
                if (srvHandbrake != value)
                {
                    srvHandbrake = value;
                    OnValueChanged(Property.SrvHandbrake, typeof(bool), SrvHandbrake, TimeStamp);
                }
            }
        }
        public static bool SrvUsingTurretView
        {
            get => srvUsingTurretView;
            set
            {
                if (srvUsingTurretView != value)
                {
                    srvUsingTurretView = value;
                    OnValueChanged(Property.SrvUsingTurretView, typeof(bool), SrvUsingTurretView, TimeStamp);
                }
            }
        }
        public static bool SrvTurretRetracted
        {
            get => srvTurretRetracted;
            set
            {
                if (srvTurretRetracted != value)
                {
                    srvTurretRetracted = value;
                    OnValueChanged(Property.SrvTurretRetracted, typeof(bool), SrvTurretRetracted, TimeStamp);
                }
            }
        }
        public static bool SrvDriveAssist
        {
            get => srvDriveAssist;
            set
            {
                if (srvDriveAssist != value)
                {
                    srvDriveAssist = value;
                    OnValueChanged(Property.SrvDriveAssist, typeof(bool), SrvDriveAssist, TimeStamp);
                }
            }
        }
        public static bool FsdMassLocked
        {
            get => fsdMassLocked;
            set
            {
                if (fsdMassLocked != value)
                {
                    fsdMassLocked = value;
                    OnValueChanged(Property.FsdMassLocked, typeof(bool), FsdMassLocked, TimeStamp);
                }
            }
        }
        public static bool FsdCharging
        {
            get => fsdCharging;
            set
            {
                if (fsdCharging != value)
                {
                    fsdCharging = value;
                    OnValueChanged(Property.FsdCharging, typeof(bool), FsdCharging, TimeStamp);
                }
            }
        }
        public static bool FsdCooldown
        {
            get => fsdCooldown;
            set
            {
                if (fsdCooldown != value)
                {
                    fsdCooldown = value;
                    OnValueChanged(Property.FsdCooldown, typeof(bool), FsdCooldown, TimeStamp);
                }
            }
        }
        public static bool LowFuel
        {
            get => lowFuel;
            set
            {
                if (lowFuel != value)
                {
                    lowFuel = value;
                    OnValueChanged(Property.LowFuel, typeof(bool), LowFuel, TimeStamp);
                }
            }
        }
        public static bool OverHeating
        {
            get => overHeating;
            set
            {
                if (overHeating != value)
                {
                    overHeating = value;
                    OnValueChanged(Property.OverHeating, typeof(bool), OverHeating, TimeStamp);
                }
            }
        }
        public static bool HasLatLong
        {
            get => hasLatLong;
            set
            {
                if (hasLatLong != value)
                {
                    hasLatLong = value;
                    OnValueChanged(Property.HasLatLong, typeof(bool), HasLatLong, TimeStamp);
                }
            }
        }
        public static bool IsInDanger
        {
            get => isInDanger;
            set
            {
                if (isInDanger != value)
                {
                    isInDanger = value;
                    OnValueChanged(Property.IsInDanger, typeof(bool), IsInDanger, TimeStamp);
                }
            }
        }
        public static bool BeingInterdicted
        {
            get => beingInterdicted;
            set
            {
                if (beingInterdicted != value)
                {
                    beingInterdicted = value;
                    OnValueChanged(Property.BeingInterdicted, typeof(bool), BeingInterdicted, TimeStamp);
                }
            }
        }
        public static bool InMainShip
        {
            get => inMainShip;
            set
            {
                if (inMainShip != value)
                {
                    inMainShip = value;
                    OnValueChanged(Property.InMainShip, typeof(bool), InMainShip, TimeStamp);
                }
            }
        }
        public static bool InFighter
        {
            get => inFighter;
            set
            {
                if (inFighter != value)
                {
                    inFighter = value;
                    OnValueChanged(Property.InFighter, typeof(bool), InFighter, TimeStamp);
                }
            }
        }
        public static bool InSRV
        {
            get => inSRV;
            set
            {
                if (inSRV != value)
                {
                    inSRV = value;
                    OnValueChanged(Property.InSRV, typeof(bool), InSRV, TimeStamp);
                }
            }
        }
        public static bool HudInAnalysisMode
        {
            get => hudInAnalysisMode;
            set
            {
                if (hudInAnalysisMode != value)
                {
                    hudInAnalysisMode = value;
                    OnValueChanged(Property.HudInAnalysisMode, typeof(bool), HudInAnalysisMode, TimeStamp);
                }
            }
        }
        public static bool NightVision
        {
            get => nightVision;
            set
            {
                if (nightVision != value)
                {
                    nightVision = value;
                    OnValueChanged(Property.NightVision, typeof(bool), NightVision, TimeStamp);
                }
            }
        }
        public static bool AltitudeFromAverageRadius
        {
            get => altitudeFromAverageRadius;
            set
            {
                if (altitudeFromAverageRadius != value)
                {
                    altitudeFromAverageRadius = value;
                    OnValueChanged(Property.AltitudeFromAverageRadius, typeof(bool), AltitudeFromAverageRadius, TimeStamp);
                }
            }
        }
        public static bool FsdJump
        {
            get => fsdJump;
            set
            {
                if (fsdJump != value)
                {
                    fsdJump = value;
                    OnValueChanged(Property.FsdJump, typeof(bool), FsdJump, TimeStamp);
                }
            }
        }
        public static bool SrvHighBeam
        {
            get => srvHighBeam;
            set
            {
                if (srvHighBeam != value)
                {
                    srvHighBeam = value;
                    OnValueChanged(Property.SrvHighBeam, typeof(bool), SrvHighBeam, TimeStamp);
                }
            }
        }
        public static int EngPips
        {
            get => engPips;
            set
            {
                if (engPips != value)
                {
                    engPips = value;
                    OnValueChanged(Property.EngPips, typeof(int), EngPips, TimeStamp);
                }
            }
        }
        public static int SysPips
        {
            get => sysPips;
            set
            {
                if (sysPips != value)
                {
                    sysPips = value;
                    OnValueChanged(Property.SysPips, typeof(int), SysPips, TimeStamp);
                }
            }
        }
        public static int WepPips
        {
            get => wepPips;
            set
            {
                if (wepPips != value)
                {
                    wepPips = value;
                    OnValueChanged(Property.WepPips, typeof(int), WepPips, TimeStamp);
                }
            }
        }
        public static int FireGroup
        {
            get => fireGroup;
            set
            {
                if (fireGroup != value)
                {
                    fireGroup = value;
                    OnValueChanged(Property.FireGroup, typeof(int), FireGroup, TimeStamp);
                }
            }
        }
        public static Gui GuiFocus
        {
            get => guiFocus;
            set
            {
                if (guiFocus != value)
                {
                    guiFocus = value;
                    OnValueChanged(Property.GuiFocus, typeof(Gui), GuiFocus, TimeStamp);
                }
            }
        }
        public static double FuelMain
        {
            get => fuelMain;
            set
            {
                if (fuelMain != value)
                {
                    fuelMain = value;
                    OnValueChanged(Property.FuelMain, typeof(double), FuelMain, TimeStamp);
                }
            }
        }
        public static double FuelReservoir
        {
            get => fuelReservoir;
            set
            {
                if (fuelReservoir != value)
                {
                    fuelReservoir = value;
                    OnValueChanged(Property.FuelReservoir, typeof(double), FuelReservoir, TimeStamp);
                }
            }
        }
        public static double Cargo
        {
            get => cargo;
            set
            {
                if (cargo != value)
                {
                    cargo = value;
                    OnValueChanged(Property.Cargo, typeof(double), Cargo, TimeStamp);
                }
            }
        }
        public static string LegalState
        {
            get => legalState;
            set
            {
                if (legalState != value)
                {
                    legalState = value;
                    OnValueChanged(Property.LegalState, typeof(string), LegalState, TimeStamp);
                }
            }
        }
        public static double Latitude
        {
            get => latitude;
            set
            {
                if (latitude != value)
                {
                    latitude = value;
                    OnValueChanged(Property.Latitude, typeof(double), Latitude, TimeStamp);
                }
            }
        }
        public static double Altitude
        {
            get => altitude;
            set
            {
                if (altitude != value)
                {
                    altitude = value;
                    OnValueChanged(Property.Altitude, typeof(double), Altitude, TimeStamp);
                }
            }
        }
        public static double Longitude
        {
            get => longitude;
            set
            {
                if (longitude != value)
                {
                    longitude = value;
                    OnValueChanged(Property.Longitude, typeof(double), Longitude, TimeStamp);
                }
            }
        }
        public static double Heading
        {
            get => heading;
            set
            {
                if (heading != value)
                {
                    heading = value;
                    OnValueChanged(Property.Heading, typeof(double), Heading, TimeStamp);
                }
            }
        }
        public static string BodyName
        {
            get => bodyName;
            set
            {
                if (bodyName != value)
                {
                    bodyName = value;
                    OnValueChanged(Property.BodyName, typeof(string), BodyName, TimeStamp);
                }
            }
        }
        public static double PlanetRadius
        {
            get => planetRadius;
            set
            {
                if (planetRadius != value)
                {
                    planetRadius = value;
                    OnValueChanged(Property.PlanetRadius, typeof(double), PlanetRadius, TimeStamp);
                }
            }
        }

        #endregion

        static StatusReader()
        {
            rawStatus = new RawStatus();
        }

        public static void Start(Settings settings)
        {
            // Add Subscribed Events
            foreach (SettingsStatusEvent statusEvent in settings.StatusEvents)
            {
                SubscribedEvents.Add((Property)Enum.Parse(typeof(Property), statusEvent.Event));
            }

            // Setup File Watcher for Status File
            statusFilePath = Environment.ExpandEnvironmentVariables(settings.EDPathInfo.DataPath);
            statusWatcher = new FileSystemWatcher
            {
                Path = statusFilePath,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = settings.EDPathInfo.StatusFile,
                IncludeSubdirectories = false
            };
            statusWatcher.Changed += (s, e) => StatusFileChanged();
            statusFilePath += settings.EDPathInfo.StatusFile;
            statusWatcher.EnableRaisingEvents = true;
        }

        public static void Stop()
        {
            statusWatcher.EnableRaisingEvents = false;
            statusWatcher.Dispose();
        }

        // Ensure file changes are complete.
        private static void StatusFileChanged()
        {
            Task.Run(() =>
            {
                if (stopwatch.IsRunning)
                    stopwatch.Restart();
                else
                {
                    stopwatch.Start();
                    while (stopwatch.ElapsedMilliseconds < 25)
                        Thread.Sleep(5);
                    stopwatch.Reset();
                    stopwatch.Stop();
                    ProcessStatusFile();
                }
            });
        }

        // Process Changes
        private static void ProcessStatusFile()
        {
            lock (_ProcessStatusFileLock)
            {
                string jsonString = "";
                bool cont = true;
                while (cont)
                {
                    FileStream fileStream = null;
                    StreamReader streamReader = null;
                    try
                    {
                        fileStream = File.Open(statusFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        streamReader = new StreamReader(fileStream);
                        jsonString = streamReader.ReadToEnd();
                        streamReader.Close();
                        streamReader.Dispose();
                        streamReader = null;
                        fileStream.Close();
                        fileStream.Dispose();
                        fileStream = null;
                        if (jsonString.StartsWith('{') && jsonString.Trim().EndsWith('}'))
                            cont = false;
                    }
                    catch (Exception) { }
                    finally
                    {
                        #region EnsureFileClosed
                        try
                        {
                            fileStream.Close();
                        }
                        catch (Exception) { }
                        finally
                        {
                            try
                            {
                                fileStream.Dispose();
                            }
                            catch (Exception) { }
                            finally
                            {
                                fileStream = null;
                            }
                        }

                        try
                        {
                            streamReader.Close();
                        }
                        catch (Exception) { }
                        finally
                        {
                            try
                            {
                                streamReader.Dispose();
                            }
                            catch (Exception) { }
                            finally
                            {
                                streamReader = null;
                            }
                        }
                        #endregion
                    }
                }

                try
                {
                    RawStatus tempRawStatus = JsonSerializer.Deserialize<RawStatus>(jsonString);
                    if (tempRawStatus.timestamp != rawStatus.timestamp)
                    {
                        rawStatus = tempRawStatus;

                        #region Set Values

                        // TimeStamp
                        try
                        {
                            TimeStamp = DateTime.Parse(rawStatus.timestamp);
                        }
                        catch (Exception) { }

                        // Flags
                        for (int i = 0; i < 32; i++)
                        {
                            bool result = ((rawStatus.Flags & ((uint)0x0001 << i)) != 0) ? true : false;

                            switch (i)
                            {
                                case 0:
                                    Docked = result;
                                    break;
                                case 1:
                                    Landed = result;
                                    break;
                                case 2:
                                    LandingGearDown = result;
                                    break;
                                case 3:
                                    ShieldsUp = result;
                                    break;
                                case 4:
                                    Supercruise = result;
                                    break;
                                case 5:
                                    FlightAssistOff = result;
                                    break;
                                case 6:
                                    HardpointsDeployed = result;
                                    break;
                                case 7:
                                    InWing = result;
                                    break;
                                case 8:
                                    LightsOn = result;
                                    break;
                                case 9:
                                    CargoScoopDeployed = result;
                                    break;
                                case 10:
                                    SilentRunning = result;
                                    break;
                                case 11:
                                    ScoopingFuel = result;
                                    break;
                                case 12:
                                    SrvHandbrake = result;
                                    break;
                                case 13:
                                    SrvUsingTurretView = result;
                                    break;
                                case 14:
                                    SrvTurretRetracted = result;
                                    break;
                                case 15:
                                    SrvDriveAssist = result;
                                    break;
                                case 16:
                                    FsdMassLocked = result;
                                    break;
                                case 17:
                                    FsdCharging = result;
                                    break;
                                case 18:
                                    FsdCooldown = result;
                                    break;
                                case 19:
                                    LowFuel = result;
                                    break;
                                case 20:
                                    OverHeating = result;
                                    break;
                                case 21:
                                    HasLatLong = result;
                                    break;
                                case 22:
                                    IsInDanger = result;
                                    break;
                                case 23:
                                    BeingInterdicted = result;
                                    break;
                                case 24:
                                    InMainShip = result;
                                    break;
                                case 25:
                                    InFighter = result;
                                    break;
                                case 26:
                                    InSRV = result;
                                    break;
                                case 27:
                                    HudInAnalysisMode = result;
                                    break;
                                case 28:
                                    NightVision = result;
                                    break;
                                case 29:
                                    AltitudeFromAverageRadius = result;
                                    break;
                                case 30:
                                    FsdJump = result;
                                    break;
                                case 31:
                                    SrvHighBeam = result;
                                    break;
                                default:
                                    break;
                            }

                        }

                        // Pips
                        if (rawStatus.Pips != null && rawStatus.Pips.Count == 3)
                        {
                            SysPips = rawStatus.Pips[0];
                            EngPips = rawStatus.Pips[1];
                            WepPips = rawStatus.Pips[2];
                        }

                        // FireGroup
                        FireGroup = rawStatus.FireGroup;

                        // GuiFocus
                        GuiFocus = (Gui)rawStatus.GuiFocus;

                        // Fuel
                        if (rawStatus.Fuel != null)
                        {
                            // FuelMain
                            FuelMain = rawStatus.Fuel.FuelMain;

                            // FuelReservoir
                            FuelReservoir = rawStatus.Fuel.FuelReservoir;
                        }

                        // Cargo
                        Cargo = rawStatus.Cargo;

                        // LegalState
                        LegalState = rawStatus.LegalState == null ? "" : rawStatus.LegalState;

                        // Latitude
                        Latitude = rawStatus.Latitude;

                        // Altitude
                        Altitude = rawStatus.Altitude;

                        // Longitude
                        Longitude = rawStatus.Longitude;

                        // Heading
                        Heading = rawStatus.Heading;

                        // BodyName
                        BodyName = rawStatus.BodyName == null ? "" : rawStatus.BodyName;

                        // PlanetRadius
                        PlanetRadius = rawStatus.PlanetRadius;

                        #endregion
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    Console.WriteLine(jsonString);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        private class RawStatus
        {
            public string timestamp { get; set; }
            public string @event { get; set; }
            public uint Flags { get; set; }
            public List<int> Pips { get; set; }
            public int FireGroup { get; set; }
            public int GuiFocus { get; set; }
            public RawFuel Fuel { get; set; }
            public double Cargo { get; set; }
            public string LegalState { get; set; }
            public double Latitude { get; set; }
            public double Altitude { get; set; }
            public double Longitude { get; set; }
            public double Heading { get; set; }
            public string BodyName { get; set; }
            public double PlanetRadius { get; set; }

            public class RawFuel
            {
                public double FuelMain { get; set; }
                public double FuelReservoir { get; set; }
            }
        }

        public enum Gui : int
        {
            NoFocus = 0,
            InternalPanel = 1,
            ExternalPanel = 2,
            CommsPanel = 3,
            RolePanel = 4,
            StationServices = 5,
            GalaxyMap = 6,
            SystemMap = 7,
            Orrery = 8,
            FSS = 9,
            SAA = 10,
            Codex = 11
        }

        public enum Property
        {
            TimeStamp,
            Docked,
            Landed,
            LandingGearDown,
            ShieldsUp,
            Supercruise,
            FlightAssistOff,
            HardpointsDeployed,
            InWing,
            LightsOn,
            CargoScoopDeployed,
            SilentRunning,
            ScoopingFuel,
            SrvHandbrake,
            SrvUsingTurretView,
            SrvTurretRetracted,
            SrvDriveAssist,
            FsdMassLocked,
            FsdCharging,
            FsdCooldown,
            LowFuel,
            OverHeating,
            HasLatLong,
            IsInDanger,
            BeingInterdicted,
            InMainShip,
            InFighter,
            InSRV,
            HudInAnalysisMode,
            NightVision,
            AltitudeFromAverageRadius,
            FsdJump,
            SrvHighBeam,
            EngPips,
            SysPips,
            WepPips,
            FireGroup,
            GuiFocus,
            FuelMain,
            FuelReservoir,
            Cargo,
            LegalState,
            Latitude,
            Altitude,
            Longitude,
            Heading,
            BodyName,
            PlanetRadius
        }
    }

    public class StatusChangedEventArgs : EventArgs
    {
        public StatusChangedEventArgs() { }
        public StatusChangedEventArgs(StatusReader.Property propertyName, Type propertyType, object value, DateTime timeStamp)
        {
            PropertyName = propertyName;
            PropertyType = propertyType;
            Value = value;
            TimeStamp = timeStamp;
        }
        public StatusReader.Property PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public object Value { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
