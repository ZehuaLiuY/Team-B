using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public int bulletCount = 10;

    public GameObject bulletPrefab;
    public Transform bulletTf;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Attack()
    {
        GameObject bulletObj = Instantiate(bulletPrefab);
        bulletObj.transform.position = bulletTf.transform.position;
        bulletObj.GetComponent<Rigidbody>().AddForce(transform.forward * 100, ForceMode.Impulse);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
