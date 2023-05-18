using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct LoreDocument
{
    public int indexOrder;
    public string tittleText;
    [TextArea(5,15)]public string bodyText;
}
