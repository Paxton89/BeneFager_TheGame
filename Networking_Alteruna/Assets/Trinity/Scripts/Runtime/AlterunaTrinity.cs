using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Alteruna.Trinity
{
    /// <summary>
    /// Class <c>AlterunaTrinity</c> is responsible for establishing a connection to a server. 
    /// Recieves data from other clients and invokes network-related events.
    /// </summary>
    ///
    [RequireComponent(typeof(SynchronizableManager), typeof(NameGenerator))]
    public class AlterunaTrinity : MonoBehaviour, IDeviceListener, ISessionListener
    {
        /// The frequency at which to update statistics.
        public const int STATISITCS_INTERVAL = 1;

        [HideInInspector]
        public bool InPlayroom { get; private set; }
        [HideInInspector]
        public ushort UserIndex { get; private set; }

        public LogBase.Severity LogLevel = LogBase.Severity.Error;
        public uint ApplicationID = 1;

        [HideInInspector]
        public DeviceType DeviceType = DeviceType.Laptop;
        [HideInInspector]
        public string ClientName = "default_name";
        [HideInInspector]
        public int ServerPort = 20000;
        [HideInInspector]
        public int PublishPort = 20000;
        [HideInInspector]
        public bool BroadcastEnabled = true;

        [HideInInspector]
        public bool UseKnownDevice = false;
        [HideInInspector]
        public string KnownDeviceIP = "localhost";
        [HideInInspector]
        public int KnownDevicePort = 20000;

        [Header("Connection Events")]
        public UnityEvent<AlterunaTrinity, IDevice, bool> Connected;
        public UnityEvent<AlterunaTrinity, IDevice, bool> ConnectionLost;
        public UnityEvent<AlterunaTrinity, IDevice, ConnectionStatus> Disconnected;
        public UnityEvent<AlterunaTrinity, IDevice> NewAvailableDevice;
        public UnityEvent<AlterunaTrinity, IDevice> LostAvailableDevice;
        public UnityEvent<AlterunaTrinity, IDevice, int> LatencyUpdate;
        public UnityEvent<AlterunaTrinity, IDevice> NetworkError;

        [Header("Session Events")]
        public UnityEvent<AlterunaTrinity, Session, IDevice, ushort> JoinedSession;
        public UnityEvent<AlterunaTrinity, Session, IDevice> LeftSession;
        public UnityEvent<AlterunaTrinity, Session, ushort, string> OtherJoined;
        public UnityEvent<AlterunaTrinity, Session, ushort, string> OtherLeft;
        public UnityEvent<AlterunaTrinity, Session, ushort> SessionClosed;
        public UnityEvent<AlterunaTrinity, Session, IDevice> SessionTransfered;

        [Header("Observer Events")]
        public UnityEvent<AlterunaTrinity, Session, IDevice, ushort> ObservedSession;
        public UnityEvent<AlterunaTrinity, Session, IDevice> UnobservedSession;
        public UnityEvent<AlterunaTrinity, Session, ushort, string> ObserverJoined;
        public UnityEvent<AlterunaTrinity, Session, ushort, string> ObserverLeft;

        [Header("Synchronizable Events")]
        public UnityEvent<AlterunaTrinity, Synchronizable> PacketSent;
        public UnityEvent<AlterunaTrinity, Synchronizable> PacketRouted;
        public UnityEvent<AlterunaTrinity, IPacketProcessor, IDevice, Reliability> PacketRecieved;
        public UnityEvent<AlterunaTrinity, Synchronizable> LockRequested;
        public UnityEvent<AlterunaTrinity, Synchronizable> LockAquired;
        public UnityEvent<AlterunaTrinity, ushort> ForceSynced;

        // Dev Settings
        [SerializeField, HideInInspector]
        public bool AutoJoinOwnSession = false;
        [SerializeField, HideInInspector]
        public bool AutoJoinFirstSession = false;
        [SerializeField, HideInInspector]
        public int DevClientIndex = 0;
        [SerializeField, HideInInspector]
        public bool IsDevClient = false;

        [HideInInspector]
        public NetworkStatistics Statistics;

        private SynchronizableManager mSynchronizableManager;
        private UnityLog mLog;
        private List<IDevice> mDevices = new List<IDevice>();
        private LNL.DeviceManager mDeviceManager;
        private AnyDeviceAuthorizer mAuth;
        private SessionManager mSessionManager;
        private NameGenerator mNameGenerator;

        /// <summary>
        /// Join a locally hosted Playroom and act as a server.
        /// </summary>
        /// 
        public void JoinOwnPlayroom()
        {
            mSessionManager.Join(mSynchronizableManager);
        }

        /// <summary>
        /// Retrieve a list contaning all playrooms currently available to join.
        /// </summary>
        /// <param name="playrooms">The list which will be populated with available playrooms.</param>
        /// 
        public void GetAvailablePlayrooms(List<Playroom> playrooms)
        {
            mDeviceManager.GetAvailablePlayrooms(playrooms);
        }

        /// <summary>
        /// Join a remotely hosted playroom.
        /// </summary>
        /// <param name="device">The playroom to join.</param>
        /// 
        public void JoinRemotePlayroom(IDevice device)
        {
            if (mDevices.Contains(device))
            {
                mSessionManager.JoinRemote(device, mSynchronizableManager);
            }
        }

        /// <summary>
        /// Join a locally hosted playroom as an observer and act as a server.
        /// </summary>
        /// 
        public void ObserveOwnPlayroom()
        {
            mSessionManager.Observe(mSynchronizableManager);
        }

        /// <summary>
        /// Join a remotely hosted playroom as an observer.
        /// </summary>
        /// <param name="device">The playroom to join.</param>
        /// 
        public void ObserveRemotePlayroom(IDevice device)
        {
            if (mDevices.Contains(device))
            {
                mSessionManager.ObserveRemote(device, mSynchronizableManager);
            }
        }

        /// <summary>
        /// Leave the current playroom. 
        /// </summary>
        /// 
        public void LeavePlayroom()
        {
            mSessionManager.Leave(UserIndex);
        }

        private void Start()
        {
            mSynchronizableManager = GetComponent<SynchronizableManager>();
            mSynchronizableManager.PacketSent.AddListener(OnPacketSent);
            mSynchronizableManager.PacketRouted.AddListener(OnPacketRouted);
            mSynchronizableManager.LockRequested.AddListener(OnLockRequested);
            mSynchronizableManager.LockAquired.AddListener(OnLockAquired);
            mSynchronizableManager.ForceSynced.AddListener(OnForceSynced);

            mNameGenerator = GetComponent<NameGenerator>();

            // Generate a new name if we are a dev client
            if (IsDevClient)
            {
                mNameGenerator.Generate();
            }
            ClientName = mNameGenerator.Name;

            mDeviceManager = new Alteruna.Trinity.LNL.DeviceManager(
                      DeviceType, ClientName,
                      (uint)Alteruna.Trinity.Organization.Any,
                      ApplicationID);

            mDeviceManager.UpdateStatisticsInterval = STATISITCS_INTERVAL;
            Statistics = mDeviceManager.Statistics;

            mAuth = new Alteruna.Trinity.AnyDeviceAuthorizer();
            mSessionManager = new Alteruna.Trinity.SessionManager(mDeviceManager);

            mDeviceManager.mCompositeListeners.Add(this);
            mSessionManager.Listener = this;

            mLog = new UnityLog();
            mLog.LogLevel = LogLevel;
            mSessionManager.Log = mLog;
            mDeviceManager.Log = mLog;

            if (UseKnownDevice)
            {
                mDeviceManager.AddKnownAvailableDevice(KnownDeviceIP, KnownDevicePort);
            }

            mDeviceManager.DeviceAuthorizer = mAuth;
            mDeviceManager.Start(ServerPort, PublishPort, BroadcastEnabled);
        }

        private void Update()
        {
            mDeviceManager.Update();
        }

        private void OnDestroy()
        {
            mDeviceManager.Stop();
        }

        // -------- Connection Events --------------------------------

        public void OnLatencyUpdate(IDevice device, int latency)
        {
            LatencyUpdate?.Invoke(this, device, latency);
        }

        public void OnConnectionLost(IDevice device, bool outgoing)
        {
            ConnectionLost?.Invoke(this, device, outgoing);
        }

        public void OnConnected(IDevice device, bool outgoing)
        {
            Connected?.Invoke(this, device, outgoing);
            if (AutoJoinOwnSession)
            {
                JoinOwnPlayroom();
            }

            if (AutoJoinFirstSession)
            {
                JoinRemotePlayroom(device);
            }
        }

        public void OnDisconnected(IDevice device, ConnectionStatus status)
        {
            Disconnected?.Invoke(this, device, status);
        }

        public void OnPacketReceived(IPacketProcessor processor, IDevice device, Reliability reliability)
        {
            PacketRecieved?.Invoke(this, processor, device, reliability);
        }

        public void OnNetworkError(IDevice device)
        {
            NetworkError?.Invoke(this, device);
        }

        public void OnAvailable(IDevice device)
        {
            mDevices.Add(device);
            NewAvailableDevice?.Invoke(this, device);
        }

        public void OnUnavailable(IDevice device)
        {
            mDevices.Remove(device);
            LostAvailableDevice?.Invoke(this, device);
        }

        // -------- Session Events -----------------------------------

        public void OnSessionJoined(Session session, IDevice device, ushort id)
        {
            UserIndex = id;
            InPlayroom = true;
            JoinedSession?.Invoke(this, session, device, id);
        }

        public void OnSessionLeft(Session session, IDevice device)
        {
            InPlayroom = false;
            LeftSession?.Invoke(this, session, device);
        }

        public void OnOtherJoined(Session session, ushort userId, string userName)
        {
            OtherJoined?.Invoke(this, session, userId, userName);
        }

        public void OnOtherLeft(Session session, ushort userId, string userName)
        {
            OtherLeft?.Invoke(this, session, userId, userName);
        }

        public void OnSessionClosed(Session session, ushort reason)
        {
            mSynchronizableManager.SessionClosed();
            SessionClosed?.Invoke(this, session, reason);
        }

        public void OnSessionTransfered(Session session, IDevice targetDevice)
        {
            SessionTransfered?.Invoke(this, session, targetDevice);
        }

        public void OnUpdatedSessionList(IDevice device, List<SessionInfo> sessions)
        { }

        // -------- Observer Events ----------------------------------

        public void OnSessionObserved(Session session, IDevice device, ushort id)
        {
            ObservedSession?.Invoke(this, session, device, id);
        }

        public void OnSessionUnobserved(Session session, IDevice device)
        {
            UnobservedSession?.Invoke(this, session, device);
        }

        public void OnObserverJoined(Session session, ushort userId, string userName)
        {
            ObserverJoined?.Invoke(this, session, userId, userName);
        }

        public void OnObserverLeft(Session session, ushort userId, string userName)
        {
            ObserverLeft?.Invoke(this, session, userId, userName);
        }

        // -------- Synchronizable Events ----------------------------
        public void OnPacketSent(SynchronizableManager codecManager, Synchronizable synchronizable)
        {
            PacketSent?.Invoke(this, synchronizable);
        }

        public void OnPacketRouted(SynchronizableManager codecManager, Synchronizable synchronizable)
        {
            PacketRouted?.Invoke(this, synchronizable);
        }

        public void OnLockRequested(SynchronizableManager codecManager, Synchronizable synchronizable)
        {
            LockRequested?.Invoke(this, synchronizable);
        }

        public void OnLockAquired(SynchronizableManager codecManager, Synchronizable synchronizable)
        {
            LockAquired?.Invoke(this, synchronizable);
        }

        public void OnForceSynced(SynchronizableManager codecManager, ushort requesterUserId)
        {
            ForceSynced?.Invoke(this, requesterUserId);
        }
    }
}
