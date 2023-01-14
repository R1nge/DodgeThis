using Unity.Netcode;
using UnityEngine;

namespace Interpolations
{
    public class InterpolationRB : Interpolation
    {
        private NetworkVariable<Vector3> _velocity = new();
        private NetworkVariable<Vector3> _angularVelocity = new();
        private Rigidbody _rigidbody;

        protected override void Awake()
        {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override void Teleport(Vector3 pos, Quaternion rot, bool shouldStopMovement = false)
        {
            base.Teleport(pos, rot, shouldStopMovement);
            if (shouldStopMovement)
            {
                _velocity.Value = Vector3.zero;
                _angularVelocity.Value = Vector3.zero;
            }
        }

        protected override void UpdatePos(Vector3 previous, Vector3 current)
        {
            if (!IsClient) return;
            PositionSnapshot positionSnapshot =
                PositionSnapshots.Find(i => i.Tick == NetworkManager.Singleton.NetworkTickSystem.ServerTime.Tick);
            if (Vector3.Distance(positionSnapshot.Position, current) > PositionThreshold * 10.0)
            {
                transform.position = current;
                transform.rotation = Rotation.Value;
                _rigidbody.velocity = _velocity.Value;
                _rigidbody.angularVelocity = _angularVelocity.Value;
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

        protected override void SnapshotPositionOnTick()
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
                _velocity.Value = _rigidbody.velocity;
                _angularVelocity.Value = _rigidbody.angularVelocity;
                Position.Value = positionSnapshot.Position;
            }
        }

        protected override void Interpolate()
        {
            if (ShouldLerp)
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, PosLerpTo, LerpSpeed * Time.deltaTime);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Rotation.Value, 1f / 500f);
                _rigidbody.velocity = Vector3.MoveTowards(_rigidbody.velocity, _velocity.Value,
                    LerpSpeed * Time.deltaTime);
                _rigidbody.angularVelocity = Vector3.MoveTowards(_rigidbody.angularVelocity,
                    _angularVelocity.Value, LerpSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, PosLerpTo) < (double)PositionThreshold)
                    ShouldLerp = false;
            }

            if (Vector3.Distance(transform.position, PosLerpTo) > PositionThreshold * 50.0)
            {
                Debug.Log("[" + gameObject.name + "." + name + "]: Ultimate Pos-snap!");
                transform.position = PosLerpTo;
            }

            if (Vector3.Distance(transform.position, PosLerpTo) >= PositionThreshold ||
                _rigidbody.velocity.magnitude >= 0.5)
                return;
            transform.position = Vector3.MoveTowards(transform.position, PosLerpTo,
                (float)(LerpSpeed * (double)Time.deltaTime * 0.25));
        }
    }
}