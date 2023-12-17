using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    private float _income;
    [SerializeField] private float _tickTime;

    [Header("UI links")]
    [SerializeField] private TextMeshProUGUI _timeToNextIncomeInfo;
    [SerializeField] private TextMeshProUGUI _moneyOnNextIncomeInfo;
    [SerializeField] private TextMeshProUGUI _moneyAmount;
    [SerializeField] private TextMeshProUGUI _soulsAmount;

    private float _time;
    private int _souls;
    private float _money;

    public float Money => _money;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

        Instance = this;
        _time = _tickTime;
        UpdateIncomeTimeInfo();
        IncreaseIncomeValue(1);
    }

    private void Start()
    {
        ChangeMoneyAmount(10);
        ChangeSoulsAmount(0);
    }

    private void Update()
    {
        _time -= Time.deltaTime;

        if ( _time <= 0 )
        {
            PerformTickActions();
            _time = _tickTime;
        }
        UpdateIncomeTimeInfo();
    }

    private void UpdateIncomeTimeInfo()
    {   
        _timeToNextIncomeInfo.text = $"Income in: {_time:F}";
    }

    private void PerformTickActions()
    {
        ChangeMoneyAmount(_income);
    }

    private void ChangeMoneyAmount(float amount)
    {
        _money += amount;
        _moneyAmount.text = "Money: " + _money;
    }

    private void ChangeSoulsAmount(int amount)
    {
        _souls += amount;
        _soulsAmount.text = "Souls: " + _souls;
    }

    public void UnitDeath()
    {
        ChangeSoulsAmount(1);
    }

    private void IncreaseIncomeValue(float value)
    {
        _income += value;
        _moneyOnNextIncomeInfo.text = $"Money on tick: {_income:F}";
    }
}
