using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour {

	// Use this for initialization
	[SerializeField] private GameObject targetObject;
	[SerializeField] private string targetMessage;
	public Color highlightColor = Color.cyan;
	private Vector3 defaultScale;

	void Start()
	{
		defaultScale = transform.localScale;
	}

	public void OnMouseEnter()
	{
		SpriteRenderer sprite = GetComponent<SpriteRenderer> ();
		if (sprite != null)
		{
			sprite.color = highlightColor;
		}
	}

	public void OnMouseExit()
	{
		SpriteRenderer sprite = GetComponent<SpriteRenderer> ();
		if (sprite != null) {
			sprite.color = Color.white;
		}
	}

	public void OnMouseDown()
	{
		transform.localScale = new Vector3 (defaultScale.x + 0.1f, defaultScale.y + 0.1f, defaultScale.z + 0.1f);
	}

	public void OnMouseUp()
	{
		transform.localScale = defaultScale;
		if (targetObject != null)
		{
			targetObject.SendMessage (targetMessage);
		}
	}
}
