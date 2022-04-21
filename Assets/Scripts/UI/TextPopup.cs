using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(TMP_Text))]
public class TextPopup : MonoBehaviour
{
    [HideInInspector]
    public string DisplayText = "!";
    public Vector3 Direction = Vector3.up;
    
    // Start is called before the first frame update
    void Start()
    {
        TMP_Text tmp_text = GetComponent<TMP_Text>();
        tmp_text.text = DisplayText;
        tmp_text.DOFade(0f, 1f);
        transform.DOMove(transform.position + Direction, 1.05f).OnComplete(() => {
            Destroy(gameObject);
        });
    }
}
