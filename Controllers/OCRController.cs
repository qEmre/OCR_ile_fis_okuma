using Microsoft.AspNetCore.Mvc;
using projectOCR.DataLayer;
using IronOcr;
using projectOCR.Models;

namespace projectOCR.Controllers
{
    public class OCRController : Controller
    {
        private readonly ProjectDbContext _projectDbContext;
        public OCRController(ProjectDbContext projectDbContext)
        {
            _projectDbContext = projectDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult imageSave(IFormFile file, string imageName)
        {

            var extension = Path.GetExtension(file.FileName); // resmin uzantısını al
            var newImage = $"{Guid.NewGuid()}{extension}"; // yeni dosya adı oluştur
            var kayitYeri = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", newImage); // resmin kaydedileceği konum

            using (var stream = new FileStream(kayitYeri, FileMode.Create))
            {
                file.CopyTo(stream); // belirlenen konuma resmi kaydeder
            }

            // burada ise veri tabanı işlemlerini yapıyoruz
            var images = new Image 
            {
                resimAdi = imageName,
                resimUrl = "/img/" + newImage
            };

            _projectDbContext.İmageTable.Add(images);
            _projectDbContext.SaveChanges();

            return RedirectToAction("imageList", "OCR");
        }

        [HttpPost]
        public IActionResult OCRead(int id)
        {
            // yüklenilen resmi veri tabanından çek
            var image = _projectDbContext.İmageTable.FirstOrDefault(i => i.resimId == id);

            // resim okunacak
            var Ocr = new IronTesseract();
            Ocr.Language = OcrLanguage.Turkish;
            string ocrResult;

            using (var Input = new OcrInput())
            {
                // resmi okutmak için resmin yoluna gidiyoruz
                Input.AddImage(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.resimUrl.TrimStart('/')));
                Input.Deskew(); // görüntü hizalama
                Input.DeNoise(); // görüntü temizleme
                ocrResult = Ocr.Read(Input).Text; // düzenlenen metni okuma
            }

            ViewBag.Result = ocrResult;
            return View(image);
        }

        public IActionResult imageList()
        {
            List<Image> imageList = _projectDbContext.İmageTable.ToList();

            return View(imageList);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var image = _projectDbContext.İmageTable.FirstOrDefault(i => i.resimId == id);

            _projectDbContext.İmageTable.Remove(image);
            _projectDbContext.SaveChanges();

            return RedirectToAction("imageList", "OCR");
        }
    }
}