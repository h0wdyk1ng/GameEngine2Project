using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    [Header("Variables")]
    [SerializeField] private Wave[] waves;
    [SerializeField] private float timeBtwnWaves = 5, waveCountdown;

    private SpawnState state = SpawnState.COUNTING;

    private int currentWave = 0;
    
    [Header("References")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private List<EnemyAiVer2> enemyList;
    // Start is called before the first frame update
    void Start()
    {
        waveCountdown = timeBtwnWaves;
    }

    // Update is called once per frame
    void Update()
    {
        if(state == SpawnState.WAITING)
        {
            if (!AllEnemiesDead())
                return;
            else
                CompleteWave();
        }

        if (waveCountdown <= 0)
        {
            if (state != SpawnState.SPAWNING)
            {
                StartCoroutine(SpawnWave(waves[currentWave]));
            }
        }
        else
            waveCountdown -= Time.deltaTime;
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        state = SpawnState.SPAWNING;

        for(int i = 0; i < wave.enemiesAmount; i++)
        {
            SpawnEnemy(wave.enemy);
            yield return new WaitForSeconds(wave.delay);
        }

        state = SpawnState.WAITING;

        yield break;
    }

    private void SpawnEnemy(GameObject enemy)
    {
        int randomIndex = Random.Range(0, spawnPoints.Length - 1);
        Transform randomSpawner = spawnPoints[randomIndex];

        GameObject newEnemy = Instantiate(enemy, randomSpawner.position, randomSpawner.rotation);
        EnemyAiVer2 newEnemyParam = newEnemy.GetComponent<EnemyAiVer2>();

        enemyList.Add(newEnemyParam);
    }

    private bool AllEnemiesDead()
    {
        int i = 0;
        foreach (EnemyAiVer2 enemy in enemyList)
        {
            if (enemy.IsDead())
            {
                i++;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    private void CompleteWave()
    {
        Debug.Log("WAVE COMPLETED!!");  // put some ui to say so

        state = SpawnState.COUNTING;
        waveCountdown = timeBtwnWaves;

        if(currentWave + 1 > waves.Length - 1)
        {
            //currentWave = 0;    // THIS IS PROVISIONAL, implement the change to the cutscene
            Debug.Log("Completed all waves!!");
            SceneManager.LoadScene("Final", LoadSceneMode.Single);
            // put the screen to fade black, then after it fades it loads the next scene
        }
        else
        {
            currentWave++;
        }
    }
}
