using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class DogListen : MonoBehaviour
{
    public InputActionReference beckonAction;
    [SerializeField] private GameObject dog;
    [SerializeField] private GameObject player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator dogAnimator;
    [SerializeField] private int dogSpeed = 5;
    private bool isRunning = false; // True when dog is running to player

    public Node currentNode;
    [SerializeField] private List<Node> path = new List<Node>(); // path to player


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        // Logic for dog idle movement/patrol/anim
        // dog.transform.Rotate(0, 15, 0);

        /* Stop other logic when running to player
        if(isRunning)
        {
            // move dog towards player position
            dog.transform.position = Vector3.MoveTowards(dog.transform.position, player.transform.position, dogSpeed * Time.deltaTime);
            //dog.transform.LookAt(player.transform);
            dog.transform.Rotate(0, 15, 0);

            // add additional check to stop a certain distance from player
            if (Vector3.Distance(dog.transform.position, player.transform.position) < 2.0f)
            {
                isRunning = false;
                dogAnimator.SetBool("isRunning", false);
            }
        }
        if(isRunning)
        {
            Debug.Log("path count: " + path.Count);
            CreatePathToPlayer();
            if (Vector3.Distance(dog.transform.position, player.transform.position) < 1.5f)
            {
                isRunning = false;
                dogAnimator.SetBool("isRunning", false);
            }
        }
        */

        if(isRunning)
        {
            agent.destination = player.transform.position;
            if (!agent.pathPending)
            {
                Debug.Log("remaining distance: " + Vector3.Distance( agent.destination, agent.transform.position));
                Debug.Log("stopping distance: " + agent.stoppingDistance);
                if ( Vector3.Distance( agent.destination, agent.transform.position) <= agent.stoppingDistance)
                {
                    dog.transform.LookAt(player.transform);
                    agent.velocity = new Vector3(0,0,0);
                    isRunning = false;
                    dogAnimator.SetBool("isRunning", false);
                }
            } 
        }
        
    }

    public void RunToPlayer(InputAction.CallbackContext context)
    {
        Debug.Log("Dog is running to player at position: " + player.transform.position);
        if (context.performed && !isRunning)
        {
            isRunning = true;
            dogAnimator.SetBool("isRunning", true);
        }
    }

    /*
    public void CreatePathToPlayer()
    {
        if (path.Count > 0)
        {
            int x = 0;
            
            dog.transform.position = Vector3.MoveTowards(dog.transform.position, new Vector3(path[x].transform.position.x, path[x].transform.position.y, path[x].transform.position.z), dogSpeed * Time.deltaTime);
            dog.transform.LookAt(path[x].transform);
            
            if (Vector3.Distance(dog.transform.position, path[x].transform.position) < 0.1f)
            {
                currentNode = path[x];
                path.RemoveAt(x);
            }
        }
        else
        {
            Node[] nodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
            Node playerNode = CreatePlayerNode(nodes);
            currentNode = CreateDogNode(nodes);
            while (path == null || path.Count == 0)
            {
                path = AStarManager.instance.GeneratePath(currentNode, playerNode);
            }
        }

    }

    private Node CreatePlayerNode(Node[] nodeList)
    {
        Node closestNode = null;
        float distanceToNode = Vector3.Distance(dog.transform.position, player.transform.position); //default / placeholder value for distance
        foreach(Node node in nodeList)
        {
            if (Vector3.Distance(node.transform.position, player.transform.position) < distanceToNode)
            {
                distanceToNode = Vector3.Distance(node.transform.position, player.transform.position);
                closestNode = node;
            }
            
        }
        Debug.Log("closest node to player: " + closestNode.transform.position);
        return closestNode;
    }

    private Node CreateDogNode(Node[] nodeList)
    {
        Node closestNode = null;
        float distanceToNode = Vector3.Distance(dog.transform.position, player.transform.position); //default / placeholder value for distance
        foreach(Node node in nodeList)
        {
            if (Vector3.Distance(node.transform.position, dog.transform.position) < distanceToNode)
            {
                distanceToNode = Vector3.Distance(node.transform.position, dog.transform.position);
                closestNode = node;
            }
            
        }
        Debug.Log("closest node to dog: " + closestNode.transform.position);
        return closestNode;
    }
    */


    

}
