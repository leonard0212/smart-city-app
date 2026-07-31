using Microsoft.AspNetCore.Mvc;
using SmartCity.Web.Models.Message;

namespace SmartCity.Web.Controllers
{
    public class MessageController : Controller
    {
        public IActionResult Index()
        {
            var messages = new List<MessageViewModel>
            {
                new MessageViewModel
                {
                    TeamName = "Infrastructure Team",
                    MessageText = "Weekly update: All systems are operational.",
                    CreatedAt = DateTime.Now.AddDays(-1)
                },
                new MessageViewModel
                {
                    TeamName = "Analytics Team",
                    MessageText = "Traffic data for last week shows a 5% increase.",
                    CreatedAt = DateTime.Now.AddHours(-3)
                },
                    new MessageViewModel
                {
                    TeamName = "Analytics Team",
                    MessageText = "Traffic data for last week shows a 5% increase.",
                    CreatedAt = DateTime.Now.AddHours(-3)
                },
                        new MessageViewModel
                {
                    TeamName = "Analytics Team",
                    MessageText = "Traffic data for last week shows a 5% increase.",
                    CreatedAt = DateTime.Now.AddHours(-3)
                },
                            new MessageViewModel
                {
                    TeamName = "Analytics Team",
                    MessageText = "Traffic data for last week shows a 5% increase.",
                    CreatedAt = DateTime.Now.AddHours(-3)
                },
                                new MessageViewModel
                {
                    TeamName = "Analytics Team",
                    MessageText = "Traffic data for last week shows a 5% increase.",
                    CreatedAt = DateTime.Now.AddHours(-3)
                },
                                    new MessageViewModel
                {
                    TeamName = "Analytics Team",
                    MessageText = "Traffic data for last week shows a 5% increase.",
                    CreatedAt = DateTime.Now.AddHours(-3)
                },
                                        new MessageViewModel
                {
                    TeamName = "Analytics Team",
                    MessageText = "Traffic data for last week shows a 5% increase.",
                    CreatedAt = DateTime.Now.AddHours(-3)
                },
                                            new MessageViewModel
                {
                    TeamName = "Analytics Team",
                    MessageText = "Traffic data for last week shows a 5% increase.",
                    CreatedAt = DateTime.Now.AddHours(-3)
                },
                                                new MessageViewModel
                {
                    TeamName = "Analytics Team",
                    MessageText = "Traffic data for last week shows a 5% increase.",
                    CreatedAt = DateTime.Now.AddHours(-3)
                },
                                                    new MessageViewModel
                {
                    TeamName = "Analytics Team",
                    MessageText = "Traffic data for last week shows a 5% increase.",
                    CreatedAt = DateTime.Now.AddHours(-3)
                },
                                                        new MessageViewModel
                {
                    TeamName = "Analytics Team",
                    MessageText = "Traffic data for last week shows a 5% increase.",
                    CreatedAt = DateTime.Now.AddHours(-3)
                },
                                                            new MessageViewModel
                {
                    TeamName = "Analytics Team",
                    MessageText = "Traffic data for last week shows a 5% increase.",
                    CreatedAt = DateTime.Now.AddHours(-3)
                },
                                                                new MessageViewModel
                {
                    TeamName = "Analytics Team",
                    MessageText = "Traffic data for last week shows a 5% increase.",
                    CreatedAt = DateTime.Now.AddHours(-3)
                },
                                                                    new MessageViewModel
                {
                    TeamName = "Analytics Team",
                    MessageText = "Traffic data for last week shows a 5% increase.",
                    CreatedAt = DateTime.Now.AddHours(-3)
                },
                                                                        new MessageViewModel
                {
                    TeamName = "Analytics Team",
                    MessageText = "Traffic data for last week shows a 5% increase.",
                    CreatedAt = DateTime.Now.AddHours(-3)
                },
                                                                            new MessageViewModel
                {
                    TeamName = "Analytics Team",
                    MessageText = "Traffic data for last week shows a 5% increase.",
                    CreatedAt = DateTime.Now.AddHours(-3)
                },
                                                                                new MessageViewModel
                {
                    TeamName = "Analytics Team",
                    MessageText = "Traffic data for last week shows a 5% increase.",
                    CreatedAt = DateTime.Now.AddHours(-3)
                },
                                                                                    new MessageViewModel
                {
                    TeamName = "Analytics Team",
                    MessageText = "Traffic data for last week shows a 5% increase.",
                    CreatedAt = DateTime.Now.AddHours(-3)
                },

                new MessageViewModel
                {
                    TeamName = "Maintenance",
                    MessageText = "Scheduled downtime on Saturday from 2 AM to 4 AM.",
                    CreatedAt = DateTime.Now
                }
            };

            return View(messages);
        }
    }
}