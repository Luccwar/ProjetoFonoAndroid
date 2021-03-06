﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuConfiguracoes : MonoBehaviour
{
    public AudioController AC;

    public AudioMixer audioMixer;

    private GameObject MenuCanvas, ConfiguracoesCanvas, SelecaoFaseCanvas;
    public Slider volumeSlider;

    private void Start() {
        AC = FindObjectOfType(typeof(AudioController)) as AudioController;
        MenuCanvas = GameObject.Find("MenuCanvas");
        ConfiguracoesCanvas = GameObject.Find("ConfiguracoesCanvas");
        SelecaoFaseCanvas = GameObject.Find("SelecaoFaseCanvas");
        ConfiguracoesCanvas.SetActive(false);
        SelecaoFaseCanvas.SetActive(false);
        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
    }

    public void Comecar()
    {
        AC.TrocarMusica(AC.MusicaFase1, "CenaTesteFacil", true);
    }

    public void RetornarAoMenuPrincipal()
    {
        ConfiguracoesCanvas.SetActive(false);
        SelecaoFaseCanvas.SetActive(false);
        MenuCanvas.SetActive(true);
    }

    public void AbrirConfiguracoes()
    {
        MenuCanvas.SetActive(false);
        ConfiguracoesCanvas.SetActive(true);
    }

    public void AbrirSelecaoFase()
    {
        MenuCanvas.SetActive(false);
        SelecaoFaseCanvas.SetActive(true);
    }

    public void SelecionarFase(string NomeFase)
    {
        AC.TrocarMusica(AC.MusicaFase1, NomeFase, true);
    }

    public void SetVolume (float volume)
    {
        audioMixer.SetFloat("masterVolume", volume);
        if(volumeSlider.value <= -40)
        {
            audioMixer.SetFloat("masterVolume", -80);
        }
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
    }

    public void DiminuirVolume()
    {
        volumeSlider.value--;
        if(volumeSlider.value <= -40)
        {
            audioMixer.SetFloat("masterVolume", -80);
        }
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
    }

    public void AumentarVolume()
    {
        volumeSlider.value++;
        if(volumeSlider.value <= -40)
        {
            audioMixer.SetFloat("masterVolume", -80);
        }
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
    }
}
