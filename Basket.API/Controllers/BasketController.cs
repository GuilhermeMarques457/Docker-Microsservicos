using Basket.API.GrpcServices;
using BasketAPI.Entities;
using BasketAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BasketAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly DiscountGrpcService _discountGrpcService;

        public BasketController(IBasketRepository repository, DiscountGrpcService discountGrpcService)
        {
            _repository = repository;
            _discountGrpcService = discountGrpcService;
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await _repository.GetBasket(userName);

            return Ok(basket ?? new ShoppingCart(userName));
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody]
        ShoppingCart basket)
        {

            foreach (var item in basket.Items)
            {
                var coupon = await _discountGrpcService.GetDiscount(item.ProductName!);
                item.Price -= coupon.Amount;
            }
            return Ok(await _repository.UpdateBasket(basket));
        }

        [HttpDelete("{userName}", Name = "DeleteBasket")]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _repository.DeleteBasket(userName);
            return Ok();
        }
    }
}
