namespace Assets.Scripts.Enums
{
    public enum CardElement
    {
        Water = 0,
        Earth = 1,
        Fire = 2,
        Nature = 3,
        Magic = 4,
        Energy = 5
    }

    public static class CardElementExtensions
    {
        public static bool Beats(this CardElement element, CardElement other)
        {
            return (element, other) switch
            {
                (CardElement.Water, CardElement.Water) => true,
                (CardElement.Earth, CardElement.Earth) => true,
                (CardElement.Fire, CardElement.Fire) => true,
                (CardElement.Nature, CardElement.Nature) => true,
                (CardElement.Magic, CardElement.Magic) => true,
                (CardElement.Energy, CardElement.Energy) => true,

                (CardElement.Water, CardElement.Fire) => true,
                (CardElement.Water, CardElement.Nature) => true,
                (CardElement.Earth, CardElement.Fire) => true,
                (CardElement.Earth, CardElement.Nature) => true,
                (CardElement.Fire, CardElement.Magic) => true,
                (CardElement.Fire, CardElement.Energy) => true,
                (CardElement.Nature, CardElement.Magic) => true,
                (CardElement.Nature, CardElement.Energy) => true,
                (CardElement.Magic, CardElement.Water) => true,
                (CardElement.Magic, CardElement.Earth) => true,
                (CardElement.Nature, CardElement.Water) => true,
                (CardElement.Nature, CardElement.Earth) => true,
                _ => false
            };
        }

        public static bool Combined(this CardElement element, CardElement other)
        {
            return (element, other) switch
            {
                (CardElement.Water, CardElement.Water) => true,
                (CardElement.Earth, CardElement.Earth) => true,
                (CardElement.Fire, CardElement.Fire) => true,
                (CardElement.Nature, CardElement.Nature) => true,
                (CardElement.Magic, CardElement.Magic) => true,
                (CardElement.Energy, CardElement.Energy) => true,

                (CardElement.Water, CardElement.Earth) => true,
                (CardElement.Water, CardElement.Energy) => true,
                (CardElement.Earth, CardElement.Water) => true,
                (CardElement.Energy, CardElement.Water) => true,
                (CardElement.Magic, CardElement.Nature) => true,
                (CardElement.Magic, CardElement.Fire) => true,
                (CardElement.Nature, CardElement.Magic) => true,
                (CardElement.Fire, CardElement.Magic) => true,
                _ => false
            };
        }
    }
}
