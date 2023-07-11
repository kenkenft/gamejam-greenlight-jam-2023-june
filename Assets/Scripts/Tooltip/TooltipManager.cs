using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class TooltipManager : MonoBehaviour
{
    public Text HeaderField, ContentField;
    public LayoutElement VertLayout;
    public int CharacterWrapLimit;

    private void Update()
    {
        if(Application.isEditor)
        {
            int headerLength = HeaderField.text.Length;
            int contentLength = ContentField.text.Length;

            VertLayout.enabled = (headerLength > CharacterWrapLimit || contentLength > CharacterWrapLimit) ? true : false;
        }
    }

    public void SetText(string content, string header = "")
    {
        if(string.IsNullOrEmpty(header))
        {
            HeaderField.gameObject.SetActive(false);
        }
        else
        {
            HeaderField.gameObject.SetActive(true);
            HeaderField.text = header;
        }

        ContentField.text = content;

        int headerLength = HeaderField.text.Length;
        int contentLength = ContentField.text.Length;

        VertLayout.enabled = (headerLength > CharacterWrapLimit || contentLength > CharacterWrapLimit) ? true : false;
    }

}
