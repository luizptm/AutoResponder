using AutoMapper;
using AutoResponder.Web.Models.Entity;
using AutoResponder.ViewModels;

namespace MvcMapping.Mappers
{
	//http://eduardopires.net.br/2013/08/asp-net-mvc-utilizando-automapper-com-view-model-pattern/
    public class DomainToViewModelMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DomainToViewModelMappings"; }
        }

        protected override void Configure()
        {
			Mapper.CreateMap<BR_AutoResponder_SendingList, BR_AutoResponder_SendingListVM>();
			Mapper.CreateMap<BR_AutoResponder_Sending, BR_AutoResponder_SendingVM>();
			Mapper.CreateMap<BR_AutoResponder_Tag, BR_AutoResponder_TagVM>();
			Mapper.CreateMap<BR_AutoResponder_Template, BR_AutoResponder_TemplateVM>();
			Mapper.CreateMap<BR_AutoResponder_UserEntry, BR_AutoResponder_UserEntryVM>();
			Mapper.CreateMap<BR_Users, BR_UsersVM>();
			Mapper.CreateMap<BR_Users, BR_Users_SmallVM>();
        }
    }
}