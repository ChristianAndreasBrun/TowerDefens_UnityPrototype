using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Variables
    TextAsset map;
    static public int idMap;
    public Transform mapPosition;

    List<List<int>> allMap;
    List<Vector2> corners;
    public List<int> walkable;

    public List<Wave> waves;
    float waveTime;
    int currentWave, currentEnemy;

    GameObject currentTurret; //siempre es null
    public List<TurretManager> allTurrets;

    public TextMeshProUGUI coinsText;
    public Image clockImage;
    TurretManager turretEvolve;

    public Transform gridLifes;
    public GameObject baseLife;
    public TextMeshProUGUI gameTimeText;
    public TextMeshProUGUI waveAlert;
    public TextMeshProUGUI totalWaves;
    float gameTime;


    void Start()
    {
        gameTime = 0;

        map = Resources.Load<TextAsset>($"Map_{idMap}");

        //Inicializcion de mapas
        allMap = new List<List<int>>();

        //Guarda en el String las filas del .txt
        string[] row = map.text.Split('\n');
        for (int i = 0; i < row.Length; i++)
        {
            List<int> ids = new List<int>();

            //Divide en columnas y por comas
            string[] column = row[i].Split(',');
            for (int j = 0; j < column.Length - 1; j++)
            {
                ids.Add(int.Parse(column[j]));

                Sprite tile = Resources.Load<Sprite>("Map/" + column[j]);
                GameObject newTile = new GameObject($"{i}-{j}");
                newTile.transform.SetParent(mapPosition);
                newTile.transform.localPosition = new Vector2(j, -i);
                newTile.AddComponent<SpriteRenderer>().sprite = tile;
                newTile.GetComponent<SpriteRenderer>().sortingOrder = -2;
            }
            allMap.Add(ids);
        }

        //print(allMap[7][2]);

        //Lee los Vectores de Corners donde se indica el X e Y
        TextAsset corn = Resources.Load<TextAsset>($"Corner_{idMap}");
        corners = new List<Vector2>();
        string[] cornRow = corn.text.Split("\n");
        for (int i = 0; i < cornRow.Length; i++)
        {
            string[] cornColum = cornRow[i].Split(",");
            corners.Add(new Vector2(float.Parse(cornColum[0]), -float.Parse(cornColum[1])));
            //print(corners[i]);
        }

        waves = Resources.Load<WaveManager>($"Waves/Waves_{idMap}").CloneWave();
        waveTime = 0; currentWave = 0; currentEnemy = 0;
        //totalEnemies = waves[currentWave].enemies.Count;

        PlayerValues.InitLife(idMap);
        PrintCoins();
        PrintHeart();
    }


    void Update()
    {
        //Calcula las oleadas
        if (currentWave < waves.Count)
        {
            gameTime += Time.deltaTime;
            System.TimeSpan ts = System.TimeSpan.FromSeconds(gameTime);
            gameTimeText.text = ts.ToString(@"mm\:ss");


            if (Input.GetMouseButton(0) && turretEvolve != null)
            {
                clockImage.fillAmount = Mathf.MoveTowards(clockImage.fillAmount, 1, 2 * Time.deltaTime);

                Vector3 clockPos = turretEvolve.transform.position;
                clockPos.z = clockImage.transform.position.z;
                clockImage.transform.position = clockPos;

                if (clockImage.fillAmount == 1)
                {
                    SetTurretEvolve();
                }
            }

            if (Input.GetMouseButtonUp(0) && turretEvolve != null)
            {
                turretEvolve = null;
                clockImage.fillAmount = 0;
            }


            //Calcula los enemigos
            if (currentEnemy < waves[currentWave].enemies.Count)
            {
                waveTime += Time.deltaTime;
                if (waveTime >= waves[currentWave].enemies[currentEnemy].time)
                {
                    waveTime = 0;
                    GameObject newEnemy = Instantiate(waves[currentWave].enemies[currentEnemy].enemy);
                    waves[currentWave].enemies[currentEnemy].enemy = newEnemy;
                    newEnemy.transform.SetParent(mapPosition);
                    newEnemy.GetComponent<EnemyManager>().Init(corners, this);
                    currentEnemy++;
                }
            }
            else
            {
                //Incializar la siguiente oleada de enemigos, cuando acaba la primera
                if (EnemyInScene() == 0)
                {
                    StartCoroutine(NextWaveText());
                    currentWave++;
                    waveTime = 0; currentEnemy = 0;
                    //totalEnemies = waves[currentWave].enemies.Count;
                }
            }

            //Imprimir numero de waves
            totalWaves.text = ($"{currentWave + 1}") + "/" + ($"{waves.Count}");
        }
        else
        {
            print("Fin de partida");

            //Guardar datos de partida:
            //LevelComplete
            PlayerPrefs.SetInt("LevelComplete_" + idMap, 1);
            PlayerPrefs.SetFloat("Timer_" + idMap, gameTime);
            PlayerPrefs.SetInt("Lifes_" + idMap, PlayerValues.lifes);

            //Salto de pantalla a Win
            SceneManager.LoadScene(3);
        }


        //Comprueba si existe la torreta
        if (currentTurret != null)
        {
            if (Input.GetMouseButton(0))
            {
                //La torreta sigue al mouse
                Vector2 turretPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Mover la torreta por el grid
                turretPosition = GetPositionInGrid(turretPosition);
                currentTurret.transform.position = turretPosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                //guarda el componente en una variable temporal para ser usada en todo el codigo
                TurretManager temp = currentTurret.GetComponent<TurretManager>();


                //Comprobar el dinero.
                if (PlayerValues.coins < temp.rubyCost)
                {
                    Destroy(currentTurret);
                    return;
                }

                //Comprobar que suelte en el tile adecuado
                int tile = allMap[-(int)currentTurret.transform.localPosition.y][(int)currentTurret.transform.localPosition.x];

                if (walkable.Contains(tile) == false)
                {
                    Destroy(currentTurret);
                    return;
                }

                //Comprobar si hay una torreta en la misama posicion
                for (int i = 0; i < allTurrets.Count; i++)
                {
                    if (allTurrets[i] != null)
                    {
                        if (allTurrets[i].transform.position == currentTurret.transform.position)
                        {
                            Destroy(currentTurret);
                            return;
                        }
                    }
                }
                //gasta el dinero
                PlayerValues.RemoveCoins(temp.rubyCost);

                PrintCoins();
                temp.InitTurret(this);
                allTurrets.Add(temp);
                currentTurret = null;
            }
        }
    }

    Vector2 GetPositionInGrid(Vector2 position)
    {
        return new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
    }


    public int EnemyInScene()
    {
        int countEnemies = 0;
        for (int i = 0; i < waves[currentWave].enemies.Count; i++)
        {
            if (waves[currentWave].enemies[i].enemy != null)
            {
                countEnemies++;
            }
        }
        return countEnemies;
    }

    public void DestroyEnemy(GameObject enemy)
    {
        Destroy(enemy);
        PrintCoins();
    }


    public void CreateTurret(GameObject turret)
    {
        //Instanciar torreta, al pulsar el boton del Canvas
        currentTurret = Instantiate(turret, mapPosition);
    }

    public void GetTurretEvolve(TurretManager turret)
    {
        turretEvolve = turret;
    }
    public void SetTurretEvolve()
    {
        GameObject newEvolve = Instantiate(turretEvolve.evolve, mapPosition);

        //Recoloca la nueva torreta encima de la atigua
        newEvolve.transform.localPosition = turretEvolve.transform.localPosition;
        newEvolve.GetComponent<TurretManager>().InitTurret(this);

        //Resta las monedas
        PlayerValues.RemoveCoins(newEvolve.GetComponent<TurretManager>().rubyCost);
        PrintCoins();

        //Elimina la torreta antigua
        Destroy(turretEvolve.gameObject);

        //Quita el icono de carga
        clockImage.fillAmount = 0;
    }


    //imprime el valor de la moneda en pantalla
    void PrintCoins()
    {
        coinsText.text = $"Ruby: {PlayerValues.coins}";
    }

    void PrintHeart()
    {
        for (int i = 0; i < PlayerValues.lifes; i++)
        {
            //instancia la vida dentro del grid
            GameObject newHeart = Instantiate(baseLife, gridLifes);
        }
    }
    public void UpdateHeart()
    {
        for (int i = 0; i < gridLifes.childCount; i++)
        {
            if (i >= PlayerValues.lifes)
            {
                gridLifes.GetChild(i).GetComponent<Image>().enabled = false;
            }
        }
    }

    IEnumerator NextWaveText()
    {
        waveAlert.enabled = true;
        yield return new WaitForSeconds(1f);
        waveAlert.enabled = false;
        yield return new WaitForSeconds(1f);
        waveAlert.enabled = true;
        yield return new WaitForSeconds(1f);
        waveAlert.enabled = false;
        yield return new WaitForSeconds(1f);
        waveAlert.enabled = true;
        yield return new WaitForSeconds(1f);
        waveAlert.enabled = false;
    }
}
