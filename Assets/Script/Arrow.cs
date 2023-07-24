using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float m_arrowSpeed;
    private float timer;
    // Start is called before the first frame update
    void OnEnable()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localScale.x>0){
            transform.Translate(Vector3.right * m_arrowSpeed * Time.deltaTime);
        }
        else{
            transform.Translate(Vector3.left * m_arrowSpeed * Time.deltaTime);
        }

        if(timer>2){
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
        else{
            timer += Time.deltaTime;
        }
    }
    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(3);
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }
    
}
