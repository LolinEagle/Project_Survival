using UnityEngine;

public class MapDisplay : MonoBehaviour{
	public Renderer		textureRenderer;
	public MeshFilter	meshFilter;
	public MeshRenderer	meshRenderer;

	public void	DrawTexture(Texture2D texture){
		if (textureRenderer == null) return;
		textureRenderer.sharedMaterial.mainTexture = texture;
		textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
	}

	public void	DrawMesh(MeshData meshData, Texture2D texture){
		if (meshFilter == null || meshRenderer == null) return;
		meshFilter.sharedMesh = meshData.CreateMesh();
		meshRenderer.sharedMaterial.mainTexture = texture;
	}
}
