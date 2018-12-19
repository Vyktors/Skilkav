using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {

    private PlayerManager playerManager;

    void Start()
    {
        playerManager = gameObject.GetComponent<PlayerManager>();
        
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        
        if(col.gameObject.tag == "Ground")
        {
            playerManager.isGrounded = true;            
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if(col.gameObject.tag == "Ground")
        {
            playerManager.isGrounded = false;
        }
    }
}
