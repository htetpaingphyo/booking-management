using BookingManagement.Models.ViewModels;
using Kendo.Mvc.UI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingManagement.Services
{
    public class ServiceCollector
    {
        public static IEnumerable<ServiceDescriptor> GetServices()
        {
            yield return ServiceDescriptor.Scoped<ISchedulerEventService<MeetingViewModel>, MeetingService>();
            yield return ServiceDescriptor.Scoped<ISchedulerEventService<AttendeeViewModel>, AttendeeService>();            
        }
    }
}
