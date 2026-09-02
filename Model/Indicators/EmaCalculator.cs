using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Model.Indicators
{
  using System;
using System.Collections.Generic;
using System.Linq;

public static class EmaCalculatorDecimal
{
    /// <summary>
    /// Вычисляет Exponential Moving Average (EMA) с использованием типа decimal.
    /// </summary>
    /// <param name="values">Исходные значения (например, цены закрытия)</param>
    /// <param name="period">Период EMA (целое положительное число)</param>
    /// <returns>Список значений EMA. Первые (period - 1) значений будут null.</returns>
    public static List<decimal?> CalculateEma(IList<decimal> values, int period)
    {
        if (values == null || values.Count == 0 || period <= 0)
            return new List<decimal?>(values?.Count ?? 0);

        var ema = new List<decimal?>(new decimal?[values.Count]);

        // Коэффициент сглаживания: α = 2 / (period + 1)
        decimal alpha = 2m / (period + 1);

        // Инициализация: первое значение EMA = SMA за первые 'period' значений
        if (values.Count >= period)
        {
            decimal sum = 0;
            for (int i = 0; i < period; i++)
                sum += values[i];

            decimal firstEma = sum / period;
            ema[period - 1] = firstEma;

            // Рекуррентный расчёт EMA
            for (int i = period; i < values.Count; i++)
            {
                decimal currentPrice = values[i];
                decimal prevEma = ema[i - 1].Value;
                decimal currentEma = currentPrice * alpha + prevEma * (1m - alpha);
                ema[i] = currentEma;
            }
        }

        return ema;
    }
}
}
