using Helpers;
using UnityEngine;

namespace EntryPoint
{
    public sealed class Main : MonoBehaviour
    {
        [SerializeField] private int _minNumber = 2;
        [SerializeField] private int _maxNumber = 90;
        private readonly Randomizer _randomizer = new ();
        private GameLifeCycleController _gameLifeCycleController;

        private void Awake()
        {
            var startRange = new IntRange(_minNumber, _maxNumber);
            _gameLifeCycleController = new GameLifeCycleController(startRange, _randomizer);
        }

        private void Start()
        {
            _gameLifeCycleController.StartGame();
        }
    }
}
