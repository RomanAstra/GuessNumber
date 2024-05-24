using System.Collections.Generic;
using Helpers;
using UniRx;

namespace Core
{
    public sealed class GameController
    {
        public int HiddenNumber { get; }
        public int MinNumber { get; private set;}
        public int MaxNumber { get; private set;}
        public IReadOnlyReactiveProperty<GameState> State => _gameState;
        public IReadOnlyReactiveProperty<int> CharacterSelectNumber => _characterSelectNumber;
        public IReadOnlyReactiveProperty<int> AISelectNumber => _aiSelectNumber;

        private readonly IntReactiveProperty _characterSelectNumber = new();
        private readonly IntReactiveProperty _aiSelectNumber = new();
        private readonly ReactiveProperty<GameState> _gameState;
        private readonly Randomizer _randomizer;
        private readonly HashSet<int> _numbersUsed;
        
        public GameController(IntRange startRange, Randomizer randomizer)
        {
            _randomizer = randomizer;
            HiddenNumber = _randomizer.GetRandom(startRange);
            MinNumber = startRange.From;
            MaxNumber = startRange.To;
            _numbersUsed = new HashSet<int>(MaxNumber);
            _gameState = new ReactiveProperty<GameState>(GameState.CharacterProcess);
        }

        public void SetCharacterNumber(int number)
        {
            _characterSelectNumber.Value = number;
            if (HiddenNumber == number)
            {
                _gameState.Value = GameState.CharacterWin;
            }
            else
            {
                SetMinMaxNumber(number);
                _gameState.Value = GameState.AIProcess;
            }
        }

        public void SetAINumber(int number)
        {
            _aiSelectNumber.Value = number;
            if (HiddenNumber == number)
            {
                _gameState.Value = GameState.AIWin;
            }
            else
            {
                SetMinMaxNumber(number);
                _gameState.Value = GameState.CharacterProcess;
            }
        }

        private void SetMinMaxNumber(int number)
        {
            _numbersUsed.Add(number);
            if (number > HiddenNumber)
            {
                MinNumber = HiddenNumber;
                if (MaxNumber > number)
                {
                    MaxNumber = number; 
                }
            }
            else if (number < HiddenNumber)
            {
                MaxNumber = HiddenNumber;
                MinNumber = number;
            }
        }

        public int GetRandom()
        {
            List<int> poolNumbers = ListPool<int>.Get();
            for (int i = MinNumber; i <= MaxNumber; i++)
            {
                if (!_numbersUsed.Contains(i))
                {
                    poolNumbers.Add(i);
                }
            }

            int random = _randomizer.GetRandom(poolNumbers);
            ListPool<int>.Release(poolNumbers);
            return random;
        }

        public void Shuffle<T>(IList<T> numbers)
        {
            _randomizer.Shuffle(numbers);
        }
    }
}
