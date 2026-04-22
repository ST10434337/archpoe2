using GLMSMVC.Services.Interfaces;

namespace GLMSMVC.Services.CurrencyConversion
{
    public class CurrencyCalculator : ICurrencyCalculator //(Geekific, 2022)
    {
        public decimal ConvertToZar(decimal amount, decimal exchangeRate)
        {
            return Math.Round(amount * exchangeRate, 2);
        }
    }
}
/*
 Geekific. 2022. The Facade Pattern Explained and Implemented in Java | Structural 
Design Patterns | Geekific. [video online] Available at 
<https://youtu.be/xWk6jvqyhAQ?si=N8kiK-mawCrqQEa6> [Accessed 28 March 2026].
 */
