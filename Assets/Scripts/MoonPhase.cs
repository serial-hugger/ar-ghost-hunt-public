using System;
using UnityEngine;

public class MoonPhase:MonoBehaviour
{
    private void Start()
    {
    }
    public double GetPhase(DateTime date)
    {
        double JD = GetJulianDate(date);
        double T = (JD - 2451545.0) / 36525.0;

        double L = 280.46645 + 36000.76983 * T + 0.0003032 * Math.Pow(T, 2);
        L = WrapAngle(L);

        double M = 357.52910 + 35999.05030 * T - 0.0001559 * Math.Pow(T, 2) - 0.00000048 * Math.Pow(T, 3);
        M = WrapAngle(M);

        double F = 93.2720993 + 483202.0175273 * T - 0.0034029 * Math.Pow(T, 2) - Math.Pow(T, 3) / 3526000 + Math.Pow(T, 4) / 863310000;
        F = WrapAngle(F);

        double L_prime = L + 1.915 * Math.Sin(M * Math.PI / 180) + 0.020 * Math.Sin(2 * M * Math.PI / 180);
        double Ω = 125.04452 - 1934.136261 * T + 0.0020708 * Math.Pow(T, 2) + Math.Pow(T, 3) / 450000;
        double ΔΨ = -0.000319 * Math.Sin(F * Math.PI / 180) - 0.000024 * Math.Sin(2 * F * Math.PI / 180);
        double Δϵ = 0.002443 * Math.Cos(F * Math.PI / 180) + 0.000011 * Math.Cos(2 * F * Math.PI / 180);
        double λ = L_prime + ΔΨ;
        double β = 0.02665 * Math.Sin(Ω * Math.PI / 180);
        double Λ = WrapAngle(λ - Ω + Δϵ);

        double phase = Math.Abs(Math.Cos((Λ * Math.PI / 180)));

        if (phase < 0.0 || phase > 1.0)
            throw new ArgumentException("Invalid date.");
        return phase;
    }

    private double GetJulianDate(DateTime date)
    {
        int year = date.Year;
        int month = date.Month;
        int day = date.Day;
        int hour = date.Hour;
        int minute = date.Minute;
        int second = date.Second;

        double dayFraction = (hour + (minute + (second / 60.0)) / 60.0) / 24.0;

        if (month < 3)
        {
            year--;
            month += 12;
        }

        double a = Math.Floor(year / 100.0);
        double b = 2 - a + Math.Floor(a / 4.0);

        double JD = Math.Floor(365.25 * (year + 4716)) + Math.Floor(30.6001 * (month + 1)) + day + b - 1524.5 + dayFraction;

        return JD;
    }

    private double WrapAngle(double angle)
    {
        return angle % 360;
    }
}