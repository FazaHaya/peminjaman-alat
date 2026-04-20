using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PeminjamanAlat.DTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PeminjamanAlat.Pages.Admin
{
    public class KategoriModel : PageModel
    {
        private readonly HttpClient _http;

        public KategoriModel(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
            _http.BaseAddress = new Uri("https://localhost:7245/");
        }

        public List<KategoriDTO> DataKategori { get; set; } = new();

        [BindProperty]
        public KategoriInput Input { get; set; }

        [BindProperty]
        public int IdKategori { get; set; }

        public async Task OnGetAsync()
        {
            var data = await _http.GetFromJsonAsync<List<KategoriDTO>>("api/Kategori");

            if (data != null)
                DataKategori = data;
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (Input == null || string.IsNullOrWhiteSpace(Input.NamaKategori))
                return RedirectToPage();

            var cookie = Request.Headers["Cookie"].ToString();

            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Add("Cookie", cookie);

            var dto = new
            {
                namaKategori = Input.NamaKategori.Trim()
            };

            var response = await _http.PostAsJsonAsync("api/Kategori", dto);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine(error);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            var cookie = Request.Headers["Cookie"].ToString();

            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Add("Cookie", cookie);

            var dto = new
            {
                namaKategori = Input.NamaKategori
            };

            await _http.PutAsJsonAsync($"api/Kategori/{Input.IdKategori}", dto);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var cookie = Request.Headers["Cookie"].ToString();

            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Add("Cookie", cookie);

            await _http.DeleteAsync($"api/Kategori/{IdKategori}");

            return RedirectToPage();
        }
    }

    public class KategoriInput
    {
        public int IdKategori { get; set; }
        public string NamaKategori { get; set; }
    }
}