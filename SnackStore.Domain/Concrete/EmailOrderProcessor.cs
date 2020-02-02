using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using SnackStore.Domain.Abstract;
using SnackStore.Domain.Entities;

namespace SnackStore.Domain.Concrete
{
    public class EmailSettings
    {
        public EmailSettings()
        {
            var localPath = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()));

            FileLocation = Path.Combine(localPath ?? "c:\\", "emails");
        }

        public string MailToAddress = "harrismooresuperbusiness@gmail.com";
        public string MailFromAddress = "harrismooresuperbusiness@gmail.com";
        public bool UseSsl = true;
        public string Username = "harrismooresuperbusiness@gmail.com";
        public string Password = "nuvneb56";
        public string ServerName = "smtp.gmail.com";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation { get; }
    }

    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSettings;

        public EmailOrderProcessor(EmailSettings settings)
        {
            emailSettings = settings;
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingInfo)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    // if directory not exists
                    if (!Directory.Exists(emailSettings.FileLocation))
                    {
                        Directory.CreateDirectory(emailSettings.FileLocation);
                    }

                    smtpClient.EnableSsl = false;
                }

                StringBuilder body =
                    new StringBuilder().AppendLine("A new order has been submitted")
                        .AppendLine("---")
                        .AppendLine("Items:");

                foreach (var line in cart.Lines)
                {
                    var subtotal = line.Product.Price*line.Quantity;
                    body.AppendFormat("{0} x {1} (subtotal: {2:c}", line.Quantity, line.Product.Name, subtotal);
                }

                body.AppendFormat("Total order value: {0:c}", cart.ComputeTotalValue())
                    .AppendLine("---")
                    .AppendLine("Ship to:")
                    .AppendLine(shippingInfo.Name)
                    .AppendLine(shippingInfo.Line1)
                    .AppendLine(shippingInfo.Line2 ?? "")
                    .AppendLine(shippingInfo.Line3 ?? "")
                    .AppendLine(shippingInfo.City)
                    .AppendLine(shippingInfo.State ?? "")
                    .AppendLine(shippingInfo.Country)
                    .AppendLine(shippingInfo.Zip)
                    .AppendLine("---")
                    .AppendFormat("Gift wrap: {0}", shippingInfo.GiftWrap ? "Yes" : "No");

                MailMessage mailMessage = new MailMessage(emailSettings.MailFromAddress,
                    // From                                        
                    emailSettings.MailToAddress,
                    // To                                       
                    "New order submitted!",
                    // Subject                                        
                    body.ToString());
                // Body 


                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }

                smtpClient.Send(mailMessage);
            }
        }
    }
}