namespace Assets.Scripts.Enums
{
    public enum CardElement
    {
        Fire = 0,
        Air = 1,
        Water = 2,
        Light = 3,
        Darkness = 4,
        Love = 5,
        Magic = 6,
        Nature = 7
    }

    public static class CardElementExtensions
    {
        public static bool Beats(this CardElement element, CardElement other)
        {
            return (element, other) switch
            {
                (CardElement.Fire, CardElement.Fire) => true,
                (CardElement.Air, CardElement.Air) => true,
                (CardElement.Water, CardElement.Water) => true,
                (CardElement.Light, CardElement.Light) => true,
                (CardElement.Darkness, CardElement.Darkness) => true,
                (CardElement.Love, CardElement.Love) => true,
                (CardElement.Magic, CardElement.Magic) => true,
                (CardElement.Nature, CardElement.Nature) => true,

                (CardElement.Fire, CardElement.Air) => true,
                (CardElement.Air, CardElement.Water) => true,
                (CardElement.Water, CardElement.Light) => true,
                (CardElement.Light, CardElement.Darkness) => true,
                (CardElement.Darkness, CardElement.Love) => true,
                (CardElement.Love, CardElement.Magic) => true,
                (CardElement.Magic, CardElement.Nature) => true,
                (CardElement.Nature, CardElement.Fire) => true,
                _ => false
            };
        }

        public static bool Combined(this CardElement element, CardElement other)
        {
            return (element, other) switch
            {
                (CardElement.Fire, CardElement.Fire) => true,
                (CardElement.Air, CardElement.Air) => true,
                (CardElement.Water, CardElement.Water) => true,
                (CardElement.Light, CardElement.Light) => true,
                (CardElement.Darkness, CardElement.Darkness) => true,
                (CardElement.Love, CardElement.Love) => true,
                (CardElement.Magic, CardElement.Magic) => true,
                (CardElement.Nature, CardElement.Nature) => true,

                (CardElement.Fire, CardElement.Air) => true,
                (CardElement.Air, CardElement.Fire) => true,
                (CardElement.Air, CardElement.Water) => true,
                (CardElement.Water, CardElement.Air) => true,
                (CardElement.Darkness, CardElement.Love) => true,
                (CardElement.Love, CardElement.Darkness) => true,
                (CardElement.Love, CardElement.Magic) => true,
                (CardElement.Magic, CardElement.Love) => true,
                _ => false
            };
        }
    }
}
