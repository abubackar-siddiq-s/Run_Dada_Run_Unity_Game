using UnityEngine;

public class LightController : MonoBehaviour
{
    public float rotateSpeed;
    
    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(rotateSpeed * Vector3.right * Time.deltaTime);
    }
}
