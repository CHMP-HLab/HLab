namespace HLab.Geo;

public static class GeoExtensions
{
    public static Segment Segment(this Rect rect, Side side)
    {
        switch (side)
        {
            case Side.Left:
                return new Segment(rect.TopLeft, rect.BottomLeft);
            case Side.Right:
                return new Segment(rect.TopRight, rect.BottomRight);
            case Side.Top:
                return new Segment(rect.TopLeft, rect.TopRight);
            case Side.Bottom:
                return new Segment(rect.BottomLeft, rect.BottomRight);
            case Side.None:
            default:
                return null;
        }
    }

        ///<summary>
    /// Matrix
    ///</summary>

}