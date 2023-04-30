using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    [SerializeField] GameObject[] guns;
    public int selectedGun 
    {get => _selectedGun;
    set {
        var pastValue = _selectedGun; 
        _selectedGun = value; 
        if(pastValue != _selectedGun) GunSelected();
    }}
    private int _selectedGun;

    private void Start() 
    {
        GunSelected();
    }

    void Update()
    {
        var input = Input.GetAxis("Mouse ScrollWheel");
        if(input > 0) selectedGun += 1;
        if(input < 0) selectedGun -= 1;
        if(Input.GetButtonDown("One")) selectedGun = 0;
        else if(Input.GetButtonDown("Two")) selectedGun = 1;
        else if(Input.GetButtonDown("Three")) selectedGun = 2;
    }

    private void GunSelected()
    {
        var index = selectedGun % guns.Length;
        if (index < 0) index = guns.Length - 1;
        if (index > guns.Length - 1) index = 0;

        foreach (var gun in guns) gun.SetActive(false);
        guns[index].SetActive(true);

        if(index != Mathf.FloorToInt(selectedGun)) selectedGun = index;
    }
}
