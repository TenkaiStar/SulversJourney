using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace MoreMountains.Tools
{
    /// <summary>
    /// Add this component to an object and it'll get moved towards the target at update, with or without interpolation based on your settings
    /// </summary>
    public class MMFollowTarget : MonoBehaviour
    {
        [Header("Target")]
        /// the target to follow
        public Transform Target;
        /// the offset to apply to the followed target
        public Vector3 Offset;
        [ReadOnly]
        /// whether or not the object is currently following its target
        public bool Following = true;

        [Header("Interpolation")]
        /// whether or not we need to interpolate the movement
        public bool InterpolateMovement = true;
        /// the speed at which to interpolate the follower's movement
        public float InterpolationSpeed = 10f;

        /// the possible update modes
        public enum Modes { Update, FixedUpdate, LateUpdate }
        [Header("Mode")]
        /// the update at which the movement happens
        public Modes UpdateMode = Modes.Update;

        [Header("Axis")]
        /// whether this object should follow its target on the X axis
        public bool FollowX = true;
        /// whether this object should follow its target on the Y axis
        public bool FollowY = true;
        /// whether this object should follow its target on the Z axis
        public bool FollowZ = true;
        
        protected Vector3 _newTarget;
        protected Vector3 _initialPosition;

        /// <summary>
        /// On start we store our initial position
        /// </summary>
        protected virtual void Start()
        {
            SetInitialPosition();
        }

        /// <summary>
        /// Prevents the object from following the target anymore
        /// </summary>
        public virtual void StopFollowing()
        {
            Following = false;
        }

        /// <summary>
        /// Makes the object follow the target
        /// </summary>
        public virtual void StartFollowing()
        {
            Following = true;
            SetInitialPosition();
        }

        /// <summary>
        /// Stores the initial position
        /// </summary>
        protected virtual void SetInitialPosition()
        {
            _initialPosition = this.transform.position;
        }

        /// <summary>
        /// At update we follow our target 
        /// </summary>
        protected virtual void Update()
        {
            if (UpdateMode == Modes.Update)
            {
                FollowTarget();
            }
        }

        /// <summary>
        /// At fixed update we follow our target 
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if (UpdateMode == Modes.FixedUpdate)
            {
                FollowTarget();
            }
        }

        /// <summary>
        /// At late update we follow our target 
        /// </summary>
        protected virtual void LateUpdate()
        {
            if (UpdateMode == Modes.LateUpdate)
            {
                FollowTarget();
            }
        }

        /// <summary>
        /// Follows the target, lerping the position or not based on what's been defined in the inspector
        /// </summary>
        protected virtual void FollowTarget()
        {
            if (Target == null)
            {
                return;
            }

            if (!Following)
            {
                return;
            }

            _newTarget = Target.position + Offset;
            if (!FollowX) { _newTarget.x = _initialPosition.x; }
            if (!FollowY) { _newTarget.y = _initialPosition.y; }
            if (!FollowZ) { _newTarget.z = _initialPosition.z; }

            if (InterpolateMovement)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, _newTarget, Time.deltaTime * InterpolationSpeed);
            }
            else
            {
                this.transform.position = _newTarget;
            }
        }
    }
}