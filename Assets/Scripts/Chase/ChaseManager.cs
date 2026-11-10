using System;
using TMPro;
using UnityEngine;

public class ChaseManager : MonoBehaviour
{
    float chaseTime;
    [SerializeField] float chaseStartTime;
    [SerializeField] TMP_Text chaseTimer;
    bool chaseActive;
    TimeSpan time;
    EnemyAI enemyAI;

    public void StartChase(EnemyAI enemy)
    {
        chaseTime = chaseStartTime;
        chaseActive = true;
        enemyAI = enemy;
    }

    // Update is called once per frame
    void Update()
    {
        // Only do anything if a chase is currently active
        if (chaseActive)
        {
            if (chaseTime > 0)
            {
                chaseTime -= Time.deltaTime;
                time = TimeSpan.FromSeconds(chaseTime);

                // Format the timer text with a leading zero if seconds are under 10
                if (time.Seconds < 10)
                {
                    chaseTimer.text = time.Minutes.ToString() + ":0" + time.Seconds.ToString();
                }
                else
                {
                    chaseTimer.text = time.Minutes.ToString() + ":" + time.Seconds.ToString();
                }
            }
            else
            {
                // The timer has run out! End the chase here.
                chaseActive = false;

                if (enemyAI != null)
                {
                    enemyAI.enraged = true;
                }
            }
        }
    }
}