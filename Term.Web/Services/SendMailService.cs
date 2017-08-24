using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using YstTerm.Models;

namespace YstProject.Services
{
    /// <summary>
    /// Class sending mail smtp messages 
    /// </summary>
    /// 
    public class SendMailService :IDisposable
    {
        private static readonly string[] _addresses = { "malyshev@yst.ru" , "serkov@yst.ru" };
      //  private static readonly string[] _addresses = { };
        private static readonly string _subject = "Отзывы и предложения по работе сайта terminal.yst.ru";

        private  MailMessage _mailmessage;

        public SendMailService()
        {
            _mailmessage = new MailMessage();
        }

        private void PrepareMailMessage(FeedbackForm model, string partnerId, string internalname, string companyName,string emailOfRecipient)
        {
            // Если имя менеджера не заполнено
            Array.ForEach(_addresses, address => _mailmessage.To.Add(new MailAddress(address)));

            var sb = new StringBuilder();
            sb.AppendLine("Имя: " + model.Name).AppendLine("Email: " + model.Email).AppendLine("Телефон: " + model.Phone).
                AppendLine("Код контрагента: " + partnerId).AppendLine("Название клиента: " + companyName).AppendLine("Головной терминал или точка: " + internalname).
                AppendLine("Сообщение: "+model.Message);

            if (!String.IsNullOrEmpty( emailOfRecipient)) _mailmessage.To.Add(emailOfRecipient);

            _mailmessage.Body = sb.ToString();
            _mailmessage.IsBodyHtml = false;
            _mailmessage.Subject = _subject;

        }

        /// <summary>
        /// Send mail smtp message 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="partnerId"></param>
        /// <param name="internalname"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public bool Send(FeedbackForm model, string partnerId, string internalname, string companyName, string emailOfRecipient)
        {
            PrepareMailMessage(model, partnerId, internalname, companyName, emailOfRecipient);
            
      using (var client = new SmtpClient())
            {
                try
                { client.Send(_mailmessage);
                  
                    return true;
                }
                
                catch 
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// Отправляет письмо асинхронно
        /// </summary>
        /// <param name="model"></param>
        /// <param name="partnerId"></param>
        /// <param name="internalname"></param>
        /// <param name="companyName"></param>
        /// <param name="emailOfRecipient"></param>
        /// <returns></returns>
        public async  Task<bool> SendAsync(FeedbackForm model, string partnerId, string internalname, string companyName, string emailOfRecipient)
        {
            
            //if (String.IsNullOrEmpty(emailOfRecipient)) return true;
            PrepareMailMessage(model, partnerId, internalname, companyName, emailOfRecipient);

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.SendMailAsync(_mailmessage);

                    return true;
                }
                catch (Exception )
                {
                    return false;
                }


            }
            
        }



        public async Task<bool> SendAsync(string emailOfRecipient,string subject,string text)
        {
            _mailmessage.To.Add(emailOfRecipient);

            _mailmessage.Body = text;
            _mailmessage.IsBodyHtml = true;
            _mailmessage.Subject = subject;

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.SendMailAsync(_mailmessage);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }


            }

        }



        public void Dispose()
        {
            if (_mailmessage != null)
            {
                _mailmessage.Dispose();
                _mailmessage = null;

            }
        }
    }
}