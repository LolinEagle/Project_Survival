using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour{
	const float             scale = 1f;

	const float             viwerMoveThresholdForChunkUpdate = 25f;
	const float             sqrViewerMoveThresholdForChunkUpdate = viwerMoveThresholdForChunkUpdate * viwerMoveThresholdForChunkUpdate;

	public LODInfo[]		detailLevels;
	public static float		maxViewDist;

	public Transform		viewer;
	public Material			mapMaterial;
	public static Vector2	viewerPos;
	Vector2					oldViewerPos;
	static MapGenerator		mapGenerator;
	static int				chunkSize;
	int						chunksVisibleInViewDist;

	public PrefabType		prefabType;

	Dictionary<Vector2, TerrainChunk>	terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	static List<TerrainChunk>			terrainChunkVisibleLastUpdate = new List<TerrainChunk>();

	private void	Start(){
		mapGenerator = FindAnyObjectByType<MapGenerator>();

		maxViewDist = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
		chunkSize = MapGenerator.mapChunkSize - 1;
		chunksVisibleInViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);

		UpdateVisibleChunks();
	}

	private void	Update(){
		viewerPos = new Vector2(viewer.position.x, viewer.position.z);

		if((oldViewerPos - viewerPos).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate){
			oldViewerPos = viewerPos;
			UpdateVisibleChunks();
		}
	}

	void	UpdateVisibleChunks(){
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

				} else {
					terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial, prefabType));
				}
			}
		}
	}

	public class TerrainChunk{
		GameObject		meshObject;
		Vector2			position;
		Bounds			bounds;

		MeshRenderer	meshRenderer;
		MeshFilter		meshFilter;
		MeshCollider	meshCollider;

		LODInfo[]		detailLevels;
		LODMesh[]		lodMeshes;
		LODMesh			collisionLODMesh;

		MapData			mapData;
		bool			mapDataReceived;
		int				previousLODIndex = -1;

		PrefabType		prefabType;

		public		TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material, PrefabType prefabType){
			this.detailLevels = detailLevels;
			this.prefabType = prefabType;

			position = coord * size;
			bounds = new Bounds(position, Vector2.one * size);

			Vector3	positionV3 = new Vector3(position.x, 0, position.y);

			meshObject = new GameObject("Terrain Chunk");
			meshRenderer = meshObject.AddComponent<MeshRenderer>();
			meshFilter = meshObject.AddComponent<MeshFilter>();
			meshCollider = meshObject.AddComponent<MeshCollider>();
			meshRenderer.material = material;

			meshObject.transform.position = positionV3 * scale;
			meshObject.transform.parent = parent;
			meshObject.transform.localScale = Vector3.one * scale;
			SetVisible(false);

			lodMeshes = new LODMesh[detailLevels.Length];
			for (int i = 0; i < detailLevels.Length; i++){
				lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
				if (detailLevels[i].useForCollider){
					collisionLODMesh = lodMeshes[i];
				}
			}

			mapGenerator.RequestMapData(position, OnMapDataReceived);
		}

		void		OnMapDataReceived(MapData mapData){
			this.mapData = mapData;
			mapDataReceived = true;
			meshRenderer.material.mainTexture = TextureGenerator.TextureFromColourMap(mapData.colourMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);

			Random.InitState(mapGenerator.seed + (int)position.x + (int)position.y);
			for (int i = 0; i < prefabType.numberToSpawn; i++){
				int rX = Random.Range(0, chunkSize);
				int rY = Random.Range(0, chunkSize);
				int centerX = 1 + rX;
				int centerY = mapData.heightMap.GetLength(1) - 2 - rY;

				float	height = mapGenerator.meshHeightMultiplier * mapGenerator.meshHeightCurve.Evaluate(mapData.heightMap[centerX, centerY]);
				Vector3	centerPos = new Vector3((position.x - chunkSize / 2f) + rX, height, (position.y - chunkSize / 2f) + rY);
				if (mapData.heightMap[centerX, centerY] < prefabType.item[0].height){
					Instantiate(prefabType.item[0].prefab, centerPos, Quaternion.identity, meshObject.transform);
				} else if (mapData.heightMap[centerX, centerY] < prefabType.item[1].height){
					Instantiate(prefabType.item[1].prefab, centerPos, Quaternion.identity, meshObject.transform);
				}
			}

			UpdateTerrainChunk();
		}

		void		OnMeshDataReceived(MeshData meshData){
			meshFilter.mesh = meshData.CreateMesh();
		}

		public void	UpdateTerrainChunk(){
			if (!mapDataReceived){
				return;
			}
			float	viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPos));
			bool	visible = viewerDstFromNearestEdge <= maxViewDist;

			if (visible){
				int	lodIndex = 0;

				for (int i = 0; i < detailLevels.Length - 1; i++){
					if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold){
						lodIndex = i + 1;
					} else {
						break;
					}
				}
				if (lodIndex != previousLODIndex){
					LODMesh	lodMesh = lodMeshes[lodIndex];

					if (lodMesh.hasMesh){
						previousLODIndex = lodIndex;
						meshFilter.mesh = lodMesh.mesh;
						meshCollider.sharedMesh = lodMesh.mesh;
					} else if (!lodMesh.hasRequestedMesh){
						lodMesh.RequestMesh(mapData);
					}
				}
				if (lodIndex == 0){
					if (collisionLODMesh.hasMesh){
						meshCollider.sharedMesh = collisionLODMesh.mesh;
					} else if (!collisionLODMesh.hasRequestedMesh){
						collisionLODMesh.RequestMesh(mapData);
					}
				}
				terrainChunkVisibleLastUpdate.Add(this);
			}
			SetVisible(visible);
		}

		public void	SetVisible(bool visible){
			meshObject.SetActive(visible);
		}

		public bool IsVisible(){
			return (meshObject.activeSelf);
		}
	}

	class LODMesh{
		public Mesh	mesh;
		public bool	hasRequestedMesh;
		public bool	hasMesh;
		int			lod;

		System.Action	updateCallback;	

		public		LODMesh(int lod, System.Action updateCallback){
			this.lod = lod;
			this.updateCallback = updateCallback;
		}

		void		OnMeshDataReceived(MeshData meshData){
			mesh = meshData.CreateMesh();
			hasMesh = true;

			updateCallback();
		}

		public void	RequestMesh(MapData mapData){
			hasRequestedMesh = true;
			mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
		}
	}

	[System.Serializable]
	public class LODInfo{
		public int		lod;
		public float	visibleDstThreshold;
		public bool		useForCollider;
	}

	[System.Serializable]
	public struct PrefabType{
		[System.Serializable]
		public struct PrefabTypeItem{
			public GameObject	prefab;
			public float		height;
		};

		public PrefabTypeItem[]	item;
		public int				numberToSpawn;
	}
}
