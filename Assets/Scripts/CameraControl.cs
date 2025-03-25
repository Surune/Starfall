using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject Player;
    public float Offset;
    public float Sizebeforemovement;
    public float Sizewhilemovement;
    public float OffsetSmoothing;
    Vector3 _playerPosition;

	void Update()
    {
        _playerPosition = new Vector3(Player.transform.position.x, Player.transform.position.y, transform.position.z);

        if (Player.transform.localScale.x>0f)
        {
            _playerPosition = new Vector3(_playerPosition.x - Offset, _playerPosition.y, _playerPosition.z);
        }
        else
        {
            _playerPosition = new Vector3(_playerPosition.x + Offset, _playerPosition.y, _playerPosition.z);
        }

        transform.position = Vector3.Lerp(transform.position, _playerPosition, OffsetSmoothing * Time.deltaTime);

        if (Mathf.Abs(Player.GetComponent<Rigidbody2D>().velocity.x) > 0f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1f, 1f, 1f), OffsetSmoothing * Time.deltaTime);
            Camera.main.orthographicSize = Sizewhilemovement*transform.localScale.x;
        }
        else
        {
            Invoke(nameof(SizeUpCamera), 1f);
        }
    }

    void SizeUpCamera()
    {
        if (Mathf.Abs(Player.GetComponent<Rigidbody2D>().velocity.x) < 0.1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(Sizebeforemovement / Sizewhilemovement, Sizebeforemovement / Sizewhilemovement, 1f), OffsetSmoothing * Time.deltaTime);
            Camera.main.orthographicSize = Sizewhilemovement * transform.localScale.x;
        }
    }
}
