using Prog;

namespace price_calculator_api.Controllers;

public class ExpenseBuilder
{
    public string? Type { get; set; }

    public string? Description { get; set; }

    public double Value { get; set; }

    public Expense ToExpense(string currency)
    {
        if (Type == "absolute")
            return new AbsoluteExpense(Description!, new Price(Value, currency));
        else if (Type == "rate")
            return new PercentExpense(Description!, Value);

        throw new ArgumentException("Expense token type must have a value of 'absolute' or 'rate' only");
    }
}