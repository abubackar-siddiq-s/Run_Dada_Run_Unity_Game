using UnityEngine;

public class CoinController : MonoBehaviour
{
    
    public float rotateSpeed;

    void Start()
    {
        
    }

    void Update(){
        transform.Rotate(rotateSpeed * Vector3.forward * Time.deltaTime);
    }
}
