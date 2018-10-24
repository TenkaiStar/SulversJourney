using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.Tools
{	
	/// <summary>
	/// Add this bar to an object and link it to a bar (possibly the same object the script is on), and you'll be able to resize the bar object based on a current value, located between a min and max value.
	/// See the HealthBar.cs script for a use case
	/// </summary>
	public class MMProgressBar : MonoBehaviour
	{
		[Header("Foreground Bar Settings")]
		/// whether or not the foreground bar should lerp
		public bool LerpForegroundBar = true;
		/// the speed at which to lerp the foreground bar
		public float LerpForegroundBarSpeed = 15f;

		[Header("Delayed Bar Settings")]
		/// the delay before the delayed bar moves (in seconds)
		public float Delay = 1f;
		/// whether or not the delayed bar's animation should lerp
		public bool LerpDelayedBar = true;
		/// the speed at which to lerp the delayed bar
		public float LerpDelayedBarSpeed = 15f;

		[Header("Bindings")]
		/// optional - the ID of the player associated to this bar
		public string PlayerID;
		/// the delayed bar
		public Transform DelayedBar;
		/// the main, foreground bar
		public Transform ForegroundBar;

		[Header("Bump")]
		/// whether or not the bar should "bump" when changing value
		public bool BumpScaleOnChange = true;
		/// the duration of the bump animation
		public float BumpDuration = 0.2f;
		/// whether or not the bar should flash when bumping
		public bool ChangeColorWhenBumping = true;
		/// the color to apply to the bar when bumping
		public Color BumpColor = Color.white;
		/// the curve to map the bump animation on
		public AnimationCurve BumpAnimationCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.3f, 1.2f), new Keyframe(1f, 1f));
		/// the current progress of the bar
		public float BarProgress { get; protected set; }
		/// whether or not the bar is bumping right now
		public bool Bumping { get; protected set; }

		protected Vector3 _targetLocalScale = Vector3.one;
		protected float _newPercent;
		protected float _lastDamageTimestamp;
		protected bool _bump = false;
		protected Color _initialColor;
		protected Image _foregroundImage;

		/// <summary>
		/// On start we store our image component
		/// </summary>
		protected virtual void Start()
		{
			_foregroundImage = ForegroundBar.GetComponent<Image>();
		}

		/// <summary>
		/// On Update we update our bars
		/// </summary>
		protected virtual void Update()
		{
			UpdateFrontBar();
			UpdateDelayedBar();
		}

		/// <summary>
		/// Updates the front bar's scale
		/// </summary>
		protected virtual void UpdateFrontBar()
		{
			if (ForegroundBar != null)
			{
				if (LerpForegroundBar)
				{
					ForegroundBar.localScale = Vector3.Lerp(ForegroundBar.localScale, _targetLocalScale, Time.deltaTime * LerpForegroundBarSpeed);
				}
				else
				{
					ForegroundBar.localScale = _targetLocalScale;
				}
			}
		}

		/// <summary>
		/// Updates the delayed bar's scale
		/// </summary>
		protected virtual void UpdateDelayedBar()
		{
			if (DelayedBar != null)
			{
				if (Time.time - _lastDamageTimestamp > Delay)
				{
					if (LerpDelayedBar)
					{
						DelayedBar.localScale = Vector3.Lerp(DelayedBar.localScale, _targetLocalScale, Time.deltaTime * LerpDelayedBarSpeed);
					}
					else
					{
						DelayedBar.localScale = _targetLocalScale;
					}
				}
			}
		}

		/// <summary>
		/// Updates the bar's values based on the specified parameters
		/// </summary>
		/// <param name="currentValue">Current value.</param>
		/// <param name="minValue">Minimum value.</param>
		/// <param name="maxValue">Max value.</param>
		public virtual void UpdateBar(float currentValue,float minValue,float maxValue)
		{
			_newPercent = MMMaths.Remap(currentValue,minValue,maxValue,0,1);
			BarProgress = _newPercent;
			_targetLocalScale.x = _newPercent;
			_lastDamageTimestamp = Time.time;
		}

		/// <summary>
		/// Triggers a camera bump
		/// </summary>
		public virtual void Bump()
		{
			if (!BumpScaleOnChange)
			{
				return;
			}
			if (this.gameObject.activeInHierarchy)
			{
				StartCoroutine(BumpCoroutine());
			}
		}

		/// <summary>
		/// A coroutine that (usually quickly) changes the scale of the bar 
		/// </summary>
		/// <returns>The coroutine.</returns>
		protected virtual IEnumerator BumpCoroutine()
		{

			float journey = 0f;
			Bumping = true;
			if (_foregroundImage != null)
			{
				_initialColor = _foregroundImage.color;
			}

			while (journey <= BumpDuration)
			{
				journey = journey + Time.deltaTime;
				float percent = Mathf.Clamp01(journey / BumpDuration);
				float curvePercent = BumpAnimationCurve.Evaluate(percent);
				this.transform.localScale = curvePercent * Vector3.one;

				if (ChangeColorWhenBumping && (_foregroundImage != null))
				{
					_foregroundImage.color = Color.Lerp(_initialColor, BumpColor, curvePercent);
				}

				yield return null;
			}
			Bumping = false;
			yield break;

		}
	}
}