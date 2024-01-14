using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class MovieCardLabel : MonoBehaviour
    {
        private TextMeshProUGUI label;
        private TypewriterText typewriterText;

        private void Awake()
        {
            label = GetComponent<TextMeshProUGUI>();
            typewriterText = GetComponent<TypewriterText>();
        }

        public void SetText(string text, float typewriterSpeed=0f)
        {
            if (typewriterSpeed > 0 && typewriterText != null)
            {
                typewriterText.PlayTypewriter(text, typewriterSpeed);
            }
            else
            {
                label.text = text;
            }
        }

        public bool IsStillTyping()
        {
            bool notFinished = typewriterText.ProgressNorm < 1;
            return notFinished;
        }

        public void FinishTyping()
        {
            typewriterText.Finish();
        }
    }
}