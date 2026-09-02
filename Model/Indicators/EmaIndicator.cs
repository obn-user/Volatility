namespace Test.Model.Indicators
{
    public class EmaIndicator
    {
        public List<decimal> Emas = new();
        public EmaIndicator(List<decimal> data)
        {
            int EmaPeriod = data.Count;
            List<decimal> emas = new List<decimal>();
            decimal initialEma = 0; // первый член последовательности ЕМА - прстое среднее


            for (int i = 0; i < EmaPeriod; i++)
            {
                initialEma += data[i];
            }
            initialEma = initialEma / EmaPeriod;

            
            for (int i = 0; i < data.Count; i++)
            {

                decimal ema = initialEma;
                if (i < EmaPeriod | i == EmaPeriod) // первые (период ЕМА )члены массива ЕМА и ст откл 
                                                    //невозможно рассчитать , поэтому вместо них нули
                {
                    emas.Add(Convert.ToDecimal(0));

                }

                if (i > EmaPeriod)
                {
                    decimal a = 2;
                    decimal b = EmaPeriod + 1;
                    decimal N = a / b;

                    for (int k = i - EmaPeriod; k < i; k++)
                    {
                        ema = data[k] * N + (ema * (1 - N));
                    }

                    emas.Add(ema); // локальный массив ЕМА
                }

            }
            Emas=emas;
        }
    }
}
/*
 * 
 * using System;
using System.Collections.Generic;

public static class EmaCalculator
{
    /// <summary>
    /// Рассчитывает экспоненциальную скользящую среднюю (EMA) для последнего значения в списке.
    /// </summary>
    /// <param name="values">Список входных значений (должен содержать хотя бы одно значение)</param>
    /// <param name="period">Период сглаживания (должен быть >= 1)</param>
    /// <returns>EMA последнего значения</returns>
    /// <exception cref="ArgumentException">Если данные или параметры некорректны</exception>
    public static decimal CalculateEma(List<decimal> values, int period)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));
        if (values.Count == 0)
            throw new ArgumentException("Список значений не должен быть пустым.", nameof(values));
        if (period < 1)
            throw new ArgumentException("Период должен быть >= 1.", nameof(period));

        // Если данных меньше, чем период — можно либо вернуть простое среднее, либо просто первое значение
        // Стандартный подход: для первых значений до `period` использовать SMA как стартовое значение
        if (values.Count < period)
        {
            // Можно использовать простое среднее за доступное количество элементов
            decimal sum = 0;
            foreach (var val in values)
                sum += val;
            return sum / values.Count;
        }

        // Вычисляем начальное значение — SMA за первые `period` элементов
        decimal sma = 0;
        for (int i = 0; i < period; i++)
            sma += values[i];
        sma /= period;

        // Коэффициент сглаживания: α = 2 / (period + 1)
        decimal alpha = 2m / (period + 1);

        // Последовательно вычисляем EMA от `period`-го элемента до конца
        decimal ema = sma;
        for (int i = period; i < values.Count; i++)
        {
            ema = (values[i] - ema) * alpha + ema;
        }

        return ema;
    }
}
 * 
 */