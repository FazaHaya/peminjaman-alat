using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PeminjamanAlat.NewFolder;
using System.Net.Http.Json;

namespace PeminjamanAlat.Pages.Admin
{
    public class AlatModel : PageModel
    {
        private readonly HttpClient _http;

        public AlatModel(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
            _http.BaseAddress = new Uri("https://localhost:7245/");
        }

        public List<AlatDTO> DataAlat { get; set; } = new();

        [BindProperty]
        public AlatInput Input { get; set; }

        [BindProperty]
        public int IdAlat { get; set; }

        public async Task OnGetAsync()
        {
            var result = await _http.GetFromJsonAsync<List<AlatDTO>>("api/Alat");

            if (result != null)
                DataAlat = result;
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            var cookie = Request.Headers["Cookie"].ToString();

            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Add("Cookie", cookie);

            var dto = new
            {
                idKategori = Input.IdKategori,
                namaAlat = Input.NamaAlat,
                stok = Input.Stok,
                status = Input.Stok > 0 ? "Tersedia" : "Kosong",
                kodeAlat = Input.KodeAlat,
                deskripsi = Input.Deskripsi,
                foto = ""
            };

            await _http.PostAsJsonAsync("api/Alat", dto);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            var cookie = Request.Headers["Cookie"].ToString();

            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Add("Cookie", cookie);

            var dto = new
            {
                idKategori = Input.IdKategori,
                namaAlat = Input.NamaAlat,
                stok = Input.Stok,
                status = Input.Stok > 0 ? "Tersedia" : "Kosong",
                kodeAlat = Input.KodeAlat,
                deskripsi = Input.Deskripsi,
                foto = ""
            };

            await _http.PutAsJsonAsync($"api/Alat/{Input.IdAlat}", dto);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var cookie = Request.Headers["Cookie"].ToString();

            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Add("Cookie", cookie);

            await _http.DeleteAsync($"api/Alat/{IdAlat}");

            return RedirectToPage();
        }
    }

    public class AlatInput
    {
        public int IdAlat { get; set; }
        public int IdKategori { get; set; }
        public string NamaAlat { get; set; }
        public int Stok { get; set; }
        public string KodeAlat { get; set; }
        public string Deskripsi { get; set; }
    }
}