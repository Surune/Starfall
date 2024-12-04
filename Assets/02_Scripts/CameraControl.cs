using UnityEngine;

public class CameraControl : MonoBehaviour {
    public GameObject player;
    public float offset;
    public float sizebeforemovement;
    public float sizewhilemovement;
    private Vector3 playerPosition;
    public float offsetSmoothing;
	
	void Update () {
        playerPosition = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        if(player.transform.localScale.x>0f)
        {
            playerPosition = new Vector3(playerPosition.x - offset, playerPosition.y, playerPosition.z);
        }
        else
        {
            playerPosition = new Vector3(playerPosition.x + offset, playerPosition.y, playerPosition.z);
        }
        transform.position = Vector3.Lerp(transform.position, playerPosition, offsetSmoothing * Time.deltaTime);
        if (Mathf.Abs(player.GetComponent<Rigidbody2D>().velocity.x) > 0f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1f, 1f, 1f), offsetSmoothing * Time.deltaTime);
            Camera.main.orthographicSize = sizewhilemovement*transform.localScale.x;
        }
        else
        {
            Invoke("sizeupcamera", 1f);
        }
    }
    
    void sizeupcamera()
    {
        if (Mathf.Abs(player.GetComponent<Rigidbody2D>().velocity.x) < 0.1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(sizebeforemovement / sizewhilemovement, sizebeforemovement / sizewhilemovement, 1f), offsetSmoothing * Time.deltaTime);
            Camera.main.orthographicSize = sizewhilemovement * transform.localScale.x;
        }
    }
}
