using UnityEngine;

public class RaycastManager : MonoBehaviour
{
	public enum RaycastDirection
	{
		Forward,
		Back,
		Up,
		Down,
		Right,
		Left
	}

	public Transform DroneParent;
	public GameObject hitMarker;
	public RaycastDirection raycastDirection;
	public float offsetX;
	public float offsetY;
	public float offsetZ;

	RaycastHit hit;
	Vector3 rayDirection;



	private void Start()
	{
		if (raycastDirection == RaycastDirection.Forward)
			rayDirection = Vector3.forward;
		else if (raycastDirection == RaycastDirection.Back)
			rayDirection = Vector3.back;
		else if (raycastDirection == RaycastDirection.Up)
			rayDirection = Vector3.up;
		else if (raycastDirection == RaycastDirection.Down)
			rayDirection = Vector3.down;
		else if (raycastDirection == RaycastDirection.Right)
			rayDirection = Vector3.right;
		else if (raycastDirection == RaycastDirection.Left)
			rayDirection = Vector3.left;
	}

	void Update()
	{
		if (Physics.Raycast(transform.position, transform.TransformDirection(rayDirection), out hit))
		{
			Debug.DrawRay(transform.position, transform.TransformDirection(rayDirection) * hit.distance, Color.red);
			hitMarker.transform.position = new Vector3(hit.point.x + offsetX, hit.point.y + offsetY, hit.point.z + offsetZ);
			hitMarker.transform.rotation = DroneParent.rotation;
		}
		else
		{
			Debug.DrawRay(transform.position, transform.TransformDirection(rayDirection) * 1000, Color.white);
		}
	}
}