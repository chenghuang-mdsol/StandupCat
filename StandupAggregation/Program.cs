using System;
using StandupAggragation.Core.Services;

namespace StandupAggregation.Sample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IStandupService service = new StandupService("rWhFopVMgXRBxHUQIzqvDlMHOLuYA5obelp3SOVx");
            //IStandupService service = new FakeStandupService();
            var result = service.GetAllStandupHistory("Rave Scrum and Announcements", "Standup");
            Console.WriteLine("Fetched {0} Hipchat messages", result.Count);
            Console.ReadKey();
        }
    }
}