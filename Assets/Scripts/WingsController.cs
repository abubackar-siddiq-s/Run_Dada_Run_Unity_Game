using UnityEngine;

public class WingsController : MonoBehaviour
{
    public float rotateSpeed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotateSpeed * Vector3.forward * Time.deltaTime);
    }
}
