﻿using System.Collections;
using System.Collections.Generic;
using BAStudio.MultiLayoutScroller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUnit : ItemInstance
{
    public Image image;
    public Image silver, gold, favored;
    public TextMeshProUGUI text;
    public override void SetData(object data)
    {
        GameInfo info = (GameInfo) data;
        favored.color = info.isFavored? new Color(0.7f, 0.2f, 0.2f, 1f) : Color.clear;
        silver.color = info.silverTierEnabled? new Color(0.8f, 0.8f, 0.8f, 1f) : Color.clear;
        gold.color = info.goldTierEnabled? new Color(0.1f, 0.8f, 0.8f, 1f) : Color.clear;
        text.text = info.nameIndex.ToString();
        //TODO: Test alpha vs toggle performance
    }

}
