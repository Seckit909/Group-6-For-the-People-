using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundHandler : MonoBehaviour
{
	[SerializeField] PlayerData playerData;
	[SerializeField, Range(-100f, 0f)] float maxDepth = -60f;

	SpriteRenderer spriteRenderer;

	const float MIN_DEPTH = 0;

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}


	void Update()
	{
		Color.RGBToHSV(spriteRenderer.color, out float hueColor, out float saturationColor, out float valueColor);
        valueColor = Mathf.InverseLerp(maxDepth, MIN_DEPTH, playerData.PlayerPosition.y);
		spriteRenderer.color = Color.HSVToRGB(hueColor, saturationColor, valueColor);
    }
}