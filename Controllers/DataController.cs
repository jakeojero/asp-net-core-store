using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Data.SqlClient;
using Casestudy.Models;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace Casestudy.Controllers
{
    public class DataController : Controller
    {
        AppDbContext _db;
        IConfiguration config;
        public DataController(AppDbContext context, IConfiguration config)
        {
            _db = context;
            this.config = config;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            string sql = await getSqlFromWeb();

            IEnumerable<string> commandStrings = Regex.Split(sql, @"^\s*GO\s*$",
                          RegexOptions.Multiline | RegexOptions.IgnoreCase);

            try
            {
                SqlConnection conn = new SqlConnection(config.GetConnectionString("DefaultConnection"));
                conn.Open();
                foreach (string commandString in commandStrings)
                {
                    if (commandString.Trim() != "")
                    {
                        using (var command = new SqlCommand(commandString, conn))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
                conn.Close();
                ViewBag.Finished = "Data Refreshed";
            }
            catch (Exception ex)
            {
                ViewBag.Finished = "Error: " + ex.Message;
            }
           
            return View();
        }

        private async Task<String> getSqlFromWeb()
        {
            string url = "https://raw.githubusercontent.com/jakeojero/sql-casestudy/master/CaseStudyDBSetup.sql";
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

    }
}