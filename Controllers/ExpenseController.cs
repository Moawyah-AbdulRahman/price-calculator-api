using Microsoft.AspNetCore.Mvc;
using Prog;

namespace price_calculator_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpenseController : ControllerBase
{
    private const double TAX_RATE = 0.21;
    private const double UNIVERSAL_DISCOUNT_RATE = 0.15;
    private readonly (uint Upc, double Rate) SPECIAL_DISCOUNT = (12345, 0.07);

    [HttpGet(Name = "GetExpenseMassage")]
    public IActionResult Get(ExpenseControllerDataContainer data)
    {
        var product = data.Product;
        var expenses = data.Expenses!.Select(b => b.ToExpense(product!.BasePrice.Currency));

        var modifier = ExpenseRequirement(expenses.ToArray());

        modifier.ModifyPrice(product!);
        return Ok(data.Product);
    }

    private PriceModifier ExpenseRequirement(params Expense[] expenses)
    {
        return new AtomicModifier(
                new AtomicModifier(expenses),
                new Tax(TAX_RATE),
                new UniversalDiscount(UNIVERSAL_DISCOUNT_RATE),
                new SpecialDiscount(SPECIAL_DISCOUNT.Upc, SPECIAL_DISCOUNT.Rate)
            );
    }
}