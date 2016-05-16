namespace NanoGames.Games.Bomberguy
{
    internal interface RectbombularThing : BomberThing
    {
        Vector TopLeft { get; }

        Vector TopRight { get; }

        Vector BottomLeft { get; }

        Vector BottomRight { get; }
    }
}
