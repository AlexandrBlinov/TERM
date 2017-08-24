using System;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YstProject.Services;
using YstTerm.Models;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTestEmailSender
    {
       
        [TestMethod]
        public  async Task TestMethodSendMessage()
        {

            var model = new FeedbackForm
            {
                Name = "тест1",
                Message = "тестовое сообщение",
                Email = "lapenkov@ya.ru",
                Phone = "82930293023"

            };
             
            var mailService = new SendMailService();
            
          var result = await mailService.SendAsync(model, "00010", "Вова", "Yst", "lapenkov@yst.ru");

            Assert.IsTrue(result);

        }


    
    }
}
