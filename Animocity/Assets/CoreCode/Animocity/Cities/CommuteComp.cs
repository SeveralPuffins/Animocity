using Animocity.Cities.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity.Cities
{
    public class CommuteComp : MonoBehaviour
    {
        private const float WALK_SPEED = 2.0f;
        public Commute commute {get;set;}
        private MultiPoint currentOrigin;
        private MultiPoint currentTarget;


        Animator animController;

        private float tlerp = 0f;
        private int idx = 1;

        private void Start()
        {
            animController = GetComponent<Animator>();
            currentOrigin = commute.Origin;
            currentTarget = commute.GetNode(idx);

            animController.speed = WALK_SPEED;
        }

        private void Update()
        {
            if(!commute.IsValid())
            {
                Destroy(this.gameObject);
            }
            else
            {
                tlerp += WALK_SPEED * Time.deltaTime;

                animController.SetFloat("Blend", (1f+tlerp*2f));

                if (tlerp > 1f )
                {
                    if (GetHasArrived())
                    {
                        Destroy(this.gameObject);
                        return;
                    }
                    else
                    {
                        tlerp -= 1f;
                        this.currentOrigin = currentTarget;
                        this.currentTarget = commute.GetNode(++idx);
                    }
                }

                Vector3 o = currentOrigin.ToWorldPoint();
                Vector3 d = currentTarget.ToWorldPoint();
                this.transform.position = Vector3.Lerp(o, d, tlerp) + new Vector3(0,0,-2);

                if(d.y > o.y || d.y < o.y)
                {
                    this.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
                }
                else if(d.x > o.x)
                {
                    this.transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
                }
                else
                {
                    this.transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
                }
            }
        }

        private bool GetHasArrived()
        {
            return commute.Destination.Equals(currentTarget) && tlerp>=0.99;
        }
    }
}
