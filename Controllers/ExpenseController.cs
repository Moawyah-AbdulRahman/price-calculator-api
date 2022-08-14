using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Prog;

namespace price_calculator_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpenseController : ControllerBase
{
    private const double TAX_RATE = 0.21;
    private const double UNIVERSAL_DISCOUNT_RATE = 0.15;
    private readonly (uint Upc, double Rate) SPECIAL_DISCOUNT = (12345, 0.07);

    [HttpGet(Name = "GetHolaMassage")]
    public IActionResult Get(JObject massage)
    {
        var product = JTokenToProduct(massage["product"]!);
        
        var currency = product.BasePrice.Currency;
        var expenses = JTokenToExpenses( massage["expenses"]!, currency);

        var modifier = ExpenseRequirement(expenses.ToArray());

        modifier.ModifyPrice(product);
        
        return Ok(product);
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

    private static IEnumerable<Expense> JTokenToExpenses(JToken expensesToken, string currency)
    {
        return expensesToken.Select(e => JTokenToSingleExpense(e, currency));
    }

    private static Expense JTokenToSingleExpense(JToken expenseToken, string currency)
    {
        var type = (string)expenseToken["type"]!;
        var description = (string)expenseToken["description"]!;
        if (type == "absolute")
        {
            return new AbsoluteExpense(
                description,
                new Price((double)expenseToken["price"]!, currency)
                );
        }
        else if (type == "rate")
        {
            return new PercentExpense(
                description,
                (double)expenseToken["rate"]!
                );
        }
        throw new ArgumentException("Expense token type must have a value of 'absolute' or 'rate' only");
    }

    private static Product JTokenToProduct(JToken productToken)
    {
        return new Product
        {
            Name = (string?)productToken["name"],
            UPC = (uint)productToken["upc"]!,
            BasePrice = new Price(
                (double)productToken["price"]!,
                (string)productToken["currency"]!
                )
        };
    }
}