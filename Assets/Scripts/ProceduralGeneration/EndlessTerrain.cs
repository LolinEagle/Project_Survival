using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour{
	public const float		maxViewDist = 500;
	public Transform		viewer;
	public static Vector2	viewerPos;
	int						chunkSize;
	int						chunksVisibleInViewDist;

	Dictionary<Vector2, TerrainChunk>	terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	List<TerrainChunk>					terrainChunkVisibleLastUpdate = new List<TerrainChunk>();

	private void	Start(){
		chunkSize = MapGenerator.mapChunkSize - 1;
		chunksVisibleInViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);
	}

	private void	Update(){
		viewerPos = new Vector2(viewer.position.x, viewer.position.z);
		UpdateVisibleChunk();
	}

	void	UpdateVisibleChunk(){
		for (int i = 0; i < terrainChunkVisibleLastUpdate.Count; i++){
			terrainChunkVisibleLastUpdate[i].SetVisible(false);
		}
		terrainChunkVisibleLastUpdate.Clear();

		int	currentChunkCoordX = Mathf.RoundToInt(viewerPos.x / chunkSize);
		int	currentChunkCoordY = Mathf.RoundToInt(viewerPos.y / chunkSize);

		for (int yOffset = -chunksVisibleInViewDist; yOffset <= chunksVisibleInViewDist; yOffset++){
			for (int xOffset = -chunksVisibleInViewDist; xOffset <= chunksVisibleInViewDist; xOffset++){
				Vector2	viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

				if (terrainChunkDictionary.ContainsKey(viewedChunkCoord)){
					terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
					if (terrainChunkDictionary[viewedChunkCoord].IsVisible()){
						terrainChunkVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
					}
				} else {
					terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform));
				}
			}
		}
	}

	public class TerrainChunk{
		GameObject	meshObject;
		Vector2		position;
		Bounds		bounds;

		public TerrainChunk(Vector2 coord, int size, Transform parent){
			position = coord * size;
			bounds = new Bounds(position, Vector2.one * size);

			Vector3	positionV3 = new Vector3(position.x, 0, position.y);

			meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
			meshObject.transform.position = positionV3;
			meshObject.transform.localScale = Vector3.one * size / 10f;
			meshObject.transform.parent = parent;
			SetVisible(false);
		}

		public void	UpdateTerrainChunk(){
			float	viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPos));
			bool	visible = viewerDstFromNearestEdge <= maxViewDist;

			SetVisible(visible);
		}

		public void	SetVisible(bool visible){
			meshObject.SetActive(visible);
		}

		public bool IsVisible(){
			return (meshObject.activeSelf);
		}
	}
}
