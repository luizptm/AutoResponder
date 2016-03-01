using AutoMapper;
using AutoResponder.Web.Models.Entity;
using AutoResponder.ViewModels;

namespace MvcMapping.Mappers
{
	//http://eduardopires.net.br/2013/08/asp-net-mvc-utilizando-automapper-com-view-model-pattern/
    public class ViewModelToDomainMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ViewModelToDomainMappings"; }
        }

        protected override void Configure()
        {
			Mapper.CreateMap<BR_AutoResponder_SendingListVM, BR_AutoResponder_SendingList>();
			Mapper.CreateMap<BR_AutoResponder_SendingVM, BR_AutoResponder_Sending>();
			Mapper.CreateMap<BR_AutoResponder_TagVM, BR_AutoResponder_Tag>();
			Mapper.CreateMap<BR_AutoResponder_TemplateVM, BR_AutoResponder_Template>();
			Mapper.CreateMap<BR_AutoResponder_UserEntryVM, BR_AutoResponder_UserEntry>();
			Mapper.CreateMap<BR_UsersVM, BR_Users>();
			Mapper.CreateMap<BR_Users_SmallVM, BR_Users>();
        }
    }
}