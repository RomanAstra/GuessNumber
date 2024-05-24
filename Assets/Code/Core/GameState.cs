namespace Core
{
    public enum GameState : byte
    {
        None              = 0,
        CharacterProcess = 1,
        AIProcess         = 2,
        CharacterWin      = 3,
        AIWin              = 4
    }
}
