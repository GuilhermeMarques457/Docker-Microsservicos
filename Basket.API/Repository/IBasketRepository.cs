﻿using BasketAPI.Entities;

namespace BasketAPI.Repository
{
    public interface IBasketRepository
    {
        Task<ShoppingCart?> GetBasket(string userName);
        Task<ShoppingCart?> UpdateBasket(ShoppingCart basket);
        Task DeleteBasket(string userName);
    }
}
