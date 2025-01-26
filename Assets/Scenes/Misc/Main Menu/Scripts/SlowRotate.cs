using UnityEngine;

public class SlowRotate : MonoBehaviour
{
  [SerializeField] private float rotationSpeed;
  void Update()
  {
    transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.forward, Space.Self);
  }
}
