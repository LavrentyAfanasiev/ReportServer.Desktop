﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReactiveUI;
using ReportServer.Desktop.Entities;

namespace ReportServer.Desktop.Interfaces
{
    public interface ICachedService
    {
        ReactiveList<ApiOperTemplate> OperTemplates { get; set; }
        ReactiveList<ApiRecepientGroup> RecepientGroups { get; set; }
        ReactiveList<ApiTelegramChannel> TelegramChannels { get; set; }
        ReactiveList<ApiSchedule> Schedules { get; set; }
        ReactiveList<ApiTask> Tasks { get; set; }
        ReactiveList<ApiTaskOper> TaskOpers { get; set; }
        Dictionary<string, Type> DataImporters { get; set; }
        Dictionary<string, Type> DataExporters { get; set; }

        bool Init(string serviceUri);
        void RefreshOperTemplates();
        void RefreshRecepientGroups();
        void RefreshTelegramChannels();
        void RefreshSchedules();
        void RefreshTasks();
        void RefreshTaskOpers();
        void RefreshData();

        List<ApiTaskInstance> GetInstancesByTaskId(int taskId);
        List<ApiOperInstance> GetOperInstancesByTaskInstanceId(int taskInstanceId);
        ApiOperInstance GetFullOperInstanceById(int id);
        Task<string> GetCurrentTaskViewById(int taskId);

        int? CreateOrUpdateOper(ApiOperTemplate operTemplateTemplate);
        int? CreateOrUpdateRecepientGroup(ApiRecepientGroup group);
        int? CreateOrUpdateTelegramChannel(ApiTelegramChannel channel);
        int? CreateOrUpdateSchedule(ApiSchedule schedule);
        int? CreateOrUpdateTask(ApiTask task);

        void DeleteOperTemplate(int id);
        void DeleteSchedule(int id);
        void DeleteTask(int id);
        void DeleteInstance(int id);

        void OpenPageInBrowser(string htmlPage);
    }
}
