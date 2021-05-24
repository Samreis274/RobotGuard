using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour {
    //float para velicidade
	float speed = 20.0F;
    //float da velocidade da rotacao
    float rotationSpeed = 120.0F;
    // gameobject da bala
    public GameObject bulletPrefab;
    //Tranform do spawn da bala
    public Transform bulletSpawn;

    void Update() {
        //float para fazer o player andar
        float translation = Input.GetAxis("Vertical") * speed;
        //float para rotacao do player
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        //coloca um deltatime na velocidade da andada
        translation *= Time.deltaTime;
        //coloca um deltatime na velocidade da rotacao
        rotation *= Time.deltaTime;
        
        //faz a transicao de andar no eixo z
        transform.Translate(0, 0, translation);
        //faz a rotacao no eixo y
        transform.Rotate(0, rotation, 0);
        
        //codicao de input da tecla space
        if(Input.GetKeyDown("space"))
        {
            //instancia a bala
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            //coloca forca na bala para ir para frente
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);
        }
    }
}
