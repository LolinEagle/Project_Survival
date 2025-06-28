using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdatableData{
	public float			uniformScale = 1f;

	public bool				useFlatShading;
	public bool				useFalloff;

	public float			meshHeightMultiplier;
	public AnimationCurve	meshHeightCurve;
}
