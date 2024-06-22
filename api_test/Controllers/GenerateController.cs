using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;
using System.Net;
using System.Reflection;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerateController : ControllerBase
    {
        AppContext _context;
        public string title = " ";
        public GenerateController(AppContext context)
        {
            _context = context;
        }
        // GET: api/<GenerateController>
        [HttpGet("pdf")]
        public async Task<IActionResult> GetAsync(string url)
        {
            try
            {
                string[] urls = url.Split("/");
                //Проверка есть ли в ссылке файл
                if (urls.Length > 1 && urls.Last().IndexOf(".") > 0 && urls.Last().IndexOf("html") <= 0 && urls.Last().IndexOf("php") <= 0 && urls.Last().IndexOf("com") <= 0 && urls.Last().IndexOf("ru") <= 0)
                {
                    //Путь к папке
                    string desktopPath = "Files";
                   //Получение имени файла через метод
                    string filename = getFilename(url);
                    //Передача имени файла в глобальную переменную
                    title = filename;

                    using (var client = new WebClient())
                    {
                        //Скачиваем файл
                        client.DownloadFile(url, desktopPath + "/" + filename);
                    }
                    //Перевод файла в байты для передачи
                    byte[] fileByte = System.IO.File.ReadAllBytes(desktopPath + "/" + filename);

                    // Тип файла
                    string mimeTyp = "application/octet-stream";
                    //Сохранение в бд
                    var data = new Data
                    {
                        Title = title,
                        Status = "Готов к скачиванию"
                    };
                    _context.Data.Add(data);
                    _context.SaveChanges();

                    // Возращение файла пользователю
                    return File(fileByte, mimeTyp, Path.GetFileName(desktopPath + "/" + filename));
                }
                //Работа с Puppeteer
                var browserFetcher = new BrowserFetcher();
                await browserFetcher.DownloadAsync();
                var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true
                });
                DateTime date = DateTime.Now;

               //Получение хеша даты для создания уникального имени
                int value = date.GetHashCode();
                var page = await browser.NewPageAsync();
                await page.GoToAsync(url);
                //Создание названия файла pdf
                string name = $"Files/{DateTime.Today.ToShortDateString().Replace("/", "-")}_{value}.pdf";
                title = name;
                await page.PdfAsync(name);
                //Перевод файла в байты для передач
                byte[] fileBytes = System.IO.File.ReadAllBytes(name);

                // Тип файла
                string mimeType = "application/octet-stream";
                var datas = new Data
                {
                    Title = title,
                    Status = "Готов к скачиванию"
                };
                _context.Data.Add(datas);
                _context.SaveChanges();

                // Возращение файла пользователю
                return File(fileBytes, mimeType, Path.GetFileName(name));
            }
            catch(Exception e)
            {

                var datas = new Data
                {
                    Title = title,
                    Status = "Ошибка"
                };
                _context.Data.Add(datas);
                _context.SaveChanges();
                return new JsonResult(e.Message);
            }
            
        }

        //Метод для получения имени файла из ссылки

        private string getFilename(string hreflink)
        {
            Uri uri = new Uri(hreflink);

            string filename = System.IO.Path.GetFileName(uri.LocalPath);

            return filename;
        }
    }
}
