using System;
using UnityEngine;


[CreateAssetMenu(fileName = "BonusData", menuName = "Bonus/Create new BonusData", order = 1)]
public class BonusData : ScriptableObject
{
    
    [Tooltip("Price")]
    [SerializeField] private int price;
    public int Price
    {
        get { return price; }
        protected set {}
    }
    
    [Tooltip("Is Default type")]
    [SerializeField] private bool isDefault;
    public bool IsDefault
    {
        get { return isDefault; }
        protected set {}
    }
    
    [Tooltip("Is Healths type")]
    [SerializeField] private bool isHealths;
    public bool IsHealths
    {
        get { return isHealths; }
        protected set {}
    }
    
    [Tooltip("Is Weapon type")]
    [SerializeField] private bool isWeapon;
    public bool IsWeapon
    {
        get { return isWeapon; }
        protected set {}
    }
    
    [Tooltip("Value of Bonus effect")]
    [SerializeField] private int value;
    public int Value
    {
        get { return value; }
        protected set {}
    }
    
    [Tooltip("Sound effect")]
    [SerializeField] private AudioClip sound;
    public AudioClip Sound
    {
        get { return sound; }
        protected set {}
    }

    public void Action(Player _player)
    {
        if (isHealths)
        {
            _player.AddHealths(this);
        }
        if (IsWeapon)
        {
            _player.SetWeapon(this);
        }
    }
    
}
