using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CubeController : MonoBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;
    public float scaleSpeed;
    public Vector3 scaleDirection;
    public int score;
    public int health;
    public TextMeshProUGUI scoreTextObj;
    public TextMeshProUGUI healthTextObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 30;
        score = 0;
        scoreTextObj.text = "Score: " + score;
        health = 100;
        healthTextObj.text = "Health: " + health;
        // Debug.Log("Start Executed!!!!");
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.wKey.isPressed)
            transform.Translate(moveSpeed * Vector3.back * Time.deltaTime);

        if(Keyboard.current.sKey.isPressed)
            transform.Translate(moveSpeed * Vector3.forward * Time.deltaTime);

        if(Keyboard.current.dKey.isPressed)
            transform.Translate(moveSpeed * Vector3.left * Time.deltaTime);

        if(Keyboard.current.aKey.isPressed)
            transform.Translate(moveSpeed * Vector3.right * Time.deltaTime);


        // if(Keyboard.current.upArrowKey.wasPressedThisFrame) 
        //     transform.Translate(moveSpeed * Vector3.forward * Time.deltaTime);

        // Debug.Log("Update Executed!!!!");
        // transform.position += moveSpeed * new Vector3(0, 0, 1);
        // transform.Translate(moveSpeed * Vector3.right * Time.deltaTime);
        // transform.Rotate(rotateSpeed * Vector3.up * Time.deltaTime);
        // transform.localScale += scaleSpeed * scaleDirection * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision){
        if(collision.gameObject.tag == "Enemy"){
            health -= 20;
            healthTextObj.text = "Health: " + health;
            if(health <= 0){
                Destroy(gameObject);
            }
        }
        if(collision.gameObject.tag == "Enemy 2"){
            health -= 20;
            healthTextObj.text = "Health: " + health;
            if(health <= 0){
                Destroy(gameObject);
            }
        }
        // Debug.Log(collision.gameObject.name);
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Coins"){
            score++;
            scoreTextObj.text = "Score: " + score;
            Destroy(other.gameObject);
        }
    }
}
