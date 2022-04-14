using System;
using Deployf.Botf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
namespace BotFFlowers
{
    public class Program : BotfProgram
    {
          public static  void Main(string[] args)
          {
              BotfProgram.StartBot(args, onConfigure: (svc, cfg) => {
                  svc.AddDbContext<ProductContext>(options => options.UseSqlServer("Server=217.28.223.127,17160;User Id=user_bc105;Password=t{6Z/Ps32n&T;Database=db_fa529;"));
                    
              }, onRun: (app, cfg) => {
                  app.UseStaticFiles();
              });

          }
        
        
    }
}




