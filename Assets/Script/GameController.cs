using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    public Slider player1HealthBar;
    public Slider player2HealthBar;
    public PlayerController player1;
    public PlayerController player2;
    public GameObject restartButton;
    public GameObject playerText;
    private float player1MaxHP;
    private float player2MaxHP;

    // Start is called before the first frame update
    void Start()
    {
        player1MaxHP = player1.m_healhPoint;
        player2MaxHP = player2.m_healhPoint;
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        player1HealthBar.value = (float)player1.m_healhPoint/player1MaxHP;
        player2HealthBar.value = (float)player2.m_healhPoint/player2MaxHP;
        if(player1.m_healhPoint <= 0 || player2.m_healhPoint <= 0){
            playerText.SetActive(true);
            playerText.GetComponentInChildren<TextMeshProUGUI>().text = "Winner"; 
            if(player1.m_healhPoint <= 0){
                playerText.transform.position = player2.transform.position;
            }
            else{
                playerText.transform.position = player1.transform.position;
            }
            StartCoroutine(EndGame());
        }
    }
    public void RestartButton(){
        SceneManager.LoadScene(1);
    }
    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2);
        restartButton.SetActive(true);
        Time.timeScale = 0.0f;
    }
}
