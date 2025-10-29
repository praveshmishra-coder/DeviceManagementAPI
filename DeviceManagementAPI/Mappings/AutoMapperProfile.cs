using AutoMapper;
using DeviceManagementAPI.Models;
using DeviceManagementAPI.DTOs;

namespace DeviceManagementAPI.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Device
            CreateMap<Device, DeviceResponseDTO>();
            CreateMap<DeviceRequestDTO, Device>();

            // Asset
            CreateMap<Asset, AssetResponseDTO>();
            CreateMap<AssetRequestDTO, Asset>();

            // SignalMeasurement
            CreateMap<SignalMeasurement, SignalMeasurementResponseDTO>();
            CreateMap<SignalMeasurementRequestDTO, SignalMeasurement>();
        }
    }
}
