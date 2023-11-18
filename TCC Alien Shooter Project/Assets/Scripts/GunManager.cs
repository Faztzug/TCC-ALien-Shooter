using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public class GunManager : MonoBehaviour
{
    [SerializeField] List<Gun> guns;
    List<Gun> avaibleGuns;
    public List<Gun> AvaibleGuns => avaibleGuns;
    public int selectedGunIndex 
    {get => _selectedGunIndex;
    set {
        scroolInput = 0.5f;
        var pastValue = _selectedGunIndex;

        if(value >= avaibleGuns.Count) _selectedGunIndex = 0;
        else if(value < 0) _selectedGunIndex = avaibleGuns.Count - 1;
        else _selectedGunIndex = value;
        
        if(pastValue != _selectedGunIndex) GunSelected();
    }}
    private int _selectedGunIndex;
    [SerializeField] private Sound gubSelectSound;
    private AudioSource audioSource;
    private Animator anim;
    [SerializeField] private float gunChangeWait;
    private bool changingGun;
    private Coroutine gunChange;
    private float scroolInput;
    [SerializeField] private ReticulaFeedback reticulaFeedback;

    private void Start() 
    {
        scroolInput = 0.5f;
        audioSource = GetComponentInChildren<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        foreach (var gun in guns) gun.gameObject.SetActive(false);
        UpdateAvaibleGuns();
        GunSelected();
    }

    public void UpdateAvaibleGuns() => avaibleGuns = guns.Where(g => GameState.SaveData.gunsColected.Contains(g.gunType)).ToList();

    void Update()
    {
        foreach (var gun in avaibleGuns)
        {
            gun.AmmoRegen();
        }

        if(GameState.isGamePaused || changingGun) return;
        scroolInput += Input.GetAxis("Mouse ScrollWheel");

        if(scroolInput > 1) selectedGunIndex -= 1;
        else if(scroolInput < 0) selectedGunIndex += 1;

        if(Input.GetButtonDown("One")) selectedGunIndex = 0;
        else if(Input.GetButtonDown("Two")) selectedGunIndex = 1;
        else if(Input.GetButtonDown("Three")) selectedGunIndex = 2;
    }

    public void SetSelectedGun(GunType gunType)
    {
        selectedGunIndex = avaibleGuns.FindIndex(g => g.gunType == gunType);
        GunSelected();
    }

    private void GunSelected()
    {
        if(gunChange != null) StopCoroutine(gunChange);
        gunChange = StartCoroutine(GunSelectedCourotine());
    }

    public void NextGun()
    {
        if(avaibleGuns.FindAll(g => g.IsAmmoEmpty()).Count >= avaibleGuns.Count)
        {
            if (selectedGunIndex == 0) return;
            else
            {
                selectedGunIndex = 0;
                return;
            }
        }
        var nextIndex = selectedGunIndex+1;
        if(nextIndex >= avaibleGuns.Count) nextIndex = 0;
        selectedGunIndex = nextIndex;
    }

    private IEnumerator GunSelectedCourotine()
    {
        anim.SetTrigger("Change");
        scroolInput = 0.5f;
        changingGun = true;
        yield return new WaitForSeconds(gunChangeWait);

        foreach (var gun in guns) gun.gameObject.SetActive(false);
        if(GameState.SaveData.gunsColected == null || GameState.SaveData.gunsColected.Count <= 0) yield return null;
        
        changingGun = false;

        var colectedGuns = guns.Where(g => GameState.SaveData.gunsColected.Contains(g.gunType)).ToArray();

        if(colectedGuns.Length <= 0) yield break;
        var index = selectedGunIndex % colectedGuns.Length;
        if (index < 0) index = colectedGuns.Length - 1;
        if (index > colectedGuns.Length - 1) index = 0;

        var selectedGun = guns[guns.IndexOf(colectedGuns[index])];
        selectedGun.gameObject.SetActive(true);

        if(index != Mathf.FloorToInt(selectedGunIndex)) _selectedGunIndex = index;
        GameState.mainCanvas.GunSelected(selectedGun.gunType);
        gubSelectSound.PlayOn(audioSource);
        reticulaFeedback.SetReticulaIndex(index);
    }
}
