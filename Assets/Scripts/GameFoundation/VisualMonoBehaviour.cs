//
// Copyright Imangi Studios, LLC, Copyright 2024. All rights reserved
//

using UnityEngine;
using UnityEngine.UI;

namespace GameFoundation
{
    public class VisualMonoBehaviour : MonoBehaviour
    {
        protected Renderer NormalRenderer;
        protected Graphic CanvasRenderer;
        protected bool RendererIsUI;

        protected virtual void Awake()
        {
            // normal renderer
            NormalRenderer = GetComponent<Renderer>();
            CanvasRenderer = GetComponent<Graphic>();
            if (CanvasRenderer != null) RendererIsUI = true;
        }

        public void SetVisible(bool visible)
        {
            if (RendererIsUI)
            {
                CanvasRenderer.enabled = visible;
            }
            else
            {
                NormalRenderer.enabled = visible;
            }
        }
    }
}