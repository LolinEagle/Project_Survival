using UnityEngine;

public static class Noise{
	public enum	NormalizeMode{Local, Global}

	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistanse, float lacunarity, Vector2 offset, NormalizeMode normalizeMode){
		float[,]		noiseMap = new float [mapWidth, mapHeight];

		float			maxLocalNoiseHeight = float.MinValue;
		float			minLocalNoiseHeight = float.MaxValue;
		float			halfWidth = mapWidth / 2;
		float			halfHeight = mapHeight / 2;

		System.Random	prng = new System.Random(seed);
		Vector2[]		octaveOffsets = new Vector2[octaves];

		float	maxPossibleHeight = 0;
		float	amplitude = 1;
		float	frequency = 1;

		for (int i = 0; i < octaveOffsets.Length; i++){
			float offsetX = prng.Next(-100000, 100000) + offset.x;
			float offsetY = prng.Next(-100000, 100000) - offset.y;

			octaveOffsets[i] = new Vector2(offsetX, offsetY);
			maxPossibleHeight += amplitude;
			amplitude *= persistanse;
		}
		if (scale <= 0){
			scale = 0.0001f;
		}
		for (int y = 0; y < mapHeight; y++){
			for (int x = 0; x < mapWidth; x++){
				amplitude = 1;
				frequency = 1;
				float	noiseHeight = 0;

				for (int i = 0; i < octaves; i++){
					float	sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
					float	sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;
					float	perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

					noiseHeight += perlinValue * amplitude;
					amplitude *= persistanse;
					frequency *= lacunarity;
				}
				if (noiseHeight > maxLocalNoiseHeight){
					maxLocalNoiseHeight = noiseHeight;
				} else if (noiseHeight < minLocalNoiseHeight){
					minLocalNoiseHeight = noiseHeight;
				}
				noiseMap[x, y] = noiseHeight;
			}
		}
		for (int y = 0; y < mapHeight; y++){
			for (int x = 0; x < mapWidth; x++){
				if (normalizeMode == NormalizeMode.Local) {
					noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
				} else {
					float	normalizeHeight = (noiseMap[x, y] + 1) / maxPossibleHeight;
					noiseMap[x, y] = Mathf.Clamp(normalizeHeight, 0, int.MaxValue);
				}
			}
		}
		return (noiseMap);
	}
}
