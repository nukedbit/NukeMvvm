﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FreshMvvm;
using Xamarin.Forms;

namespace FreshMvvm
{
    public class FreshShellNavigationService : ShellNavigationService, IFreshNavigationService
    {
        static string NavServiceName = "FreshShellNavigationService";
        string IFreshNavigationService.NavigationServiceName => NavServiceName;

        public FreshShellNavigationService()
        {
            FreshIOC.Container.Register<IFreshNavigationService>(this, NavServiceName);
        }

        public override void ApplyParameters(ShellLifecycleArgs args)
        {
            if (args?.PathPart?.NavigationParameters?.Count > 0)
                Debugger.Break();

            Debug.WriteLine($"ApplyParams {args?.PathPart?.Path}");
            base.ApplyParameters(args);
        }

        public override Task<ShellRouteState> NavigatingToAsync(ShellNavigationArgs args)
        {
            return base.NavigatingToAsync(args);
        }

        public override Task AppearingAsync(ShellLifecycleArgs args)
        {
            Debug.WriteLine($"AppearingAsync {args?.PathPart?.Path}");
            var test = args.PathPart;
            return base.AppearingAsync(args);
        }

        public override Page Create(ShellContentCreateArgs args)
        {
            var content = base.Create(args);

            if (content.BindingContext == null)
                            {
                string pageModelName = content.GetType().FullName + "Model";
                var pageModel = FreshIOC.Container.Resolve(Type.GetType(pageModelName)) as FreshBasePageModel;
                pageModel.PreviousNavigationServiceName = NavServiceName;
                pageModel.CurrentNavigationServiceName = NavServiceName;
                FreshPageModelResolver.BindingPageModel(null, content, pageModel);
                content.BindingContext = pageModel;
            }

            return content;
        }

        public override Task AppearedAsync(ShellLifecycleArgs args)
        {
            return base.AppearedAsync(args);
        }

        //TODO: at the moment we're implementing 
        Task IFreshNavigationService.PopToRoot(bool animate)
        {
            return Shell.Current.Navigation.PopToRootAsync(animate);
        }

        Task IFreshNavigationService.PushPage(Page page, FreshBasePageModel model, bool modal, bool animate)
        {
            if (modal)
                return Shell.Current.Navigation.PushModalAsync(page, animate);
            else
                return Shell.Current.Navigation.PushAsync(page, animate);
        }

        Task IFreshNavigationService.PopPage(bool modal, bool animate)
        {
            if (modal)
                return Shell.Current.Navigation.PopModalAsync(animate);
            else
                return Shell.Current.Navigation.PopAsync(animate);
        }

        Task<FreshBasePageModel> IFreshNavigationService.SwitchSelectedRootPageModel<T>()
        {
            throw new Exception("Not available in Shell");
        }

        void IFreshNavigationService.NotifyChildrenPageWasPopped()
        {
            //throw new Exception("Not available in Shell");
        }
    }
}