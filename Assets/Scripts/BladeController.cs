using UnityEngine;

public class BladeController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float rotateSpeed;

    void Start()
    {
        
    }

    void Update(){
        transform.Rotate(rotateSpeed * Vector3.down * Time.deltaTime);
    }
}
