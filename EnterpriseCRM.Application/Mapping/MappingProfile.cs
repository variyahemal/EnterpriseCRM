using AutoMapper;
using EnterpriseCRM.Application.Features.Contacts.Commands.CreateContact;
using EnterpriseCRM.Application.Features.Contacts.Commands.UpdateContact;
using EnterpriseCRM.Application.Features.Leads.Commands.CreateLead;
using EnterpriseCRM.Application.Features.Leads.Commands.UpdateLead;
using EnterpriseCRM.Domain;

namespace EnterpriseCRM.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateContactCommand, Contact>();
            CreateMap<UpdateContactCommand, Contact>();
            CreateMap<CreateLeadCommand, Lead>();
            CreateMap<UpdateLeadCommand, Lead>();
        }
    }
}