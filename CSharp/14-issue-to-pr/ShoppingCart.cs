using System;
using System.Collections.Generic;
using System.Linq;

// Demo 14 — Issue to Pull Request (the full Copilot workflow)
// ===========================================================
// This file contains an INTENTIONAL bug. It is the starting point for an
// end-to-end demo: file a GitHub Issue → let Copilot fix it → open a Pull
// Request → review → merge.
//
// The bug (now fixed):
//   ApplyDiscount() treated a PERCENTAGE discount as a flat amount. A "10%"
//   coupon subtracted 10 currency units instead of 10% of the subtotal.
//   See ISSUE.md in this folder for the bug report to copy into GitHub.
//
// Demo flow (see the language README for the full script):
//   1. Run the program → observe the wrong total
//   2. Create a GitHub Issue from ISSUE.md
//   3. Assign it to the Copilot coding agent (or use Chat → "Fix issue #N")
//   4. Review the generated branch + Pull Request
//   5. Merge the PR and close the issue

namespace IssueToPr;

/// <summary>A single line item in the cart.</summary>
public record CartItem(string Name, decimal UnitPrice, int Quantity)
{
    public decimal LineTotal => UnitPrice * Quantity;
}

/// <summary>A discount coupon. <see cref="IsPercentage"/> decides how it is applied.</summary>
public record Coupon(string Code, decimal Value, bool IsPercentage);

/// <summary>A simple shopping cart with subtotal, discount, and total calculation.</summary>
public class ShoppingCart
{
    private readonly List<CartItem> _items = new();
    private Coupon? _coupon;

    public void AddItem(string name, decimal unitPrice, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        if (unitPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice));
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        _items.Add(new CartItem(name, unitPrice, quantity));
    }

    public void ApplyCoupon(Coupon coupon) => _coupon = coupon;

    public decimal GetSubtotal() => _items.Sum(item => item.LineTotal);

    /// <summary>
    /// Applies the active coupon to the subtotal.
    /// </summary>
    public decimal ApplyDiscount(decimal subtotal)
    {
        if (_coupon is null)
            return subtotal;

        decimal discountAmount = _coupon.IsPercentage
            ? subtotal * _coupon.Value / 100m
            : _coupon.Value;

        decimal discounted = subtotal - discountAmount;

        return discounted < 0 ? 0 : discounted;
    }

    public decimal GetTotal() => ApplyDiscount(GetSubtotal());
}

public static class Program
{
    public static void Main()
    {
        var cart = new ShoppingCart();
        cart.AddItem("Mechanical Keyboard", 120.00m, 1);
        cart.AddItem("USB-C Cable", 15.00m, 2);

        cart.ApplyCoupon(new Coupon("SAVE10", 10m, IsPercentage: true));

        decimal subtotal = cart.GetSubtotal();   // 150.00
        decimal total = cart.GetTotal();

        Console.WriteLine($"Subtotal: {subtotal:C}");
        Console.WriteLine($"Coupon:   SAVE10 (10%)");
        Console.WriteLine($"Total:    {total:C}");
        Console.WriteLine();
        Console.WriteLine($"Expected total with 10% off: {subtotal * 0.90m:C}");
        Console.WriteLine($"Actual total:                {total:C}");
    }
}
