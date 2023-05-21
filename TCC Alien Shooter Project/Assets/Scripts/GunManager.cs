using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GunManager : MonoBehaviour
{
    [SerializeField] List<Gun> guns;
    public int selectedGunIndex 
    {get => _selectedGunIndex;
    set {
        var pastValue = _selectedGunIndex; 
        _selectedGunIndex = value; 
        if(pastValue != _selectedGunIndex) GunSelected();
    }}
    private int _selectedGunIndex;

    private void Start() 
    {
        GunSelected();
    }

    void Update()
    {
        if(GameState.isGamePaused) return;
        var input = Input.GetAxis("Mouse ScrollWheel");
        if(input > 0) selectedGunIndex += 1;
        if(input < 0) selectedGunIndex -= 1;
        if(Input.GetButtonDown("One")) selectedGunIndex = 0;
        else if(Input.GetButtonDown("Two")) selectedGunIndex = 1;
        else if(Input.GetButtonDown("Three")) selectedGunIndex = 2;
    }

    public void SetSelectedGun(GunType gunType)
    {
        selectedGunIndex = guns.FindIndex(g => g.gunType == gunType);
        GunSelected();
    }

    private void GunSelected()
    {
        foreach (var gun in guns) gun.gameObject.SetActive(false);
        if(GameState.SaveData.gunsColected == null || GameState.SaveData.gunsColected.Count <= 0) return;

        var colectedGuns = guns.Where(g => GameState.SaveData.gunsColected.Contains(g.gunType)).ToArray();

        var index = selectedGunIndex % colectedGuns.Length;
        if (index < 0) index = colectedGuns.Length - 1;
        if (index > colectedGuns.Length - 1) index = 0;

        var selectedGun = guns[guns.IndexOf(colectedGuns[index])];
        selectedGun.gameObject.SetActive(true);

        if(index != Mathf.FloorToInt(selectedGunIndex)) selectedGunIndex = index;
        GameState.mainCanvas.GunSelected(selectedGun.gunType);
    }
}
