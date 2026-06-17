using UnityEngine;

public class VolumeButtonManager : MonoBehaviour
{
    [SerializeField] private AudioController audioController;
    [SerializeField] private AudioClip extraClip;
    [SerializeField]private GameObject panel2;
    private AudioClip voiceClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        voiceClip = this.GetComponent<AudioSource>().clip;
        if (panel2 == null)
        {
            panel2 = new GameObject("whatever");
            panel2.SetActive(false);
        }
    }


    public void PlayVoiceClip()
    {
        if (panel2.activeSelf)
        {
            audioController.PlaySFX(extraClip, this.gameObject.transform, 0.4f);
            return;
        }
        audioController.PlaySFX(voiceClip, this.gameObject.transform, 0.4f);
    }
}
