using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PeminjamanAlat.NewFolder;
using System.Net.Http.Json;

namespace PeminjamanAlat.Pages.Petugas
{
    [Authorize(Roles = "Petugas")]
    public class DaftarAlatModel : PageModel
    {
        private readonly HttpClient _http;

        public DaftarAlatModel(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
            _http.BaseAddress = new Uri("https://localhost:7245/");
        }

        public List<AlatDTO> DataAlat { get; set; } = new();

        public async Task OnGetAsync()
        {
            var result = await _http.GetFromJsonAsync<List<AlatDTO>>("api/Alat");

            if (result != null)
                DataAlat = result;
        }
    }
}