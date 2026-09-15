using Xunit;

// Regression tests for the bug described in ISSUE.md: a percentage coupon was
// subtracted as a flat currency amount instead of a percentage of the subtotal.

namespace IssueToPr.Tests;

public class ShoppingCartTests
{
    private static ShoppingCart CreateCart()
    {
        var cart = new ShoppingCart();
        cart.AddItem("Mechanical Keyboard", 120.00m, 1);
        cart.AddItem("USB-C Cable", 15.00m, 2);
        return cart;
    }

    [Fact]
    public void Percentage_coupon_discounts_a_percentage_of_the_subtotal()
    {
        var cart = CreateCart();
        cart.ApplyCoupon(new Coupon("SAVE10", 10m, IsPercentage: true));

        Assert.Equal(150m, cart.GetSubtotal());
        Assert.Equal(135m, cart.GetTotal());
    }

    [Fact]
    public void Flat_coupon_subtracts_the_value_directly()
    {
        var cart = CreateCart();
        cart.ApplyCoupon(new Coupon("MINUS10", 10m, IsPercentage: false));

        Assert.Equal(140m, cart.GetTotal());
    }

    [Fact]
    public void Total_never_goes_below_zero()
    {
        var cart = CreateCart();
        cart.ApplyCoupon(new Coupon("HUGE", 999m, IsPercentage: false));

        Assert.Equal(0m, cart.GetTotal());
    }

    [Fact]
    public void Percentage_coupon_over_one_hundred_clamps_to_zero()
    {
        var cart = CreateCart();
        cart.ApplyCoupon(new Coupon("FREE", 150m, IsPercentage: true));

        Assert.Equal(0m, cart.GetTotal());
    }

    [Fact]
    public void Cart_without_coupon_returns_the_subtotal()
    {
        var cart = CreateCart();

        Assert.Equal(150m, cart.GetTotal());
    }
}
