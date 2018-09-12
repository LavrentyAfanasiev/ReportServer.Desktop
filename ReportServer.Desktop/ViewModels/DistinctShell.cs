﻿using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;
using ReportServer.Desktop.Entities;
using ReportServer.Desktop.Interfaces;
using ReportServer.Desktop.Models;
using ReportServer.Desktop.Views;
using ReportServer.Desktop.Views.WpfResources;
using Ui.Wpf.Common;
using Ui.Wpf.Common.ShowOptions;

namespace ReportServer.Desktop.ViewModels
{

    public class DistinctShell : Shell
    {
        private readonly ICachedService cachedService;
        private readonly IDialogCoordinator dialogCoordinator;

        public ReactiveCommand SaveCommand { get; set; }
        public ReactiveCommand RefreshCommand { get; set; }
        public ReactiveCommand CreateTaskCommand { get; set; }
        public ReactiveCommand CreateOperCommand { get; set; }
        public ReactiveCommand CreateScheduleCommand { get; set; }
        public ReactiveCommand DeleteCommand { get; set; }

        public DistinctShell(ICachedService cachedService, IDialogCoordinator dialogCoordinator)
        {
            this.cachedService = cachedService;
            this.dialogCoordinator = dialogCoordinator;

            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (DocumentPane.Children.First(child => child.IsActive).Content is
                        IView v && v.ViewModel is ISaveableViewModel vm)
                    await vm.Save();
            });

            RefreshCommand = ReactiveCommand.Create(this.cachedService.RefreshData);

            CreateTaskCommand = ReactiveCommand.Create(() =>
                ShowDistinctView<TaskEditorView>("Creating new Task",
                    new TaskEditorRequest {Task = new ApiTask {Id = 0}},
                    new UiShowOptions {Title = "Creating new Task"}));

            CreateScheduleCommand = ReactiveCommand.Create(() =>
                ShowDistinctView<CronEditorView>("Creating new Schedule",
                    new CronEditorRequest { Schedule = new ApiSchedule { Id = 0 } },
                    new UiShowOptions { Title = "Creating new Schedule" }));

            CreateOperCommand = ReactiveCommand.Create(() =>
                ShowDistinctView<OperEditorView>("Creating new operation",
                    new OperEditorRequest
                    {
                        Oper = new ApiOper {Id = 0},
                        FullId = "Creating new operation"
                    },
                    new UiShowOptions {Title = "Creating new operation" }));
           
            DeleteCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (DocumentPane.Children.First(child => child.IsActive).Content is
                        IView v && v.ViewModel is IDeleteableViewModel vm)
                    await vm.Delete();
            });
        }

        public async Task InitCachedService(int tries)
        {
            while (tries-- > 0)
            {
                var serviceUri = await dialogCoordinator.ShowInputAsync(this, "Login",
                    "Enter working Report service instance url",
                    new MetroDialogSettings
                    {
                        DefaultText = "http://localhost:12345/",
                        DefaultButtonFocus = MessageDialogResult.Affirmative
                    });

                if (!cachedService.Init(serviceUri))
                    continue;

                ShowView<TaskManagerView>(
                    options: new UiShowOptions {Title = "Task Manager", CanClose = false});

                ShowView<OperManagerView>(
                    options: new UiShowOptions {Title = "Oper Manager", CanClose = false});

                ShowView<RecepientManagerView>(
                    options: new UiShowOptions { Title = "Recepient Manager", CanClose = false });

                ShowView<ScheduleManagerView>(
                    options: new UiShowOptions { Title = "Schedule Manager", CanClose = false });
                return;
            }

            Application.Current.MainWindow?.Close();
        }

        public void ShowDistinctView<TView>(string value,
                                            ViewRequest viewRequest = null,
                                            UiShowOptions options = null) where TView : class, IView
        {
            var child = DocumentPane.Children
                .FirstOrDefault(ch =>
                    ch.Content is TView view && view.ViewModel.FullTitle == value);

            if (child != null)
                child.IsActive = true;

            else
                ShowView<TView>(viewRequest, options);
        }
    }
}