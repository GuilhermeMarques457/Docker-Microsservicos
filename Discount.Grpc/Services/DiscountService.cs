using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<DiscountService> _logger;

        public DiscountService(IDiscountRepository repository, IMapper mapper, ILogger<DiscountService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            // taking it from the database using the paramater from request
            var coupon = await _repository.GetDiscount(request.ProductName);

            if(coupon == null)
            {
                throw new RpcException(
                    new Status(StatusCode.NotFound,
                    $"Discount with Product Name = {request.ProductName} was not found"
                    ));
            }

            _logger.LogInformation("Coupong Id: " + coupon.Id);

            var couponModel = _mapper.Map<CouponModel>(coupon);

            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);

            bool isAdded = await _repository.CreateDiscount(coupon);

            if(!isAdded) throw new RpcException(
                    new Status(StatusCode.Cancelled,
                    $"We cannot add {request.Coupon.ProductName} to database"
                    ));

            var couponModel = _mapper.Map<CouponModel>(coupon);

            return couponModel;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);

            bool isUpdated = await _repository.UpdateDiscount(coupon);

            if (!isUpdated) throw new RpcException(
                    new Status(StatusCode.Cancelled,
                    $"We cannot update {request.Coupon.ProductName} to database"
                    ));

            var couponModel = _mapper.Map<CouponModel>(coupon);

            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var isDeleted = await _repository.DeleteDiscount(request.ProductName);

            var response = new DeleteDiscountResponse()
            {
                Success = isDeleted
            };

            return response;
        }
    }


}
