using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Pool;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Animocity.UI
{
    public class GridHighlightManager : MonoBehaviour
    {
        public RectTransform highlightCanvas;
        public Material mat;

        private Queue<RectHighlight> worldspaceHighlights; 

        private void Awake()
        {
            worldspaceHighlights = new Queue<RectHighlight>();
        }

        private void OnEnable()
        {
            RenderPipelineManager.endCameraRendering += this.DrawHighlights;
        }

        private void OnDisable()
        {
            RenderPipelineManager.endCameraRendering -= this.DrawHighlights;
        }

        private Vector2[] ScreenSpaceCorners(Camera cam, Rect worldspaceRect)
        {
            var sMin = cam.WorldToViewportPoint(worldspaceRect.min);
            var sMax = cam.WorldToViewportPoint(worldspaceRect.max);

            //print($"Drawing from {sMin} to {sMax}");

            //var scX = 1f / Screen.width;
            //var scY = 1f / Screen.height;

            return new Vector2[] 
            { 
                new Vector2(sMin.x, sMin.y),
                new Vector2(sMin.x, sMax.y),
                new Vector2(sMax.x, sMax.y),
                new Vector2(sMax.x, sMin.y),
            };
        } 

        public void PushHighlight(RectHighlight newHighlight)
        {
            worldspaceHighlights.Enqueue(newHighlight);
        }

        private void DrawHighlights(ScriptableRenderContext context, Camera cam)
        {
            GL.PushMatrix();
            mat.SetPass(0);
            GL.LoadOrtho();
            GL.Begin(GL.QUADS);

            while(worldspaceHighlights.Count>0)
            {
                var highlight = worldspaceHighlights.Dequeue();
                var verts = ScreenSpaceCorners(cam, highlight.rect);

                GL.Color(highlight.clr);
                GL.Vertex3(verts[0].x, verts[0].y, 0);
                GL.Vertex3(verts[1].x, verts[1].y, 0);
                GL.Vertex3(verts[2].x, verts[2].y, 0);
                GL.Vertex3(verts[3].x, verts[3].y, 0);
            }

            GL.End();
            GL.PopMatrix();

        }
    }
}
