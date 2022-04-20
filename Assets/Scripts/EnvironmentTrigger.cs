using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentTrigger : MonoBehaviour
{
    public PrincessController princess;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (tag == "SoundTrigger" && collision.gameObject.CompareTag("Noisy") && GameManager.S.gameState == GameState.sleeping)
        {
            Debug.Log("Princess was woken up by noise from " + collision.gameObject.name);
            princess.WakeUp();
        }
        if (tag == "TempTrigger" && collision.gameObject.CompareTag("Cold") && GameManager.S.gameState == GameState.sleeping)
        {
            Debug.Log("Princess was woken up by chills from " + collision.gameObject.name);
            princess.WakeUp();
        }
    }
}
