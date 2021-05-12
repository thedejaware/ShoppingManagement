using AutoMapper;
using Stock.Domain.Entities;
using Stock.Grpc.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock.Grpc.Mappings
{
    public class StockProfile : Profile
    {
        public StockProfile()
        {
            CreateMap<StockItem, StockModel>().ReverseMap();
        }
    }
}
