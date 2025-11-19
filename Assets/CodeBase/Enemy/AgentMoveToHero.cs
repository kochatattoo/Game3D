using CodeBase.Infrastructure;
using UnityEngine;
using UnityEngine.AI;

namespace CodeBase.Enemy
{
    public class AgentMoveToHero : Follow
    {
        private const float MinimalDistance = 0.8f;
        public NavMeshAgent Agent;
        private Transform _heroTransform;
        private IGameFactory _gameFactory;

        public void Construct(Transform heroTransform) => 
            _heroTransform = heroTransform;


        private void Update()
        {
            if ( HeroNotReached() && !IsDied)
                Agent.destination = _heroTransform.position;
        }

        private bool HeroNotReached() =>
            Vector3.Distance(Agent.transform.position, _heroTransform.position) >= MinimalDistance;
    }
}
