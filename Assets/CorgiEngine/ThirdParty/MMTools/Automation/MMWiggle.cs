using UnityEngine;
using System.Collections;

namespace MoreMountains.Tools
{	
	public enum WiggleTypes { None, Random, PingPong }

	/// <summary>
	/// Add this class to a GameObject to be able to control its position/rotation/scale individually and periodically, allowing it to "wiggle" (or just move however you want on a periodic basis)
	/// </summary>
	public class MMWiggle : MonoBehaviour 
	{		
		[Header("Position")]
		/// the position mode : none, random or ping pong - none won't do anything, random will randomize min and max bounds, ping pong will oscillate between min and max bounds
		public WiggleTypes PositionMode = WiggleTypes.Random;
		/// if this is true, position will be ping ponged with an ease in/out curve
		public bool SmoothPositionPingPong = true;

		[Header("Position Frequency")]
		/// the minimum time (in seconds) between two position changes
		public float PositionFrequencyMin = 0;
		/// the maximum time (in seconds) between two position changes
		public float PositionFrequencyMax = 1;
		[Header("Position Amplitude")]
		/// the minimum position the object can have
		public Vector3 PositionAmplitudeMin = Vector3.zero;
		/// the maximum position the object can have
		public Vector3 PositionAmplitudeMax = Vector3.one;

		[Header("Rotation")]
		/// the rotation mode : none, random or ping pong - none won't do anything, random will randomize min and max bounds, ping pong will oscillate between min and max bounds
		public WiggleTypes RotationMode = WiggleTypes.Random;
		/// if this is true, rotation will be ping ponged with an ease in/out curve
		public bool SmoothRotationPingPong = true;

		[Header("Rotation Frequency")]
		/// the minimum time (in seconds) between two rotation changes
		public float RotationFrequencyMin = 0;
		/// the maximum time (in seconds) between two rotation changes
		public float RotationFrequencyMax = 1;
		[Header("Rotation Amplitude")]
		/// the object's minimum rotation
		public Vector3 RotationAmplitudeMin = Vector3.zero;
		/// the object's maximum rotation
		public Vector3 RotationAmplitudeMax = Vector3.one;

		[Header("Scale")]
		/// the scale mode : none, random or ping pong - none won't do anything, random will randomize min and max bounds, ping pong will oscillate between min and max bounds
		public WiggleTypes ScaleMode = WiggleTypes.Random;
		/// if this is true, scale will be ping ponged with an ease in/out curve
		public bool SmoothScalePingPong = true;

		[Header("Scale Frequency")]
		/// the minimum time (in seconds) between two scale changes
		public float ScaleFrequencyMin = 0;
		/// the maximum time (in seconds) between two scale changes
		public float ScaleFrequencyMax = 1;
		[Header("Scale Amplitude")]
		/// the object's minimum scale
		public Vector3 ScaleAmplitudeMin = Vector3.zero;
		/// the object's maximum scale
		public Vector3 ScaleAmplitudeMax = Vector3.one;

		[Header("Startup")]
		public bool StartWigglingAutomatically = true;

		protected Vector3 _startPosition;
		protected Quaternion _startRotation;
		protected Vector3 _startScale;

		protected float _randomPositionFrequency = 0;
		protected Vector3 _randomPositionAmplitude = Vector3.zero;

		protected float _randomRotationFrequency = 0;
		protected Vector3 _randomRotationAmplitude = Vector3.zero;

		protected float _randomScaleFrequency = 0;
		protected Vector3 _randomScaleAmplitude = Vector3.zero;

		protected float _positionTimer = 0;
		protected float _rotationTimer = 0;
		protected float _scaleTimer = 0;

		protected float _positionT = 0;
		protected float _rotationT = 0;
		protected float _scaleT = 0;

		protected Vector3 _newPosition = Vector3.zero;
		protected Quaternion _newRotation = Quaternion.identity;
		protected Vector3 _newScale = Vector3.zero;

		protected Vector3 _positionStartValue;
		protected Quaternion _rotationStartValue;
		protected Vector3 _scaleStartValue;

		protected Quaternion _rotationPingPongStart;
		protected Quaternion _rotationPingPongEnd;

		protected bool _wiggling = true;

		/// <summary>
		/// On Start() we trigger the initialization
		/// </summary>
		protected virtual void Start()
		{
			Initialization ();
		}

		/// <summary>
		/// On init we get the start values and trigger our coroutines for each property
		/// </summary>
		protected virtual void Initialization()
		{
			_startPosition = transform.localPosition;
			_startRotation = transform.localRotation;
			_startScale = transform.localScale;

			_positionStartValue = this.transform.localPosition;
			_rotationStartValue = this.transform.rotation;
			_scaleStartValue = this.transform.localScale;

			if (!StartWigglingAutomatically)
			{
				_wiggling = false;
			}

			StartWiggleCoroutines();
		}

		protected virtual void StartWiggleCoroutines()
		{
			if (PositionMode == WiggleTypes.Random)
			{
				StartCoroutine(RandomizePosition ());
			}

			if (RotationMode == WiggleTypes.Random)
			{
				StartCoroutine(RandomizeRotation ());
			}

			if (ScaleMode == WiggleTypes.Random)
			{
				StartCoroutine(RandomizeScale ());
			}

			if (PositionMode == WiggleTypes.PingPong)
			{
				_positionTimer = Time.time;
				StartCoroutine(PingPongPosition(PositionAmplitudeMin, PositionAmplitudeMax));
			}

			if (RotationMode == WiggleTypes.PingPong)
			{
				_rotationTimer = Time.time;
				StartCoroutine(PingPongRotation(RotationAmplitudeMin, RotationAmplitudeMax));
			}

			if (ScaleMode == WiggleTypes.PingPong)
			{
				_scaleTimer = Time.time;
				StartCoroutine(PingPongScale(ScaleAmplitudeMin, ScaleAmplitudeMax));
			}
		}

		/// <summary>
		/// Oscillates the position between min and max bounds
		/// </summary>
		/// <returns>The pong position.</returns>
		/// <param name="startPosition">Start position.</param>
		/// <param name="endPosition">End position.</param>
		protected virtual IEnumerator PingPongPosition(Vector3 startPosition, Vector3 endPosition)
		{			
			_randomPositionFrequency = UnityEngine.Random.Range(PositionFrequencyMin,PositionFrequencyMax);
			float i = _randomPositionFrequency;
			while((i > 0f) && _wiggling)
			{
				_positionT = (Time.time - _positionTimer) / _randomPositionFrequency;
				_positionT = SmoothPositionPingPong ? Mathf.SmoothStep(0f, 1f, _positionT) : _positionT;

				this.transform.localPosition = Vector3.Lerp(startPosition, endPosition, _positionT);
				i -= Time.deltaTime;
				yield return null;
			}
			if (startPosition == PositionAmplitudeMin)
			{
				_positionTimer = Time.time;
				StartCoroutine(PingPongPosition(PositionAmplitudeMax, PositionAmplitudeMin));
			}
			else
			{
				_positionTimer = Time.time;
				StartCoroutine(PingPongPosition(PositionAmplitudeMin, PositionAmplitudeMax));
			}
		}

		/// <summary>
		/// Oscillates the rotation between the min and max bounds
		/// </summary>
		/// <returns>The pong rotation.</returns>
		/// <param name="startRotation">Start rotation.</param>
		/// <param name="endRotation">End rotation.</param>
		protected virtual IEnumerator PingPongRotation(Vector3 startRotation, Vector3 endRotation)
		{			
			_randomRotationFrequency = UnityEngine.Random.Range(RotationFrequencyMin,RotationFrequencyMax);

			_rotationPingPongStart = Quaternion.Euler(startRotation);
			_rotationPingPongEnd = Quaternion.Euler(endRotation);

			float i = _randomRotationFrequency;
			while ((i > 0f) && _wiggling)
			{
				_rotationT = (Time.time - _rotationTimer) / _randomRotationFrequency;
				_rotationT = SmoothRotationPingPong ? Mathf.SmoothStep(0f, 1f, _rotationT) : _rotationT;

				this.transform.localRotation = Quaternion.Lerp(_rotationPingPongStart, _rotationPingPongEnd, _rotationT);
				i -= Time.deltaTime;
				yield return null;
			}
			if (startRotation == RotationAmplitudeMin)
			{
				_rotationTimer = Time.time;
				StartCoroutine(PingPongRotation(RotationAmplitudeMax, RotationAmplitudeMin));
			}
			else
			{
				_rotationTimer = Time.time;
				StartCoroutine(PingPongRotation(RotationAmplitudeMin, RotationAmplitudeMax));
			}
		}

		/// <summary>
		/// Oscillates the scale between min and max positions
		/// </summary>
		/// <returns>The pong scale.</returns>
		/// <param name="startScale">Start scale.</param>
		/// <param name="endScale">End scale.</param>
		protected virtual IEnumerator PingPongScale(Vector3 startScale, Vector3 endScale)
		{			
			_randomScaleFrequency = UnityEngine.Random.Range(ScaleFrequencyMin,ScaleFrequencyMax);
			float i = _randomScaleFrequency;
			while ((i > 0f) && _wiggling)
			{
				_scaleT = (Time.time - _scaleTimer) / _randomScaleFrequency;
				_scaleT = SmoothScalePingPong ? Mathf.SmoothStep(0f, 1f, _scaleT) : _scaleT;

				this.transform.localScale = Vector3.Lerp(startScale, endScale, _scaleT);
				i -= Time.deltaTime;
				yield return null;
			}
			if (startScale == ScaleAmplitudeMin)
			{
				_scaleTimer = Time.time;
				StartCoroutine(PingPongScale(ScaleAmplitudeMax, ScaleAmplitudeMin));
			}
			else
			{
				_scaleTimer = Time.time;
				StartCoroutine(PingPongScale(ScaleAmplitudeMin, ScaleAmplitudeMax));
			}
		}

		/// <summary>
		/// Every frame we update our object's position
		/// </summary>
		protected virtual void Update()
		{
			UpdatePosition ();
		}

		/// <summary>
		/// Lerps each property towards the new computed target position/rotation/scale
		/// </summary>
		protected virtual void UpdatePosition()
		{
			if (!_wiggling)
			{
				return;
			}

			if (PositionMode == WiggleTypes.Random)
			{
				if (_randomPositionFrequency > 0)
				{
					_positionT = (Time.time - _positionTimer) / _randomPositionFrequency;
					transform.localPosition = Vector3.Lerp (_positionStartValue, _newPosition, _positionT);				
				}
			}

			if (RotationMode == WiggleTypes.Random)
			{
				if (_randomRotationFrequency > 0)
				{
					_rotationT = (Time.time - _rotationTimer) / _randomRotationFrequency;
					transform.localRotation = Quaternion.Lerp (_rotationStartValue, _newRotation, _rotationT);
				}
			}

			if (ScaleMode == WiggleTypes.Random)
			{
				if (_randomScaleFrequency > 0)
				{
					_scaleT = (Time.time - _scaleTimer) / _randomScaleFrequency;
					transform.localScale = Vector3.Lerp (_scaleStartValue, _newScale, _scaleT);
				}
			}
		}

		/// <summary>
		/// Computes a new target position
		/// </summary>
		/// <returns>The position.</returns>
		protected virtual IEnumerator RandomizePosition()
		{
			while (true)
			{
				// if it's time to decide of a new position
				_randomPositionFrequency = UnityEngine.Random.Range(PositionFrequencyMin,PositionFrequencyMax);

				_randomPositionAmplitude.x = UnityEngine.Random.Range(PositionAmplitudeMin.x,PositionAmplitudeMax.x);
				_randomPositionAmplitude.y = UnityEngine.Random.Range(PositionAmplitudeMin.y,PositionAmplitudeMax.y);
				_randomPositionAmplitude.z = UnityEngine.Random.Range(PositionAmplitudeMin.z,PositionAmplitudeMax.z);

				_newPosition = _startPosition + _randomPositionAmplitude;
				_positionStartValue = this.transform.localPosition;
				_positionTimer = Time.time;

				yield return new WaitForSeconds (_randomPositionFrequency*0.2f);
			}
		}

		/// <summary>
		/// Computes a new target rotation
		/// </summary>
		/// <returns>The rotation.</returns>
		protected virtual IEnumerator RandomizeRotation()
		{
			while (true)
			{
				// if it's time to decide of a new position
				_randomRotationFrequency = UnityEngine.Random.Range(RotationFrequencyMin,RotationFrequencyMax);

				_randomRotationAmplitude.x = UnityEngine.Random.Range(RotationAmplitudeMin.x,RotationAmplitudeMax.x);
				_randomRotationAmplitude.y = UnityEngine.Random.Range(RotationAmplitudeMin.y,RotationAmplitudeMax.y);
				_randomRotationAmplitude.z = UnityEngine.Random.Range(RotationAmplitudeMin.z,RotationAmplitudeMax.z);

				_newRotation = Quaternion.Euler(_randomRotationAmplitude);
				_rotationStartValue = this.transform.rotation;
				_rotationTimer = Time.time;

				yield return new WaitForSeconds (_randomRotationFrequency*0.2f);
			}
		}

		/// <summary>
		/// Computes a new target scale
		/// </summary>
		/// <returns>The scale.</returns>
		protected virtual IEnumerator RandomizeScale()
		{
			while (true)
			{
				// if it's time to decide of a new position
				_randomScaleFrequency = UnityEngine.Random.Range(ScaleFrequencyMin,ScaleFrequencyMax);

				_randomScaleAmplitude.x = UnityEngine.Random.Range(ScaleAmplitudeMin.x,ScaleAmplitudeMax.x);
				_randomScaleAmplitude.y = UnityEngine.Random.Range(ScaleAmplitudeMin.y,ScaleAmplitudeMax.y);
				_randomScaleAmplitude.z = UnityEngine.Random.Range(ScaleAmplitudeMin.z,ScaleAmplitudeMax.z);

				_newScale = _startScale + _randomScaleAmplitude;
				_scaleStartValue = this.transform.localScale;
				_scaleTimer = Time.time;

				yield return new WaitForSeconds (_randomScaleFrequency);
			}
		}
	}
}