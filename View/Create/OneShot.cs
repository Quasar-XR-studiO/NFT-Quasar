using System;
using UnityEngine;

namespace ZBoom.Common.SpatialMap
{
    public class OneShot : MonoBehaviour
    {
        private bool m_Mirror;
        private Action<Texture2D> m_Callback;
        private bool m_Capturing;

        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination);
            if (!m_Capturing) { return; }

            var destTexture = new RenderTexture(Screen.width, Screen.height, 0);
            if (m_Mirror)
            {
                var mat = Instantiate(Resources.Load<Material>("Sample_MirrorTexture"));
                mat.mainTexture = source;
                Graphics.Blit(null, destTexture, mat);
            }
            else
            {
                Graphics.Blit(source, destTexture);
            }

            RenderTexture.active = destTexture;
            var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;
            Destroy(destTexture);

            m_Callback(texture);
            Destroy(this);
        }

        public void Shot(bool mirror, Action<Texture2D> callback)
        {
            if (callback == null) { return; }
            this.m_Mirror = mirror;
            this.m_Callback = callback;
            m_Capturing = true;
        }
    }
}
