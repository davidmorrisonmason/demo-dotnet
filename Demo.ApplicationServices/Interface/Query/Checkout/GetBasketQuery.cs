namespace Demo.ApplicationServices.Interface.Query.Checkout;

public record GetBasketQuery(int Id) : IQuery<Model.Domain.Checkout.Basket?>;
