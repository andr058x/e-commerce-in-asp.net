using System;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using SnackStore.Domain.Entities;
using SnackStore.Domain.Abstract;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Moq;
using SnackStore.Domain.Concrete;
using SnackStore.WebUI.Infrastructure.Abstract;
using SnackStore.WebUI.Infrastructure.Concrete;

namespace SnackStore.WebUI.Infrastructure
{

    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectKernel;

        public NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController) ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {
            ninjectKernel.Bind<IProductRepository>().To<EFProductRepository>();

            EmailSettings emailSettings = new EmailSettings
            {
                WriteAsFile = bool.Parse(ConfigurationManager.AppSettings["Email.WriteAsFile"] ?? "false"),
                UseSsl = bool.Parse(ConfigurationManager.AppSettings["Email.UseSsl"] ?? "false"),
                MailFromAddress = ConfigurationManager.AppSettings["Email.MailFromAddress"],
                MailToAddress = ConfigurationManager.AppSettings["Email.MailToAddress"],
                ServerName = ConfigurationManager.AppSettings["Email.ServerName"],
                ServerPort = int.Parse(ConfigurationManager.AppSettings["Email.ServerPort"]),
                Username = ConfigurationManager.AppSettings["Email.Username"],
                Password = ConfigurationManager.AppSettings["Email.Password"]
            };

            ninjectKernel.Bind<IOrderProcessor>().To<EmailOrderProcessor>().WithConstructorArgument("settings", emailSettings);

            ninjectKernel.Bind<IAuthProvider>().To<FormsAuthProvider>();
        }
    }
}  