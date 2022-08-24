using Prog;

namespace price_calculator_api.Controllers;

public class ExpenseControllerDataContainer
{
    public Product? Product { get; set; }

    public ExpenseBuilder[]? Expenses { get; set; }
}