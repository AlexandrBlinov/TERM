namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Term.DAL;

    internal sealed class Configuration : DbMigrationsConfiguration<Yst.Context.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "Yst.Context.YstContext";
            AutomaticMigrationDataLossAllowed = true;
            
        }

        protected override void Seed(Yst.Context.AppDbContext context)
        {
      /*      if (!context.DiskColours.Any()) { 
            string[] colours = new[] { "BKF", "B+R", "BK", "BK+BL", "BK+plastic (BK+Y)", "BK+W", "BK+Y", "BKBSI", "BKF+plastic", "BKFBSI", 
               "BKFPL","BKFPSI","BKFRS","BKFRSI","BKFWSI","BKFYS","BKFYSI","BKPL","BKPRS","BKPS","BKRS","BKRSI","BKWS","BKYS","GM","GM+CH","GM+plastic",
               "GMBSI","GMF","GMF+plastic","GMRSI","MB","MB+BL","MB+R","MB+RM","MB+WL","MBF","MBF+R","MBF+RM","MBFRS","MBOGS", "MBPL","MBRS" , "MBRSI", "MBYS",
               "MGM+RM","MGM+Y","MGMBSI","MWPL","R+B","S","S+B","S+CH","S+plastic","S+PLASTIC+RS","SF","W","W+B","W+B+BSI","W+B+RS","W+B+RS+BSI","W+B+WSI","W+BL",
               "W+R", "W+RS", "WF", "WFRSI", "WRS", "Black", "Silver", "White" };
            
            foreach(var colour in colours)
            {
                context.DiskColours.Add(new DiskColour { ColourName = colour });
            }
            context.SaveChanges();
            }
       */ 
        } 
    }
}
