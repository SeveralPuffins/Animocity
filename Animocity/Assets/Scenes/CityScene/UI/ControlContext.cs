using Animocity.Cities;
using Unity.VisualScripting;
using UnityEngine;

namespace Animocity.UI
{
    public class ControlContext
    {
        private static ControlContext _default;
        public static ControlContext Current { get; private set; }

        public virtual void Activate()
        {
            if (Current != null && Current != this)
            {
                Current.Release();
            }
            Current = this;
        }

        protected static Vector2 GetCurrentMousePosition()
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            return (Vector2)mouseRay.GetPoint(-Camera.main.transform.position.z);
        }

        public virtual void Release()
        {
            Current = _default;
        }
        public static void SetDefault(ControlContext _def)
        {
            _default = _def;
        }

        public virtual void OnHover(CityGrid grid, Vector3 hoverPositionWorld, bool drag=false, Vector3 dragFrom=default)
        {

        }

        public virtual void OnInteract(CityGrid grid, Vector3 interactPositionWorld, bool isDrag=false, Vector3 dragStartPositionWorld = default)
        {

        }
        public virtual void OnCommitInteract(CityGrid grid, Vector3 interactPositionWorld)
        {

        }

        public virtual void OnInspect(CityGrid grid, Vector3 inspectPositionWorld)
        {

        }
    }
}
