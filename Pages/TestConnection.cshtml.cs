using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using VONetData;

namespace VONetData.Pages
{
    public class TestConnectionModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        public bool ConnectionSuccessful { get; private set; }
        public string? ErrorMessage { get; private set; }

        public TestConnectionModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void OnGet()
        {
            try
            {
                // Try to open a connection and query the database
                _dbContext.Database.OpenConnection();
                _dbContext.Database.CloseConnection();
                ConnectionSuccessful = true;
            }
            catch (Exception ex)
            {
                ConnectionSuccessful = false;
                ErrorMessage = ex.Message;
            }
        }
    }
}
