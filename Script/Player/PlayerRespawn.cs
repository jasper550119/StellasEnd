using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{

    [SerializeField] private AudioClip cheackpointSound;
    private Transform currentCheckpoint;
    private PlayerHP playerHealth;
    private PlayerShoot Gun;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GetComponent<PlayerHP>();
        Gun = GetComponent<PlayerShoot>();
    }

    public void Respawn()
    {
        transform.position = currentCheckpoint.position;
        playerHealth.Respawn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Cheackpoint")
        {
            currentCheckpoint = collision.transform;
            //SoundManager.instance.PlaySound(cheackpointSound);
            collision.GetComponent<Collider2D>().enabled = false;
            Gun.Reload();
            Debug.Log("1");
        }
    }
}
