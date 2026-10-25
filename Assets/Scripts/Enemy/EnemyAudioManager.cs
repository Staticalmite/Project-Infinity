using UnityEngine;

public class EnemyAudioManager : MonoBehaviour
{
    private EnemyAI enemyAI;
    private AudioSource enemyAudioSource;

    [SerializeField] private AudioClip chaseAudioClip;
    [SerializeField] private AudioClip screamAudioClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyAI = FindFirstObjectByType<EnemyAI>();
        enemyAudioSource = enemyAI.audioSource;
    }

    public void PlayChaseAudio()
    {
        enemyAudioSource.clip = chaseAudioClip;
        enemyAudioSource.Play();
    }

    public void PlayScream()
    {
        enemyAudioSource.clip = screamAudioClip;
        enemyAudioSource.Play();
    }
}
