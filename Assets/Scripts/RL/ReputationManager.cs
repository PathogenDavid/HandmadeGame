using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RL
{
    public class ReputationManager : MonoBehaviour
    {
        public static ReputationManager instance;

        public UnityAction<int> OnReputationGain;
        public UnityAction<int> OnReputationLoss;

        protected int _reputationPoints;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void AddReputationPoints(int po)
        {
            _reputationPoints += po;
            OnReputationGain.Invoke(po);
        }
        public void RemoveReputationPoints(int po)
        {
            _reputationPoints -= po;
            OnReputationLoss.Invoke(po);
        }
        public bool CheckReputationPoints(int po)
        {
            if(_reputationPoints >= po)
            {
                return true;
            }
            return false;
        }
    }
}