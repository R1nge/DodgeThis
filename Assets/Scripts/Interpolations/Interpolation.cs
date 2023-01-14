using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Interpolations
{
    public class Interpolation : NetworkBehaviour
    {
        protected NetworkVariable<Vector3> Position = new();
        protected NetworkVariable<Quaternion> Rotation = new();
        protected List<PositionSnapshot> PositionSnapshots;
        protected Vector3 PosLerpTo;
        protected const float LerpSpeed = 0.75f;
        protected const float PositionThreshold = 0.25f;
        protected bool ShouldLerp;

        public virtual void Teleport(Vector3 pos, Quaternion rot, bool shouldStopMovement = false)
        {
            if (!IsServer) return;
            Debug.Log("[" + ToString() + "]: Teleporting!");
            Position.Value = pos;
            Rotation.Value = rot;

            transform.position = pos;
            transform.rotation = rot;
            PosLerpTo = pos;
        }

        protected virtual void Awake()
        {
            Position.OnValueChanged += UpdatePos;
            PositionSnapshots = new List<PositionSnapshot>();
            PosLerpTo = transform.position;
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner || IsServer)
            {
                NetworkManager.Singleton.NetworkTickSystem.Tick += SnapshotPositionOnTick;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner || IsServer)
            {
                NetworkManager.Singleton.NetworkTickSystem.Tick -= SnapshotPositionOnTick;
            }
        }

        protected virtual void UpdatePos(Vector3 previous, Vector3 current)
        {
            if (!IsClient) return;
            PositionSnapshot positionSnapshot =
                PositionSnapshots.Find(i => i.Tick == NetworkManager.Singleton.NetworkTickSystem.ServerTime.Tick);
            if (Vector3.Distance(positionSnapshot.Position, current) > PositionThreshold * 10.0)
            {
                transform.position = current;
                transform.rotation = Rotation.Value;
            }
            else if (Vector3.Distance(positionSnapshot.Position, current) > (double)PositionThreshold)
            {
                ShouldLerp = true;
            }

            PosLerpTo = current;
            if (Quaternion.Angle(positionSnapshot.Rotation, Rotation.Value) > 2.0)
            {
                transform.rotation = Rotation.Value;
            }
        }

        protected virtual void SnapshotPositionOnTick()
        {
            PositionSnapshot positionSnapshot = new PositionSnapshot
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Tick = NetworkManager.Singleton.NetworkTickSystem.LocalTime.Tick
            };

            if (IsOwner)
            {
                PositionSnapshots.Add(positionSnapshot);
            }

            if (IsServer)
            {
                Rotation.Value = positionSnapshot.Rotation;
                Position.Value = positionSnapshot.Position;
            }
        }

        private void Update()
        {
            if (IsServer && !IsHost) return;
            Interpolate();
        }

        protected virtual void Interpolate()
        {
            if (ShouldLerp)
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, PosLerpTo, LerpSpeed * Time.deltaTime);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Rotation.Value, 1f / 500f);
                if (Vector3.Distance(transform.position, PosLerpTo) < (double)PositionThreshold)
                    ShouldLerp = false;
            }

            if (Vector3.Distance(transform.position, PosLerpTo) > PositionThreshold * 50.0)
            {
                Debug.Log("[" + gameObject.name + "." + name + "]: Ultimate Pos-snap!");
                transform.position = PosLerpTo;
            }

            if (Vector3.Distance(transform.position, PosLerpTo) < PositionThreshold)
            {
                transform.position = Vector3.MoveTowards(transform.position, PosLerpTo,
                    (float)(LerpSpeed * (double)Time.deltaTime * 0.25));
            }
        }
    }
}