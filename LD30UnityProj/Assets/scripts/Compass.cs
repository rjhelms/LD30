using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour {

	public Sprite SpriteW;
	public Sprite SpriteNW;
	public Sprite SpriteN;
	public Sprite SpriteNE;
	public Sprite SpriteE;
	public Sprite SpriteSE;
	public Sprite SpriteS;
	public Sprite SpriteSW;

	private SpriteRenderer my_Renderer;

	void Start()
	{
		my_Renderer = transform.GetComponent<SpriteRenderer>();
	}

	public void PointCompass(Sprite compass_point)
	{
		my_Renderer.sprite = compass_point;
	}
}
