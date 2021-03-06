﻿using UnityEngine;
using System.Collections;

public class RepeaterShot : MonoBehaviour
{

    //Estadísticas de la torre
    public GameObject projectile;
    public float[] damage = { 2, 3, 4, 5 };
    public float[] range = { 5, 6, 7, 8 };
    public float[] minDelay = { 0.5f, 0.4f, 0.3f, 0.2f };
    public float[] maxDelay = { 2.0f, 1.8f, 1.6f, 1.4f };
    public int[] shotsToMax = { 30, 26, 22, 18 };

    public int level;
    public int maxLevel;

    float experience;
    public float[] levelUpCost = { 30, 80, 150 };


    public Vector2 projectileOffset;
    GameObject radioDisplay;
    Transform cannon;

    GameObject[] enemies;
    GameObject lockedEnemy;

    float timer;
	float delay;
	bool powerUp;

    void Start()
    {
        level = 0;
        delay = maxDelay[level];
		timer = 0.0f;
		powerUp = false;
        foreach (Transform child in transform)
        {

            if (child.name == "Attack_Radio")
            {

                radioDisplay = child.gameObject;

            }
            if (child.name == "Base")
            {
                cannon = child;
            }
        }
    }

    void FixedUpdate()
    {

        if (timer <= 0)
        {
            radioDisplay.transform.localScale = new Vector3(range[level] * 2 / transform.localScale.x, radioDisplay.transform.localScale.y, range[level] * 2 / transform.localScale.y);

            //Si ya tiene un enemigo y aun esta en rango, tiene prioridad
            if (lockedEnemy)
            {
                if (Vector3.Distance(transform.position, lockedEnemy.transform.position) <= range[level])
                {
                    delay -= (maxDelay[level] - minDelay[level]) / shotsToMax[level];

                    if (delay < minDelay[level])
                    {
                        delay = minDelay[level];
                    }
                    Shoot(lockedEnemy);
                    timer = delay;
                }
                else {
                    lockedEnemy = null;
                }
            }
            else {

                enemies = GameObject.FindGameObjectsWithTag("Enemy");

                for (int i = 0; i < enemies.Length; i++)
                {

                    if (Vector3.Distance(transform.position, enemies[i].transform.position) <= range[level])
                    {
                        lockedEnemy = enemies[i];

                        delay = maxDelay[level];
                        Shoot(lockedEnemy);
                        timer = delay;
                        break;
                    }
                }


            }
        }
        timer -= Time.deltaTime;

        if (powerUp)
        {
            getExperience(Time.deltaTime);
        }

        if (lockedEnemy)
        {
            Quaternion objectiveRot = cannon.rotation;
            objectiveRot.eulerAngles = new Vector3(objectiveRot.eulerAngles.x, objectiveRot.eulerAngles.y + Quaternion.LookRotation(lockedEnemy.transform.position - cannon.transform.position).eulerAngles.y - cannon.rotation.eulerAngles.y + 90, objectiveRot.eulerAngles.z);
            cannon.rotation = Quaternion.Lerp(cannon.rotation, objectiveRot, 0.05f);
        }
    }

    void Shoot(GameObject enemy)
    {
        getExperience(delay);
        GameObject bullet = Instantiate(projectile);
        bullet.transform.position = transform.position + new Vector3(-Mathf.Cos(Mathf.Deg2Rad * cannon.rotation.eulerAngles.y) * projectileOffset.x, projectileOffset.y, Mathf.Sin(Mathf.Deg2Rad * cannon.rotation.eulerAngles.y) * projectileOffset.x);
        bullet.SendMessage("setObjective", enemy);
        bullet.SendMessage("setDamage", damage[level]);
    }

    void levelUp()
    {
        if (level < maxLevel)
        {
            level++;
            GameObject fx = Instantiate(Resources.Load("LevelUp")) as GameObject;
            fx.transform.position = transform.position;
        }
    }

	public void changePowerUp(bool x)
	{
		Debug.Log("Holis entro aquiiii y el valor antiguo es: " + powerUp);
		Debug.Log("Torre de tipo: " + this.name);
		powerUp = x;
		Debug.Log("Adioooos salgo de aquiiii y el valor nuevo es: " + powerUp);
	}
    
    public void getExperience(float exp)
    {
        experience += exp;
        if (level < maxLevel)
        {
            if (experience > levelUpCost[level])
            {
                levelUp();
            }
        }
    }
}
