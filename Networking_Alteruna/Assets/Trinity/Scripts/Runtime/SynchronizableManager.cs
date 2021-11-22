using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Events;

namespace Alteruna.Trinity
{
    /// <summary>
    /// Class <c>SynchronizableManager</c> is responsible for routing incoming and outgoing data to and from all <c>Synchronizables</c> in the Playroom.
    /// </summary>
    /// <seealso cref="Synchronizable"/>
    /// 
    public class SynchronizableManager : MonoBehaviour, ISynchronizationManager, ICodecRequest
    {
        [HideInInspector]
        public UnityEvent<SynchronizableManager, Synchronizable> PacketSent;
        [HideInInspector]
        public UnityEvent<SynchronizableManager, Synchronizable> PacketRouted;
        [HideInInspector]
        public UnityEvent<SynchronizableManager, Synchronizable> LockRequested;
        [HideInInspector]
        public UnityEvent<SynchronizableManager, Synchronizable> LockAquired;
        [HideInInspector]
        public UnityEvent<SynchronizableManager, ushort> ForceSynced;

        [HideInInspector]
        public bool IsObserver;
        private Dictionary<Guid, Synchronizable> mSynchonizables = new Dictionary<Guid, Synchronizable>();
        private Session mSession;
        private bool mProcessingPacket = false;

        public void AttachSession(Session attachedSession, ushort userId, bool isObserver)
        {
            mSession = attachedSession;
            IsObserver = isObserver;
        }

        public void SessionClosed()
        {
            mSession = null;
        }

        public void RegisterSynchronizable(Guid id, Synchronizable serializable)
        {
            if (!mSynchonizables.ContainsKey(id) && serializable != null)
            {
                mSynchonizables.Add(id, serializable);
            }
        }

        public void DeregisterCodec(Guid id)
        {
            if (mSynchonizables.ContainsKey(id))
            {
                mSynchonizables.Remove(id);
            }
        }

        public void DecodePacket(SessionSyncPacket packet)
        {
            mProcessingPacket = true;
            packet.UnserializeData(this);
            mProcessingPacket = false;
        }

        public void DecodePacket(SessionForceSyncReplyPacket packet)
        {
            packet.UnserializeData(this);
        }

        public void Sync(Guid id)
        {
            if (mSession != null && mSynchonizables.ContainsKey(id))
            {
                SessionSyncPacket packet = new SessionSyncPacket();
                Synchronizable synchronizable = mSynchonizables[id];
                packet.Synchronizables.Add(
                        new SynchronizableElement
                        {
                            CodecID = id,
                            Synchronizable = synchronizable
                        }
                    );

                mSession.Route(packet, null, Reliability.Unreliable);
                PacketSent.Invoke(this, synchronizable);
            }
        }

        public void ForceSync(ushort requesterUserId)
        {
            SessionForceSyncReplyPacket packet = new SessionForceSyncReplyPacket();
            packet.RequestedUserId = requesterUserId;
            
            foreach (var codec in mSynchonizables)
            {
                packet.Synchronizables.Add(
                    new SynchronizableElement
                    {
                        CodecID = codec.Key,
                        Synchronizable = codec.Value
                    });
            }

            mSession.SendForceSyncReply(packet);
            ForceSynced.Invoke(this, requesterUserId);
        }

        public void WaitLockResource(Guid id)
        {
            if (mSession != null && mSynchonizables.ContainsKey(id))
            {
                mSession.WaitResource(id, null);
                LockRequested.Invoke(this, mSynchonizables[id]);
            }
        }

        public void TryLockResource(Guid id)
        {
            if (mSession != null && mSynchonizables.ContainsKey(id))
            {
                mSession.TryLockResource(id, null);
                LockRequested.Invoke(this, mSynchonizables[id]);
            }
        }

        public void LockRequestResponse(Guid codecId, bool isAcquired)
        {
            Synchronizable codec = mSynchonizables.FirstOrDefault(c => c.Key == codecId).Value;
            if (codec != null)
            {
                codec.LockRequestResponse(isAcquired);
            }
        }

        public void UnlockResource(Guid id)
        {
            if (mSession != null && mSynchonizables.ContainsKey(id))
            {
                Synchronizable synchronizable = mSynchonizables[id];
                mSession.UnlockResource(id, null);
                LockRequested.Invoke(this, synchronizable);
            }
        }

        public ISerializable GetCodecForId(Guid codecId)
        {
            if (mSynchonizables.ContainsKey(codecId))
            {
                if (mProcessingPacket)
                {
                    PacketRouted.Invoke(this, mSynchonizables[codecId]);
                }

                return mSynchonizables[codecId];
            }

            return null;
        }

        public void ClearCodecs()
        {
            mSynchonizables.Clear();
        }
    }
}