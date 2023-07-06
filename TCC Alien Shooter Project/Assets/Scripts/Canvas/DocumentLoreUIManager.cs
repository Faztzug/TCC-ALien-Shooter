using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DocumentLoreUIManager : MonoBehaviour
{
    [HideInInspector] public LoreDocument loreDocument;
    [SerializeField] private TextMeshProUGUI tittle;
    [SerializeField] private TextMeshProUGUI body;

    public void SetLoreDucmentUI(LoreDocument document)
    {
        this.gameObject.SetActive(true);
        this.loreDocument = document;
        tittle.text = loreDocument.tittleText;
        body.text = loreDocument.bodyText;
        LayoutRebuilder.ForceRebuildLayoutImmediate(tittle.transform.parent as RectTransform);
    }
}
