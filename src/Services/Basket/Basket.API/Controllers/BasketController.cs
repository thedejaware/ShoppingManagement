using Basket.API.GrpcServices;
using Basket.API.Model;
using Basket.Application.Contracts.Persistence;
using Basket.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly StockGrpcService _stockGrpcService;
        private readonly ILogger<BasketController> _logger;

        public BasketController(IBasketRepository basketRepository, StockGrpcService stockGrpcService, ILogger<BasketController> logger)
        {
            _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            _stockGrpcService = stockGrpcService ?? throw new ArgumentNullException(nameof(stockGrpcService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Route("[action]/{username}", Name = "Get")]
        [HttpGet]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> Get(string username)
        {
            var basket = await _basketRepository.GetBasket(username);
            if (basket == null)
                return BasketNotFound(username);

            return Success(basket);
        }

        [HttpPost]
        [Route("[action]/{username}", Name = "AddItemToBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> AddItemToBasket(string username, [FromBody] ShoppingCartItem cartItem)
        {
            // Check if the basket exists
            var basket = await _basketRepository.GetBasket(username);
            if (basket == null)
                return BasketNotFound(username);


            // Check if the item already exists in the basket
            if (await _basketRepository.GetItemInBasket(cartItem.ProductId, username))
                return ItemAlreadyInBasket(username, cartItem.ProductId);


            // Stock control 
            // Communicate with Stock Grpc Service to check stock quantity
            var stock = await _stockGrpcService.GetStock(cartItem.ProductId);
            if (stock != null && stock.Quantity < cartItem.Quantity)
                return NotEnoughStock(cartItem.ProductName);


            // Add Item into Basket
            basket.Items.Add(cartItem);

            // Update basket
            var result = await _basketRepository.UpdateBasket(basket);
            return Success(result);

        }

        [HttpPost]
        [Route("[action]/{username}", Name = "UpdateItemQuantity")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateItemQuantity(string username, [FromBody] ShoppingCartItem cartItem)
        {
            // Check if the basket exists
            var basket = await _basketRepository.GetBasket(username);
            if (basket == null)
                return BasketNotFound(username);


            // Check if the item already exists in the basket
            if (!await _basketRepository.GetItemInBasket(cartItem.ProductId, username))
                return ItemNotFound(username, cartItem.ProductId);


            // Stock control 
            // Communicate with Stock Grpc Service to check stock quantity
            var stock = await _stockGrpcService.GetStock(cartItem.ProductId);
            if (stock != null && stock.Quantity < cartItem.Quantity)
                return NotEnoughStock(cartItem.ProductName);


            // Update Item Quantity
            var itemIndex = basket.Items.FindIndex(p => p.ProductId == cartItem.ProductId);
            basket.Items[itemIndex].Quantity = cartItem.Quantity;

            // Update basket
            var result = await _basketRepository.UpdateBasket(basket);
            return Success(result);

        }

        [HttpPost]
        [Route("[action]", Name = "UpdateBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            var result = await _basketRepository.UpdateBasket(basket);
            return Success(result);
        }

        [HttpPost]
        [Route("[action]", Name = "CreateBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> CreateBasket([FromBody] ShoppingCart basket)
        {
            // Check if the basket exists
            var basketInCache = await _basketRepository.GetBasket(basket.UserName);
            if (basketInCache != null)
                return BasketAlreadyCreated(basket.UserName);

            // Stock control 
            // Communicate with Stock Grpc Service to check stock quantity
            foreach (var item in basket.Items)
            {
                var stock = await _stockGrpcService.GetStock(item.ProductId);
                if (stock != null && stock.Quantity < item.Quantity)
                {
                    return NotEnoughStock(item.ProductName);

                }

            }

            var result = await _basketRepository.CreateBasket(basket);
            return Success(result);
        }

        [HttpDelete("{username}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string username)
        {
            await _basketRepository.DeleteBasket(username);

            return new JsonResult(new ResponseModel
            {
                Success = true
            });
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] ShoppingCart basketCheckout)
        {
            // Get existing basket with total prica by userName
            var basket = await _basketRepository.GetBasket(basketCheckout.UserName);
            if (basket == null)
                return BasketNotFound(basketCheckout.UserName);

            // TO DO 
            // Publishing basketcheckout event using MassTransit
            // Send checkout event to rabbitmq
            // var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            // await _publishEndpoint.Publish(eventMessage);

            // remove the basket
            await _basketRepository.DeleteBasket(basket.UserName);

            return Accepted();

        }

        #region Private Methods
        private ActionResult BasketNotFound(string username)
        {
            var message = $"Basket not found for user {username}.";
            _logger.LogWarning(message);
            return new JsonResult(new ResponseModel
            {
                Success = false,
                ErrorMessage = message
            });
        }

        private ActionResult BasketAlreadyCreated(string username)
        {
            var message = $"Basket for {username} has already been created.";
            _logger.LogWarning(message);
            return new JsonResult(new ResponseModel
            {
                Success = false,
                ErrorMessage = message
            });
        }

        private ActionResult ItemAlreadyInBasket(string username, object itemId)
        {
            var message = $"Product {itemId} already in basket for user {username}, and cannot be added again.";
            _logger.LogWarning(message);
            return new JsonResult(new ResponseModel
            {
                Success = false,
                ErrorMessage = message
            });
        }

        private ActionResult ItemNotFound(string username, string itemId)
        {
            var message = $"Product {itemId} could not be found in basket for customer {username}.";
            _logger.LogWarning(message);
            return new JsonResult(new ResponseModel
            {
                Success = false,
                ErrorMessage = message
            });
        }


        private ActionResult Success(object data)
        {
            return new JsonResult(new ResponseModel
            {
                Success = true,
                Data = data
            });
        }

        private ActionResult Error(string errorMessage)
        {
            return new JsonResult(new ResponseModel
            {
                Success = true,
                ErrorMessage = errorMessage
            });
        }

        private ActionResult NotEnoughStock(string productName)
        {
            return new JsonResult(new ResponseModel
            {
                Success = false,
                ErrorMessage = $"You can not add the product {productName} to your cart. Because there is not enough stock for that product."
            });
        }
        #endregion


    }
}
