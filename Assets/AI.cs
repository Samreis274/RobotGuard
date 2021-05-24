using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    //variavel para tranform do player
    public Transform player;
    //tranform do spawn da bala
    public Transform bulletSpawn;
    //slider da barra de vida
    public Slider healthBar; 
    //gameobject da bala
    public GameObject bulletPrefab;

    //referecia do navmesh
    NavMeshAgent agent;

    //vector do destino do player
    public Vector3 destination;
    //vector para o onde o player ta apontando
    public Vector3 target; 
   //variavel para vida
    float health = 100.0f;
    //variavel de velocidade de rotacao
    float rotSpeed = 5.0f;
    //tempo da bala visivel
    float visibleRange = 80.0f;
    //Distancia da bala
    float shotRange = 40.0f;

    void Start()
    {
        //pegando component navmeshagent
        agent = this.GetComponent<NavMeshAgent>();
        //comando para vida restaurar se tiver parado 
        agent.stoppingDistance = shotRange - 5; 
        InvokeRepeating("UpdateHealth",5,0.5f);
    }

    [Task]
    bool Turn(float angle)
    {
        var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) *
        this.transform.forward;
        target = p;
        return true;
    }

    [Task]
    public void LookAtTarget()
    {
        Vector3 direction = target - this.transform.position;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
        Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("angle={0}",
            Vector3.Angle(this.transform.forward, direction));
        if (Vector3.Angle(this.transform.forward, direction) < 5.0f)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public void PickDestination(int x, int z)
    {
        Vector3 dest = new Vector3(x, 0, z);
        agent.SetDestination(dest);
        Task.current.Succeed();
    }
    
    [Task]
    public void MoveToDestination()
    {
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("t={0:0.00", Time.time);
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public void TargetPlayer()
    {
        target = player.transform.position;
        Task.current.Succeed();
    }

    [Task]
    public bool Fire()
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab,
        bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);
        return true;
    }

    [Task]
    public void PickRandomDestination()
    {
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        agent.SetDestination(dest);
        Task.current.Succeed();
    }

    [Task]
    bool SeePlayer()
    {
        Vector3 distance = player.transform.position - this.transform.position;
        RaycastHit hit;
        bool seeWall = false;
        Debug.DrawRay(this.transform.position, distance, Color.red);
        if (Physics.Raycast(this.transform.position, distance, out hit))
        {
            if (hit.collider.gameObject.tag == "wall")
            {
                seeWall = true;
            }
        }
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("wall={0}", seeWall);
        if (distance.magnitude < visibleRange && !seeWall)
        {
            return true;
        }

        else
        {
            return false;
        }
    }


    void Update()
    
    
    {

        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        healthBar.value = (int)health;
        healthBar.transform.position = healthBarPos + new Vector3(0,60,0);
    }

    void UpdateHealth()
    {
        //condicao se a vida estiver menor que 100 ela recarrega a vida
       if(health < 100)
        //adiciona 1 na vida 
        health ++;
    }

    void OnCollisionEnter(Collision col)
    {   
        //condicao para conferir se a tag pode colidir
        if(col.gameObject.tag == "bullet")
        {
            //tira 10 de vida
            health -= 10;
        }
    }
}

