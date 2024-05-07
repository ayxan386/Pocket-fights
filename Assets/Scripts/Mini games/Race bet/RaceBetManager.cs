using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class RaceBetManager : MonoBehaviour
{
    [SerializeField] private List<BetOption> bettingOptions;
    [SerializeField] private List<PossibleLoot> prizePrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform[] destinationPoints;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float speedDelayFactor;
    [SerializeField] private CinemachineVirtualCamera miniGameCamera;
    [SerializeField] private GameObject choiceUIElements;

    [Header("Awarding part")]
    [SerializeField] private int betAmount;

    [SerializeField] private GameObject awardElements;
    [SerializeField] private TextMeshProUGUI awardText;

    [Header("SFX")]
    [SerializeField] private AudioSource source;

    [SerializeField] private AudioClip optionOpenedSound;
    [SerializeField] private AudioClip pairMatchedSound;
    [SerializeField] private AudioClip nonPairMatchedSound;

    private TwoOfKindOption prevOpennedOption;
    private bool isFirstOption = true;

    private int pairCounter = 0;
    private int selectedChoice;
    private bool raceComplete;

    [ContextMenu("Generate options")]
    public void InitialSetup()
    {
        for (var index = 0; index < bettingOptions.Count; index++)
        {
            var bettingOption = bettingOptions[index];
            var spawnPoint = spawnPoints[index];
            bettingOption.worldObj =
                Instantiate(bettingOption.worldObj, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            bettingOption.speed = baseSpeed * speedDelayFactor;
            bettingOption.animator = bettingOption.worldObj.GetComponent<Animator>();
        }

        var roll = Random.value;
        print($"The roll for slime race is {roll}");
        for (var index = 0; index < bettingOptions.Count; index++)
        {
            var bettingOption = bettingOptions[index];
            roll -= bettingOption.chance;
            if (roll <= 0)
            {
                bettingOption.speed /= speedDelayFactor;
                break;
            }
        }

        TheStartGame();
    }

    private void TheStartGame()
    {
        miniGameCamera.Priority = 15;
        FloorManager.Instance.ToggleFollowCamera(false);
        PlayerInputController.Instance.playerInput.DeactivateInput();
        choiceUIElements.SetActive(true);
    }

    private void TheEndGame()
    {
        miniGameCamera.Priority = 5;
        FloorManager.Instance.ToggleFollowCamera(true);
        PlayerInputController.Instance.playerInput.ActivateInput();
        choiceUIElements.SetActive(false);
        awardElements.SetActive(false);
    }

    public void MakeChoice(int choiceNumber)
    {
        InventoryController.Instance.AddGold(-betAmount);
        selectedChoice = choiceNumber;
        choiceUIElements.SetActive(false);
        StartRace();
    }

    public void CloseAwardUi()
    {
        TheEndGame();
    }

    private void StartRace()
    {
        for (var index = 0; index < bettingOptions.Count; index++)
        {
            var bettingOption = bettingOptions[index];
            bettingOption.tween =
                bettingOption.worldObj.DOMove(destinationPoints[index].position, bettingOption.speed);
            bettingOption.tween.onComplete = OnComplete;
            bettingOption.animator.SetBool(bettingOption.animationName, true);
        }
    }

    private void OnComplete()
    {
        if (raceComplete) return;
        raceComplete = true;

        for (var index = 0; index < bettingOptions.Count; index++)
        {
            var betOption = bettingOptions[index];
            print($"Checking slime {index} for winning {betOption.tween.IsComplete()}");
            if(!betOption.tween.IsComplete()) continue;
            if (index == selectedChoice)
            {
                PlayerVictory();
                break;
            }

            if (index != selectedChoice)
            {
                PlayerLoss();
                break;
            }
        }

        foreach (var option in bettingOptions)
        {
            option.animator.SetBool(option.animationName, false);
            option.tween.Kill();
        }
    }

    private void PlayerLoss()
    {
        awardElements.SetActive(true);
        awardText.text = "You lost";
    }

    private void PlayerVictory()
    {
        awardElements.SetActive(true);
        awardText.text = "You won";
        InventoryController.Instance.AddGold((int)(bettingOptions[selectedChoice].returnRate * betAmount));
    }
}

[Serializable]
public class BetOption
{
    public Transform worldObj;
    public Animator animator;
    public string animationName;
    public float returnRate;
    public float chance;
    public float speed;
    public TweenerCore<Vector3, Vector3, VectorOptions> tween;
}