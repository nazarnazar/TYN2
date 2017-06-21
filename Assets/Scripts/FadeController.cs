using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fade controller.
/// Script to fade out panel when we get to our target.
/// On object: Fade Panel.
/// Uses: -.
/// </summary>

public class FadeController : MonoBehaviour {

	Image image;
	Color color;

	public float fadeInSpeed;
	public float fadeOutSpeed;


	void Start ()
	{
		image = GetComponent<Image> ();
		color = image.color;
	}

	public IEnumerator FadeInPanel(float partSize, Color c)
	{
		// color = c;

		while (color.a < 0.75f)
		{
			float timer = Time.deltaTime;

			while (timer > 0)
			{
				timer -= Time.deltaTime;
				yield return null;
			}

			color = new Color (color.r, color.g, color.b, color.a + partSize * fadeInSpeed * Time.deltaTime);
			image.color = color;
		}

		StartCoroutine (FadeOutPanel ());
	}

	IEnumerator FadeOutPanel()
	{
		while (color.a > 0)
		{
			float timer = Time.deltaTime;

			while (timer > 0)
			{
				timer -= Time.deltaTime;
				yield return null;
			}

			color = new Color (color.r, color.g, color.b, color.a - fadeOutSpeed * Time.deltaTime);
			image.color = color;
		}

		gameObject.SetActive (false);
	}
}
