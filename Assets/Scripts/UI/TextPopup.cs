using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(TMP_Text))]
public class TextPopup : MonoBehaviour
{
    [HideInInspector]
    public string displayText;
    
    // Start is called before the first frame update
    void Start()
    {
        TMP_Text tmp_text = GetComponent<TMP_Text>();
        tmp_text.text = displayText;
        tmp_text.DOFade(0f, 1f);
        transform.DOMove(transform.position + Vector3.up, 1.05f).OnComplete(() => {
            Destroy(gameObject);
        });
    }
}
