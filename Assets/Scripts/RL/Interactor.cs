using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RL
{
    public class Interactor : MonoBehaviour
    {
        public float RayCastLength = 3f;

        protected bool _isRaycastActive = true;

        [SerializeField] private bool _enableDebugging;
        private Transform _transform;
        private IInteractable _interactable;

        private void Awake()
        {
            _transform = this.gameObject.transform;
        }
        private void LateUpdate()
        {
            if( _isRaycastActive )
            {
                //TODO: Multiple selections can cause unselect to not work properly.
                RaycastHit hit;
                if(Physics.Raycast(_transform.position, _transform.forward * RayCastLength, out hit))
                {
                    var col = hit.collider.GetComponent<IInteractable>();
                    if(col != null)
                    {
                        _interactable = col;
                        _interactable.Selected();
                    }
                }
                else
                {
                    if(_interactable != null)
                    {
                        _interactable.Unselected();
                        _interactable = null;
                    }
                }

                if (_enableDebugging)
                {
                    Debug.DrawRay(_transform.position, _transform.forward * RayCastLength);
                    Debug.Log("Debug Ray:" + _transform.position.ToString() + " - " + (_transform.forward * RayCastLength).ToString());
                }
            }
        }

        public void EnableRaycast(bool bl)
        {
            _isRaycastActive = bl;
        }
    }

    public interface IInteractable
    {
        void Interact();
        void Selected();
        void Unselected();
    }
}