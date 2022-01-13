using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities;
using AutoMapper;
using Domain;

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        // Config for AutoMapper
        public MappingProfiles()
        {
            // Maps Activity to Activity
            CreateMap<Activity, Activity>();
            // Maps Activity values to ActivityDto values 
            CreateMap<Activity, ActivityDto>()
                // HostUsername maps from attendees where IsHost is true and grabs that username
                .ForMember(d => d.HostUsername, o => o.MapFrom(s => s.Attendees
                    .FirstOrDefault(x => x.IsHost).AppUser.UserName));
            // Maps ActivityAttendee to Profile
            CreateMap<ActivityAttendee, Profiles.Profile>()
                // DisplayName maps from AppUser.DisplayName provided
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser.DisplayName))
                // Username maps from AppUser.Username provided
                .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName))
                // Bio maps from AppUser.Bio provided
                .ForMember(d => d.Bio, o => o.MapFrom(s => s.AppUser.Bio));
        }
    }
}