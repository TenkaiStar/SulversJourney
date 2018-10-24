using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace MoreMountains.Tools
{
	/// <summary>
	/// An event to trigger to flash something for a frame
	/// </summary>
	public struct MMFlashEvent
	{
		/// the color to apply to the flash
		public Color FlashColor;
		/// <summary>
		/// Initializes a new instance of the <see cref="MoreMountains.Tools.MMFlashEvent"/> struct.
		/// </summary>
		/// <param name="flashColor">Flash color.</param>
		public MMFlashEvent(Color flashColor)
		{
			FlashColor = flashColor;
		}
	}

	[RequireComponent(typeof(Image))]
	/// <summary>
	/// Add this class to an image and it'll flash when getting a MMFlashEvent
	/// </summary>
	public class MMFlash : MonoBehaviour, MMEventListener<MMFlashEvent> 
	{
		protected Image _image;
		protected bool _flashActive = false;

		/// <summary>
		/// On start we grab our image component
		/// </summary>
		protected virtual void Start()
		{
			_image = GetComponent<Image>();
		}

		/// <summary>
		/// On update we flash our image if needed
		/// </summary>
		protected virtual void Update()
		{
			if (_flashActive)
			{
				_image.enabled = true;
				_flashActive = false;
			}
			else
			{
				_image.enabled = false;
			}
		}

		/// <summary>
		/// When getting a flash event, we turn our image on
		/// </summary>
		public void OnMMEvent(MMFlashEvent flashEvent)
		{
			_flashActive = true;
			_image.material.color = flashEvent.FlashColor;
		} 

		/// <summary>
		/// On enable we start listening for events
		/// </summary>
		protected virtual void OnEnable()
		{
			this.MMEventStartListening<MMFlashEvent>();
		}

		/// <summary>
		/// On disable we stop listening for events
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<MMFlashEvent>();
		}		
	}
}