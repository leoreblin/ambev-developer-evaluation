using System.Text.RegularExpressions;

namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public readonly struct CNPJ(string cnpj)
{
    private readonly string _cnpj = new Regex(@"\D").Replace(cnpj, "");

    public string Value => ToString();

    public string ToStringWithMask()
    {
        return long.Parse(_cnpj).ToString("00\\.000\\.000/0000-00");
    }

    public string ToStringWithoutMask()
    {
        return _cnpj;
    }

    public override string ToString() => ToStringWithMask();

    public static implicit operator string(CNPJ cnpj)
    {
        return cnpj.ToString();
    }

    public readonly bool IsValid()
    {
        int[] multiplier1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] multiplier2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int sum;
        int remainder;
        string digit;
        string tempCnpj;

        if (_cnpj.Length != 14)
        {
            return false;
        }

        tempCnpj = _cnpj[..12];
        sum = 0;

        for (int i = 0; i < 12; i++)
        {
            sum += int.Parse(tempCnpj[i].ToString()) * multiplier1[i];
        }

        remainder = (sum % 11);

        if (remainder < 2)
        {
            remainder = 0;
        }
        else
        {
            remainder = 11 - remainder;
        }

        digit = remainder.ToString();
        tempCnpj += digit;
        sum = 0;

        for (int i = 0; i < 13; i++)
        {
            sum += int.Parse(tempCnpj[i].ToString()) * multiplier2[i];
        }

        remainder = (sum % 11);

        if (remainder < 2)
        {
            remainder = 0;
        }
        else
        {
            remainder = 11 - remainder;
        }

        digit += remainder.ToString();

        return _cnpj.EndsWith(digit);
    }
}
