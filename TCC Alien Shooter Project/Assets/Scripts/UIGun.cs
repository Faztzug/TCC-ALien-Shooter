using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIGun : MonoBehaviour
{
    public GunType GunType {get => _gunType; private set{_gunType = value;}}
    [SerializeField] private GunType _gunType;
    public RectTransform rectTransform => transform as RectTransform;
    [SerializeField] private TextMeshProUGUI ammoTMP;
    public void UpdateAmmo(float value)
    {
        ammoTMP.text = ((int)(value * 100)).ToString() + "%";
    }
}
