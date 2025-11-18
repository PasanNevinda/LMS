namespace LMS.Helpers
{
    public static class Money
    {
        // Use decimal for money. Optionally centralize precision/rounding here.
        public static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }

}
