using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeCubes : MonoBehaviour
{
    private bool _collisionSet;
    public float param1 = 70f;
    public float param2 = 5f;

    public GameObject restartButton, explosion;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Cube" && !_collisionSet)
        {
            for(int i = collision.transform.childCount-1; i >= 0; i--)
            {
                Transform child = collision.transform.GetChild(i);
                child.gameObject.AddComponent<Rigidbody>();
                child.gameObject.GetComponent<Rigidbody>().AddExplosionForce(param1, Vector3.up, param2);
                child.SetParent(null);
            }
            restartButton.SetActive(true);
            
            //Camera.main.transform.localPosition -= new Vector3(0,0,5f);
            //Camera.main.transform.localRotation = Quaternion.RotateTowards(Camera.main.transform.localRotation, Quaternion.FromToRotation(Camera.main.transform.position, Vector3.zero), Time.deltaTime);
            Camera.main.gameObject.AddComponent<CameraShake>();

            GameObject newVfx = Instantiate(explosion, new Vector3(collision.contacts[0].point.x, collision.contacts[0].point.y, collision.contacts[0].point.z), Quaternion.identity) as GameObject;
            Destroy(newVfx, 2.5f);
            if (PlayerPrefs.GetString("music") == "Yes") GetComponent<AudioSource>().Play();

            Destroy(collision.gameObject);
            _collisionSet = true;
        }
    }
}