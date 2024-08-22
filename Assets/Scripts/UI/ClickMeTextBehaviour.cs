using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class ClickMeTextBehaviour : MonoBehaviour
    {
        public void Update()
        {
            if (ApplicationLifetime.CurrentMode is not TitleMenuMode)
            {
                gameObject.SetActive(false);
            }
        }
    }
}