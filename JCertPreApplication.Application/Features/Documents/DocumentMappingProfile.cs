using AutoMapper;
using JCertPreApplication.Application.Dtos.Document;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Features.Documents
{
    public class DocumentMappingProfile : Profile
    {
        public DocumentMappingProfile()
        {
            CreateMap<Document, DocumentDto>();
            CreateMap<DocumentDto, Document>();
        }
    }
} 