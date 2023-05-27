using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public class GunManager : MonoBehaviour
{
    [SerializeField] List<Gun> guns;
    public int selectedGunIndex 
    {get => _selectedGunIndex;
    set {
        var pastValue = _selectedGunIndex; 
        _selectedGunIndex = value % guns.Count; 
        if(pastValue != _selectedGunIndex) GunSelected();
    }}
    private int _selectedGunIndex;
    [SerializeField] private Sound gubSelectSound;
    private AudioSource audioSource;
    private Animator anim;
    [SerializeField] private float gunChangeWait;
    private Coroutine gunChange;
    private float scroolInput;

    private void Start() 
    {
        audioSource = GetComponentInChildren<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        foreach (var gun in guns) gun.gameObject.SetActive(false);
        GunSelected();
    }

    void Update()
    {
        if(GameState.isGamePaused) return;
        scroolInput += Input.GetAxis("Mouse ScrollWheel");
        if(scroolInput > 1) selectedGunIndex += 1;
        if(scroolInput < 0) selectedGunIndex -= 1;
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
        if(gunChange != null) StopCoroutine(gunChange);
        gunChange = StartCoroutine(GunSelectedCourotine());
    }

    private IEnumerator GunSelectedCourotine()
    {
        anim.SetTrigger("Change");
        scroolInput = 0.5f;
        yield return new WaitForSeconds(gunChangeWait);

        foreach (var gun in guns) gun.gameObject.SetActive(false);
        if(GameState.SaveData.gunsColected == null || GameState.SaveData.gunsColected.Count <= 0) yield return null;

        var colectedGuns = guns.Where(g => GameState.SaveData.gunsColected.Contains(g.gunType)).ToArray();

        var index = selectedGunIndex % colectedGuns.Length;
        if (index < 0) index = colectedGuns.Length - 1;
        if (index > colectedGuns.Length - 1) index = 0;

        var selectedGun = guns[guns.IndexOf(colectedGuns[index])];
        selectedGun.gameObject.SetActive(true);

        if(index != Mathf.FloorToInt(selectedGunIndex)) _selectedGunIndex = index;
        GameState.mainCanvas.GunSelected(selectedGun.gunType);
        gubSelectSound.PlayOn(audioSource);
    }
}
