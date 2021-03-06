﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameController GC;
    private VoiceController VC;
    private DialogueManager DM;
    private Rigidbody2D  playerRigidbody;
    private Animator playerAnimator;
    protected Joystick joystick;
    protected GameObject dashJoybutton;
    public float velocidadeX;
    public float velocidadeXBase;
    public float velocidadeY;
    public float velocidadeYBase;
    private int direcao;
    public float dashVelocidade;
    public float dashTime;
    private float dashTimeBase;
    private float dashCooldown;
    public float dashCooldownBase;
    private bool dashCooldownAtivo;
    private GameObject dashCooldownBar;
    private bool isDashing;
    public float HP;
    public float HPMax;
    public Transform BarraHP;
    private float percVida;
    public GameObject ExplosaoPrefab;
    public GameObject[] ArmasRed;
    public GameObject[] ArmasBlue;
    public GameObject[] ArmasGreen;
    public int powerUpsRedColetados;
    public int powerUpsBlueColetados;
    public int powerUpsGreenColetados;
    public GameObject Shield;

    private bool danoRed, danoBlue, danoGreen;

    public GameObject[] PowerUps;
    private Transform SpawnPowerUp;

    private Transform Cima, Esquerda, Direita, Baixo;

    private bool Invencivel;
    private bool ShieldAtivo;

    private Collider2D[] Colisores;

    // Start is called before the first frame update
    void Start()
    {
        GC = FindObjectOfType(typeof(GameController)) as GameController;
        VC = FindObjectOfType(typeof(VoiceController)) as VoiceController;
        DM = FindObjectOfType(typeof(DialogueManager)) as DialogueManager;

        SpawnPowerUp = GameObject.Find("SpawnPowerUp").transform;

        Cima = GameObject.Find("Cima").transform;
        Esquerda = GameObject.Find("Esquerda").transform;
        Direita = GameObject.Find("Direita").transform;
        Baixo = GameObject.Find("Baixo").transform;

        playerRigidbody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();

        BarraHP = GameObject.Find("BarraVida").transform;
        BarraHP.localScale = new Vector3(1,1,1);

        dashCooldownBar = GameObject.Find("DashCooldownBar");

        joystick = FindObjectOfType<Joystick>();
        dashJoybutton = GameObject.Find("DashButton");

        HP = HPMax;
        percVida = HP / HPMax;

        velocidadeXBase = velocidadeX;
        velocidadeYBase = velocidadeY;
        dashTimeBase = dashTime;

        ArmasRed[powerUpsRedColetados].SetActive(true);
        ArmasBlue[powerUpsBlueColetados].SetActive(true);
        ArmasGreen[powerUpsGreenColetados].SetActive(true);

        VC.PegaArmas();

        Shield.GetComponent<Collider2D>().enabled = false;

        StartCoroutine("Invencibilidade", 1.5f / GameController.instance.GameSpeed);

        Colisores = this.gameObject.GetComponents<Collider2D>();

        switch (PlayerPrefs.GetInt("Skin"))
        {
            case 0:
                playerAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Controllers/Player/Skin0/PlayerController");
                Colisores[0].enabled = true;
                break;

            case 1:
                playerAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Controllers/Player/Skin1/PlayerController");
                Colisores[1].enabled = true;
                break;
        }
    }

    void Update() {
        if(joystick == null)
        {
            joystick = FindObjectOfType<Joystick>();
        }

        if(Invencivel || isDashing || ShieldAtivo)
        {
            gameObject.tag = "PlayerInvencivel";
        }
        else
        {
            gameObject.tag = "Player";
        }

        dashCooldownBar.GetComponent<ProgressBar>().current = dashCooldown;
        //DASH
        if(Input.GetButtonDown("Dash") || dashJoybutton.GetComponent<JoyButton>().Pressed && !isDashing && dashCooldown == 0)
        {
            Dash();
        }
        if(isDashing)
        {
            dashTime -= Time.deltaTime;
        }
        if(dashTime <= 0f)
        {
            isDashing = false;
            velocidadeX = velocidadeXBase;
            velocidadeY = velocidadeYBase;
            dashTime = dashTimeBase;
            dashCooldown = dashCooldownBase;
            dashCooldownAtivo = true;
        }
        if(dashCooldownAtivo)
        {
            dashCooldown -= Time.deltaTime;
            if(dashCooldown <= 0)
            {
                dashCooldown = 0;
                dashCooldownAtivo = false;
            }
        }
        if(DM != null)
        {
            if(DM.DialogueBoxOpen)
            {
                playerRigidbody.velocity = new Vector2 (0, 0);
            }
        }
    }

    void FixedUpdate()
    {
        if(DM != null)
        {
            if(!DM.DialogueBoxOpen)
            {
                VC.PegaArmas();

                float movimentoX = Input.GetAxis("Horizontal");
                float movimentoY = Input.GetAxis("Vertical");

                if (movimentoY < -.333f || joystick.Vertical < -.333f)
                {
                    direcao = -1;
                }
                else if (movimentoY == 0 || joystick.Vertical == 0)
                {
                        direcao = 0;
                }
                else if (movimentoY > .333f || joystick.Vertical > .333f)
                {
                    direcao = 1;
                }
                    
                //playerRigidbody.velocity = new Vector2 (movimentoX * velocidadeX * GameController.instance.GameSpeed, movimentoY * velocidadeY * GameController.instance.GameSpeed);
                playerRigidbody.velocity = new Vector2 (joystick.Horizontal * velocidadeX * GameController.instance.GameSpeed + movimentoX * velocidadeX * GameController.instance.GameSpeed, joystick.Vertical * velocidadeY * GameController.instance.GameSpeed + movimentoY * velocidadeY * GameController.instance.GameSpeed);
                    

                if(transform.position.x < Esquerda.position.x)
                {
                    transform.position = new Vector3(Esquerda.position.x, transform.position.y, transform.position.z);
                }
                else if(transform.position.x > Direita.position.x)
                {
                    transform.position = new Vector3(Direita.position.x, transform.position.y, transform.position.z);
                }
                if(transform.position.y > Cima.position.y)
                {
                    transform.position = new Vector3(transform.position.x, Cima.position.y, transform.position.z);
                }
                else if(transform.position.y < Baixo.position.y)
                {
                    transform.position = new Vector3(transform.position.x, Baixo.position.y, transform.position.z);
                }

                    playerAnimator.SetInteger("Direcao", direcao * -1);
                    playerAnimator.SetBool("Invencivel", Invencivel);
            }
        }
        else
        {
            VC.PegaArmas();

                float movimentoX = Input.GetAxis("Horizontal");
                float movimentoY = Input.GetAxis("Vertical");

                if (movimentoY < -.333f || joystick.Vertical < -.333f)
                {
                    direcao = -1;
                }
                else if (movimentoY == 0 || joystick.Vertical == 0)
                {
                        direcao = 0;
                }
                else if (movimentoY > .333f || joystick.Vertical > .333f)
                {
                    direcao = 1;
                }
                    
                //playerRigidbody.velocity = new Vector2 (movimentoX * velocidadeX * GameController.instance.GameSpeed, movimentoY * velocidadeY * GameController.instance.GameSpeed);
                playerRigidbody.velocity = new Vector2 (joystick.Horizontal * velocidadeX * GameController.instance.GameSpeed + movimentoX * velocidadeX * GameController.instance.GameSpeed, joystick.Vertical * velocidadeY * GameController.instance.GameSpeed + movimentoY * velocidadeY * GameController.instance.GameSpeed);
                    

                if(transform.position.x < Esquerda.position.x)
                {
                    transform.position = new Vector3(Esquerda.position.x, transform.position.y, transform.position.z);
                }
                else if(transform.position.x > Direita.position.x)
                {
                    transform.position = new Vector3(Direita.position.x, transform.position.y, transform.position.z);
                }
                if(transform.position.y > Cima.position.y)
                {
                    transform.position = new Vector3(transform.position.x, Cima.position.y, transform.position.z);
                }
                else if(transform.position.y < Baixo.position.y)
                {
                    transform.position = new Vector3(transform.position.x, Baixo.position.y, transform.position.z);
                }

                    playerAnimator.SetInteger("Direcao", direcao * -1);
                    playerAnimator.SetBool("Invencivel", Invencivel);
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        switch (col.gameObject.tag)
        {
            case "InimigoRed":
                if(DM != null)
                {
                    if(!(Invencivel || isDashing || ShieldAtivo || DM.DialogueBoxOpen)){
                        danoRed = true;
                        danoBlue = false;
                        danoGreen = false;
                        TomarDano(2);
                        StartCoroutine("Invencibilidade", 3.0f / GameController.instance.GameSpeed);
                    }
                }
                else
                {
                    if(!(Invencivel || isDashing || ShieldAtivo))
                    {
                        danoRed = true;
                        danoBlue = false;
                        danoGreen = false;
                        TomarDano(2);
                        StartCoroutine("Invencibilidade", 3.0f / GameController.instance.GameSpeed);
                    }
                }
                        break;

            case "InimigoBlue":
                if(DM != null)
                {
                    if(!(Invencivel || isDashing || ShieldAtivo || DM.DialogueBoxOpen)){
                        danoRed = false;
                        danoBlue = true;
                        danoGreen = false;
                        TomarDano(2);
                        StartCoroutine("Invencibilidade", 3.0f / GameController.instance.GameSpeed);
                    }
                }
                else
                {
                    if(!(Invencivel || isDashing || ShieldAtivo)){
                        danoRed = false;
                        danoBlue = true;
                        danoGreen = false;
                        TomarDano(2);
                        StartCoroutine("Invencibilidade", 3.0f / GameController.instance.GameSpeed);
                    }
                }
                        break;

            case "InimigoGreen":
                if(DM != null)
                {
                    if(!(Invencivel || isDashing || ShieldAtivo || DM.DialogueBoxOpen)){
                        danoRed = false;
                        danoBlue = false;
                        danoGreen = true;
                        TomarDano(2);
                        StartCoroutine("Invencibilidade", 3.0f / GameController.instance.GameSpeed);
                    }
                }
                else
                {
                    if(!(Invencivel || isDashing || ShieldAtivo)){
                        danoRed = false;
                        danoBlue = false;
                        danoGreen = true;
                        TomarDano(2);
                        StartCoroutine("Invencibilidade", 3.0f / GameController.instance.GameSpeed);
                    }
                }
                        break;

            case "ProjetilRedInimigo":
                if(DM != null)
                {
                    if(!(Invencivel || isDashing || ShieldAtivo || DM.DialogueBoxOpen)){
                        danoRed = true;
                        danoBlue = false;
                        danoGreen = false;
                        TomarDano(1);
                        StartCoroutine("Invencibilidade", 3.0f / GameController.instance.GameSpeed);
                    }
                }
                else
                {
                    if(!(Invencivel || isDashing || ShieldAtivo)){
                        danoRed = true;
                        danoBlue = false;
                        danoGreen = false;
                        TomarDano(1);
                        StartCoroutine("Invencibilidade", 3.0f / GameController.instance.GameSpeed);
                    }
                }
                        break;

            case "ProjetilBlueInimigo":
                if(DM != null)
                {
                    if(!(Invencivel || isDashing || ShieldAtivo || DM.DialogueBoxOpen)){
                        danoRed = false;
                        danoBlue = true;
                        danoGreen = false;
                        TomarDano(1);
                        StartCoroutine("Invencibilidade", 3.0f / GameController.instance.GameSpeed);
                    }
                }
                else
                {
                    if(!(Invencivel || isDashing || ShieldAtivo)){
                        danoRed = false;
                        danoBlue = true;
                        danoGreen = false;
                        TomarDano(1);
                        StartCoroutine("Invencibilidade", 3.0f / GameController.instance.GameSpeed);
                    }
                }
                        break;

            case "ProjetilGreenInimigo":
                if(DM != null)
                {
                    if(!(Invencivel || isDashing || ShieldAtivo || DM.DialogueBoxOpen)){
                        danoRed = false;
                        danoBlue = false;
                        danoGreen = true;
                        TomarDano(1);
                        StartCoroutine("Invencibilidade", 3.0f / GameController.instance.GameSpeed);
                    }
                }
                else
                {
                    if(!(Invencivel || isDashing || ShieldAtivo)){
                        danoRed = false;
                        danoBlue = false;
                        danoGreen = true;
                        TomarDano(1);
                        StartCoroutine("Invencibilidade", 3.0f / GameController.instance.GameSpeed);
                    }
                }
                        break;

            case "PowerUpRed":
                PowerUpRed();
                VC.PegaArmas();
                Destroy(col.gameObject);
                break;

            case "PowerUpBlue":
                PowerUpBlue();
                VC.PegaArmas();
                Destroy(col.gameObject);
                break;

            case "PowerUpGreen":
                PowerUpGreen();
                VC.PegaArmas();
                Destroy(col.gameObject);
                break;

            case "PowerUpShieldBronze":
                StartCoroutine("Escudo", 10.0f / GameController.instance.GameSpeed);
                Destroy(col.gameObject);
                break;

        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        /*switch (col.gameObject.tag)
        {
            
        }*/
    }
    

    void TomarDano(int danoTomado)
    {
        HP -= danoTomado;
        percVida = HP / HPMax;
        Vector3 theScale = BarraHP.localScale;
        theScale.x = percVida;
        BarraHP.localScale = theScale;
        if(HP <= 0)
        {
            HP = 0;
            Explodir();
        }
    }

    void Explodir()
    {
            GameObject tempPrefab = Instantiate(ExplosaoPrefab) as GameObject;
            tempPrefab.transform.position = transform.position;
            if(danoRed)
            {
                GameObject tempPowerUp = Instantiate(PowerUps[0]) as GameObject;
                tempPowerUp.transform.position = SpawnPowerUp.position;
            }
            else if(danoBlue)
            {
                GameObject tempPowerUp = Instantiate(PowerUps[1]) as GameObject;
                tempPowerUp.transform.position = SpawnPowerUp.position;
            }
            else if(danoGreen)
            {
                GameObject tempPowerUp = Instantiate(PowerUps[2]) as GameObject;
                tempPowerUp.transform.position = SpawnPowerUp.position;
            }
            GC.Morreu();
            Destroy(this.gameObject);
    }

    void PowerUpRed()
    {
        
        ArmasRed[powerUpsRedColetados].SetActive(false);
        powerUpsRedColetados += 1;

        if (powerUpsRedColetados <= ArmasRed.Length - 1)
        {
            
            ArmasRed[powerUpsRedColetados].SetActive(true);
        }
         if(powerUpsRedColetados > ArmasRed.Length - 1)
        {
            powerUpsRedColetados -= 1;
            ArmasRed[powerUpsRedColetados].SetActive(true);
            GC.pontos += 1000;
        }

    }


    void PowerUpBlue()
    {
        
        ArmasBlue[powerUpsBlueColetados].SetActive(false);
        powerUpsBlueColetados += 1;

        if (powerUpsBlueColetados <= ArmasBlue.Length - 1)
        {
            
            ArmasBlue[powerUpsBlueColetados].SetActive(true);
        }
         if(powerUpsBlueColetados > ArmasBlue.Length - 1)
        {
            powerUpsBlueColetados -= 1;
            ArmasBlue[powerUpsBlueColetados].SetActive(true);
            GC.pontos += 1000;
        }

    }

    void PowerUpGreen()
    {
        
        ArmasGreen[powerUpsGreenColetados].SetActive(false);
        powerUpsGreenColetados += 1;

        if (powerUpsGreenColetados <= ArmasGreen.Length - 1)
        {
            ArmasGreen[powerUpsGreenColetados].SetActive(true);
        }
         if(powerUpsGreenColetados > ArmasGreen.Length - 1)
        {
            powerUpsGreenColetados -= 1;
            ArmasGreen[powerUpsGreenColetados].SetActive(true);
            GC.pontos += 1000;
        }

    }

    void Dash()
    {
        isDashing = true;
        velocidadeX = velocidadeX * dashVelocidade;
        velocidadeY = velocidadeY * dashVelocidade;
    }

    IEnumerator Invencibilidade(float segundosInvencivel)
    {
        Invencivel = true;
        yield return new WaitForSeconds(segundosInvencivel);
        Invencivel = false;
    }

    IEnumerator Escudo(float Duracao)
    {
        ShieldAtivo = true;
        Shield.GetComponent<Animator>().SetTrigger("Pegou");
        Shield.GetComponent<Collider2D>().enabled = true;
        yield return new WaitForSeconds(Duracao * 0.75f);
        Shield.GetComponent<Animator>().SetTrigger("Terminando");
        yield return new WaitForSeconds(Duracao * 0.25f);
        Shield.GetComponent<Animator>().SetTrigger("Terminou");
        ShieldAtivo = false;
        Shield.GetComponent<Collider2D>().enabled = false;
    }

}
