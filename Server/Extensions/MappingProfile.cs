using AutoMapper;
using Server.Dtos.Files;
using Server.Dtos.Folders;
using Server.Models;

namespace Server.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<FileModel, FileResponseDto>();
            CreateMap<FileResponseDto, FileModel>()
                .ForMember(x => x.User, opt => opt.Ignore());
            CreateMap<Folder, FolderItemDto>()
                            .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => "folder"))
                            .ForMember(dest => dest.Size, opt => opt.Ignore())
                            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
            CreateMap<FileResponseDto, FolderItemDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => "file"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.UploadedAt));
        }
    }
}
