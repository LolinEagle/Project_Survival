using UnityEngine;

public class BuildingGrid : MonoBehaviour{
	[SerializeField] private float	sizeX;
	[SerializeField] private float	sizeY;
	[SerializeField] private float	sizeZ;

	public Vector3 GetNearestPointOnGrid(Vector3 position){
		int	xCount = Mathf.RoundToInt(position.x / sizeX);
		int	yCount = Mathf.RoundToInt(position.y / sizeY);
		int	zCount = Mathf.RoundToInt(position.z / sizeZ);

		return (new Vector3(xCount * sizeX, yCount * sizeY, zCount * sizeZ));
	}
}
