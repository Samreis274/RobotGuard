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
        //variavel para posicao 
        var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) *
        this.transform.forward;
        //target pega valor de p
        target = p;
        //retorna true
        return true;
    }

    [Task]
    bool SeePlayer()
    {
        //pegando a distancia do player
        Vector3 distance = player.transform.position - this.transform.position;
        //cria o raycast
        RaycastHit hit;
        //cria um bool com valor de false
        bool seeWall = false;
        //debug para o raycast
        Debug.DrawRay(this.transform.position, distance, Color.red);
        //verifica se o raycast encontar em algo
        if (Physics.Raycast(this.transform.position, distance, out hit))
        {
            //verifica se a tag do objeto que encostar é wall
            if (hit.collider.gameObject.tag == "wall")
            {
                //bool vira true
                seeWall = true;
            }
        }

        if (Task.isInspected)
            Task.current.debugInfo = string.Format("wall={0}", seeWall);
        // verifica for diferente ele retorna verdadeiro, se nao para falso
        if (distance.magnitude < visibleRange && !seeWall)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    [Task]
    public void TargetPlayer()
    {
        //pega a posicao do player
        target = player.transform.position;
        //caso de tudo certo no metodo ele da succeed
        Task.current.Succeed();
    }

    [Task]
    public void LookAtTarget()
    {
        //pega a posicao target
        Vector3 direction = target - this.transform.position;
        //faz o robo rotacionar para o target
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
        Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
        if (Task.isInspected)
            //um debug para verificar se o angle n ser 0
            Task.current.debugInfo = string.Format("angle={0}",
            Vector3.Angle(this.transform.forward, direction));
        if (Vector3.Angle(this.transform.forward, direction) < 5.0f)
        {
            //caso de tudo certo no metodo ele da succeed
            Task.current.Succeed();
        }
    }

    [Task]
    public bool Fire()
    {
        //cria a intanciacao da bala
        GameObject bullet = GameObject.Instantiate(bulletPrefab,
        bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        //coloca forca na bala para se mover
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);
        //retorna true
        return true;
    }

    [Task]
    public void PickRandomDestination()
    {
        //achando um destino aleatorio
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        //setando destino
        agent.SetDestination(dest);
        //caso de tudo certo no metodo ele da succeed
        Task.current.Succeed();
    }


    [Task]
        public bool IsHealthLessThan(float health)
    {
        //retorna o valor de vida que ele tem 
        return this.health < health;
    }

    [Task]
    public bool Explode()
    {
        //destrui a barra de vida
        Destroy(healthBar.gameObject);
        //destroi o robo
        Destroy(this.gameObject);
        //retorna true
        return true;
    }

    [Task]
    public void PickDestination(int x, int z)
    {
        //setando o destino
        Vector3 dest = new Vector3(x, 0, z);
        //seta o destino
        agent.SetDestination(dest);
        //caso de tudo certo no metodo ele da succeed
        Task.current.Succeed();
    }
    [Task] 
    public void MoveToDestination() 
    { 
        //move o player para o destino setado
        if (Task.isInspected) 
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time); 
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending) 
        {
            //caso de tudo certo no metodo ele da succeed
            Task.current.Succeed(); 
        } 
    }

    void Update()
    
    
    {
        //codigo para possicionar a barra de vida 
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

