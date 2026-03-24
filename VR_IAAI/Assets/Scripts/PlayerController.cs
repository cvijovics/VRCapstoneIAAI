using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private AudioController audioController;
    [SerializeField] private Transform player;
    [SerializeField] private AudioClip[] footstepClips;
    private bool isWalking = false;
    private Vector3 previousPosition;
    private void Awake()
    {
        player = this.gameObject.transform;
    }

    private void Start()
    {
        previousPosition = player.transform.position;
    }
    
    private void Update()
    {
        Vector3 movement = player.transform.position - previousPosition;

        if (movement.magnitude > 0.01f && !isWalking)
        {
            isWalking = true;
            Debug.Log("Player is moving");
            StartCoroutine(StepWait(0.4f));
        }
        previousPosition = player.transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Door"))
        {
            SceneManager.LoadScene("HouseScene");
        }
    }

    IEnumerator StepWait(float waitTime)
    {
        audioController.PlaySFXRandom(footstepClips, player.transform, 0.4f);
        yield return new WaitForSeconds(waitTime);
        isWalking = false;
    }


}
