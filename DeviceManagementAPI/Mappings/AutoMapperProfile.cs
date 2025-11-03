using AutoMapper;
using DeviceManagementAPI.Models;
using DeviceManagementAPI.DTOs;

namespace DeviceManagementAPI.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            
            CreateMap<Device, DeviceResponseDTO>();
            CreateMap<DeviceRequestDTO, Device>();

            
            CreateMap<Asset, AssetResponseDTO>();
            CreateMap<AssetRequestDTO, Asset>();

            
            CreateMap<SignalMeasurement, SignalMeasurementResponseDTO>();
            CreateMap<SignalMeasurementRequestDTO, SignalMeasurement>();
        }
    }
}
